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
        }

        public IEnumerable<OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenCallList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<OpenCallInList>), typeof(OpenCallsWindow), new PropertyMetadata(null));


        // Selected filter from ComboBox
        public BO.CallType SelectedFiled { get; set; } = BO.CallType.None;

        // Selected call from DataGrid
        public BO.VolunteerInList? SelectedCall { get; set; }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenCallList = (SelectedFiled == BO.CallType.None) ?
            s_bl.call.openCallsForSelectionByVolunteer(id, null, null) : s_bl.call.openCallsForSelectionByVolunteer(id, SelectedFiled, null);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }


    }

}
