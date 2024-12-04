namespace BlApi;
/// <summary>
/// Logical Service Entity
/// </summary>
public interface IAdmin
{
    public DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
    int GetMaxRange();
    void SetMaxRange(int maxRange);
    void ResetDB();
    void InitializeDB();
}
