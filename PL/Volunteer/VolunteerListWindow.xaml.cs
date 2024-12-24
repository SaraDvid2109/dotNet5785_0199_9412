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

namespace PL.Volunteer;

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

    public BO.VolunteerField SelectedFiled { get; set; } = BO.VolunteerField.None;
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        VolunteerList = (SelectedFiled == BO.VolunteerField.None) ?
            s_bl?.volunteer.VolunteerList(null, null)! : s_bl?.volunteer.VolunteerList(null, SelectedFiled)!;

    }
    private void queryVolunteerList()
       => VolunteerList = (SelectedFiled == BO.VolunteerField.None) ?
            s_bl?.volunteer.VolunteerList(null, null)! : s_bl?.volunteer.VolunteerList(null, SelectedFiled)!;

    private void VolunteerListObserver()
        => queryVolunteerList();

    private void Window_Loaded(object sender, RoutedEventArgs e)
     => s_bl.volunteer.AddObserver(VolunteerListObserver);

    private void Window_Closed(object sender, EventArgs e)
        => s_bl.volunteer.RemoveObserver(VolunteerListObserver);

    private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
    {
        if (SelectedVolunteer == null) 
            new VolunteerWindow().Show();
        else
            new VolunteerWindow(SelectedVolunteer.Id).Show();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        // קבלת ה-Id מתוך ה-Tag של הכפתור
        if (sender is Button button && button.Tag is int volunteerId)
        {
            // בקשת אישור למחיקה
            var result = MessageBox.Show(
                "Are you sure you want to delete this volunteer?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // קריאה למחיקה ב-BL
                    s_bl.volunteer.DeleteVolunteer(volunteerId);
                    MessageBox.Show("Volunteer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // טיפול בחריגה
                    MessageBox.Show($"Failed to delete volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
