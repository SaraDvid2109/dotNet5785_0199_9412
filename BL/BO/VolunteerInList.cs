using Helpers;
namespace BO;

/// <summary>
/// Represents a logical data entity for a "volunteer in a list," containing summary details.
/// This entity is for read-only purposes and is displayed as part of a list of volunteers.
/// </summary>
public class VolunteerInList
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public bool Active { get; init; }
    public int TotalCallsHandled { get; init; }
    public int TotalCallsCanceled { get; init; }
    public int TotalCallsChosenHandleExpired { get; init; }
    public int? CallHandledId { get; init; }
    public CallType CallHandledType { get; init; }
    public override string ToString() => this.ToStringProperty();
}
