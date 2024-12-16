using Helpers;
namespace BO;

/// <summary>
/// Represents a logical data entity for an "open call in a list," containing details of calls available for volunteers to handle.
/// This entity is for read-only purposes and is displayed as part of a list of open calls.
/// </summary>
public class OpenCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; init; }
    public string? Destination { get; init; }
    public string? Address { get; init; }
    public DateTime OpenTime { get; init; }
    public DateTime? MaxTime { get; init; }
    public double Distance { get; init; }
    public override string ToString() => this.ToStringProperty();
}
