using Helpers;
namespace BO;

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
