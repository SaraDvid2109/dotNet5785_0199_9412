using Helpers;
namespace BO;
public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? Name { get; init; }

    public DateTime EnterTime { get; init; }
    public DateTime? EndTime { get; init; }
    public EndType EndType { get; init; }
    public EndType? TypeEndOfTreatment { get; init; }
    public override string ToString() => this.ToStringProperty();
}
