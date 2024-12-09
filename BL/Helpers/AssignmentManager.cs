using DalApi;

namespace Helpers;

internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static IEnumerable<DO.Assignment> findAssignment(int id)
    {
        return s_dal.Assignment.ReadAll()
                     .Where(a => a.CallId == id);
    }
    // כל המתודות במחלקה יהיו internal static
}
