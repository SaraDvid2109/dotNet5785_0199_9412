using BlApi;
using BO;
using DalApi;
using DO;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4

    public static void IntegrityCheck(BO.Volunteer volunteer)
    {
        if (!Enum.IsDefined(typeof(BO.Roles), volunteer.Role))
        {
            throw new BO.BlFormatException("Invalid Role format.");
        }

        if (!Enum.IsDefined(typeof(BO.DistanceType), volunteer.Type))
        {
            throw new BO.BlFormatException("Invalid DistanceType format.");
        }

        if (!string.IsNullOrEmpty(volunteer.Mail) && !volunteer.Mail.EndsWith("@gmail.com"))
        {
            throw new BO.BlFormatException("Invalid email format.");
        }

        if (volunteer.TotalCallsCanceled < 0 || volunteer.TotalCallsChosenHandleExpired < 0 || volunteer.TotalCallsHandled < 0 )
        {
            throw new BO.BlFormatException("Total calls values cannot be negative.");
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
        { 
            throw new BO.BlFormatException("The MaximumDistance must be 0–10 kilometer."); 
        }

        var coordinates = Tools.CheckAddressVolunteer;

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

    public static IEnumerable<DO.Assignment> GetAssignments(List<DO.Assignment> assignment, DO.Volunteer v, DO.EndType type)
    {
        var assignments = from item in assignment
                          where item.VolunteerId == v.Id && item.TypeEndOfTreatment == type
                          select item;
       return assignments;
    }

    public static DO.Volunteer ToDOVolunteer(BO.Volunteer volunteer)
    {
        return new DO.Volunteer(
                volunteer.Id,
                volunteer.Name ?? string.Empty,
                volunteer.Phone ?? string.Empty,
                volunteer.Mail ?? string.Empty,
                volunteer.Password ?? string.Empty,
                volunteer.Address ?? string.Empty,
                volunteer.Latitude ?? 0,
                volunteer.Longitude ?? 0,
                volunteer.Active,
                volunteer.MaximumDistance,
                (DO.Roles)volunteer.Role,
                (DO.DistanceType)volunteer.Type);

    }

    public static BO.Volunteer ToBOVolunteer(DO.Volunteer volunteer)
    {
        List<DO.Assignment> assignments = s_dal.Assignment.ReadAll().ToList();
        var idCall = assignments.FirstOrDefault(item => item.VolunteerId == volunteer.Id && item.TypeEndOfTreatment == null);
        var treated = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.Treated) ?? Enumerable.Empty<DO.Assignment>();
        var selfCancellation = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.SelfCancellation) ?? Enumerable.Empty<DO.Assignment>();
        var expiredCancellation = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.ExpiredCancellation) ?? Enumerable.Empty<DO.Assignment>();
        BO.CallInProgress? progress = null;
        if (idCall != null)
        {
            var call = s_dal.Call.Read(idCall.CallId);

            if (call != null)
            {
                progress = new BO.CallInProgress
                {
                    Id = idCall.Id,
                    CallId = call.Id,
                    CallType = (BO.CallType)call.CarTaypeToSend,
                    Destination = call.Description,
                    Address = call.Address,
                    OpenTime = call.OpenTime,
                    MaxTime = call.MaxTime ?? DateTime.MinValue,
                    EnterTime = idCall.EnterTime,
                    Distance = Helpers.Tools.CalculateDistanceBetweenAddresses(volunteer.Address ?? string.Empty, call.Address),
                    Status = Helpers.CallManager.Status(call.Id)
                };
            }

        }
        //return VolunteerManager.ToBOVolunteer(volunteer);
        return new BO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Mail = volunteer.Mail,
            Password = volunteer.Password,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = (BO.Roles)volunteer.Role,
            Active = volunteer.Active,
            MaximumDistance = volunteer.MaximumDistance,
            Type = (BO.DistanceType)volunteer.Type,
            Progress = progress,
            TotalCallsHandled = treated.Count(),
            TotalCallsCanceled = selfCancellation.Count(),
            TotalCallsChosenHandleExpired = expiredCancellation.Count(),
        };

    }


    //internal static BO.Volunteer ToDOVolunteer(DO.Volunteer? volunteer)
    //{
    //    return new BO.Volunteer
    //    {
    //        Id = volunteer.Id,
    //        Name = volunteer.Name,
    //        Phone = volunteer.Phone,
    //        Mail = volunteer.Mail,
    //        Password = volunteer.Password,
    //        Address = volunteer.Address,
    //        Latitude = volunteer.Latitude,
    //        Longitude = volunteer.Longitude,
    //        Role = (BO.Roles)volunteer.Role,
    //        Active = volunteer.Active,
    //        MaximumDistance = volunteer.MaximumDistance,
    //        Type = (BO.DistanceType)volunteer.Type,
    //        TotalCallsHandled = 0,
    //        TotalCallsCanceled = 0,
    //        TotalCallsChosenHandleExpired = 0,
    //        Progress = new BO.CallInProgress(),
    //    };

    //}


    // כל המתודות במחלקה יהיו internal static
}
