namespace DO; 
using DalApi;
using System;
using System.Reflection.Metadata;

/// <summary>
/// Represents a volunteer entity with all their details, including personal information, contact details, and activity status.
/// </summary>
/// <param name="Id">The personal unique ID of the volunteer (as in national ID card).</param>
/// <param name="Name">The private name of the volunteer.</param>
/// <param name="Phone">The volunteer's phone number.</param>
/// <param name="Mail">The volunteer's email address.</param>
/// <param name="Password">The volunteer's password for authentication purposes.</param>
/// <param name="Address">The volunteer's physical address.</param>
/// <param name="Latitude">The geographical latitude of the volunteer's address.</param>
/// <param name="Longitude">The geographical longitude of the volunteer's address.</param>
/// <param name="Active">Indicates whether the volunteer is currently active and available for calls.</param>
/// <param name="MaximumDistance">The maximum distance (in kilometers) the volunteer is willing to travel to respond to a call. Defaults to air distance.</param>
/// <param name="Role">The volunteer's role or permissions level.</param>
/// <param name="Type">The type of distance calculation used (e.g., air distance or road distance).</param>
public record Volunteer
(
   int Id,
   string Name,
   string Phone,
   string Mail,
   string? Password,
   string? Address, 
   double? Latitude, 
   double? Longitude, 
   bool Active,
   double? MaximumDistance,
   Roles Role,
   DistanceType Type
   //int TotalCallsHandled, //next stage
   //int TotalCallsCanceled, //next stage
   //int TotalCallsChosenHandleExpired //next stage
   //BO.CallInProgress? Progress //next stage
)
{
    public Volunteer() : this(0, string.Empty, string.Empty, string.Empty, null,null,null,null,false,null, 0, 0 ) { }
}

