using BO;
using PL.Volunteer;
using System;
using System.Collections;
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
using System.Windows.Threading;

namespace PL.Call;

/// <summary>
/// Interaction logic for CallListWindow.xaml
/// </summary>
public partial class CallListWindow : Window, INotifyPropertyChanged
{
    // Reference to the business logic layer
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the CallListWindow class.
    /// </summary>
    public CallListWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the list of calls to be displayed.
    /// </summary>
    public IEnumerable<CallInList> CallList
    {
        get { return (IEnumerable<CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CallList. This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the selected field for filtering the call list.
    /// </summary>
    public BO.CallInListFields SelectedFiled { get; set; } = BO.CallInListFields.None;

    /// <summary>
    /// Gets or sets the selected field for sorting the call list.
    /// </summary>
    public BO.CallInListFields SelectedSort { get; set; } = BO.CallInListFields.None;

    /// <summary>
    /// Gets or sets the selected value for filtering the call list.
    /// </summary>
    public object SelectedFilterValue { get; set; } = new object();

    /// <summary>
    /// Gets or sets the selected field for grouping the call list.
    /// </summary>
    public BO.CallInListFields SelectedGroup { get; set; } = BO.CallInListFields.None;

    /// <summary>
    /// Gets or sets the selected call from the DataGrid.
    /// </summary>
    public BO.CallInList? SelectedCall { get; set; }

    /// <summary>
    /// Handles the ComboBox selection change to filter the call list.
    /// </summary>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CallList = (SelectedFiled == BO.CallInListFields.None)
            ? s_bl?.call.CallInLists(null, null, SelectedSort) ?? Enumerable.Empty<CallInList>()
            : s_bl?.call.CallInLists(SelectedFiled, SelectedFilterValue, SelectedSort) ?? Enumerable.Empty<CallInList>();
    }

    /// <summary>
    /// Queries the call list based on the selected filter, sort, and group options.
    /// </summary>
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

        CallList = s_bl?.call.CallInLists(filterField, filterValue, sortField) ?? Enumerable.Empty<CallInList>();
    }

    private volatile DispatcherOperation? _observerOperation = null; //stage 7

    /// <summary>
    /// Observer to update the call list when changes occur.
    /// </summary>
    private void CallListObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                queryCallList();
            });
    }

    /// <summary>
    /// Adds the observer when the window loads.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.call.AddObserver(CallListObserver);

    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
        => s_bl.call.RemoveObserver(CallListObserver);

    /// <summary>
    /// Opens the call details window on double-clicking a row in the DataGrid.
    /// </summary>
    private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow(SelectedCall.CallId).Show();
    }

    /// <summary>
    /// Opens a new window to add a call.
    /// </summary>
    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow().Show();
    }

    /// <summary>
    /// Handles the deletion of a call.
    /// </summary>
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult mbResult =
            MessageBox.Show("Are you sure you want to delete this call?", "Delete",
            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

        if (mbResult != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            if (SelectedCall != null)
                s_bl.call.DeleteCall(SelectedCall.CallId);
            MessageBox.Show("Call deleted successfully", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (BO.BlDoesNotExistException ex)
        {
            MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.BlDeletionImpossible ex)
        {
            MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (BO.UnauthorizedAccessException ex)
        {
            MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Handles the selection change in the DataGrid to update the visibility of the delete button.
    /// </summary>

    /// <summary>
    /// Applies the selected filter to the call list.
    /// </summary>
    private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
    {
        queryCallList();
    }

    /// <summary>
    /// Handles the ComboBox selection change to group the call list.
    /// </summary>
    private void ComboBox_GroupBySelectionChanged(object sender, SelectionChangedEventArgs e)
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

        var comboBox = sender as ComboBox;
        if (comboBox?.SelectedItem is GroupBy selectedField)
        {
            if (selectedField == GroupBy.CallType)
            {
                IEnumerable<CallInList> Calls = s_bl?.call.CallInLists(filterField, filterValue, sortField) ?? Enumerable.Empty<CallInList>();
                IEnumerable<IGrouping<CallType, CallInList>> GroupCalls = Calls.GroupBy(call => call.CallType) ?? Enumerable.Empty<IGrouping<CallType, CallInList>>();
                IEnumerable<CallInList> GroupList = GroupCalls.SelectMany(group => group) ?? Enumerable.Empty<CallInList>();
                CallList = GroupList;
            }
            else if (selectedField == GroupBy.Status)
            {
                IEnumerable<CallInList> Calls = s_bl?.call.CallInLists(filterField, filterValue, sortField) ?? Enumerable.Empty<CallInList>();
                IEnumerable<IGrouping<BO.CallStatus, CallInList>> GroupCalls = Calls.GroupBy(call => call.Status) ?? Enumerable.Empty<IGrouping<BO.CallStatus, CallInList>>();
                IEnumerable<CallInList> GroupList = GroupCalls.SelectMany(group => group) ?? Enumerable.Empty<CallInList>();
                CallList = GroupList;
            }
            else
            {
                CallList = s_bl?.call.CallInLists(filterField, filterValue, sortField) ?? Enumerable.Empty<CallInList>();
            }
        }
    }
}
