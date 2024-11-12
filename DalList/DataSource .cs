namespace Dal;

/// <summary>
/// Represents a static data source for managing collections of entities.
/// </summary>
internal static class DataSource
{
    internal static List<DO.Call> Calls { get; } = new();
    internal static List<DO.Assignment> Assignments { get; } = new();
    internal static List<DO.Volunteer> Volunteers { get; } = new();
}

