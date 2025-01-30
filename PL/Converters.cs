using BO;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL;

// A class that implements IValueConverter to convert a Role to a corresponding color.
class ConvertRoleToColor : IValueConverter
    {
    // Converts the given Role value to a color.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Roles role = (BO.Roles)value;
            return role switch
           {
            BO.Roles.Manager => Brushes.Yellow,
            BO.Roles.Volunteer => Brushes.Green,
            _ => Brushes.White,
           };
    }

    // The ConvertBack method is not implemented because it's not needed in this case.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
class ConvertboolToVisibilty: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool visibilty = (bool)value;
        switch (visibilty)
        {
            case true:
                return Visibility.Visible;
            case false:
                return Visibility.Collapsed;
        }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
class ConvertHaveCallToVisibilty : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool visibilty = (bool)value;
        switch (visibilty)
        {
            case true:
                return Visibility.Visible;
            case false:
                return Visibility.Collapsed;
        }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(bool)value)
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
            return !booleanValue;
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
            return !booleanValue;
        return DependencyProperty.UnsetValue;
    }
}

public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

class ConvertStatusToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((BO.CallStatus)value == BO.CallStatus.Open || (BO.CallStatus)value == BO.CallStatus.OpenAtRisk)
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class CallDeleteVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CallInList call)
        {
            // בדיקה אם הסטטוס הוא פתוח ואין הקצאות
            if (call.Status == CallStatus.Open && call.TotalAssignments == 0)
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class BorderColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double? visibilty = (double?)value;
        if (visibilty != null)
        {
            return Brushes.Red;  
        }
        return Brushes.Yellow;  

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class StatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallStatus visibilty = (BO.CallStatus)value;
        switch (visibilty)
        {
            case BO.CallStatus.Open:
                return Brushes.Pink;

            case BO.CallStatus.OpenAtRisk:
                return Brushes.DeepSkyBlue;

            case BO.CallStatus.Treatment:
                return Brushes.Turquoise;

            case BO.CallStatus.TreatmentOfRisk:
                return Brushes.SkyBlue;

            case BO.CallStatus.Expired:
                return Brushes.LightGray;

            case BO.CallStatus.Close:
                return Brushes.LightGreen;
            default:
                return Brushes.Yellow;  
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class CallTypeColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallType visibilty = (BO.CallType)value;
        switch (visibilty)
        {
            case BO.CallType.IntensiveCareAmbulance:
                return Brushes.LightSkyBlue;

            case BO.CallType.Ambulance:
                return Brushes.LightGoldenrodYellow;

            case BO.CallType.RegularVehicle:
                return Brushes.LightGreen;
            default:
                return Brushes.Yellow;
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}







