
namespace BlImplementation;
using BlApi;
internal class Bl : IBl
{
    public IVolunteer volunteer { get; } = new volunteerImplementation();
    public ICall call { get; } = new CallImplementation();

    public IAdmin Admin { get; } = new AdminImplementation();   
}