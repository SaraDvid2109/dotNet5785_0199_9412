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
        public IEnumerable<ClosedCallInList> ClosedCallList
        {
            get { return (IEnumerable<ClosedCallInList>)GetValue(ClosedCallListProperty); }
            set { SetValue(ClosedCallListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClosedCallList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClosedCallListProperty =
            DependencyProperty.Register("ClosedCallList", typeof(IEnumerable<ClosedCallInList>), typeof(ClosedCallsWindow), new PropertyMetadata(null));
        public BO.CallType SelectedFiled { get; set; } = BO.CallType.None;

        private void QueryVolunteerCalls()
        {
            ClosedCallList = (SelectedFiled == BO.CallType.None) ?
            s_bl.call.closedCallsHandledByVolunteer(id, null, null) : s_bl.call.closedCallsHandledByVolunteer(id, SelectedFiled, null);
        }
        /// <summary>
        /// Observer to update the volunteer calls list when changes occur
        /// </summary>
        private void VolunteerCallsListObserver()
            => QueryVolunteerCalls();

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.call.AddObserver(VolunteerCallsListObserver);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.call.RemoveObserver(VolunteerCallsListObserver);

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClosedCallList = (SelectedFiled == BO.CallType.None) ?
            s_bl.call.closedCallsHandledByVolunteer(id, null, null) : s_bl.call.closedCallsHandledByVolunteer(id, SelectedFiled, null);
        }

    }
}
