﻿using DalApi;
using DO;

namespace Helpers;

/// <summary>
/// The <c>AssignmentManager</c> class provides helper methods for managing assignments.
/// It interacts with the data access layer (DAL) to retrieve and manage assignment data.
/// </summary>
internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5 

    public static IEnumerable<DO.Assignment> findAssignment(int id)
    {
        IEnumerable<Assignment> assignment;
        lock (AdminManager.BlMutex) //stage 7
        {
           assignment=s_dal.Assignment.ReadAll()
                     .Where(a => a.CallId == id);
        }
        return assignment;
    }
}
