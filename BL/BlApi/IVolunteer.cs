namespace BlApi;

/// <summary>
/// Logical service entity interface for managing volunteers.
/// </summary>
public interface IVolunteer : IObservable //stage 5
{
    /// <summary>
    /// Authenticates a volunteer by username and password and returns their role.
    /// </summary>
    /// <param name="username">The username of the volunteer.</param>
    /// <param name="password">The password of the volunteer.</param>
    /// <returns>The role of the volunteer (Role).</returns>
    public BO.Roles Login(string username, string password);
    
    /// <summary>
    /// Filters and sorts the volunteers based on the given activity status and sorting field, then returns the filtered and sorted list.
    /// </summary>
    /// <param name="active">Filter for active/inactive volunteers (null for all).</param>
    /// <param name="field">The field by which to sort the volunteers.</param>
    /// <returns>A filtered and sorted list of volunteers.</returns>
    public IEnumerable<BO.VolunteerInList> VolunteerList(bool? active, BO.VolunteerField? field);
   
    /// <summary>
    /// Returns the details of a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>The volunteer details.</returns>
    public BO.Volunteer GetVolunteerDetails(int id);
    
    /// <summary>
    /// Updates the details of a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to update.</param>
    /// <param name="volunteer">The updated volunteer information.</param>
    public void UpdatingVolunteerDetails(int id, BO.Volunteer volunteer);
    
    /// <summary>
    /// Deletes a volunteer by their ID if the volunteer is inactive.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    public void DeleteVolunteer(int id);
   
    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteer">The volunteer details to be added.</param>
    public void AddVolunteer(BO.Volunteer volunteer);

    public IEnumerable<BO.VolunteerInList> FilterVolunteerListByCallType(BO.CallType type);

    public bool CanVolunteerAttendCall(BO.Volunteer volunteer, BO.Call call);
   
    public bool VolunteerHaveCall(int id);

}
