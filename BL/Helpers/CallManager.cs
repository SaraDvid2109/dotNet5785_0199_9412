using BO;
using DalApi;
using DO;
using System.Net;
namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    public static BO.CallStatus Status(int callId) //stage 4
    {
        // didnt finshed
        var call = s_dal.Call.Read(callId);
        if (call == null)
            throw new ArgumentException($"Call with id {callId} does not exist");
        if (call.MaxTime > DateTime.Now)
            return BO.CallStatus.Expired;
        if (call.OpenTime < DateTime.Now)
            return BO.CallStatus.Treatment;
        return BO.CallStatus.Open;
        //צריך להוסיף בדיקה גם בשביל :Close,TreatmentOfRisk,OpenAtRisk
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
    public static BO.ClosedCallInList ToBOClosedCall(DO.Call call)
    {
        var assignments = s_dal.Assignment.ReadAll();
        //need to read assignment of call and volunteer
        return new BO.ClosedCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CarTaypeToSend,
            Address = call.Address,
            OpenTime = call.OpenTime,
            //EnterTime=s_dal.Assignment.Read(a=>a.CallId==call.Id && (assignments.MaxBy(a=>a.Id)).Id== a.Id).EnterTime,
            //EndTime
            //TypeEndOfTreatment

            //EntryTimeForTreatment = assignment.EnterTime,
            //EndTimeOfTreatment = assignment.EndTime,
            //EndOfTreatmentType = (BO.EndType)assignment.EndOfTreatmentType
        };
    }

    public static BO.OpenCallInList ToBOOpenCall(DO.Call call)
    {
        return new BO.OpenCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CarTaypeToSend,
            Destination = "",/////////////////////////////////
            Address = call.Address,
            OpenTime = call.OpenTime,
            MaxTime=call.MaxTime,
            Distance=0/////////////////////////////////////////
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
    public static IEnumerable<DO.Assignment> findAssignment(int id)
    {
        return s_dal.Assignment.ReadAll()
                     .Where(a => a.CallId == id);
    }
    public static string getLastVolunteer(DO.Call call)
    {
        var assignments = findAssignment(call.Id);
        var last = assignments.MaxBy(a => a.EnterTime);
        var volunteer = s_dal.Volunteer.Read(last.VolunteerId);
        return volunteer.Name;
    }
    // כל המתודות במחלקה יהיו internal static


}
