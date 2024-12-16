using Helpers;
namespace BO;

/// <summary>
/// Represents a logical data entity for a "call in a list," providing summarized details 
/// for viewing calls in a list of general calls.
/// This entity is read-only and serves for display purposes.
/// </summary>
public class CallInList
{
    public int? Id { get; init; }
    public int CallId { get; init; }
    public CallType CallType { get; init; }
    public DateTime OpenTime { get; init; }
    public TimeSpan? TimeLeftToFinish { get; init; }
    public string? LastVolunteer { get; init; }
    public TimeSpan? TreatmentTimeLeft { get; init; }
    public CallStatus Status { get; init; }
    public int TotalAssignments { get; init; }
    public override string ToString() => this.ToStringProperty();
}
