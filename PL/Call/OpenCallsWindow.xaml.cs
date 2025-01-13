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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for OpenCallsWindow.xaml
    /// </summary>
    public partial class OpenCallsWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int id;
        public OpenCallsWindow(int id = 0)
        {
            this.id = id;
            InitializeComponent();
            VolunteerAddrees = s_bl.volunteer.GetVolunteerDetails(id).Address??"";
        }

        public IEnumerable<OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenCallList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<OpenCallInList>), typeof(OpenCallsWindow), new PropertyMetadata(null));



        public string VolunteerAddrees
        {
            get { return (string)GetValue(VolunteerAddreesProperty); }
            set { SetValue(VolunteerAddreesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerAddrees.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerAddreesProperty =
            DependencyProperty.Register("VolunteerAddrees", typeof(string), typeof(OpenCallsWindow), new PropertyMetadata(""));


        // Selected filter from ComboBox
        public BO.CallType SelectedFiled { get; set; } = BO.CallType.None;

        // Selected call from DataGrid
        //public BO.OpenCallInList? SelectedCall { get; set; }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenCallList = (SelectedFiled == BO.CallType.None) ?
            s_bl.call.openCallsForSelectionByVolunteer(id, null, null) : s_bl.call.openCallsForSelectionByVolunteer(id, SelectedFiled, null);
        }

        private void ButtonChoose_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int callId)
            {
                s_bl.call.ChooseCallForHandling(id, callId);
            }
        }

        private void ChangeAddressButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string address) {
                BO.Volunteer currentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
                currentVolunteer.Address = address;
                BO.Volunteer updateVolunteer = currentVolunteer;
                s_bl.volunteer.UpdatingVolunteerDetails(id, updateVolunteer);

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
        /// <summary>
        /// Observer to update the volunteer calls list when changes occur
        /// </summary>
        private void VolunteerCallsListObserver()
            => QueryVolunteerCalls();
        // Add observer when the window loads
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.call.AddObserver(VolunteerCallsListObserver);
        }
        // Remove observer when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.call.RemoveObserver(VolunteerCallsListObserver);
        }

        
    }

}
