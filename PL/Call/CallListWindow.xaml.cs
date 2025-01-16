using BO;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
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
/// Interaction logic for CallListWindow.xaml
/// </summary>
public partial class CallListWindow : Window, INotifyPropertyChanged
{
    // Reference to the business logic layer
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public event PropertyChangedEventHandler? PropertyChanged;

    public CallListWindow()
    {
        InitializeComponent();
    }

    public IEnumerable<CallInList> CallList
    {
        get { return (IEnumerable<CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    // Using a DependencyProperty as the backing store for VolunteerList.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

    // Selected filter from ComboBox
    public BO.CallInListFields SelectedFiled { get; set; } = BO.CallInListFields.None;
    public BO.CallInListFields SelectedSort { get; set; } = BO.CallInListFields.None;
    public object SelectedFilterValue { get; set; } = new object();

    // Selected volunteer from DataGrid
    public BO.CallInList? SelectedCall { get; set; }

    //Handle ComboBox selection change to filter the call list

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // וודאי שהערך שנבחר מהקומבו בוקס נשמר
        var comboBox = sender as ComboBox;
        var selectedValue = comboBox?.SelectedItem;

        // עדכון קריאה לפונקציה CallInLists
        CallList = (SelectedFiled == BO.CallInListFields.None)
            ? s_bl?.call.CallInLists(null, null, SelectedSort) ?? Enumerable.Empty<CallInList>()
            : s_bl?.call.CallInLists(SelectedFiled, selectedValue, SelectedSort) ?? Enumerable.Empty<CallInList>();
    }

    private void queryCallList()
    {
        var filterField = SelectedFiled != BO.CallInListFields.None
            ? (BO.CallInListFields?)SelectedFiled
            : null;

        var filterValue = !string.IsNullOrEmpty(SelectedFilterValue?.ToString())
            ? SelectedFilterValue
            : null;

        var sortField = SelectedSort != BO.CallInListFields.None
            ? (BO.CallInListFields?)SelectedSort
            : null;

        // קריאה לפונקציה עם הפרמטרים המסוננים
        CallList = s_bl?.call.CallInLists(filterField, filterValue, sortField) ?? Enumerable.Empty<CallInList>();
    }



    //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    CallList = (SelectedFiled == BO.CallInListFields.None) ?
    //        s_bl?.call.CallInLists(null, null, null)! : s_bl?.call.CallInLists(SelectedFiled, sender, SelectedSort)!;
    //}

    ////Query the call list based on the selected filter
    //private void queryCallList()
    //{
    //    // הגדרה של פרמטרים למיון וסינון
    //    var filterField = SelectedFiled != BO.CallInListFields.None ? (BO.CallInListFields?)SelectedFiled : null;
    //    var sortField = BO.CallInListFields.OpenTime; // דוגמה לשדה ברירת מחדל למיון

    //    // קריאה לפונקציה עם פרמטרים מתאימים
    //    CallList = s_bl?.call.CallInLists(filterField, null, SelectedSort);

    //}


    // Observer to update the volunteer list when changes occur
    private void CallListObserver()
        => queryCallList();

    // Add observer when the window loads
    private void Window_Loaded(object sender, RoutedEventArgs e)
     => s_bl.call.AddObserver(CallListObserver);

    // Remove observer when the window is closed
    private void Window_Closed(object sender, EventArgs e)
        => s_bl.call.RemoveObserver(CallListObserver);

    // Open the volunteer details window on double-clicking a row
    //private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
    //{
    //    if (SelectedCall != null)
    //        new CallWindow(SelectedCall.Id).Show();
    //}

    // Open a new window to add a volunteer

    private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow(SelectedCall.CallId).Show();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow().Show();
    }

    // Handle deletion of a volunteer
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int callId)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this call?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.call.DeleteCall(callId);
                    MessageBox.Show("Call deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    queryCallList(); // Refresh the call list after deletion
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedCall != null)
        {
            UpdateDeleteButtonVisibility(SelectedCall.CallId);
        }
    }

    public void UpdateDeleteButtonVisibility(int id)
    {
        try
        {
            var callToCheck = s_bl.call.GetCallDetails(id); // קריאה מהשכבה הלוגית
            IsDeleteButtonVisible = callToCheck != null && callToCheck.Status == BO.CallStatus.Open;
        }
        catch
        {
            IsDeleteButtonVisible = false; // הסתרה במקרים של בעיה
        }
    }

    private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
    {
        queryCallList();
    }

    //public IEnumerable<string> FilterOptions { get; set; } = Enum.GetNames(typeof(BO.CallInListFields));
    //public IEnumerable<string> SortOptions { get; set; } = Enum.GetNames(typeof(BO.CallInListFields));
    //public IEnumerable<string> GroupOptions { get; set; } = Enum.GetNames(typeof(BO.CallInListFields));

    //public BO.CallInListFields? SelectedFilter { get; set; }
    //public string FilterValue { get; set; } = string.Empty;
    //public BO.CallInListFields? SelectedSort { get; set; }
    //public BO.CallInListFields? SelectedGroup { get; set; }

    //// עדכון נתונים בהתבסס על בחירות המשתמש
    //private void UpdateCallList()
    //{
    //    var filter = SelectedFilter;
    //    var value = string.IsNullOrEmpty(FilterValue) ? null : (object)FilterValue;
    //    var sort = SelectedSort;
    //    var groupBy = SelectedGroup;

    //    CallList = s_bl.call.CallInLists(filter, value, sort, groupBy).SelectMany(g => g); // ביטול הקיבוץ להצגה בדאטהגריד
    //}

    //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    UpdateCallList();
    //}


}
