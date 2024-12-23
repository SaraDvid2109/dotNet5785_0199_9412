using BO;
using System;
using System.Collections;
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
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerListWindow()
        {
            InitializeComponent();
        }

        public IEnumerable<VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));


        public BO.VolunteerField SelectedFiled { get; set; }= BO.VolunteerField.All;
        public BO.VolunteerInList SelectedVolunteer { get; set; }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VolunteerList = (SelectedFiled == BO.VolunteerField.All) ?
                s_bl?.volunteer.VolunteerList(null,null)! : s_bl?.volunteer.VolunteerList(null,SelectedFiled)!;

        }
        private void queryVolunteerList()
           => VolunteerList = (SelectedFiled == BO.VolunteerField.All) ?
                s_bl?.volunteer.VolunteerList(null, null)! : s_bl?.volunteer.VolunteerList(null, SelectedFiled)!;

        private void VolunteerListObserver()
            => queryVolunteerList();
 
       private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.volunteer.AddObserver(VolunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.volunteer.RemoveObserver(VolunteerListObserver);

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
