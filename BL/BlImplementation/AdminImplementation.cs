using BL.Helpers;
using BlApi;
using BO;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public void ForwardClock(TimeUnit unit)
    {
        ClockManager.UpdateClock(unit switch
        {
            BO.TimeUnit.Hour => ClockManager.Now.AddHours(1),
            BO.TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
            BO.TimeUnit.Year => ClockManager.Now.AddYears(1),
            BO.TimeUnit.Month => ClockManager.Now.AddMonths(1),
            BO.TimeUnit.Day => ClockManager.Now.AddDays(1),
            _ => throw new Exception($"Unhandled time unit:{unit}")
        });

    }

    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }

    public void SetMaxRange(TimeSpan range)
    {
        _dal.Config.RiskRange = range;
    }

    public void ResetDB()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);

    }

    public void InitializeDB()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);

    }
}
