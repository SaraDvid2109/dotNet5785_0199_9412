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
            CurrentVolunteer= s_bl.volunteer.GetVolunteerDetails(id);
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
            //int id = CurrentVolunteer!.Id;
            //CurrentVolunteer = null;
            //CurrentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
            BO.CallInProgress? call= s_bl.volunteer.GetVolunteerDetails(id).Progress;
            if (call != null) 
            {
                int callId = call.CallId;
                VolunteerCall = s_bl.call.GetCallDetails(callId);
            }
            VolunteerCall = new BO.Call();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
            {
                s_bl.volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
               
            }
            //if (CurrentVolunteer!.Id != 0)
            //    s_bl.volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
            if(VolunteerCall!=null)
            {
                HaveCall= true;
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            VolunteerWindow volunteerWindow = new VolunteerWindow(id);
            volunteerWindow.Show();
        }

        private void ViewCallHistory_Click(object sender, RoutedEventArgs e)
        {

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

       
    }
}
