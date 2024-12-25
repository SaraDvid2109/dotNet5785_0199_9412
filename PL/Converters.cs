
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PL;

    class ConvertRoleToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Roles role = (BO.Roles)value;
            return role switch
           {
            BO.Roles.Manager => Brushes.Yellow,
            BO.Roles.Volunteer => Brushes.RoyalBlue,
            _ => Brushes.White,
           };
    }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


