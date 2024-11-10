namespace Dal;

internal static class Config
{
    internal const int StartCallId = 0;
    private static int nextCallId = StartCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal static int NextAssignmentId { get => nextAssignmentId++; }
    internal const int StartAssignmentId = 0;
    private static int nextAssignmentId = StartAssignmentId;

    internal static  DateTime Clock { get; set; } = DateTime.Now;

    internal static  TimeSpan RiskRange;


    internal static void Reset()
    {
        nextCallId = StartCallId;
        nextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;
      
    }

}
