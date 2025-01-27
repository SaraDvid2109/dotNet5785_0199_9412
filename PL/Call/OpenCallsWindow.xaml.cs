using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for OpenCallsWindow.xaml
    /// </summary>
    public partial class OpenCallsWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int id;
       
        /// <summary>
        /// Initializes the OpenCallsWindow, sets the volunteer ID, and updates their address.
        /// </summary>
        /// <param name="id">The volunteer ID</param>
        public OpenCallsWindow(int id = 0)
        {
            this.id = id;
            InitializeComponent();
            VolunteerAddrees = s_bl.volunteer.GetVolunteerDetails(id).Address ?? "";
        }

        /// <summary>
        ///Dependency property for binding open calls list to UI
        /// </summary>
        public IEnumerable<OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenCallList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<OpenCallInList>), typeof(OpenCallsWindow), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property for binding volunteer address to UI
        /// </summary>
        public string VolunteerAddrees
        {
            get { return (string)GetValue(VolunteerAddreesProperty); }
            set { SetValue(VolunteerAddreesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerAddrees.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerAddreesProperty =
            DependencyProperty.Register("VolunteerAddrees", typeof(string), typeof(OpenCallsWindow), new PropertyMetadata(""));


        /// <summary>
        /// Selected filter from ComboBox
        /// </summary>
        public BO.CallType SelectedFiled { get; set; } = BO.CallType.None;

        /// <summary>
        /// Selected call from DataGrid
        /// </summary>
        public BO.OpenCallInList? SelectedCall { get; set; }

        /// <summary>
        /// Selected call details
        /// </summary>
        public BO.Call CallDetails
        {
            get { return (BO.Call)GetValue(CallDetailsProperty); }
            set { SetValue(CallDetailsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallDetails.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallDetailsProperty =
            DependencyProperty.Register("CallDetails", typeof(BO.Call), typeof(OpenCallsWindow), new PropertyMetadata(null));

        /// <summary>
        /// Updates the open calls list based on the selected field in the ComboBox.
        /// </summary>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenCallList = (SelectedFiled == BO.CallType.None) ?
            s_bl.call.openCallsForSelectionByVolunteer(id, null, null) : s_bl.call.openCallsForSelectionByVolunteer(id, SelectedFiled, null);
        }
      
        /// <summary>
        /// Handles the "Choose" button click to assign the volunteer to a call.
        /// </summary>
        private void ButtonChoose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int callId)
                {
                    s_bl.call.ChooseCallForHandling(id, callId);
                    QueryVolunteerCalls();
                    MessageBox.Show("You have successfully registered for the call", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (BO.BlFileLoadCreateException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlFormatException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Failed to register for call : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        
        /// <summary>
        /// Handles the click event for changing the volunteer's address and updates it in the system.
        /// </summary>
        private void ChangeAddressButton_Click(object sender, RoutedEventArgs e)
        {

            BO.Volunteer currentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
            currentVolunteer.Address = VolunteerAddrees;
            BO.Volunteer updateVolunteer = currentVolunteer;
            try
            {
                s_bl.volunteer.UpdatingVolunteerDetails(id, updateVolunteer);
                MessageBox.Show("address update successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Failed to update address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        /// <summary>
        /// Query the volunteer calls list based on the selected filter
        /// </summary>
        private void QueryVolunteerCalls()
        {
            OpenCallList = (SelectedFiled == BO.CallType.None) ?
            s_bl.call.openCallsForSelectionByVolunteer(id, null, null) : s_bl.call.openCallsForSelectionByVolunteer(id, SelectedFiled, null);
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        /// <summary>
        /// Observer to update the volunteer calls list when changes occur
        /// </summary>
        private void VolunteerCallsListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    QueryVolunteerCalls();
                });
        }
            

        /// <summary>
        /// Add observer to monitor changes in calls data when the window loads
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.call.AddObserver(VolunteerCallsListObserver);
        }
       
        /// <summary>
        /// Remove observer when the window is closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.call.RemoveObserver(VolunteerCallsListObserver);
        }
        
        /// <summary>
        /// Handles the selection change in the data grid and updates details for the selected call.
        /// </summary>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectedCall != null)
                CallDetails = s_bl.call.GetCallDetails(SelectedCall.Id);
        }
    }
}
