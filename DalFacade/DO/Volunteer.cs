using DalApi;
using System;
using System.Reflection.Metadata;

namespace DO;

/// <summary>
/// A volunteer entity represents a volunteer with all their details.
/// </summary>
/// <param name="Id"></param>Personal unique ID of the volunteer (as in national id card)
/// <param name="Name"></param>Private Name of the volunteer
/// <param name="Phone"></param> Volunteer's phone number
/// <param name="Mail"></param> Volunteer's email address
/// <param name="Password"></param> Volunteer's password
/// <param name="Address"></param> Volunteer's address
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="Active"></param> whether the volunteer is active
/// <param name="MaximumDistance"></param> The maximum distance to receive a call,the default is air distance.
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

