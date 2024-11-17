namespace DalApi;

public interface IDal
{
    ICall Call { get; }
    IAssignment Assignment { get; }
    IVolunteer Volunteer { get; }
    void resetDB ();    
}
