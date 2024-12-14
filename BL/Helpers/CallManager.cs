using BL.Helpers;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Net;
namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    public static void IntegrityCheck(BO.Call call)
    {
        if (!Enum.IsDefined(typeof(BO.CallType), call.CarTaypeToSend))
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

    public static BO.CallStatus Status(int callId) //stage 4
    {
        var call = s_dal.Call.Read(callId);
        if (call == null)
            throw new BO.BlDoesNotExistException($"Call with id {callId} does not exist");

        var assignments = GetAssignmentCall(callId);
        if (assignments == null)
        {
            if (call!.MaxTime - ClockManager.Now <= s_dal.Config.RiskRange)
                return BO.CallStatus.OpenAtRisk;
            else
                return BO.CallStatus.Open;
        }
        else
        {
            var assignment = GetLastAssignment(assignments);
            if (assignment!.TypeEndOfTreatment == null)
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

    public static IEnumerable<DO.Assignment>? GetAssignmentCall(int callId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments != null)
            assignments = from assignment in assignments
                          where assignment.CallId == callId
                          select assignment;
        return assignments;
    }

    public static DO.Assignment? GetLastAssignment(IEnumerable<DO.Assignment>? assignments)
    {
        if (assignments == null)
            return null;
        return assignments.OrderByDescending(a => a.TypeEndOfTreatment).FirstOrDefault();
    }

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

    public static IEnumerable<DO.Call> Filter(IEnumerable<DO.Call> toFilter, BO.CallType? type)
    {
        if (type != null)
        {
            toFilter = from call in toFilter
                       where call.CarTaypeToSend == (DO.CallType)type
                       select call;
        }
        return toFilter;
    }

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
            CallType = (BO.CallType)call.CarTaypeToSend,
            Address = call.Address,
            OpenTime = call.OpenTime,
            EnterTime = assignment.EnterTime,
            EndTime = assignment.EndTime,
            TypeEndOfTreatment = (BO.EndType?)assignment.TypeEndOfTreatment
        };
    }

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
            CallType = (BO.CallType)call.CarTaypeToSend,
            Destination = call.Description,
            Address = call.Address,
            OpenTime = call.OpenTime,
            MaxTime=call.MaxTime,
            Distance= Tools.DistanceCalculator.CalculateDistance(call.Address, volunteer.Address, volunteer.Type)
        };
    }

    public static DO.Call ToDOCall(BO.Call call)
    {
        return  new DO.Call(
                call.Id,
                call.Description,
                call.Address ?? string.Empty,
                call.Latitude,
                call.Longitude,
                call.OpenTime,
                call.MaxTime,
                (DO.CallType)call.CarTaypeToSend);
    }

    //public static BO.CallInProgress ToBOCallInProgress(DO.Call call)
    //{
    //    return new BO.CallInProgress
    //    {
    //        Id = call.Id,
    //        CallType = (BO.CallType)call.CarTaypeToSend,
    //        Destination = call.Description,
    //        Address = call.Address,
    //        OpenTime = call.OpenTime,
    //        MaxTime = call.MaxTime ?? DateTime.MinValue,
    //        EnterTime = ClockManager.Now,
    //        Distance = 0,
    //        Status = Status(call.Id)
    //    };
    //}

    //public static BO.ClosedCallInList ToBOClosedCall(DO.Call call)
    //{
    //    var assignments = s_dal.Assignment.ReadAll();
    //    //need to read assignment of call and volunteer
    //    return new BO.ClosedCallInList
    //    {
    //        Id = call.Id,
    //        CallType = (BO.CallType)call.CarTaypeToSend,
    //        Address = call.Address,
    //        OpenTime = call.OpenTime,
    //        EnterTime = s_dal.Assignment.Read(a => a.CallId == call.Id && (assignments.MaxBy(a => a.Id)).Id == a.Id).EnterTime,
    //        EndTime
    //        TypeEndOfTreatment

    //        EntryTimeForTreatment = assignment.EnterTime,
    //        EndTimeOfTreatment = assignment.EndTime,
    //        EndOfTreatmentType = (BO.EndType)assignment.EndOfTreatmentType
    //    };
    //}



    //public static IEnumerable<DO.Call> SortClosedCall(IEnumerable<DO.Call> toSort, BO.ClosedCallInListField? sortBy)
    //{
    //if (sortBy != null)
    //{
    //    toFilter = from call in toFilter
    //               orderby sortBy
    //               select call;
    //}
    //else
    //{
    //    toFilter = from call in toFilter
    //               orderby call.Id
    //               select call;
    //}
    //return toFilter;
    //}
    //public static List<Assignment> GetAssignments(int callId)
    //{
    //    return s_dal.Assignment.ReadAll(a => a.CallId == callId ).Select(callId);

    //}

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

    // כל המתודות במחלקה יהיו internal static
}
