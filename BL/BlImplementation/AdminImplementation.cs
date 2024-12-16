using BL.Helpers;
using BlApi;
using BO;

namespace BlImplementation;
/// <summary>
/// 
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
        return ClockManager.Now;
    }
    /// <summary>
    /// Advances the system clock by the specified time unit.
    /// </summary>
    /// <param name="unit">The unit of time by which to advance the clock.</param>
    /// <exception cref="BO.BlFormatException">Thrown when an unhandled time unit is provided.</exception>
    public void ForwardClock(TimeUnit unit)
    {
        ClockManager.UpdateClock(unit switch
        {
            BO.TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
            BO.TimeUnit.Hour => ClockManager.Now.AddHours(1),
            BO.TimeUnit.Day => ClockManager.Now.AddDays(1),
            BO.TimeUnit.Month => ClockManager.Now.AddMonths(1),
            BO.TimeUnit.Year => ClockManager.Now.AddYears(1),
            _ => throw new BO.BlFormatException($"Unhandled time unit:{unit}")
        });

    }
    /// <summary>
    /// Returns the maximum range for risk calculations.
    /// </summary>
    /// <returns>Returns a value representing the configured maximum risk range.</returns>
    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }
    /// <summary>
    /// Sets the maximum range for risk calculations.
    /// </summary>
    /// <param name="range">The  value to set as the maximum risk range.</param>
    public void SetMaxRange(TimeSpan range)
    {
        _dal.Config.RiskRange = range;
    }
    /// <summary>
    /// Resets the database to its initial state and synchronizes the system clock.
    /// </summary>
    public void ResetDB()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);

    }
    /// <summary>
    /// Initializes the database  and synchronizes the system clock.
    /// </summary>
    public void InitializeDB()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);

    }
}
