using Helpers;
namespace BO;

/// <summary>
/// Represents a logical data entity for a volunteer, including personal details
/// and current active call details if the volunteer is handling one.
/// </summary>
public class Volunteer
{
    public int Id { get; init; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Mail { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Roles Role { get; set; }
    public bool Active { get; set; }
    public double? MaximumDistance { get; set; }
    public DistanceType Type { get; init; }
    public int TotalCallsHandled { get; set; }
    public int TotalCallsCanceled { get; set; }
    public int TotalCallsChosenHandleExpired { get; set; }
    public BO.CallInProgress? Progress { get; set; }
    public override string ToString() => this.ToStringProperty();
}

