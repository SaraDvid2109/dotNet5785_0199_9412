using Helpers;
namespace BO;

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
