
using BlApi;
using BO;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public Array CallQuantities()
    {
        var calls=_dal.Call.ReadAll();
        
    }
    public void AddCall(Call call)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CallInList> CallInLists(Enum? field, object? filter)
    {
        throw new NotImplementedException();
    }
    public void CancelCallHandling(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void ChooseCallForHandling(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedCallInList> closedCallsHandledByVolunteer(int id, CallType? filter, EndType? sortBy)
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int id)
    {
        throw new NotImplementedException();
    }

    public Call GetCallDetails(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenCallInList> openCallsForSelectionByVolunteer(int id, CallType? filter, EndType? sortBy)
    {
        throw new NotImplementedException();
    }

    public void UpdateEndOfTreatmentCall(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void UpdatingCallDetails(Call call)
    {
        throw new NotImplementedException();
    }
}
