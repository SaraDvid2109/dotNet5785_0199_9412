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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // check i add it by myself

    private int id;
    public string ButtonText { get; set; }
    public VolunteerWindow(int id = 0)
    {
        this.id = id;
        ButtonText = id == 0 ? "Add" : "Update";
        DataContext = this;
        InitializeComponent();
        LoadVolunteer(id);

    }

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

    public BO.Volunteer? CurrentVolunteer
    {
        get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        
        try
        {
            if (ButtonText == "Add")
            {
                // ביצוע הוספה
                s_bl.volunteer.AddVolunteer(CurrentVolunteer!);
                MessageBox.Show("הסטודנט נוסף בהצלחה!");
            }
            else if (ButtonText == "Update")
            {
                // ביצוע עדכון
                s_bl.volunteer.UpdatingVolunteerDetails(id,CurrentVolunteer!);
                MessageBox.Show("הסטודנט עודכן בהצלחה!");
            }

            // לאחר ההוספה/עדכון, סגור את החלון
            this.Close();
        }
        catch (Exception ex)
        {
            // כל חריגה אחרת
            MessageBox.Show($"שגיאה בלתי צפויה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    /// <summary>
    /// An observer method that refills the item
    /// </summary>
    private void VolunteerObserver() 
    {
        int id = CurrentVolunteer!.Id;
        CurrentVolunteer = null;
        CurrentVolunteer = s_bl.volunteer.GetVolunteerDetails(id);

    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {


        if (CurrentVolunteer!.Id != 0)
            s_bl.volunteer.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);

    }

    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.volunteer.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
    }
}

