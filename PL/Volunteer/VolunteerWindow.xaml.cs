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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    // Reference to the Business Logic layer (BL) for volunteer operations
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // check i add it by myself

    private int id;
    public string ButtonText { get; set; }
    public VolunteerWindow(int id = 0)
    {
        this.id = id;
        ButtonText = id == 0 ? "Add" : "Update"; // Determine button text based on ID
        DataContext = this;
        InitializeComponent();
        LoadVolunteer(id); // Load volunteer data

    }

    /// <summary>
    /// Load a volunteer's data. Creates a new volunteer if ID is 0; otherwise, fetches details from BL.
    /// </summary>
    private void LoadVolunteer(int id)
    {
        try
        {
            if (id == 0)
            {
                // Create a new Volunteer object with default values
                CurrentVolunteer = new BO.Volunteer();
            }
            else
            {
                // Fetch the existing Volunteer object from the BL
                CurrentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    /// <summary>
    /// Handles Add/Update button click.
    /// Adds a new volunteer or updates existing details.
    /// </summary>
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        
        try
        {
            if (ButtonText == "Add")
            {
                s_bl.volunteer.AddVolunteer(CurrentVolunteer!);
                MessageBox.Show("The volunteer was added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (ButtonText == "Update")
            {
                s_bl.volunteer.UpdatingVolunteerDetails(id,CurrentVolunteer!);
                MessageBox.Show("The volunteer was updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unexpected error: {ex.Message}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private volatile DispatcherOperation? _observerOperation = null; //stage 7

    /// <summary>
    /// An observer method that refills the item
    /// </summary>
    private void VolunteerObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                int id = CurrentVolunteer!.Id;
                CurrentVolunteer = null;
                CurrentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);
            });

    }

    // Adds an observer to monitor changes to the volunteer data.
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            s_bl.volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);

    }

    // Removes the observer when the window is closed.
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
    }
}

