using DalApi;
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

    public static IEnumerable<DO.Call> SortClosedCall(IEnumerable<DO.Call> toFilter, BO.EndType? sortBy)
    {
        if (sortBy != null)
        {
            toFilter = from call in toFilter
                       orderby sortBy
                       select call;
        }
        else
        {
            toFilter = from call in toFilter
                       orderby call.Id
                       select call;
        }
        return toFilter;
    }

    public static BO.ClosedCallInList ConvertFromDOToBOClosedCall(DO.Call call, DO.Volunteer volunteer)
    {
        //need to read assignment of call and volunteer
        return new BO.ClosedCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CarTaypeToSend,
            Address = call.Address,
            OpenTime = call.OpenTime,
            //EntryTimeForTreatment = assignment.EnterTime,
            //EndTimeOfTreatment = assignment.EndTime,
            //EndOfTreatmentType = (BO.EndType)assignment.EndOfTreatmentType
        };
    }
    // כל המתודות במחלקה יהיו internal static


}
