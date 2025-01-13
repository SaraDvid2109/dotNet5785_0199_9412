
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






