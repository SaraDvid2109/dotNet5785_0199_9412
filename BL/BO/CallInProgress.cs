using Helpers;
namespace BO;

/// <summary>
/// Represents a logical data entity for a "call in progress," containing detailed information about a call being handled by a volunteer.
/// This entity is for read-only purposes and is displayed as part of the volunteer's details.
/// </summary>
public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType CallType { get; init; }
    public string? Destination { get; init; }
    public string? Address { get; init; }
    public DateTime OpenTime { get; init; }
    public DateTime? MaxTime { get; init; }
    public DateTime EnterTime { get; init; }
    public double Distance { get; init; }
    public CallStatus Status { get; init; }
    public override string ToString() => this.ToStringProperty();
}
