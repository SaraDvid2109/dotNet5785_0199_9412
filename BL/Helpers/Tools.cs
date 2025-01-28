using BlApi;
using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers;

/// <summary>
/// A utility class containing helper functions for performing common tasks on objects, geographical addresses, and data.
/// All methods in this class are static, meaning they can be called without needing to instantiate the class.
/// </summary>
internal static class Tools 
{
    /// <summary>
    /// Converts an object's properties to a string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="t">The object to convert.</param>
    /// <returns>A string representation of the object's properties and their values.</returns>
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
            throw new BlNullPropertyException("Object cannot be null.");

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

    /// <summary>
    /// Gets the geographic coordinates (latitude and longitude) for a given address.
    /// </summary>
    public static async Task<(double Latitude, double Longitude)> GetAddressCoordinates(string? address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new BlNullPropertyException("Address cannot be null or empty.");
        }

        const string LocationIqApiKey = "pk.a0941b60144dc7fe0b85814d99ab3be7";
        const string BaseUrl = "https://us1.locationiq.com/v1/search.php";
        const int MaxRetries = 5;
        const int InitialDelayMilliseconds = 1000;

        string requestUrl = $"{BaseUrl}?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";

        using (var client = new HttpClient())
        {
            for (int retry = 0; retry < MaxRetries; retry++)
            {
                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync(requestUrl);
                }
                catch (Exception ex)
                {
                    throw new BlFileLoadCreateException("Error sending request to LocationIQ API.", ex);
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var locationData = System.Text.Json.JsonSerializer.Deserialize<List<LocationIqResponse>>(responseContent);

                    if (locationData == null || locationData.Count == 0)
                    {
                        throw new BlDoesNotExistException("No coordinates found for the provided address.");
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
                        throw new BlFormatException($"Invalid coordinate data. Latitude valid: {isLatValid}, Longitude valid: {isLonValid}");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // Wait before retrying
                    int delay = InitialDelayMilliseconds * (int)Math.Pow(2, retry);
                    await Task.Delay(delay);
                }
                else
                {
                    throw new BlFileLoadCreateException($"Error fetching data from LocationIQ: {response.ReasonPhrase}");
                }
            }

            throw new BlFileLoadCreateException("Exceeded maximum retry attempts for LocationIQ API.");
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

     public class Location
        {
            public double? Lat { get; set; }
            public double? Lon { get; set; }
        }

    /// <summary>
    /// function that checks if the coordinates of a volunteer match the coordinates based on his address. 
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    //public static bool CheckAddressVolunteer(BO.Volunteer vol)
    //{
    //    if (vol.Latitude == null || vol.Longitude == null)
    //    {
    //        throw new BlNullPropertyException("Latitude or Longitude is null.");
    //    }

    //    if (string.IsNullOrEmpty(vol.Address))
    //    {
    //        throw new BlNullPropertyException("Address is null or empty.");
    //    }
    //    var (expectedLatitude, expectedLongitude) = Tools.GetAddressCoordinates(vol.Address);

    //    const double tolerance = 0.0001;
    //    bool isLatitudeMatch = Math.Abs(vol.Latitude.Value - expectedLatitude) < tolerance;
    //    bool isLongitudeMatch = Math.Abs(vol.Longitude.Value - expectedLongitude) < tolerance;

    //    return isLatitudeMatch && isLongitudeMatch;
    //}

    /// <summary>
    /// function that checks if the coordinates of a call match the coordinates based on his address. 
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    //public static bool CheckAddressCall(BO.Call c)
    //{
    //    if (string.IsNullOrEmpty(c.Address))
    //    {
    //        throw new BlNullPropertyException("Address is null or empty.");
    //    }
    //    var (expectedLatitude, expectedLongitude) = Tools.GetAddressCoordinates(c.Address);
    //    const double tolerance = 0.0001;

    //    bool isLatitudeMatch = Math.Abs(c.Latitude.GetValueOrDefault() - expectedLatitude) < tolerance;
    //    bool isLongitudeMatch = Math.Abs(c.Longitude.GetValueOrDefault() - expectedLongitude) < tolerance;

    //    return isLatitudeMatch && isLongitudeMatch;
    //}

    /// <summary>
    ///  function to calculate the distance between two addresses
    ///  we use Haversine formula
    /// </summary>
    //public static double CalculateDistanceBetweenAddresses(string address1, string address2)
    //{
    //    var (latitude1, longitude1) = GetAddressCoordinates(address1);
    //    var (latitude2, longitude2) = GetAddressCoordinates(address2);

    //    const double EarthRadiusKm = 6371.0;

    //    double latitude1Rad = DegreesToRadians(latitude1);
    //    double longitude1Rad = DegreesToRadians(longitude1);
    //    double latitude2Rad = DegreesToRadians(latitude2);
    //    double longitude2Rad = DegreesToRadians(longitude2);

    //    double deltaLatitude = latitude2Rad - latitude1Rad;
    //    double deltaLongitude = longitude2Rad - longitude1Rad;

    //    double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
    //               Math.Cos(latitude1Rad) * Math.Cos(latitude2Rad) *
    //               Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

    //    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    //    double distance = EarthRadiusKm * c;

    //    return distance;
    //}

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
       

        public static double CalculateDistanceOSRMSync(Location locSource, Location locDest, DO.DistanceType Mode) //(double lat1, double lon1, double lat2, double lon2) //(string origin, string destination)
        {
            string mode;
            if (Mode == DO.DistanceType.Walking)
                mode = "walking";
            else
                mode = "driving";

            string requestUrl = $"http://router.project-osrm.org/route/v1/{mode}/{locSource.Lon},{locSource.Lat};{locDest.Lon},{locDest.Lat}?overview=false";

            using HttpClient client = new();
            HttpResponseMessage response = client.GetAsync(requestUrl).Result;
            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                using JsonDocument doc = JsonDocument.Parse(responseContent);
                JsonElement root = doc.RootElement;

                if (root.GetProperty("code").GetString() == "Ok")
                {
                    JsonElement route = root.GetProperty("routes")[0];
                    double distance = route.GetProperty("distance").GetDouble(); // במטרים
                    return distance / 1000; // המרחק בקילומטרים
                }
                else
                {
                    throw new BO.BlDistance($"Error while calc {Mode} distance: {root.GetProperty("code").GetString()}");
                }
            }
            else
            {
                throw new BO.BlDistance($"Error while calc {Mode} distance");
            }

        }

        /// <summary>
        /// calulate the air distance with the coordinates
        /// </summary>
        public static double CalculateAirDistance(double? latitude1, double? longitude1, double? latitude2, double? longitude2)
        {
            //var (latitude1, longitude1) = GetAddressCoordinates(address1);
            //var (latitude2, longitude2) = GetAddressCoordinates(address2);
            if (latitude1 != null && longitude1 != null && latitude2 != null && longitude2 != null)
            {
                return CalculateDistanceBetweenCoordinates(latitude1.Value, longitude1.Value, latitude2.Value, longitude2.Value);
            }
            return 0;
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
