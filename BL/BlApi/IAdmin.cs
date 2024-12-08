namespace BlApi;
/// <summary>
/// Logical Service Entity
/// </summary>
public interface IAdmin
{
    public DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
    TimeSpan GetMaxRange();
    void SetMaxRange(TimeSpan range);
    void ResetDB();
    void InitializeDB();
}
