namespace Dal;
using DalApi;

/// <summary>
/// Resets the database by deleting all entities and resetting configurations.
/// </summary>
sealed internal class DalList : IDal
{
    private static readonly Lazy<IDal> lazyInstance = new Lazy<IDal>(() => new DalList());

    public static IDal Instance => lazyInstance.Value;

    /// <summary>
    /// Provides access to CRUD operations for Volunteer entities.
    /// </summary>
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    /// <summary>
    /// Provides access to CRUD operations for Assignment entities.
    /// </summary>
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// <summary>
    /// Provides access to CRUD operations for Call entities.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();

    /// <summary>
    /// Provides access to configuration operations.
    /// </summary>
    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the database by deleting all entities and resetting configurations.
    /// </summary>
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.Reset();

    }
}
