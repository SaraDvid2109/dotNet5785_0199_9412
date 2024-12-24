using System;
using System.Collections;
using System.Collections.Generic;
namespace PL;
internal class VolunteersCollection : IEnumerable
{
    static readonly IEnumerable<BO.VolunteerField> s_fields =
         (Enum.GetValues(typeof(BO.VolunteerField)) as IEnumerable<BO.VolunteerField>)!;
    public IEnumerator GetEnumerator() => s_fields.GetEnumerator();
}

internal class RolesCollection : IEnumerable
{
    static readonly IEnumerable<BO.Roles> s_roles =
         (Enum.GetValues(typeof(BO.Roles)) as IEnumerable<BO.Roles>)!;
    public IEnumerator GetEnumerator() => s_roles.GetEnumerator();
}

internal class DistanceTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.DistanceType> s_distance =
         (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;
    public IEnumerator GetEnumerator() => s_distance.GetEnumerator();
}
