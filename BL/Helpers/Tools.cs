using BlApi;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

internal static class Tools 
{
    public static string ToStringProperty<T>(this T t)
    {
        //string result = "";
        //PropertyInfo[] properties = t!.GetType().GetProperties();
        //foreach (var prop in properties)
        //{
        //    result += $"{prop.Name}: {prop.GetValue(null) ?? "null"}\n";
        //}
        //return result;
        if (t == null)
            throw new ArgumentNullException(nameof(t), "Object cannot be null.");

        var result = new StringBuilder();
        PropertyInfo[] properties = t.GetType().GetProperties();

        foreach (var prop in properties)
        {
            // Skip properties without a getter
            if (!prop.CanRead)
                continue;

            // Get the value of the property
            object? value;
            try
            {
                value = prop.GetValue(t);
            }
            catch (TargetInvocationException)
            {
                value = "Error accessing property";
            }

            // If the property is a collection, list its elements
            if (value is System.Collections.IEnumerable enumerable && value is not string)
            {
                result.AppendLine($"{prop.Name}: [");
                foreach (var item in enumerable)
                {
                    result.AppendLine($"  {item}");
                }
                result.AppendLine("]");
            }
            else
            {
                result.AppendLine($"{prop.Name}: {value ?? "null"}");
            }
        }

        return result.ToString();
        //if (t == null)
        //    throw new ArgumentNullException(nameof(t), "Object cannot be null.");

        //string result = "";
        //PropertyInfo[] properties = t.GetType().GetProperties();
        //foreach (var prop in properties)
        //{
        //    var value = prop.GetValue(t) ?? "null";
        //    result += $"{prop.Name}: {value}\n";
        //}
        //return result;

    }

    #region Adress & Latitude & Longitude calculation

    public static (double Latitude, double Longitude) GetAddressCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be null or empty.", nameof(address));
        }

        const string LocationIqApiKey = "pk.a0941b60144dc7fe0b85814d99ab3be7";
        const string BaseUrl = "https://us1.locationiq.com/v1/search.php";

        // בניית כתובת הבקשה
        string requestUrl = $"{BaseUrl}?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";

        using (var client = new HttpClient())
        {
            HttpResponseMessage response;
            try
            {
                // ביצוע הקריאה הסינכרונית
                response = client.GetAsync(requestUrl).Result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending request to LocationIQ API.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching data from LocationIQ: {response.ReasonPhrase}");
            }

            string responseContent = response.Content.ReadAsStringAsync().Result;


            var locationData = System.Text.Json.JsonSerializer.Deserialize<List<LocationIqResponse>>(responseContent);

            if (locationData == null || locationData.Count == 0)
            {
                throw new Exception("No coordinates found for the provided address.");
            }

            var coordinates = locationData[0];


            bool isLatValid = double.TryParse(coordinates.lat, out double latitude);
            bool isLonValid = double.TryParse(coordinates.lon, out double longitude);



            if (isLatValid && isLonValid)
            {
                return (latitude, longitude);
            }
            else
            {
                throw new Exception($"Invalid coordinate data. Latitude valid: {isLatValid}, Longitude valid: {isLonValid}");
            }
        }
    }

    /// <summary>
    /// class to get the latitude and longitude of a LocationIqResponse
    /// </summary>
    private class LocationIqResponse
    {
        public string? lat { get; set; }
        public string? lon { get; set; }
    }

    /// <summary>
    /// function that checks if the coordinates of a volunteer match the coordinates based on his address. 
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    public static bool CheckAddressVolunteer(BO.Volunteer vol)
    {
        if (vol.Latitude == null || vol.Longitude == null)
        {
            throw new Exception("Latitude or Longitude is null.");
        }

        if (string.IsNullOrEmpty(vol.Address))
        {
            throw new Exception("Address is null or empty.");
        }
        var (expectedLatitude, expectedLongitude) = Tools.GetAddressCoordinates(vol.Address);

        const double tolerance = 0.0001;
        bool isLatitudeMatch = Math.Abs(vol.Latitude.Value - expectedLatitude) < tolerance;
        bool isLongitudeMatch = Math.Abs(vol.Longitude.Value - expectedLongitude) < tolerance;

        return isLatitudeMatch && isLongitudeMatch;
    }

    /// <summary>
    /// function that checks if the coordinates of a call match the coordinates based on his address. 
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    public static bool CheckAddressCall(BO.Call c)
    {
        if (string.IsNullOrEmpty(c.Address))
        {
            throw new Exception("Address is null or empty.");
        }
        var (expectedLatitude, expectedLongitude) = Tools.GetAddressCoordinates(c.Address);
        const double tolerance = 0.0001;

        bool isLatitudeMatch = Math.Abs(c.Latitude.GetValueOrDefault() - expectedLatitude) < tolerance;
        bool isLongitudeMatch = Math.Abs(c.Longitude.GetValueOrDefault() - expectedLongitude) < tolerance;

        return isLatitudeMatch && isLongitudeMatch;
    }

    /// <summary>
    ///  function to calculate the distance between two addresses
    ///  we use Haversine formula
    /// </summary>
    public static double CalculateDistanceBetweenAddresses(string address1, string address2)
    {
        var (latitude1, longitude1) = GetAddressCoordinates(address1);
        var (latitude2, longitude2) = GetAddressCoordinates(address2);

        const double EarthRadiusKm = 6371.0;

        double latitude1Rad = DegreesToRadians(latitude1);
        double longitude1Rad = DegreesToRadians(longitude1);
        double latitude2Rad = DegreesToRadians(latitude2);
        double longitude2Rad = DegreesToRadians(longitude2);

        double deltaLatitude = latitude2Rad - latitude1Rad;
        double deltaLongitude = longitude2Rad - longitude1Rad;

        double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                   Math.Cos(latitude1Rad) * Math.Cos(latitude2Rad) *
                   Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusKm * c;

        return distance;
    }

    /// <summary>
    /// function to transform Degrees To Radians
    /// </summary>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }


    /// <summary>
    ///class to calculate any type of distance between to entries
    /// </summary>
    public static class DistanceCalculator
    {
        public static double CalculateDistance(string address1, string address2, DO.DistanceType distanceType)
        {
            if (string.IsNullOrWhiteSpace(address1) || string.IsNullOrWhiteSpace(address2))
            {
                throw new ArgumentException("Addresses cannot be null or empty.");
            }

            switch (distanceType)
            {
                case DO.DistanceType.Aerial:
                    return CalculateAirDistance(address1, address2);

                case DO.DistanceType.Walking:
                    return CalculateWalkingDistance(address1, address2);

                case DO.DistanceType.Car:
                    return CalculateDrivingDistance(address1, address2);

                default:
                    throw new ArgumentOutOfRangeException(nameof(distanceType), "Invalid distance type.");
            }
        }

        /// <summary>
        /// calulate the air distance with the coordinates
        /// </summary>
        private static double CalculateAirDistance(string address1, string address2)
        {
            var (latitude1, longitude1) = GetAddressCoordinates(address1);
            var (latitude2, longitude2) = GetAddressCoordinates(address2);

            return CalculateDistanceBetweenCoordinates(latitude1, longitude1, latitude2, longitude2);
        }
        private static double CalculateWalkingDistance(string address1, string address2)
        {
            return CalculateTravelDistance(address1, address2, "foot");
        }

        private static double CalculateDrivingDistance(string address1, string address2)
        {
            return CalculateTravelDistance(address1, address2, "driving");
        }


        /// <summary>
        /// calculate driving and walking distance
        /// </summary>=

        private static double CalculateTravelDistance(string address1, string address2, string mode)
        {
            const string LocationIqApiKey = "pk.a0941b60144dc7fe0b85814d99ab3be7";
            const string BaseUrl = "https://us1.locationiq.com/v1/directions/";
            // מקבל את הקואורדינטות של הכתובות
            var (latitude1, longitude1) = GetAddressCoordinates(address1);
            var (latitude2, longitude2) = GetAddressCoordinates(address2);

            // בונה את ה-URL לבקשה עם סדר קואורדינטות נכון
            //string requestUrl = $"{BaseUrl}{mode}/{latitude1},{longitude1};{latitude2},{longitude2}?key={LocationIqApiKey}&overview=false";
            string requestUrl = $"{BaseUrl}driving/{latitude1},{longitude1};{latitude2},{longitude2}?key={LocationIqApiKey}&overview=false";

            // Console.WriteLine($"Request URL: {requestUrl}");

            using (var client = new HttpClient())
            {
                HttpResponseMessage response;

                try
                {
                    // שליחת בקשה
                    response = client.GetAsync(requestUrl).Result;

                    //  Console.WriteLine($"HTTP Status Code: {response.StatusCode} ({(int)response.StatusCode})");

                    // בדיקת סטטוס הקוד של התגובה
                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = response.Content.ReadAsStringAsync().Result;
                        //      Console.WriteLine($"Error Response: {response.StatusCode} - {response.ReasonPhrase}");
                        //      Console.WriteLine($"Error Content: {errorContent}");
                        throw new Exception($"Error fetching route data: {response.ReasonPhrase} (HTTP {response.StatusCode})\n{errorContent}");
                    }

                    // קריאה לתוכן התגובה
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    //      Console.WriteLine($"Response Content: {responseContent}");

                    // פירוש תגובת ה-JSON
                    var routeData = System.Text.Json.JsonSerializer.Deserialize<RouteResponse>(responseContent);

                    if (routeData == null || routeData.Routes == null || routeData.Routes.Length == 0)
                    {
                        throw new Exception("No route data found for the provided addresses.");
                    }

                    // חישוב המרחק בקילומטרים
                    return routeData.Routes[0].Distance / 1000.0;
                }
                catch (HttpRequestException)
                {
                    //         Console.WriteLine($"HTTP Request Exception: {httpEx.Message}");
                    throw;
                }
                catch (AggregateException)
                {
                    //        Console.WriteLine($"Aggregate Exception: {aggEx.InnerException?.Message ?? aggEx.Message}");
                    throw;
                }
                catch (Exception)
                {
                    //          Console.WriteLine($"General Exception: {ex.Message}");
                    throw;
                }
            }
        }

        public class RouteResponse
        {
            public Route[]? Routes { get; set; }
        }

        public class Route
        {
            public double Distance { get; set; }
        }
        /// <summary>
        /// calculate the distances between coordinates
        /// </summary>
        private static double CalculateDistanceBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double EarthRadiusKm = 6371.0;

            double latitude1Rad = DegreesToRadians(latitude1);
            double longitude1Rad = DegreesToRadians(longitude1);
            double latitude2Rad = DegreesToRadians(latitude2);
            double longitude2Rad = DegreesToRadians(longitude2);

            double deltaLatitude = latitude2Rad - latitude1Rad;
            double deltaLongitude = longitude2Rad - longitude1Rad;

            double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                       Math.Cos(latitude1Rad) * Math.Cos(latitude2Rad) *
                       Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }
        private class LocationIqDirectionsResponse
        {
            public Route[]? Routes { get; set; }
        }
        private class Routes
        {
            public double Distance { get; set; }
        }
        private class LocationIqResponse
        {
            public string? Lat { get; set; }
            public string? Lon { get; set; }
        }
    }

    #endregion Adress & Latitude & Longitude calculation

}
