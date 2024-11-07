using System;

namespace DO;
public enum ROLE { management, volunteer };
enum DistanceType { car, aerial };
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
   double? Latitude,
   double? Longitude,
   bool   Active,
   double? MaximumDistance
   
)
{
    public Volunteer() : this(0, string.Empty, string.Empty, string.Empty, null,null,0,0,false,0) { }

}

