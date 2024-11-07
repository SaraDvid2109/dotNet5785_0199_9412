namespace DO;
enum EndType {ExpiredCancellation,SelfCancellation,AdminCancellation,Processed}
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="CallId"></param>
/// <param name="VolunteerId"></param>
/// <param name="EnterTime"></param>
/// <param name="EndTime"></param>
public record Assignment
(
  int Id,
  int CallId,
  int VolunteerId,
  DateTime? EnterTime,
  DateTime? EndTime
)
{
    public Assignment() : this(0, 0, 0, null, null) { }

}