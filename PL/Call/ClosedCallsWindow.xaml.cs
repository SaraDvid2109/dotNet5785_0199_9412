using BO;
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
    /// Interaction logic for ClosedCallsWindow.xaml
    /// </summary>
    public partial class ClosedCallsWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private int id;
        public ClosedCallsWindow(int id = 0)
        {
            this.id = id;
            InitializeComponent();
            
        }
       
        /// <summary>
        ///Dependency property for binding closed calls list to UI
        /// </summary>
        public IEnumerable<ClosedCallInList> ClosedCallList
        {
            get { return (IEnumerable<ClosedCallInList>)GetValue(ClosedCallListProperty); }
            set { SetValue(ClosedCallListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClosedCallList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClosedCallListProperty =
            DependencyProperty.Register("ClosedCallList", typeof(IEnumerable<ClosedCallInList>), typeof(ClosedCallsWindow), new PropertyMetadata(null));
       
        /// <summary>
        /// Selected filter from ComboBox
        /// </summary>
        public BO.CallType SelectedFiled { get; set; } = BO.CallType.None;

        /// <summary>
        /// Query the volunteer's closed calls list based on the selected filter
        /// </summary>
        private void QueryVolunteerCalls()
        {
            ClosedCallList = (SelectedFiled == BO.CallType.None) ?
            s_bl.call.closedCallsHandledByVolunteer(id, null, null) : s_bl.call.closedCallsHandledByVolunteer(id, SelectedFiled, null);
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        /// <summary>
        /// Observer to update the volunteer's closed calls list when changes occur
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
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.call.AddObserver(VolunteerCallsListObserver);

        }

        /// <summary>
        /// Remove observer when the window is closed
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.call.RemoveObserver(VolunteerCallsListObserver);

        }
      
        /// <summary>
        /// Updates the closed calls list based on the selected field in the ComboBox.
        /// </summary>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClosedCallList = (SelectedFiled == BO.CallType.None) ?
            s_bl.call.closedCallsHandledByVolunteer(id, null, null) : s_bl.call.closedCallsHandledByVolunteer(id, SelectedFiled, null);
        }

    }
}
