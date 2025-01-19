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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerProfile.xaml
    /// </summary>
    public partial class VolunteerProfile : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int id;
        public VolunteerProfile(int id)
        {
            this.id = id;
            InitializeComponent();
            DataContext = this;
            CurrentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
            VolunteerName = CurrentVolunteer.Name!;
            HaveCall = s_bl.volunteer.VolunteerHaveCall(id);
            BO.CallInProgress call = s_bl.volunteer.GetVolunteerDetails(id).Progress??new BO.CallInProgress();
            VolunteerCall = s_bl.call.GetCallDetails(call.CallId)?? new BO.Call();
        }

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerProfile), new PropertyMetadata(null));


        //public BO.Call VolunteerCall { get; set; }
        public BO.Call VolunteerCall
        {
            get { return (BO.Call)GetValue(VolunteerCallProperty); }
            set { SetValue(VolunteerCallProperty, value); }
        }

        public static readonly DependencyProperty VolunteerCallProperty =
            DependencyProperty.Register("VolunteerCall", typeof(BO.Call), typeof(VolunteerProfile), new PropertyMetadata(null));

        public string VolunteerName
        {
            get { return (string)GetValue(VolunteerNameProperty); }
            set { SetValue(VolunteerNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerNameProperty =
            DependencyProperty.Register("VolunteerName", typeof(string), typeof(VolunteerProfile), new PropertyMetadata(""));



        public bool HaveCall
        {
            get { return (bool)GetValue(HaveCallProperty); }
            set { SetValue(HaveCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HaveCall.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HaveCallProperty =
            DependencyProperty.Register("HaveCall", typeof(bool), typeof(VolunteerProfile), new PropertyMetadata(false));


        private void VolunteerObserver()
        {
            int id = CurrentVolunteer!.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
            BO.CallInProgress? call = s_bl.volunteer.GetVolunteerDetails(id).Progress;
            if (call != null)
            {
                int callId = call.CallId;
                VolunteerCall = s_bl.call.GetCallDetails(callId);
            }
            VolunteerCall = new BO.Call();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
            //{
            //    s_bl.volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);

            //}
            ////if (CurrentVolunteer!.Id != 0)
            ////    s_bl.volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);

            //string latitude = CurrentVolunteer.Latitude.ToString();  
            //string longitude = CurrentVolunteer.Longitude.ToString();  
            if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
            {
                s_bl.volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);

            }
            //if (CurrentVolunteer!.Id != 0)
            //    s_bl.volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);

            var latitudeVolunteer = CurrentVolunteer?.Latitude??0;
            var longitudeVolunteer = CurrentVolunteer?.Longitude??0;
           

            var latitudeCall = VolunteerCall?.Latitude??0; // אם אין מיקום לקריאה, נציב null
            var longitudeCall = VolunteerCall?.Longitude??0;

            // יצירת תוכן HTML עם מפה מבוססת Leaflet
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
           height: 500px; /* קבע גובה לדוגמה */
         }}
      </style>
     </head>
     <body>
        <div id='map'></div>
        <script>
        // קואורדינטות המתנדב
        var latitudeVolunteer = parseFloat({latitudeVolunteer});
        var longitudeVolunteer = parseFloat({longitudeVolunteer});
  
       // קואורדינטות הקריאה
        var latitudeCall = parseFloat({latitudeCall});
        var longitudeCall = parseFloat({longitudeCall});

         // יצירת מפה
         if (latitudeVolunteer!==0 && latitudeVolunteer!==0) 
           {{
              var map = L.map('map').setView([latitudeVolunteer, longitudeVolunteer], 13);
              L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
              attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors'
              }}).addTo(map);

           }}


        // הוספת סמן למתנדב
        L.marker([latitudeVolunteer, longitudeVolunteer]).addTo(map)
        .bindPopup('מיקום המתנדב')
        .openPopup();
        
       // בדיקה אם יש מיקומים לקריאה
           if (parseFloat(latitudeCall)!==0 && parseFloat(longitudeCall)!==0) 
            
        {{  
          L.marker([latitudeCall, longitudeCall]).addTo(map).bindPopup('מיקום הקריאה')
           // התאמת המפה כך שתראה את שני המיקומים
           var bounds = L.latLngBounds(
           [latitudeVolunteer, longitudeVolunteer],
           [latitudeCall, longitudeCall]
           );
        }}
            
            map.fitBounds(bounds);
           </script>
         </body>
        </html>";

            // הצגת תוכן ה-HTML ב-WebBrowser
            mapWebBrowser.NavigateToString(htmlContent);
        }

    private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
            if (VolunteerCall != null)
            {
                HaveCall = true;
            }
        }

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

        private void ViewCallHistory_Click(object sender, RoutedEventArgs e)
        {
            ClosedCallsWindow closedCallsWindow= new ClosedCallsWindow(id);
            closedCallsWindow.Show();
        }
        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            OpenCallsWindow openCallsWindow = new OpenCallsWindow(id);
            openCallsWindow.Show();
        }
        private void EndOfTreatment_Click(object sender, RoutedEventArgs e)
        {
            s_bl.call.UpdateEndOfTreatmentCall(id, VolunteerCall.Id);
        }

        private void CancelTreatment_Click(object sender, RoutedEventArgs e)
        {
            s_bl.call.CancelCallHandling(id, VolunteerCall.Id);
        }


        //private void VolunteerObserver()
        //{
        //    int id = CurrentVolunteer!.Id;
        //    CurrentVolunteer = null;
        //    CurrentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);

        //}


        ////// Adds an observer to monitor changes to the volunteer data.
        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (CurrentVolunteer!.Id != 0)
        //        s_bl.volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);

        //}

        //// Removes the observer when the window is closed.
        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    s_bl.volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
        //}
    }
}
