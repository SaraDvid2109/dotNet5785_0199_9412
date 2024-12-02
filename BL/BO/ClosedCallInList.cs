using Helpers;
namespace BO;

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
