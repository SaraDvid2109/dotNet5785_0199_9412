namespace BlApi;

/// <summary>
/// Logical service entity interface for managing administrative tasks
/// </summary>
public interface IAdmin
{
    /// <summary>
    /// Returns the current clock time managed by ClockManager.
    /// </summary>
    /// <returns>The current  value representing the system's configured clock time.</returns>
    public DateTime GetClock();
    /// <summary>
    /// Advances the system clock by the specified time unit.
    /// </summary>
    /// <param name="unit">The unit of time by which to advance the clock.</param>
    void ForwardClock(BO.TimeUnit unit);
    /// <summary>
    /// Returns the maximum range for risk calculations.
    /// </summary>
    /// <returns>Returns a value representing the configured maximum risk range.</returns>
    TimeSpan GetMaxRange();
    /// <summary>
    /// Sets the maximum range for risk calculations.
    /// </summary>
    /// <param name="range">The  value to set as the maximum risk range.</param>
    void SetMaxRange(TimeSpan range);
    /// <summary>
    /// Resets the database to its initial state and synchronizes the system clock.
    /// </summary>
    void ResetDB();
    /// <summary>
    /// Initializes the database  and synchronizes the system clock.
    /// </summary>
    void InitializeDB();
}
