namespace DO;
using DalApi;
using System.Collections.Generic;

/// <summary>
/// A Call entity represents a Call with all their details.
/// <summary>
/// Represents a call entity with all its details, including location, timing, and type.
/// </summary>
/// <param name="Id">A number that uniquely identifies the call.</param>
/// <param name="Description">A description of the call's nature or purpose.</param>
/// <param name="Address">The address where the call originated or is directed to.</param>
/// <param name="Latitude">The geographical latitude of the call's location.</param>
/// <param name="Longitude">The geographical longitude of the call's location.</param>
/// <param name="OpenTime">The time (date and time) when the call was opened by the administrator.</param>
/// <param name="MaxTime">The time (date and time) by which the call should close.</param>
/// <param name="CarTaypeToSend">The type of the call (e.g., emergency, routine).</param>
public record Call
(
    int Id,
    string? Description,
    string Address,
    double? Latitude,
    double? Longitude,
    DateTime OpenTime,
    DateTime? MaxTime,
    CallType CarTaypeToSend
    //CallStatus Status //next stage
    //List<BO.CallAssignInList> ListAssignmentsForCalls //next stag
)
{
    public Call() : this(0, null, string.Empty, null, null, DateTime.MinValue, null, 0) { }
}

