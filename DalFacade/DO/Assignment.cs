namespace DO;

/// <summary>
/// Represents an assignment that links a volunteer to a specific call, including details about the timing and type of treatment end.
/// </summary>
/// <param name="Id">A number that uniquely identifies the assignment.</param>
/// <param name="CallId">The ID of the call associated with the assignment.</param>
/// <param name="VolunteerId">The ID of the volunteer assigned to the call.</param>
/// <param name="EnterTime">The time (date and time) when the current call entered processing.</param>
/// <param name="EndTime">The time (date and time) when the current volunteer finished handling the current call.</param>
/// <param name="TypeEndOfTreatment">The type of conclusion for the treatment (e.g., successful, unresolved).</param>
public record Assignment
(
  int Id,
  int CallId,
  int VolunteerId,
  DateTime EnterTime,
  DateTime? EndTime,
  EndType? TypeEndOfTreatment
)
{
    public Assignment() : this(0, 0, 0, DateTime.MinValue, null, null) { }
}