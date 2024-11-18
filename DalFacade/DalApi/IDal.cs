namespace DalApi;

/// <summary>
/// Interface representing the Data Access Layer (DAL).
/// Provides access to various entity-related interfaces and database operations.
/// </summary>
public interface IDal
{
    /// <summary>
    /// Provides access to CRUD operations for Volunteer entities.
    /// </summary>
    IVolunteer Volunteer { get; }
    /// <summary>
    /// Provides access to CRUD operations for Assignment entities.
    /// </summary>
    IAssignment Assignment { get; }
    /// <summary>
    /// Provides access to CRUD operations for Call entities.
    /// </summary>
    ICall Call { get; }
    /// <summary>
    /// Provides access to configuration data and operations.
    /// </summary>
    IConfig Config { get; }
    /// <summary>
    /// Resets the database to its initial state.
    /// Clears or reinitializes all stored data.
    /// </summary>
    void ResetDB();
}
