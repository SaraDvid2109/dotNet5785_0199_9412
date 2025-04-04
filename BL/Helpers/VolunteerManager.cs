﻿using BlApi;
using BlImplementation;
using BO;
using DalApi;
using DO;
using System.Globalization;
namespace Helpers;

/// <summary>
/// The VolunteerManager class handles operations related to volunteers, 
/// including data validation checks and object conversion between BO and DO models.
/// </summary>
internal static class VolunteerManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Performs integrity checks on a volunteer's data. Throws exceptions if any data is invalid.
    /// </summary>
    /// <param name="volunteer">The volunteer object to validate.</param>
    /// <exception cref="BO.BlFormatException">Thrown when any of the volunteer's data is invalid.</exception>
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

        if (volunteer.TotalCallsCanceled < 0 || volunteer.TotalCallsChosenHandleExpired < 0 || volunteer.TotalCallsHandled < 0)
        {
            throw new BO.BlFormatException("Total calls values cannot be negative.");
        }

        if (!IsValidIsraeliPhoneNumber(volunteer.Phone))
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

    }

    /// <summary>
    /// Validates if the phone number is a valid Israeli phone number.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <returns>True if the phone number is valid, otherwise false.</returns>
    public static bool IsValidIsraeliPhoneNumber(string? phoneNumber)
    {
        // If the phone number is null, it is invalid
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return true;
        }

        
        phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

        // Check that the length is 10 digits
        if (phoneNumber.Length != 10)
        {
            return false;
        }

        // Check that the number contains only digits
        if (!phoneNumber.All(char.IsDigit))
        {
            return false;
        }

        // Checking for valid prefixes
        string[] validPrefixes = { "050", "051", "052", "053", "054", "055", "056", "057", "058", "059" };
        string prefix = phoneNumber.Substring(0, 3);

        if (!validPrefixes.Contains(prefix))
        {
            return false;
        }

        return true; // The phone number is valid.
    }

    /// <summary>
    /// Validates if the ID is a valid Israeli ID number.
    /// </summary>
    /// <param name="id">The ID number to validate.</param>
    /// <returns>True if the ID is valid, otherwise false.</returns>
    public static bool IsValidIsraeliID(string id)
    {
        // Initial check: Does the input consist of only numbers and has a valid length?
        if (string.IsNullOrWhiteSpace(id) || id.Length < 5 || id.Length > 9 || !long.TryParse(id, out _))
        {
            return false;
        }

        // Complete the number to 9 digits by adding zeros to the left
        id = id.PadLeft(9, '0');

        int sum = 0;

        for (int i = 0; i < id.Length; i++)
        {
            int digit = id[i] - '0'; // Convert the character to a digit
            int value = digit * (i % 2 == 0 ? 1 : 2); // Alternately multiply by 1 or 2
            if (value > 9)
            {
                value -= 9;// If the value is greater than 9, subtract 9 (add the digits of the number)
            }
            sum += value;
        }
        return sum % 10 == 0;
    }

    /// <summary>
    /// Gets the assignments of a volunteer based on the treatment end type.
    /// </summary>
    /// <param name="assignment">The list of assignments to check.</param>
    /// <param name="v">The volunteer whose assignments are being checked.</param>
    /// <param name="type">The end type of treatment.</param>
    /// <returns>Filtered list of assignments based on the volunteer ID and end type.</returns>
    public static IEnumerable<DO.Assignment> GetAssignments(List<DO.Assignment> assignment, DO.Volunteer v, DO.EndType type)
    {
        var assignments = from item in assignment
                          where item.VolunteerId == v.Id && item.TypeEndOfTreatment == type
                          select item;
        return assignments;
    }

    /// <summary>
    /// Converts a Business Object (BO) volunteer to a Data Object (DO) volunteer.
    /// </summary>
    /// <param name="volunteer">The BO.Volunteer object to convert.</param>
    /// <returns>A DO.Volunteer object.</returns>
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

    /// <summary>
    /// Converts a Data Object (DO) volunteer to a Business Object (BO) volunteer.
    /// Also calculates the volunteer's call statistics (handled, canceled, expired).
    /// </summary>
    /// <param name="volunteer">The DO.Volunteer object to convert.</param>
    /// <returns>A BO.Volunteer object with additional call statistics.</returns>
    public static BO.Volunteer ToBOVolunteer(DO.Volunteer volunteer)
    {
        List<DO.Assignment> assignments;
        DO.Call? call;
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll().ToList();
        var idCall = assignments.LastOrDefault(item => item.VolunteerId == volunteer.Id && (item.TypeEndOfTreatment == DO.EndType.Treated));
        var treated = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.Treated) ?? Enumerable.Empty<DO.Assignment>();
        var selfCancellation = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.SelfCancellation) ?? Enumerable.Empty<DO.Assignment>();
        var expiredCancellation = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.ExpiredCancellation) ?? Enumerable.Empty<DO.Assignment>();
        BO.CallInProgress? progress = null;
        if (idCall != null)
        {
            lock (AdminManager.BlMutex) //stage 7
                call = s_dal.Call.Read(idCall.CallId);

            if (call != null && !string.IsNullOrEmpty(volunteer.Address))
            {
                progress = new BO.CallInProgress
                {
                    Id = idCall.Id,
                    CallId = call.Id,
                    CallType = (BO.CallType)call.CarTypeToSend,
                    Destination = call.Description,
                    Address = call.Address,
                    OpenTime = call.OpenTime,
                    MaxTime = call.MaxTime ?? DateTime.MinValue,
                    EnterTime = idCall.EnterTime,
                    Distance = volunteer.Type == DO.DistanceType.Aerial
                     ? Tools.DistanceCalculator.CalculateAirDistance(call.Latitude,call.Longitude, volunteer.Latitude,volunteer.Longitude)
                     : Tools.DistanceCalculator.CalculateDistanceOSRMSync(
                         new Tools.Location { Lat = call.Latitude, Lon = call.Longitude },
                         new Tools.Location { Lat = volunteer.Latitude, Lon = volunteer.Longitude },
                         (DO.DistanceType)volunteer.Type),
                    Status = Helpers.CallManager.Status(call.Id)
                };
            }


        }
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

    /// <summary>
    /// Returns the details of a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>The volunteer details.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if no volunteer is found with the given ID.</exception>
    public static BO.Volunteer GetVolunteerDetails(int id)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = s_dal.Volunteer.Read(id);
        if (volunteer == null)
        {
            throw new BO.BlDoesNotExistException("There is no volunteer with this ID.");
        }
        BO.Volunteer BoVolunteer = VolunteerManager.ToBOVolunteer(volunteer);
        return BoVolunteer;

    }
    /// <summary>
    /// Simulates the process of volunteer call registration and assignment.
    /// Volunteers who are not currently assigned to a call may randomly select an open call.
    /// If a volunteer is already assigned to a call, their treatment progress is checked,
    /// and the call may be completed or canceled based on probability.
    /// </summary>
    /// <exception cref="BO.BLTemporaryNotAvailableException">Thrown when the system is unavailable to process a call selection.</exception>
    internal static void SimulateCallRegistration() //stage 7
    {
        List<DO.Volunteer> doVolunteerList;
        IEnumerable<DO.Assignment> assignmants;
        lock (AdminManager.BlMutex) //stage 7
            doVolunteerList = s_dal.Volunteer.ReadAll(v => v.Active == true).ToList();
        foreach (var doVolunteer in doVolunteerList)
        {
            lock (AdminManager.BlMutex) //stage 7
                assignmants = s_dal.Assignment.ReadAll(a => a.EndTime == null && a.VolunteerId == doVolunteer.Id);
            DO.Assignment? assignment = assignmants.FirstOrDefault();
            if (assignment == null)
            {
                var openCalls = CallManager.openCallsForSelectionByVolunteer(doVolunteer.Id, null, null).ToList();
                if (openCalls.Any())
                {
                    var random = new Random();
                    int randomIndex = random.Next(openCalls.Count());
                    try
                    {
                        if (Random.Shared.NextDouble() <= 0.2)//20 percent probability
                            CallManager.ChooseCallForHandling(doVolunteer.Id, openCalls[randomIndex].Id);
                    }
                    catch (BO.BLTemporaryNotAvailableException ex)
                    {
                        throw new BO.BLTemporaryNotAvailableException(ex.Message);
                    }
                }
            }
            else
            {
                BO.Volunteer boVolunteer = GetVolunteerDetails(doVolunteer.Id);
                double minutes;
                if (boVolunteer.Progress != null)
                {
                    minutes = boVolunteer.Progress.Distance + 20;

                    if (AdminManager.Now >= assignment.EnterTime.AddMinutes(minutes))//If enough time has passed since the start of treatment
                    {
                        CallManager.UpdateEndOfTreatmentCall(doVolunteer.Id, assignment.Id);
                    }
                    else
                    {
                        if (Random.Shared.NextDouble() <= 0.1)//10 percent probability
                            CallManager.CancelCallHandling(doVolunteer.Id, assignment.Id);
                    }
                }
            }
        }
    }

}
