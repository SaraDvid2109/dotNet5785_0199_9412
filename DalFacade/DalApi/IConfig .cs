namespace DalApi;

/// <summary>
/// Interface for managing configuration settings in the Data Access Layer (DAL).
/// Provides properties and methods for managing global configurations.
/// </summary>
public interface IConfig
{
   DateTime Clock { get; set; }
   void Reset();
   TimeSpan RiskRange { get; set; }
}
