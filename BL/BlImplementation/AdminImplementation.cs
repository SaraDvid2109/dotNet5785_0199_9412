using BlApi;
using BO;
using Helpers;

namespace BlImplementation;

/// <summary>
/// Implementation of the logical service entity interface for managing administrative tasks
/// </summary>
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Returns the current clock time managed by ClockManager.
    /// </summary>
    /// <returns>The current  value representing the system's configured clock time.</returns>
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }
    /// <summary>
    /// Advances the system clock by the specified time unit.
    /// </summary>
    /// <param name="unit">The unit of time by which to advance the clock.</param>
    /// <exception cref="BO.BlFormatException">Thrown when an unhandled time unit is provided.</exception>
    public void ForwardClock(TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.UpdateClock(unit switch
        {
            BO.TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
            BO.TimeUnit.Hour => AdminManager.Now.AddHours(1),
            BO.TimeUnit.Day => AdminManager.Now.AddDays(1),
            BO.TimeUnit.Month => AdminManager.Now.AddMonths(1),
            BO.TimeUnit.Year => AdminManager.Now.AddYears(1),
            _ => throw new BO.BlFormatException($"Unhandled time unit:{unit}")
        });

    }

    /// <summary>
    /// Returns the maximum range for risk calculations.
    /// </summary>
    /// <returns>Returns a value representing the configured maximum risk range.</returns>
    public TimeSpan GetMaxRange() => AdminManager.RiskRange;

    /// <summary>
    /// Sets the maximum range for risk calculations.
    /// </summary>
    /// <param name="range">The  value to set as the maximum risk range.</param>
    public void SetMaxRange(TimeSpan range) 
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.RiskRange = range;
        CallManager.Observers.NotifyListUpdated();
    } 

    /// <summary>
    /// Resets the database to its initial state and synchronizes the system clock.
    /// </summary>
    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }

    /// <summary>
    /// Initializes the database  and synchronizes the system clock.
    /// </summary>
    public void InitializeDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7
    }

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }

    public void StopSimulator()
    => AdminManager.Stop(); //stage 7

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
}
