using DalApi;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public void Reset()
    {
        Config.Reset();
    }
   TimeSpan IConfig.RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }//
}
