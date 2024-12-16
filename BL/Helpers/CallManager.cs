using BL.Helpers;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Net;
namespace Helpers;

/// <summary>
/// The <c>CallManager</c> class provides helper methods for managing and processing calls.
/// It interacts with the data access layer (DAL) to perform operations related to calls and assignments.
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// Performs an integrity check on the provided call, validating its fields and ensuring it meets business logic constraints.
    /// </summary>
    /// <param name="call">The call to be validated.</param>
    public static void IntegrityCheck(BO.Call call)
    {
        if (!Enum.IsDefined(typeof(BO.CallType), call.CarTypeToSend))
        {
            throw new BO.BlFormatException("Invalid CallType format.");
        }

        if (call.OpenTime > DateTime.Now)
        {
            throw new BO.BlFormatException("Invalid OpenTime format.");
        }

        if (call.MaxTime.HasValue)
        {
            TimeSpan timeDifference = call.MaxTime.Value - call.OpenTime;
            if (timeDifference.TotalMinutes < 5 || timeDifference.TotalMinutes > 30)
            {
                throw new BO.BlFormatException("Invalid MaxTime format.");
            }
        }
        else
        {
            throw new BO.BlFormatException("MaxTime cannot be null.");
        }

        if (!Enum.IsDefined(typeof(BO.CallStatus), call.Status))
        {
            throw new BO.BlFormatException("Invalid CallStatus format.");
        }

        var coordinate = Tools.CheckAddressCall;
        if (coordinate == null || call.Address == null)
        {
            throw new BO.BlFormatException("Invalid address.");
        }

        var coordinates = Helpers.Tools.GetAddressCoordinates(call.Address);
        if (call.Latitude != coordinates.Latitude)
        {
            throw new BO.BlFormatException("Invalid Latitude.");
        }
        if (call.Longitude != coordinates.Longitude)
        {
            throw new BO.BlFormatException("Invalid Longitude.");
        }
    }

    /// <summary>
    /// Returns the status of a specific call based on its assignments and current time.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>The status of the call as an enum value of type <see cref="BO.CallStatus"/>.</returns>
    public static BO.CallStatus Status(int callId) //stage 4
    {
        var call = s_dal.Call.Read(callId);
        var assignments = GetAssignmentCall(callId);
        if (assignments == null || !assignments.Any())
        {
            if (call!.MaxTime - ClockManager.Now <= s_dal.Config.RiskRange)
                return BO.CallStatus.OpenAtRisk;
            else
                return BO.CallStatus.Open;
        }
        else
        {
            var assignment = GetLastAssignment(assignments);
            if (assignment == null)
            {
                // Handle the null case appropriately
                throw new BO.BlNullReferenceException("Assignment does not exist");
            }
            if (assignment.TypeEndOfTreatment == null)
            {
                if (call!.MaxTime - ClockManager.Now <= s_dal.Config.RiskRange)
                    return BO.CallStatus.TreatmentOfRisk;
                else
                    return BO.CallStatus.Treatment;
            }
            else
            {
                if (assignment.TypeEndOfTreatment == DO.EndType.ExpiredCancellation)
                    return BO.CallStatus.Expired;
                else
                    return BO.CallStatus.Close;
            }
        }
    }

    /// <summary>
    /// Retrieves all assignments for a given call ID.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>A collection of <see cref="DO.Assignment"/> related to the call.</returns>
    public static IEnumerable<DO.Assignment>? GetAssignmentCall(int callId)
    {
        IEnumerable<DO.Assignment>? assignments = s_dal.Assignment.ReadAll();
        if (assignments != null)
            assignments = from assignment in assignments
                          where assignment.CallId == callId
                          select assignment;

        return assignments;
    }

    /// <summary>
    /// Retrieves the last assignment of a call from a list of assignments.
    /// </summary>
    /// <param name="assignments">The list of assignments to search through.</param>
    /// <returns>The last <see cref="DO.Assignment"/> or null if no assignment exists.</returns>
    public static DO.Assignment? GetLastAssignment(IEnumerable<DO.Assignment>? assignments)
    {
        if (assignments == null)
            return null;
        return assignments.OrderByDescending(a => a.TypeEndOfTreatment).FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the name of the last volunteer assigned to a specific call.
    /// </summary>
    /// <param name="call">The call for which the volunteer is being queried.</param>
    /// <returns>The name of the last volunteer assigned to the call.</returns>
    public static string getLastVolunteer(DO.Call call)
    {
        var assignments = AssignmentManager.findAssignment(call.Id);
        var last = assignments.MaxBy(a => a.EnterTime);
        if (last == null)
        {
            throw new BO.BlNullPropertyException("No assignments found for the given call.");
        }
        var volunteer = s_dal.Volunteer.Read(last.VolunteerId) ?? throw new BO.BlNullPropertyException("No assignments found for the given call.");
        return volunteer.Name;
    }

    /// <summary>
    /// Filters a collection of calls by the specified <see cref="BO.CallType"/>.
    /// </summary>
    /// <param name="toFilter">The collection of calls to filter.</param>
    /// <param name="type">The <see cref="BO.CallType"/> to filter by.</param>
    /// <returns>A filtered collection of calls.</returns>
    public static IEnumerable<DO.Call> Filter(IEnumerable<DO.Call> toFilter, BO.CallType? type)
    {
        if (type != null)
        {
            toFilter = from call in toFilter
                       where call.CarTypeToSend == (DO.CallType)type
                       select call;
        }
        return toFilter;
    }

    /// <summary>
    /// Filters a collection of <see cref="BO.CallInList"/> objects based on the provided filter field and value.
    /// </summary>
    /// <param name="toFilter">The collection of <see cref="BO.CallInList"/> to filter.</param>
    /// <param name="obj">The value to filter by.</param>
    /// <param name="filter">The field to filter by.</param>
    /// <returns>A filtered collection of <see cref="BO.CallInList"/>.</returns>
    public static IEnumerable<BO.CallInList> FilterCallInList(IEnumerable<BO.CallInList> toFilter, object? obj, BO.CallInListFields? filter)
    {
        if (filter == null || obj == null)
            return toFilter;

        var calls = toFilter;
        string? value = obj.ToString();

        switch (filter)
        {
            case BO.CallInListFields.Id:
                if (!int.TryParse(value, out int id))
                    throw new BO.BlInvalidData("Invalid number format");
                calls = from call in calls
                        where call.Id == id
                        select call;
                break;
            case BO.CallInListFields.CallId:
                if (!int.TryParse(value, out int callId))
                    throw new BO.BlInvalidData("Invalid number format");
                calls = from call in calls
                        where call.CallId == callId
                        select call;
                break;
            case BO.CallInListFields.CallType:
                if (!Enum.TryParse(typeof(BO.CallType), value, true, out var type))
                    throw new BO.BlInvalidData("Invalid CallType value");
                calls = from call in calls
                        where call.CallType == (BO.CallType)type
                        select call;
                break;
            case BO.CallInListFields.OpenTime:
                if (!DateTime.TryParse(value, out DateTime openTime))
                    throw new BO.BlInvalidData("Invalid open time format");
                calls = from call in calls
                        where call.OpenTime == openTime
                        select call;
                break;
            case BO.CallInListFields.TimeLeftToFinish:
                if (!TimeSpan.TryParse(value, out TimeSpan timeLeft))
                    throw new BO.BlInvalidData("Invalid time left format");
                calls = from call in calls
                        where call.TimeLeftToFinish == timeLeft
                        select call;
                break;
            case BO.CallInListFields.LastVolunteer:
                calls = from call in calls
                        where call.LastVolunteer == value
                        select call;
                break;
            case BO.CallInListFields.TreatmentTimeLeft:
                if (!TimeSpan.TryParse(value, out TimeSpan timeOfTreatment))
                    throw new BO.BlInvalidData("Invalid time of treatment format");
                calls = from call in calls
                        where call.TreatmentTimeLeft == timeOfTreatment
                        select call;
                break;
            case BO.CallInListFields.Status:
                if (!Enum.TryParse(typeof(BO.CallStatus), value, true, out var callStatus))
                    throw new BO.BlInvalidData($"Invalid CallStatus value");
                calls = from call in calls
                        where call.Status == (BO.CallStatus)callStatus
                        select call;
                break;
            case BO.CallInListFields.TotalAssignments:
                if (!int.TryParse(value, out int numAssignments))
                    throw new BO.BlInvalidData("Invalid number format");
                calls = from call in calls
                        where call.TotalAssignments == numAssignments
                        select call;
                break;
            default:
                calls = toFilter;
                break;
        }

        return calls;
    }

    /// <summary>
    /// Converts a <see cref="DO.Call"/> to a <see cref="BO.ClosedCallInList"/> object.
    /// </summary>
    /// <param name="call">The call to be converted.</param>
    /// <returns>A <see cref="BO.ClosedCallInList"/> object representing the call.</returns>
    public static BO.ClosedCallInList ToBOClosedCall(DO.Call call)
    {
        var assignments = s_dal.Assignment.ReadAll();

        var assignment = s_dal.Assignment.Read(a => a.CallId == call.Id);

        if (assignment == null)
        {
            throw new BO.BlNullPropertyException("No assignment found for the given call.");
        }

        return new BO.ClosedCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CarTypeToSend,
            Address = call.Address,
            OpenTime = call.OpenTime,
            EnterTime = assignment.EnterTime,
            EndTime = assignment.EndTime,
            TypeEndOfTreatment = (BO.EndType?)assignment.TypeEndOfTreatment
        };
    }

    /// <summary>
    /// Converts a <see cref="DO.Call"/> and a <see cref="DO.Volunteer"/> to a <see cref="BO.OpenCallInList"/> object.
    /// </summary>
    /// <param name="call">The call to be converted.</param>
    /// <param name="volunteer">The volunteer associated with the call.</param>
    public static BO.OpenCallInList ToBOOpenCall(DO.Call call, DO.Volunteer volunteer)
    {
        if (call.Address == null)
        {
            throw new BO.BlNullPropertyException("Call address cannot be null.");
        }

        if (volunteer.Address == null)
        {
            throw new BO.BlNullPropertyException("Volunteer address cannot be null.");
        }
        return new BO.OpenCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CarTypeToSend,
            Destination = call.Description,
            Address = call.Address,
            OpenTime = call.OpenTime,
            MaxTime=call.MaxTime,
            Distance= Tools.DistanceCalculator.CalculateDistance(call.Address, volunteer.Address, volunteer.Type)
        };
    }

    /// <summary>
    /// Converts a <see cref="BO.Call"/> object to a <see cref="DO.Call"/> object.
    /// </summary>
    /// <param name="call">The <see cref="BO.Call"/> object to be converted.</param>
    /// <returns>A <see cref="DO.Call"/> object representing the call.</returns>
    public static DO.Call ToDOCall(BO.Call call)
    {
        return  new DO.Call(
                call.Id,
                call.Description,
                call.Address ?? throw new BO.BlNullPropertyException("No address entered!"),
                call.Latitude,
                call.Longitude,
                call.OpenTime,
                call.MaxTime,
                (DO.CallType)call.CarTypeToSend);
    }

    /// <summary>
    /// Method to perform periodic updates on students based on the clock update.
    /// </summary>
    /// <param name="oldClock">The previous clock value.</param>
    /// <param name="newClock">The updated clock value.</param>
    /// 
    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        var Calls = s_dal.Call.ReadAll();
        var noAssignments = from Call in Calls
                            let assignments = GetAssignmentCall(Call.Id)
                            where ClockManager.Now >= Call.MaxTime && assignments == null
                            select Call;
        var haveAssignments = from Call in Calls
                              let assignments = GetAssignmentCall(Call.Id)
                              where ClockManager.Now >= Call.MaxTime && assignments != null
                              let lastAssignment = GetLastAssignment(assignments)
                              where lastAssignment.EndTime == null
                              select Call;

        foreach (var Call in noAssignments)
        {
            s_dal.Assignment.Create(new Assignment
            {
                CallId = Call.Id,
                EnterTime = DateTime.Now,
                EndTime = DateTime.Now,
                TypeEndOfTreatment = DO.EndType.ExpiredCancellation
            });
        }

        var convertHaveAssignments = haveAssignments.Select(c => GetLastAssignment(GetAssignmentCall(c.Id)!)!);
        var updatedHaveAssignments = convertHaveAssignments.Select(a => a = a with { EndTime = ClockManager.Now, TypeEndOfTreatment = DO.EndType.ExpiredCancellation });
        foreach (var assignment in updatedHaveAssignments)
        {
            s_dal.Assignment.Update(assignment);
        }
    }

    /// <summary>
    /// Converts a <see cref="DO.Call"/> and its assignments to a <see cref="BO.CallInList"/> object.
    /// </summary>
    /// <param name="call">The <see cref="DO.Call"/> to be converted.</param>
    /// <param name="assignments">The assignments related to the call.</param>
    /// <returns>A <see cref="BO.CallInList"/> object representing the call.</returns>
    public static BO.CallInList ConvertFromDOToCallInList(DO.Call call, IEnumerable<DO.Assignment> assignments)
    {
        var assignmentOfCall = assignments.FirstOrDefault(a => a.CallId == call.Id);
        if (assignmentOfCall == null)
        {
            return new BO.CallInList
            {
                Id = null,
                CallId = call.Id,
                CallType = (BO.CallType)call.CarTypeToSend,
                OpenTime = call.OpenTime,
                TimeLeftToFinish = null,
                LastVolunteer = null,
                TreatmentTimeLeft = null,
                Status = Status(call.Id),
                TotalAssignments = 0
            };
        }

        var volunteer = s_dal.Volunteer.Read(assignmentOfCall.VolunteerId);
        TimeSpan? time = null;
        if (assignmentOfCall.EndTime != null)
        {
            time = assignmentOfCall.EndTime - assignmentOfCall.EnterTime;
        }
        return new BO.CallInList
        {
            Id = assignmentOfCall.Id,
            CallId = call.Id,
            CallType = (BO.CallType)call.CarTypeToSend,
            OpenTime = call.OpenTime,
            TimeLeftToFinish = ClockManager.Now - call.MaxTime> TimeSpan.Zero ? ClockManager.Now - call.MaxTime: TimeSpan.Zero,
            LastVolunteer = volunteer!.Name,
            TreatmentTimeLeft = time,
            Status = Status(call.Id),
            TotalAssignments = assignments.Count()
        };
    }

    // כל המתודות במחלקה יהיו internal static
}
