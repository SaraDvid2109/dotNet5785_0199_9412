namespace Dal;

/// <summary>
/// Static class for managing configuration settings and shared counters for unique identifiers.
/// </summary>
internal static class Config
{
    internal const int StartCallId = 0; // The starting ID for Call entities.
    private static int nextCallId = StartCallId; // Counter for generating the next unique ID for Call entities.
    internal static int NextCallId { get => nextCallId++; } // Gets the next unique ID for Call entities and increments the counter.

    internal static int NextAssignmentId { get => nextAssignmentId++; } // The starting ID for Assignment entities.
    internal const int StartAssignmentId = 0; // Counter for generating the next unique ID for Assignment entities.
    private static int nextAssignmentId = StartAssignmentId; // Gets the next unique ID for Assignment entities and increments the counter.

    internal static  DateTime Clock { get; set; } = DateTime.Now; // The current time in the system, used as a clock for managing time-related operations.

    internal static  TimeSpan RiskRange; // The current time in the system, used as a clock for managing time-related operations.

    /// <summary>
    /// Resets the configuration to its initial state.
    /// Resets counters for Call and Assignment IDs and sets the Clock to the current time.
    /// </summary>
    internal static void Reset()
    {
        nextCallId = StartCallId;
        nextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;    
        RiskRange = TimeSpan.Zero;
    }
}
