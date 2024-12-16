namespace BlApi;

/// <summary>
/// Logical service entity interface for managing calls.
/// </summary>
public interface ICall
{
    /// <summary>
    /// Retrieves the quantities of calls grouped by their status.
    /// </summary>
    /// <returns>An array where each index represents a call status and the value represents the count of calls with that status.</returns>
    public IEnumerable<int> CallQuantities();
    /// <summary>
    /// Returns a list of calls with optional filtering and sorting.
    /// </summary>
    /// <param name="filter">The field to filter the calls by (optional).</param>
    /// <param name="value">The value to filter the calls with (optional).</param>
    /// <param name="sort">The field to sort the calls by (optional).</param>
    /// <returns>A sorted and/or filtered list of calls in the BO.CallInList format.</returns>
    public IEnumerable<BO.CallInList> CallInLists(BO.CallInListFields? filter, object? value, BO.CallInListFields? sort);
    /// <summary>
    /// Returns the details of a specific call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call </param>
    /// <returns> the call details.</returns>
    public BO.Call GetCallDetails(int id);
    /// <summary>
    /// Updates the details of an existing call.
    /// </summary>
    /// <param name="id">The ID of the call to update.</param>
    /// <param name="call">The updated call details.</param>
    public void UpdatingCallDetails(int id,BO.Call call);
    /// <summary>
    /// Deletes a call by its ID if it meets the necessary conditions.
    /// </summary>
    /// <param name="id">The ID of the call to be deleted.</param>
    public void DeleteCall(int id);
    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="call">The call object containing the details of the call to be added.</param>
    public void AddCall(BO.Call call);
    /// <summary>
    /// Returns a list of closed calls handled by a specific volunteer, with optional filtering and sorting.
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer whose closed calls are being retrieved.</param>
    /// <param name="filter">The type of the call to filter the closed calls (RegularVehicle, Ambulance, IntensiveCareAmbulance, None).</param>
    /// <param name="sortBy">The field by which to sort the list of closed calls (e.g., ID, Address, EnterTime, etc.).</param>
    /// <returns>Sorted and filtered list of closed calls handled by the volunteer.</returns>
    public IEnumerable<BO.ClosedCallInList> closedCallsHandledByVolunteer(int id , BO.CallType? filter, BO.ClosedCallInListField? sortBy);
    /// <summary>
    /// Returns a list of open calls available for selection by a specific volunteer, with optional filtering and sorting.
    /// </summary>
    /// <param name="id">The ID of the volunteer whose open calls are being retrieved.</param>
    /// <param name="filter">The type of the call to filter the open calls (RegularVehicle, Ambulance, IntensiveCareAmbulance, None).</param>
    /// <param name="sortBy">The field by which to sort the list of open calls (e.g., ID, Address, OpenTime, etc.).</param>
    /// <returns>Sorted and filtered list of open calls available for selection by the volunteer.</returns>
    public IEnumerable<BO.OpenCallInList> openCallsForSelectionByVolunteer(int id, BO.CallType? filter, BO.OpenCallInListField? sortBy);
    /// <summary>
    /// Updates the end of treatment details for a specific assignment handled by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer who is updating the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment to be updated.</param>
    public void UpdateEndOfTreatmentCall(int volunteerId,int assignmentId);
    /// <summary>
    /// Cancels the handling of a specific assignment by a volunteer or an administrator.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer attempting to cancel the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment to be canceled.</param>
    public void CancelCallHandling(int volunteerId, int assignmentId);
    /// <summary>
    /// Assigns a specific call to a volunteer for handling.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer choosing to handle the call.</param>
    /// <param name="callId">The ID of the call to be handled by the volunteer.</param>
    public void ChooseCallForHandling(int volunteerId,int callId);
}
