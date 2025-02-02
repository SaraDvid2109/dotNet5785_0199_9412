using PL.Volunteer;
using PL.Call;
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
using System.Windows.Threading;
using System.ComponentModel;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            StartStopButtonText = "Start Simulator";
            StatusObserver();
        }


        /// <summary>
        /// A dependency property that stores the number of calls with a "Closed" status.
        /// </summary>
        public int CloseStatus
        {
            get { return (int)GetValue(CloseStatusProperty); }
            set { SetValue(CloseStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseStatusProperty =
            DependencyProperty.Register("CloseStatus", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        /// <summary>
        /// A dependency property that stores the number of calls with an "Open" status.
        /// </summary>
        public int OpenStatus
        {
            get { return (int)GetValue(OpenStatusProperty); }
            set { SetValue(OpenStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenStatusProperty =
            DependencyProperty.Register("OpenStatus", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        /// <summary>
        /// A dependency property that stores the number of calls with an "Expired" status.
        /// </summary>
        public int ExpiredStatus
        {
            get { return (int)GetValue(ExpiredStatusProperty); }
            set { SetValue(ExpiredStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpiredStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpiredStatusProperty =
            DependencyProperty.Register("ExpiredStatus", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        /// <summary>
        /// A dependency property that stores the number of calls with an "OpenAtRisk" status.
        /// </summary>
        public int OpenAtRiskStatus
        {
            get { return (int)GetValue(OpenAtRiskStatusProperty); }
            set { SetValue(OpenAtRiskStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenAtRiskStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenAtRiskStatusProperty =
            DependencyProperty.Register("OpenAtRiskStatus", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        /// <summary>
        /// A dependency property that stores the number of calls with an "Treatment" status.
        /// </summary>
        public int TreatmentStatus
        {
            get { return (int)GetValue(TreatmentStatusProperty); }
            set { SetValue(TreatmentStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreatmentStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreatmentStatusProperty =
            DependencyProperty.Register("TreatmentStatus", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        /// <summary>
        /// A dependency property that stores the number of calls with an "TreatmentOfRisk" status.
        /// </summary>
        public int TreatmentOfRiskStatus
        {
            get { return (int)GetValue(TreatmentOfRiskStatusProperty); }
            set { SetValue(TreatmentOfRiskStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreatmentOfRiskStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreatmentOfRiskStatusProperty =
            DependencyProperty.Register("TreatmentOfRiskStatus", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        /// <summary>
        /// Sets the window size to match the dimensions of an image.
        /// </summary>
        private void SetWindowSizeToImage()
        {
            string imagePath = "Images/starOfDavid.jpg";
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath, UriKind.Absolute));

            this.Width = bitmap.PixelWidth;
            this.Height = bitmap.PixelHeight;
        }

        private string _startStopButtonText = "Start Simulator";
       
        /// <summary>
        /// Gets or sets the text displayed on the Start/Stop button.
        /// </summary>
        public string StartStopButtonText
        {
            get => _startStopButtonText;
            set
            {
                if (_startStopButtonText != value)
                {
                    _startStopButtonText = value;
                    OnPropertyChanged(nameof(StartStopButtonText));
                    OnPropertyChanged(nameof(ButtonsEnabled));
                }
            }
        }

        /// <summary>
        /// Interval property with dependency property support for binding
        /// </summary>
        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        private bool _isSimulatorRunning;

        /// <summary>
        /// Gets or sets a value indicating whether the simulator is running.
        /// </summary>
        public bool IsSimulatorRunning
        {
            get => _isSimulatorRunning;
            set
            {
                if (_isSimulatorRunning != value)
                {
                    _isSimulatorRunning = value;
                    OnPropertyChanged(nameof(IsSimulatorRunning));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Handles the click event of the Start/Stop button to start or stop the simulator.
        /// </summary>
        private void OnStartStopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsSimulatorRunning)
                {
                    // Stop the simulator
                    s_bl.Admin.StopSimulator();
                    IsSimulatorRunning = false;
                    StartStopButtonText = "Start Simulator";
                }
                else
                {
                    // Start the simulator
                    s_bl.Admin.StartSimulator(Interval);
                    IsSimulatorRunning = true;
                    StartStopButtonText = "Stop Simulator";
                }
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the buttons are enabled based on the simulator's running state.
        /// </summary>
        public bool ButtonsEnabled => !IsSimulatorRunning;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handles the event when the window is closed to stop the simulator if it is running.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
            }
        }

        /// <summary>
        /// CurrentTime property with dependency property support for binding
        /// </summary>
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow)/*, new PropertyMetadata(DateTime.Now)*/);

        /// <summary>
        /// MaxRiskRange property with dependency property support for binding
        /// </summary>
        public TimeSpan MaxRiskRange
        {
            get { return (TimeSpan)GetValue(MaxRiskRangeProperty); }
            set { SetValue(MaxRiskRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxRiskRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxRiskRangeProperty =
            DependencyProperty.Register("MaxRiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// Method called when the window is loaded to initialize values
        /// </summary>
        private void InitializeMainWindow(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            MaxRiskRange = s_bl.Admin.GetMaxRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.call.AddObserver(StatusObserver);

        }

        /// <summary>
        /// Button click handlers to advance the system time by different units
        /// </summary>
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Button click handler to update the MaxRiskRange in the system
        /// </summary>
        private void btUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetMaxRange(MaxRiskRange);
                MessageBox.Show("Max Risk Range updated successfully!", "Update Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private volatile DispatcherOperation? _observerOperationClock = null; //stage 7

        /// <summary>
        /// Observer method to update the CurrentTime whenever the clock changes
        /// </summary>
        private void clockObserver()
        {
            if (_observerOperationClock is null || _observerOperationClock.Status == DispatcherOperationStatus.Completed)
                _observerOperationClock = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                });
        }

        private volatile DispatcherOperation? _observerOperationConfig = null; //stage 7

        /// <summary>
        /// Observer method to update the MaxRiskRange whenever the configuration changes
        /// </summary>
        private void configObserver() 
        {
            if (_observerOperationConfig is null || _observerOperationConfig.Status == DispatcherOperationStatus.Completed)
                _observerOperationConfig = Dispatcher.BeginInvoke(() =>
                {
                    MaxRiskRange = s_bl.Admin.GetMaxRange();
                });
        }

        /// <summary>
        /// Cleanup method to remove observers when the window is closed
        /// </summary>
        private void CleanupOnWindowClose(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.call.RemoveObserver(StatusObserver);

        }

        /// <summary>
        /// Button click handler to open the Volunteer List Window
        /// </summary>
        private void btnVolunteers_Click(object sender, RoutedEventArgs e) => new VolunteerListWindow().Show();

        /// <summary>
        /// Button click handler to open the Call List Window
        /// </summary>
        private void btnCalls_Click(object sender, RoutedEventArgs e) => new CallListWindow().Show();

        /// <summary>
        /// Button click handler to initialize the database
        /// </summary>
        private void DatabaseInitialization_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Database initialization", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.InitializeDB();
                    MessageBox.Show("Database initialization completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred during initialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
          
        }

        /// <summary>
        /// Button click handler to reset the database
        /// </summary>
        private void DatabaseReset_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the data?", "Database reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    s_bl.Admin.ResetDB();
                    MessageBox.Show("Database reset completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred during reset: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }

        }

        /// <summary>
        /// Method to close all windows except for the main window
        /// </summary>
        private void CloseAllWindowsExceptThis()
        {
            // Get the main window
            var mainWindow = Application.Current.MainWindow;

            // Close all windows except the main window
            foreach (Window window in Application.Current.Windows)
            {
                if (window != mainWindow)
                {
                    window.Close();
                }
            }
        }

        /// <summary>
        /// Updates dependency properties with the number of calls per status. 
        /// </summary>
        private void StatusObserver()
        {
            IEnumerable<int> calls = s_bl.call.CallQuantities();
            List<int> ListCalls = calls.ToList();

            Dispatcher.Invoke(() =>
            {
                OpenStatus = ListCalls[0];
                TreatmentStatus = ListCalls[1];
                OpenAtRiskStatus = ListCalls[2];
                TreatmentOfRiskStatus = ListCalls[3];
                ExpiredStatus = ListCalls[4];
                CloseStatus = ListCalls[5];
            });
        }

        /// <summary>
        /// Opens the window displaying calls with just the "Open" status.
        /// </summary>
        private void OpenStatusCalls_Click(object sender, RoutedEventArgs e)
            =>new CallListWindow(BO.CallStatus.Open).Show();

        /// <summary>
        /// Opens the window displaying calls with just the "Treatment" status.
        /// </summary>
        private void TreatmentStatusCalls_Click(object sender, RoutedEventArgs e)
           => new CallListWindow(BO.CallStatus.Treatment).Show();

        /// <summary>
        /// Opens the window displaying calls with just the "OpenAtRisk" status.
        /// </summary>
        private void OpenAtRiskStatusCalls_Click(object sender, RoutedEventArgs e)
           => new CallListWindow(BO.CallStatus.OpenAtRisk).Show();

        /// <summary>
        /// Opens the window displaying calls with just the " TreatmentOfRisk" status.
        /// </summary>
        private void TreatmentOfRiskStatusCalls_Click(object sender, RoutedEventArgs e)
           => new CallListWindow(BO.CallStatus.TreatmentOfRisk).Show();

        /// <summary>
        /// Opens the window displaying calls with just the "Expired" status.
        /// </summary>
        private void ExpiredStatusCalls_Click(object sender, RoutedEventArgs e)
           => new CallListWindow(BO.CallStatus.Expired).Show();

        /// <summary>
        /// Opens the window displaying calls with just the "Close" status.
        /// </summary>
        private void CloseStatusCalls_Click(object sender, RoutedEventArgs e)
        => new CallListWindow(BO.CallStatus.Close).Show();
    }
}