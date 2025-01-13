using BO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    // Reference to the business logic layer
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public VolunteerListWindow()
    {
        InitializeComponent();
    }

    // Dependency property for binding volunteer list to UI
    public IEnumerable<VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    // Using a DependencyProperty as the backing store for VolunteerList.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

    // Selected filter from ComboBox
    public BO.CallType SelectedFiled { get; set; } = BO.CallType.None;

    // Selected volunteer from DataGrid
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    // Handle ComboBox selection change to filter the volunteer list
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        VolunteerList = (SelectedFiled == BO.CallType.None) ?
            s_bl?.volunteer.VolunteerList(null, null)! : s_bl?.volunteer.FilterVolunteerListByCallType(SelectedFiled)!;

    }

    // Query the volunteer list based on the selected filter
    private void queryVolunteerList()
       => VolunteerList = (SelectedFiled == BO.CallType.None) ?
            s_bl?.volunteer.VolunteerList(null, null)! : s_bl?.volunteer.FilterVolunteerListByCallType(SelectedFiled)!;

    // Observer to update the volunteer list when changes occur
    private void VolunteerListObserver()
        => queryVolunteerList();

    // Add observer when the window loads
    private void Window_Loaded(object sender, RoutedEventArgs e)
     => s_bl.volunteer.AddObserver(VolunteerListObserver);

    // Remove observer when the window is closed
    private void Window_Closed(object sender, EventArgs e)
        => s_bl.volunteer.RemoveObserver(VolunteerListObserver);

    // Open the volunteer details window on double-clicking a row
    private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
    {
        if (SelectedVolunteer != null) 
            new VolunteerWindow(SelectedVolunteer.Id).Show();
    }

    // Open a new window to add a volunteer
    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow().Show();
    }

    // Handle deletion of a volunteer
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int volunteerId)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this volunteer?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.volunteer.DeleteVolunteer(volunteerId);
                    MessageBox.Show("Volunteer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private bool _isDeleteButtonVisible;
    public bool IsDeleteButtonVisible
    {
        get => _isDeleteButtonVisible;
        set
        {
            if (_isDeleteButtonVisible != value)
            {
                _isDeleteButtonVisible = value;
                OnPropertyChanged();
            }
        }
    }

    public void UpdateDeleteButtonVisibility(int id)
    {
        try
        {
            var volunteerToCheck = s_bl.volunteer.GetVolunteerDetails(id);
            IsDeleteButtonVisible = volunteerToCheck != null && !volunteerToCheck.Active;
        }
        catch
        {
            IsDeleteButtonVisible = false; // הסתרה אם יש שגיאה
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedVolunteer != null)
        {
            UpdateDeleteButtonVisibility(SelectedVolunteer.Id);
        }
    }

   

}
