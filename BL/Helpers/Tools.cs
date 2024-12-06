using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

internal static class Tools 
{
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return string.Empty;

        var properties = typeof(T).GetProperties();
        var sb = new StringBuilder();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(t, null);
            if (value is System.Collections.IEnumerable enumerable && !(value is string))
            {
                sb.AppendLine($"{prop.Name}: [");
                foreach (var item in enumerable)
                {
                    sb.AppendLine(item.ToStringProperty());
                }
                sb.AppendLine("]");
            }
            else
            {
                sb.AppendLine($"{prop.Name}: {value}");
            }
        }

        return sb.ToString();
    }

    public static void IntegrityCheck(BO.Volunteer volunteer)
    {
        //// בדיקת פורמט הערכים
        //if (!volunteer.Mail.EndsWith("@gmail.com"))
        //{
        //    throw new ArgumentException("Invalid email format.");
        //}

        //if (volunteer.Phone?.Length != 10 || !volunteer.Phone.All(char.IsDigit))
        //{
        //    throw new ArgumentException("Invalid phone number format.");
        //}
        //string idString = volunteer.Id.ToString();
        //if (idString.Length != 9)
        //{
        //    throw new ArgumentException("Invalid ID format.");
        //}

        //// בדיקת תקינות לוגית
        //var coordinates = GetCoordinates(volunteer.Address ?? string.Empty);
        //if (coordinates == null)
        //{
        //    throw new ArgumentException("Invalid address.");
        //}
        //volunteer.Latitude = coordinates.Latitude;
        //volunteer.Longitude = coordinates.Longitude;

    }

    public static void IntegrityCheck(BO.Call volunteer)
    {
        //// בדיקת פורמט הערכים
        //if (!volunteer.Mail.EndsWith("@gmail.com"))
        //{
        //    throw new ArgumentException("Invalid email format.");
        //}

        //if (volunteer.Phone?.Length != 10 || !volunteer.Phone.All(char.IsDigit))
        //{
        //    throw new ArgumentException("Invalid phone number format.");
        //}
        //string idString = volunteer.Id.ToString();
        //if (idString.Length != 9)
        //{
        //    throw new ArgumentException("Invalid ID format.");
        //}

        //// בדיקת תקינות לוגית
        //var coordinates = GetCoordinates(volunteer.Address ?? string.Empty);
        //if (coordinates == null)
        //{
        //    throw new ArgumentException("Invalid address.");
        //}
        //volunteer.Latitude = coordinates.Latitude;
        //volunteer.Longitude = coordinates.Longitude;

    }


}
