using Helpers;
namespace BO;

/// <summary>
/// Represents a logical data entity for a "closed call in a list," containing details of completed or canceled calls.
/// This entity is for read-only purposes and is displayed as part of a list of closed calls.
/// </summary>
public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; init; }
    public string? Address { get; init; }
    public DateTime OpenTime { get; init; }
    public DateTime EnterTime { get; init; }
    public DateTime? EndTime { get; init; }
    public EndType? TypeEndOfTreatment { get; init; }
    public override string ToString() => this.ToStringProperty();
}
