namespace BlApi;
/// <summary>
/// Logical Service Entity
/// </summary>
public interface IVolunteer
{
    public DO.Roles Login(string username, string password);
    public IEnumerable<BO.VolunteerInList> VolunteerList(bool? active, BO.VolunteerField? field);
    public BO.Volunteer GetVolunteerDetails(int id);
    public void UpdatingVolunteerDetails(int id, BO.Volunteer volunteer);
    public void DeleteVolunteer(int id);
    public void AddVolunteer(BO.Volunteer volunteer);
}
