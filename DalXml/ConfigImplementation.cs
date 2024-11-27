namespace Dal;
using DalApi;

/// <summary>
/// Implementation of the IConfig interface to provide access to the system configuration settings.
/// </summary>
internal class ConfigImplementation : IConfig
{
    public DateTime Clock { get => Config.Clock; set => Config.Clock = value; }
    public void Reset() => Config.Reset();
    TimeSpan IConfig.RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }

    /// <summary>
    /// Gets the next available call ID from the configuration. 
    /// </summary>
    public int nextCallId
    {
        get => Config.NextCallId;
    }
    /// <summary>
    /// Gets the next available assignment ID from the configurationb. 
    ///</summary> 
    public int nextAssignmentId
    {
        get => Config.NextAssignmentId;
    }
}
