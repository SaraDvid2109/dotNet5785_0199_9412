using DalApi;
using System.Collections.Generic;

namespace DO;

/// <summary>
/// A Call entity represents a Call with all their details.
/// </summary>
/// <param name="Id"></param>A number that uniquely identifies the call.
/// <param name="Description"></param> Description of the call.
/// <param name="Address"></param>  address of the call.
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="OpenTime"></param> Time (date and time) when the call was opened by the administrator.
/// <param name="MaxTime"></param> Time (date and time) by which the call should close.
/// <param name="type"></param> Types of calls.
public record Call
(
    int Id,
    string? Description,
    string Address,
    double? Latitude, //?
    double? Longitude, //?
    DateTime OpenTime,
    DateTime? MaxTime,
    CallType type
    //CallStatus Status
    //List<BO.CallAssignInList> ListAssignmentsForCalls


)
{
public Call() : this(0, null, string.Empty, null, null, DateTime.MinValue, null, 0) { }
}

