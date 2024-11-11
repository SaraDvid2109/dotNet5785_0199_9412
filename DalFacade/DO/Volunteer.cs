using DalApi;
using System;
using System.Reflection.Metadata;

namespace DO;

/// <summary>
/// A volunteer entity represents a volunteer with all their details.
/// </summary>
/// <param name="Id"></param>Personal unique ID of the volunteer (as in national id card)
/// <param name="Name"></param>Private Name of the volunteer
/// <param name="Phone"></param>
/// <param name="Mail"></param>
/// <param name="Password"></param>
/// <param name="Address"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="Active"></param>
/// <param name="MaximumDistance"></param>
public record Volunteer
(
   int Id,
   string Name,
   string Phone,
   string Mail,
   string? Password,
   string? Address, 
   double? Latitude, //?
   double? Longitude, //?
   bool   Active,
   double? MaximumDistance,
   Roles Role,
   DistanceType Type
   //int TotalCallsHandled,
   //int TotalCallsCanceled,
   //int TotalCallsChosenHandleExpired
   //BO.CallInProgress? Progress


)
{
    public Volunteer() : this(0, string.Empty, string.Empty, string.Empty, null,null,null,null,false,null, 0, 0 ) { }

}

