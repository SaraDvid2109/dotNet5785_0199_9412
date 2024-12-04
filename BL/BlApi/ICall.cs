
namespace BlApi;
/// <summary>
/// Logical Service Entity
/// </summary>
public interface ICall
{
    public Array CallQuantities();
    public IEnumerable<BO.CallInList> CallInLists(Enum? field, object? filter);
    public BO.Call GetCallDetails(int id);
    public void UpdatingCallDetails(BO.Call call);
    public void DeleteCall(int id);
    public void AddCall(BO.Call call);
    //לא בטוחה לגבי הפרמטרים
    public IEnumerable<BO.ClosedCallInList> closedCallsHandledByVolunteer(int id , BO.CallType? filter, /*ClosedCallInList*/ BO.EndType? sortBy);
    //לא בטוחה לגבי הפרמטרים
    public IEnumerable<BO.OpenCallInList> openCallsForSelectionByVolunteer(int id, BO.CallType? filter, /*OpenCallInList*/ BO.EndType? sortBy);
    public void UpdateEndOfTreatmentCall(int volunteerId,int assignmentId);
    public void CancelCallHandling(int volunteerId, int assignmentId);
    public void ChooseCallForHandling(int volunteerId,int callId);
}
