using System.Threading.Tasks;
namespace BlApi;

/// <summary>
/// A main logical interface that centralizes access to all logical service entities (interfaces)
/// </summary>
public interface IBl
{
    /// <summary>
    /// Logical service entity interface for managing volunteers.
    /// </summary>
    IVolunteer volunteer { get; }
    /// <summary>
    /// Logical service entity interface for managing calls.
    /// </summary>
    ICall call { get; }
    /// <summary>
    /// Logical service entity interface for managing administrative tasks
    /// </summary>
    IAdmin Admin { get; }
}
