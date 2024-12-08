
namespace BlApi;
/// <summary>
/// Logical Service Entity
/// </summary>
public interface ICall
{
    public Array CallQuantities();
    public IEnumerable<BO.CallInList> CallInLists(BO.CallField? filter, object? value, BO.CallField? sort);
    public BO.Call GetCallDetails(int id);
    public void UpdatingCallDetails(BO.Call call);
    public void DeleteCall(int id);
    public void AddCall(BO.Call call);
    //לא בטוחה לגבי הפרמטרים
    public IEnumerable<BO.ClosedCallInList> closedCallsHandledByVolunteer(int id , BO.CallType? filter, BO.ClosedCallInListField? sortBy);
    //לא בטוחה לגבי הפרמטרים
    public IEnumerable<BO.OpenCallInList> openCallsForSelectionByVolunteer(int id, BO.CallType? filter, BO.OpenCallInListField? sortBy);
    public void UpdateEndOfTreatmentCall(int volunteerId,int assignmentId);
    public void CancelCallHandling(int volunteerId, int assignmentId);
    public void ChooseCallForHandling(int volunteerId,int callId);
}
