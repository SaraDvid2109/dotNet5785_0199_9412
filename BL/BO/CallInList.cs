using Helpers;
namespace BO;
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
