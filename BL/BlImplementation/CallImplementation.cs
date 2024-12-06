
using BL.Helpers;
using BlApi;
using BO;
using DO;
using Helpers;
using System;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public Array CallQuantities()
    {
        var calls = _dal.Call.ReadAll();
        // לא הבנתי מה צריך לעשות
        var callQuantities = new int[Enum.GetValues(typeof(CallStatus)).Length];

        var groupedCalls = calls.GroupBy();

        foreach (var group in groupedCalls)
        {
            callQuantities[(int)group.Key] = group.Count();
        }
        return callQuantities;
    }
    public void AddCall(BO.Call call)
    {

        Helpers.Tools.IntegrityCheck(call);
        try
        {
            DO.Call callToAdd = new DO.Call(
                call.Id,
                call.Description,
                call.Address ?? string.Empty,
                call.Latitude,
                call.Longitude,
                call.OpenTime,
                call.MaxTime,
                (DO.CallType)call.CarTaypeToSend);

            _dal.Call.Create(callToAdd);
        }
        catch (Exception ex) { throw new Exception(ex.Message); }
    }

    public IEnumerable<CallInList> CallInLists(BO.CallField? sort, BO.CallField? filter, object? value)
    {
        // didnt finshed
        IEnumerable<DO.Call> calls;
        IEnumerable<IGrouping<bool, DO.Call>> groupedCalls;
        IEnumerable<DO.Call> sotrtCalls;
        if (sort == null)
            calls = _dal.Call.ReadAll();
        else
        {
            groupedCalls = _dal.Call.ReadAll().GroupBy(c => c.);
           calls = groupedCalls.FirstOrDefault(c => c.Key == active.Value) ?? Enumerable.Empty<DO.Call>();
        }
        if (value == null)
            sotrtCalls = calls.OrderBy(c => c.Id);
        else
        {
            sotrtCalls = value switch
            {
                BO.CallField.Address => calls.OrderBy(c => c.Address),
                BO.CallField.CarTaypeToSend => calls.OrderBy(c => c.CarTaypeToSend),
                BO.CallField.Id => calls.OrderBy(c => c.Id),
                _ => calls.OrderBy(c => c.Id)
            };
        }

        return sotrtCalls.Select(call => new BO.CallInList()
        {
            CallId = call.Id,
            CallType = (BO.CallType)call.CarTaypeToSend,
            Status = CallManager.Status(call.Id)
        });
    }
    public void CancelCallHandling(int volunteerId, int assignmentId)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new Exception($"Volunteer with {volunteerId} not found");
        DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);
        if (assignment == null)
            throw new Exception($"Assignment with {assignmentId} not found");

        if (volunteer.Role == DO.Roles.Volunteer && assignment.VolunteerId != volunteerId)
            throw new Exception("Sorry! You do not have access permission to revoke the assignment");
        if (assignment.TypeEndOfTreatment != null || assignment.EndTime != null)
            throw new Exception("You cannot cancel this assignment");

        DO.Assignment assignmentToUpdate;
        if (assignment.VolunteerId == volunteerId)
            assignmentToUpdate = assignment with { EndTime = ClockManager.Now, TypeEndOfTreatment = DO.EndType.SelfCancellation };
        else
            assignmentToUpdate = assignment with { EndTime = ClockManager.Now, TypeEndOfTreatment = DO.EndType.AdminCancellation };
    }

    public void ChooseCallForHandling(int volunteerId, int callId)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new Exception($"Volunteer with {volunteerId} not found");
        DO.Call? call = _dal.Call.Read(callId);
        if (call == null)
            throw new Exception($"Call with {callId} not found");

        var assignments = _dal.Assignment.ReadAll();
        if (assignments != null)
        {
            var assignmentVolunteer = assignments.FirstOrDefault(a => a.VolunteerId == volunteerId && a.TypeEndOfTreatment == null);
            if (assignmentVolunteer != null)
                throw new Exception($"Volunteer with {volunteerId} is already treating a call");
        }
        if (CallManager.Status(callId) != BO.CallStatus.Open && CallManager.Status(callId) != BO.CallStatus.OpenAtRisk)
            throw new Exception($"Volunteer with {volunteerId} is already treating a call");

        DO.Assignment assignmentToAdd = new DO.Assignment(0, callId, volunteerId, ClockManager.Now, null, null);
        _dal.Assignment.Create(assignmentToAdd);
    }

    public IEnumerable<ClosedCallInList> closedCallsHandledByVolunteer(int id, BO.CallType? filter, BO.EndType? sortBy)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(id);
        if (volunteer == null)
            throw new Exception($"Volunteer with {id} not found");

        var calls = _dal.Call.ReadAll();

        var fitCalls = from call in calls
                       where (CallManager.Status(call.Id) == BO.CallStatus.Close || CallManager.Status(call.Id) == BO.CallStatus.Expired)
                       select call;
        //filter to calls of volunteer using assignment
        var callsForList = CallManager.Filter(fitCalls, filter);
        var finalListCalls = CallManager.SortClosedCall(callsForList, sortBy);
        return finalListCalls.Select(call => CallManager.ConvertFromDOToBOClosedCall(call, volunteer));
    }

    public void DeleteCall(int id)
    {
        try
        {
            // not sure about this
            DO.Call? callToDelete = _dal.Call.Read(id);
            if (callToDelete == null) throw new Exception("");
            var assignments = _dal.Assignment.ReadAll();
            if (assignments != null)
            {
                var assignmentsOfCall = from assignment in assignments
                                        where assignment.CallId == id
                                        select assignment;
                if (assignmentsOfCall != null)
                    throw new Exception($"Cannot delete call with {id}");
            }
            else
                _dal.Volunteer.Delete(id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting volunteer:" + ex.Message);
        }
    }

    public BO.Call GetCallDetails(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenCallInList> openCallsForSelectionByVolunteer(int id, BO.CallType? filter, BO.EndType? sortBy)
    {
        throw new NotImplementedException();
    }

    public void UpdateEndOfTreatmentCall(int volunteerId, int assignmentId)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new Exception($"Volunteer with {volunteerId} not found");
        DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);
        if (assignment == null)
            throw new Exception($"Assignment with {assignmentId} not found");

        if (assignment.VolunteerId != volunteerId)
            throw new Exception("You do not have access permission to update the assignment");
        if (assignment.TypeEndOfTreatment != null || assignment.EndTime != null)
            throw new Exception("You cannot update this assignment");

        DO.Assignment assignmentToUpdate = assignment with { EndTime = ClockManager.Now, TypeEndOfTreatment = DO.EndType.Treated };
    }

    public void UpdatingCallDetails(BO.Call call)
    {
        throw new NotImplementedException();
    }
}
