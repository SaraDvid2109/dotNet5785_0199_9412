using PL.Volunteer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public MainWindow()
        {
            InitializeComponent();
        }

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow)/*, new PropertyMetadata(DateTime.Now)*/);

        public TimeSpan MaxRiskRange
        {
            get { return (TimeSpan)GetValue(MaxRiskRangeProperty); }
            set { SetValue(MaxRiskRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxRiskRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxRiskRangeProperty =
            DependencyProperty.Register("MaxRiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.Zero));

        private void InitializeMainWindow(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            MaxRiskRange = s_bl.Admin.GetMaxRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);

        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(BO.TimeUnit.Day);

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(BO.TimeUnit.Month);

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e) => s_bl.Admin.ForwardClock(BO.TimeUnit.Year);

        private void btUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetMaxRange(MaxRiskRange);
            MessageBox.Show("Max Risk Range updated successfully!", "Update Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void clockObserver() => CurrentTime = s_bl.Admin.GetClock();

        private void configObserver() => MaxRiskRange = s_bl.Admin.GetMaxRange();

        private void CleanupOnWindowClose(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

        private void btnVolunteers_Click(object sender, RoutedEventArgs e) => new VolunteerListWindow().Show();

        private void DatabaseInitialization_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Database initialization", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CloseAllWindowsExceptThis();
                Mouse.OverrideCursor = Cursors.Wait;
                s_bl.Admin.InitializeDB();
                Mouse.OverrideCursor = null;
            }
            //if (result == MessageBoxResult.Yes)
            //{
            //    CloseAllWindowsExceptThis();
            //    Mouse.OverrideCursor = Cursors.Wait;
            //    try
            //    {
            //        // אתחול בסיס הנתונים ברקע
            //        s_bl.Admin.InitializeDB();
            //        MessageBox.Show("Database initialization completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"An error occurred during initialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //    finally
            //    {
            //        // החזרת הסמן למצב רגיל
            //        Mouse.OverrideCursor = null;
            //    }
            //}
        }
        private void DatabaseReset_Click(object sender, RoutedEventArgs e)
        {
            var result=MessageBox.Show("Are you sure you want to reset the data?", "Database reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CloseAllWindowsExceptThis();
                Mouse.OverrideCursor = Cursors.Wait;
                s_bl.Admin.ResetDB();
                Mouse.OverrideCursor = null;
            }

        }

        private void CloseAllWindowsExceptThis()
        {
            // השגת החלון הראשי
            var mainWindow = Application.Current.MainWindow;

            // סגירת כל החלונות פרט לחלון הראשי
            foreach (Window window in Application.Current.Windows)
            {
                if (window != mainWindow)
                {
                    window.Close();
                }
            }
        }
    }
}