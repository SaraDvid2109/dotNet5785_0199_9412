using Helpers;
namespace BO;
public class Call
{
    public int Id { get; init; }
    public CallType CallType { get; init; }
    public string? Destination { get; init; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime OpenTime { get; init; } // set or init?
    public DateTime? MaxTime { get; init; }
    public CallStatus Status { get; init; }
    public List<BO.CallAssignInList>? ListAssignmentsForCalls { get; set; } // set or init?
    public override string ToString() => this.ToStringProperty();
}
