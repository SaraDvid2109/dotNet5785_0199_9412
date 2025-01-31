using BO;
using DO;
using PL.Call;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerProfile.xaml
    /// </summary>
    public partial class VolunteerProfile : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int id;
        BO.CallInProgress call;
        /// <summary>
        /// Initializes the VolunteerProfile window, displaying the volunteer's details and current call (if any).
        /// Handles errors during setup.
        /// </summary>
        /// <param name="id">The volunteer ID.</param>
        public VolunteerProfile(int id)
        {
            try
            {
                this.id = id;
                InitializeComponent();
                DataContext = this;
                CurrentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
                VolunteerName = CurrentVolunteer.Name!;

                HaveCall = s_bl.volunteer.VolunteerHaveCall(id); //Checks if a volunteer currently has a call they are handling.

                call = s_bl.volunteer.GetVolunteerDetails(id).Progress ?? new BO.CallInProgress();
                VolunteerCall = s_bl.call.GetCallDetails(call.CallId)??new BO.Call(); //The call the volunteer is currently handling
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                //call = new BO.CallInProgress(); // Ensure 'call' is initialized even if an exception occurs
            }
        }
        /// <summary>
        /// Current volunteer data, bound to UI controls via DependencyProperty.
        /// </summary>
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerProfile), new PropertyMetadata(null));

        /// <summary>
        /// The volunteer's current call data, bound to the UI controls via DependencyProperty.
        /// </summary>
        public BO.Call VolunteerCall
        {
            get { return (BO.Call)GetValue(VolunteerCallProperty); }
            set { SetValue(VolunteerCallProperty, value); }
        }

        public static readonly DependencyProperty VolunteerCallProperty =
            DependencyProperty.Register("VolunteerCall", typeof(BO.Call), typeof(VolunteerProfile), new PropertyMetadata(null));

        /// <summary>
        /// Volunteer name to display on screen
        /// </summary>
        public string VolunteerName
        {
            get { return (string)GetValue(VolunteerNameProperty); }
            set { SetValue(VolunteerNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerNameProperty =
            DependencyProperty.Register("VolunteerName", typeof(string), typeof(VolunteerProfile), new PropertyMetadata(""));

        /// <summary>
        /// Indicates whether the volunteer is currently handling a call.
        /// </summary>
        public bool HaveCall
        {
            get { return (bool)GetValue(HaveCallProperty); }
            set { SetValue(HaveCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HaveCall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HaveCallProperty =
            DependencyProperty.Register("HaveCall", typeof(bool), typeof(VolunteerProfile), new PropertyMetadata(false));
       
        private volatile DispatcherOperation? _observerOperationVolunteer = null; //stage 7
        
        /// <summary>
        /// Observer method that updates the volunteer or their current call upon any changes.
        /// </summary>
        private void VolunteerObserver()
        {
            if (_observerOperationVolunteer is null || _observerOperationVolunteer.Status == DispatcherOperationStatus.Completed)
                _observerOperationVolunteer = Dispatcher.BeginInvoke(() =>
                {

                    int id = CurrentVolunteer!.Id;
                    //CurrentVolunteer = null;
                    var updatedVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
                    CurrentVolunteer = updatedVolunteer;
                    VolunteerName = CurrentVolunteer.Name!;
                    HaveCall = s_bl.volunteer.VolunteerHaveCall(id); //Checks if a volunteer currently has a call they are handling.
                    call = s_bl.volunteer.GetVolunteerDetails(id).Progress ?? new BO.CallInProgress();
                    VolunteerCall = s_bl.call.GetCallDetails(call.CallId) ?? new BO.Call(); //The call the volunteer is currently handling
                    BindingOperations.GetBindingExpression(this, CurrentVolunteerProperty)?.UpdateTarget();
                    BindingOperations.GetBindingExpression(this, VolunteerCallProperty)?.UpdateTarget();
                    getMap();
                });
        }

        private volatile DispatcherOperation? _observerOperationCall = null; //stage 7

        private void VolunteerCallObserver() 
        {
            if (_observerOperationCall is null || _observerOperationCall.Status == DispatcherOperationStatus.Completed)
                _observerOperationCall = Dispatcher.BeginInvoke(() =>
                {

                    BO.CallInProgress? call = s_bl.volunteer.GetVolunteerDetails(id).Progress;
                    if (call != null)
                    {
                        int callId = call.CallId;
                        VolunteerCall = s_bl.call.GetCallDetails(callId);
                    }
                    else
                    {
                        VolunteerCall = new BO.Call();
                    }
                    HaveCall = s_bl.volunteer.VolunteerHaveCall(id);
                });
        }

        /// <summary>
        /// Adds observers to monitor changes in volunteer data and their current call,
        /// and creates a map showing the volunteer's location and the call they are handling.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
            {
                s_bl.volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
                s_bl.call.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
            }

            getMap();
        }

        /// <summary>
        /// Removes the observer when the window is closed.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
            s_bl.call.RemoveObserver(CurrentVolunteer.Id, VolunteerObserver);
              if (VolunteerCall != null)
                   HaveCall = true;
            

        }

        /// <summary>
        /// Handles the Update button click to update the volunteer's details.
        /// </summary>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.volunteer.UpdatingVolunteerDetails(id, CurrentVolunteer!);
                MessageBox.Show("The volunteer was updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Opens the "Volunteer Call History" screen when the "View Call History" button is clicked.
        /// </summary>
        private void ViewCallHistory_Click(object sender, RoutedEventArgs e)
        {
            ClosedCallsWindow closedCallsWindow= new ClosedCallsWindow(id);
            closedCallsWindow.Show();
        }

        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            if (VolunteerCall.Status == BO.CallStatus.Treatment || VolunteerCall.Status == BO.CallStatus.TreatmentOfRisk)
            {
                MessageBox.Show("You cannot select a new call because you are already handling another call!", "error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                OpenCallsWindow openCallsWindow = new OpenCallsWindow(id);
                openCallsWindow.Show();
            }
        }

        /// <summary>
        /// Handles the "End Treatment" button click, allowing the volunteer to update the status after completing the treatment for the current call.
        /// </summary>
        private void EndOfTreatment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.call.UpdateEndOfTreatmentCall(id, call.Id);
                MessageBox.Show("The call completion has been successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        /// <summary>
        /// Handles the "Cancel Treatment" button click, allowing the volunteer to update the status when they choose to cancel the treatment for the current call.
        /// </summary>
        private void CancelTreatment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.call.CancelCallHandling(id, call.Id);
                MessageBox.Show("Your registration for the call has been successfully canceled!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void getMap()
        {
            var latitudeVolunteer = CurrentVolunteer?.Latitude ?? 0;
            var longitudeVolunteer = CurrentVolunteer?.Longitude ?? 0;


            var latitudeCall = VolunteerCall?.Latitude ?? null;
            var longitudeCall = VolunteerCall?.Longitude ?? null;

            // Creating HTML content with a Leaflet-based map
            string htmlContent = $@"<!DOCTYPE html>
            <html>
              <head>
               <meta charset='utf-8' />
                <title>Leaflet Map</title>
                  <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
                  <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
                  <style>
                     #map 
                     {{
                        width: 100%;
                        height: 500px; 
                     }}
                 </style>
               </head>
              <body>
               <div id='map'></div>
               <script>
                 // Volunteer coordinates
                  var latitudeVolunteer = parseFloat({latitudeVolunteer});
                  var longitudeVolunteer = parseFloat({longitudeVolunteer});
  
                  // Call coordinates
                
                      var latitudeCall = parseFloat({latitudeCall});
                      var longitudeCall = parseFloat({longitudeCall});
                if (latitudeCall!==null && longitudeCall!==null) 
                {{
                     // Creating a map
                     if (latitudeVolunteer!==0 && latitudeVolunteer!==0) 
                     {{
                        var map = L.map('map').setView([latitudeVolunteer, longitudeVolunteer], 13);
                        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                        attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors'
                         }}).addTo(map);
                      }}
                   }}
                else
                {{
                    var map = L.map('map').setView([latitudeVolunteer], 13);
                    L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                    attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors'
                    }}).addTo(map);
                }}

                 //Add a marker for the volunteer
                 L.marker([latitudeVolunteer, longitudeVolunteer]).addTo(map)
                .bindPopup('Volunteer location')
                .openPopup();
        
                //Checking if there are locations for the call.
                if (parseFloat(latitudeCall)!==0 && parseFloat(longitudeCall)!==0) 
                 {{  
                     L.marker([latitudeCall, longitudeCall]).addTo(map).bindPopup('Call location')
                     // Adjusting the map to display both locations
                     var bounds = L.latLngBounds(
                     [latitudeVolunteer, longitudeVolunteer],
                     [latitudeCall, longitudeCall] );
                 }}
            
                   map.fitBounds(bounds);
                </script>
               </body>
            </html>";

            // Display the HTML content in the WebBrowser
            mapWebBrowser.NavigateToString(htmlContent);
        }
    }
}
