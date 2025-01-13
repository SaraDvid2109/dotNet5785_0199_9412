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

namespace PL.Call;

/// <summary>
/// Interaction logic for CallWindow.xaml
/// </summary>
public partial class CallWindow : Window
{
    // Reference to the Business Logic layer (BL) for call operations
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // check i add it by myself

    private int id;
    public string ButtonText { get; set; }
    public CallWindow(int id = 0)
    {
        this.id = id;
        ButtonText = id == 0 ? "Add" : "Update"; // Determine button text based on ID
        DataContext = this;
        InitializeComponent(); // Ensure this method is defined in the generated partial class
        LoadCall(id); // Load call data

    }

    /// <summary>
    /// Load a call's data. Creates a new call if ID is 0; otherwise, fetches details from BL.
    /// </summary>
    private void LoadCall(int id)
    {
        try
        {
            if (id == 0)
            {
                // Create a new call object with default values
                CurrentCall = new BO.Call();
            }
            else
            {
                // Fetch the existing call object from the BL
                CurrentCall = s_bl.call.GetCallDetails(id);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Current call data, bound to UI controls via DependencyProperty.
    /// </summary>
    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

    /// <summary>
    /// Handles Add/Update button click.
    /// Adds a new call or updates existing details.
    /// </summary>
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {

        try
        {
            if (ButtonText == "Add")
            {
                s_bl.call.AddCall(CurrentCall!);
                MessageBox.Show("The call was added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (ButtonText == "Update")
            {
                s_bl.call.UpdatingCallDetails(id, CurrentCall!);
                MessageBox.Show("The call was updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"שגיאה בלתי צפויה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// An observer method that refills the item
    /// </summary>
    private void callObserver()
    {
        int id = CurrentCall!.Id;
        CurrentCall = null;
        CurrentCall = s_bl.call.GetCallDetails(id);

    }

    // Adds an observer to monitor changes to the call data.
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentCall!.Id != 0)
            s_bl.call.AddObserver(CurrentCall!.Id, callObserver);

    }

    // Removes the observer when the window is closed.
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.call.RemoveObserver(CurrentCall!.Id, callObserver);
    }
}

