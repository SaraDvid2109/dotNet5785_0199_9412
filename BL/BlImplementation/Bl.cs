namespace BlImplementation;
using BlApi;

/// <summary>
/// Implementation of the Bl interface, initialized with instances for managing volunteers, calls, and admin operations.
/// </summary>
internal class Bl : IBl
{
    
    public IVolunteer volunteer { get; } = new volunteerImplementation();
    public ICall call { get; } = new CallImplementation();

    public IAdmin Admin { get; } = new AdminImplementation();   
}