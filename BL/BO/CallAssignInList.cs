using Helpers;
namespace BO;

/// <summary>
/// Represents a logical data entity for a "call assignment in a list," containing details of a specific volunteer's assignment to a call.
/// This entity is for read-only purposes and is displayed as part of a list of assignments for a specific call.
/// </summary>
public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? Name { get; init; }
    public DateTime EnterTime { get; init; }
    public DateTime? EndTime { get; init; }
    public EndType? TypeEndOfTreatment { get; init; }
    public override string ToString() => this.ToStringProperty();
}
