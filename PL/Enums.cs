using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace PL;

// Class that implements IEnumerable for CallType enumeration.
internal class CallTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums =
                     (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

// next stage
//internal class VolunteersCollection : IEnumerable
//{
//    static readonly IEnumerable<BO.VolunteerField> s_fields =
//         (Enum.GetValues(typeof(BO.VolunteerField)) as IEnumerable<BO.VolunteerField>)!;
//    public IEnumerator GetEnumerator() => s_fields.GetEnumerator();
//}

// Class that implements IEnumerable for Roles enumeration.
internal class RolesCollection : IEnumerable
{
    static readonly IEnumerable<BO.Roles> s_roles =
         (Enum.GetValues(typeof(BO.Roles)) as IEnumerable<BO.Roles>)!;
    public IEnumerator GetEnumerator() => s_roles.GetEnumerator();
}

// Class that implements IEnumerable for DistanceType enumeration.

internal class DistanceTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.DistanceType> s_distance =
         (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;
    public IEnumerator GetEnumerator() => s_distance.GetEnumerator();
}

internal class CallStatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallStatus> s_enums =
                     (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
            return booleanValue ? Visibility.Visible : Visibility.Collapsed;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}