using DalApi;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    public static void IntegrityCheck(BO.Volunteer volunteer)
    {
        // בדיקת פורמט הערכים
        if (string.IsNullOrEmpty(volunteer.Mail) || !volunteer.Mail.EndsWith("@gmail.com"))
        {
            throw new ArgumentException("Invalid email format.");
        }

        if (volunteer.Phone?.Length != 10 || !volunteer.Phone.All(char.IsDigit))
        {
            throw new ArgumentException("Invalid phone number format.");
        }
        if (!IsValidIsraeliID(volunteer.Id.ToString()))
        {
            throw new ArgumentException("Invalid ID format.");
        }

        // בדיקת תקינות לוגית
        var coordinates = Tools.CheckAddressVolunteer;
        if (coordinates == null)
        {
            throw new ArgumentException("Invalid address.");
        }

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

    /// <summary>
    /// Method to perform periodic updates on students based on the clock update.
    /// </summary>
    /// <param name="oldClock">The previous clock value.</param>
    /// <param name="newClock">The updated clock value.</param>
    internal static void PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
    {
        // Implement the logic for periodic updates on students.
        // For example, update properties that are affected by the clock update.
        // (students become not active after 5 years, etc.)

        // Example implementation:
        Console.WriteLine($"Updating students from {oldClock} to {newClock}");
        // Add your logic here...
    }
    // כל המתודות במחלקה יהיו internal static
}
