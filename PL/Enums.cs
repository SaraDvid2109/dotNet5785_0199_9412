
using System;
using System.Collections;
using System.Collections.Generic;
namespace PL;
internal class VolunteersCollection : IEnumerable
{
    static readonly IEnumerable<BO.VolunteerField> s_enums =
         (Enum.GetValues(typeof(BO.VolunteerField)) as IEnumerable<BO.VolunteerField>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


