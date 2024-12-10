using BO;
using DalApi;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    public static void IntegrityCheck(BO.Volunteer volunteer)
    {
        // בדיקת פורמט הערכים
        if (!string.IsNullOrEmpty(volunteer.Mail) && !volunteer.Mail.EndsWith("@gmail.com"))
        {
            throw new BO.BlFormatException("Invalid email format.");
        }

        if (!IsValidIsraeliPhoneNumber (volunteer.Phone))
        {
            throw new BO.BlFormatException("Invalid phone number format.");
        }
        if (!IsValidIsraeliID(volunteer.Id.ToString()))
        {
            throw new BO.BlFormatException("Invalid ID format.");
        }

        if (!string.IsNullOrEmpty(volunteer.Password))
        {
            if (volunteer.Password.Length < 8 || volunteer.Password.Length > 12)
                throw new BO.BlFormatException("The password must be 8–12 characters long.");
            //chat GPT-How to check if a password contains all the required characters
            bool hasLower = volunteer.Password.Any(char.IsLower); //Lowercase letters
            bool hasUpper = volunteer.Password.Any(char.IsUpper); //Uppercase letters
            bool hasDigit = volunteer.Password.Any(char.IsDigit); //Numbers
            bool hasSpecial = volunteer.Password.Any(ch => "!@#$%^&*(),.?\"{}|<>".Contains(ch)); //Special characters
            bool isStrongPassword = hasLower && hasUpper && hasDigit && hasSpecial;
            if (!isStrongPassword)
                throw new BO.BlFormatException("The password must include letters, numbers, and special characters.");
        }
        if (volunteer.MaximumDistance < 0 || volunteer.MaximumDistance > 10)
                throw new BO.BlFormatException("The MaximumDistance must be 0–10 kilometer.");


        // בדיקת תקינות לוגית
        var coordinates = Tools.CheckAddressVolunteer;
        if (coordinates == null)
        {
            throw new BO.BlFormatException("Invalid address.");
        }
        if (!string.IsNullOrEmpty(volunteer.Address))
        {
            var coordinate = Helpers.Tools.GetAddressCoordinates(volunteer.Address);
            if (volunteer.Latitude != coordinate.Latitude)
            {
                throw new BO.BlFormatException("Invalid Latitude.");
            }
            if (volunteer.Longitude != coordinate.Longitude)
            {
                throw new BO.BlFormatException("Invalid Longitude.");
            }
        }
        

    }

    public static bool IsValidIsraeliPhoneNumber(string? phoneNumber)
    {
        // אם מספר הטלפון הוא null, הוא אינו תקין
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return true;
        }

        // הסרה של רווחים ולוכסן 
        phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

        // בדיקת אורך: מספר טלפון ישראלי הוא 10 ספרות
        if (phoneNumber.Length != 10)
        {
            return false;
        }

        // בדיקה שהמספר מכיל רק ספרות
        if (!phoneNumber.All(char.IsDigit))
        {
            return false;
        }

        // בדיקת קידומות חוקיות
        string[] validPrefixes = { "050", "051", "052", "053", "054", "055", "056", "057", "058", "059" };
        string prefix = phoneNumber.Substring(0, 3);

        if (!validPrefixes.Contains(prefix))
        {
            return false;
        }

        return true; // מספר הטלפון תקין
    }


    public static bool IsValidIsraeliID(string id)
    {
        // בדיקה ראשונית: האם הקלט מורכב מספרות בלבד ובעל אורך חוקי
        if (string.IsNullOrWhiteSpace(id) || id.Length < 5 || id.Length > 9 || !long.TryParse(id, out _))
        {
            return false;
        }

        // השלמת המספר ל-9 ספרות על ידי הוספת אפסים משמאל
        id = id.PadLeft(9, '0');

        int sum = 0;

        for (int i = 0; i < id.Length; i++)
        {
            int digit = id[i] - '0'; // המרת התו לספרה
            int value = digit * (i % 2 == 0 ? 1 : 2); // הכפלה לסירוגין ב-1 או ב-2
            if (value > 9)
            {
                value -= 9; // אם הערך גדול מ-9, מפחיתים 9 (חיבור ספרות המספר)
            }
            sum += value;
        }
        return sum % 10 == 0;

    }

    // כל המתודות במחלקה יהיו internal static
}
