namespace DO;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param> A number that uniquely identifies the assignment.
/// <param name="CallId"></param>
/// <param name="VolunteerId"></param>
/// <param name="EnterTime"></param>Time (date and time) when the current call entered processing.
/// <param name="EndTime"></param> Time (date and time) when the current volunteer finished handling the current call.
public record Assignment
(
  int Id,
  int CallId,
  int VolunteerId,
  DateTime? EnterTime,
  DateTime? EndTime,
  EndType? TypeEndOfTreatment
)
{
    public Assignment() : this(0, 0, 0, null, null, null) { }

}