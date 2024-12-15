
namespace BlApi;
/// <summary>
/// Logical Service Entity
/// </summary>
public interface ICall
{
    public IEnumerable<int> CallQuantities();
    public IEnumerable<BO.CallInList> CallInLists(BO.CallInListFieldsFilter? filter, object? value, BO.CallInListFieldsSort? sort);
    public BO.Call GetCallDetails(int id);
    public void UpdatingCallDetails(int id,BO.Call call);
    public void DeleteCall(int id);
    public void AddCall(BO.Call call);
    public IEnumerable<BO.ClosedCallInList> closedCallsHandledByVolunteer(int id , BO.CallType? filter, BO.ClosedCallInListField? sortBy);
    public IEnumerable<BO.OpenCallInList> openCallsForSelectionByVolunteer(int id, BO.CallType? filter, BO.OpenCallInListField? sortBy);
    public void UpdateEndOfTreatmentCall(int volunteerId,int assignmentId);
    public void CancelCallHandling(int volunteerId, int assignmentId);
    public void ChooseCallForHandling(int volunteerId,int callId);
}
