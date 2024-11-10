using DalApi;
using System.Collections.Generic;

namespace DO;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Description"></param>
/// <param name="Address"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="OpenTime"></param>
/// <param name="MaxTime"></param>
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

