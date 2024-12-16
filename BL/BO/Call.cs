using Helpers;
namespace BO;
public class Call
{
    public int Id { get; init; }
    public CallType CarTypeToSend { get; init; }
    public string? Description { get; init; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime OpenTime { get; init; }  
    public DateTime? MaxTime { get; init; }
    public CallStatus Status { get; init; }
    public List<BO.CallAssignInList>? ListAssignmentsForCalls { get; set; } 
    public override string ToString() => this.ToStringProperty();
}
