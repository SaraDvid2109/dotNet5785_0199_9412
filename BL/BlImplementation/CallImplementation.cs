namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

/// <summary>
/// Implementation of the logical service entity interface for call management
/// </summary>
internal class CallImplementation : ICall
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Retrieves the quantities of calls grouped by their status.
    /// </summary>
    /// <returns>An array where each index represents a call status and the value represents the count of calls with that status.</returns>
    public IEnumerable<int> CallQuantities()
    {
        IEnumerable<DO.Call> ListCall;
        lock (AdminManager.BlMutex) //stage 7
             ListCall = _dal.Call.ReadAll();
        var groupedCalls = ListCall
        .GroupBy(call => CallManager.Status(call.Id))
        .ToDictionary(group => group.Key, group => group.Count());
        // Create an array to store the counts for each call type
        int maxTypeOfCallValue = Enum.GetValues(typeof(BO.CallStatus)).Cast<int>().Max();
        int[] result = new int[maxTypeOfCallValue + 1];
        foreach (var group in groupedCalls)
        {
            result[(int)group.Key] = group.Value;
        }

        return result;
      
    }

    /// <summary>
    /// Returns a list of calls with optional filtering and sorting.
    /// </summary>
    /// <param name="filter">The field to filter the calls by (optional).</param>
    /// <param name="value">The value to filter the calls with (optional).</param>
    /// <param name="sort">The field to sort the calls by (optional).</param>
    /// <returns>A sorted and/or filtered list of calls in the BO.CallInList format.</returns>
    public IEnumerable<BO.CallInList> CallInLists(BO.CallInListFields? filter, object? value, BO.CallInListFields? sort)
    {
        IEnumerable<DO.Call> calls;
        //IEnumerable<IGrouping<object, DO.Call>> groupedCalls;
        IEnumerable<BO.CallInList> sortCalls;
        List<BO.CallInList> callInLists = new List<BO.CallInList>();
        lock (AdminManager.BlMutex) //stage 7
        {
            calls = _dal.Call.ReadAll();
            foreach (var c in calls)
            {
                var assignment = _dal.Assignment.ReadAll(a => a.CallId == c.Id);
                callInLists.Add(CallManager.ConvertFromDOToCallInList(c, assignment));
            }
        }
        var filterCalls = CallManager.FilterCallInList(callInLists, value, filter);
       
        {
            sortCalls = sort switch
            {
                BO.CallInListFields.Id => filterCalls.OrderBy(c => c.Id),
                BO.CallInListFields.CallId => filterCalls.OrderBy(c => c.CallId),
                BO.CallInListFields.CallType => filterCalls.OrderBy(c => c.CallType),
                BO.CallInListFields.OpenTime => filterCalls.OrderBy(c => c.OpenTime),
                BO.CallInListFields.TimeLeftToFinish => filterCalls.OrderBy(c => c.TimeLeftToFinish),
                BO.CallInListFields.LastVolunteer => filterCalls.OrderBy(c => c.LastVolunteer),
                BO.CallInListFields.TreatmentTimeLeft => filterCalls.OrderBy(c => c.TreatmentTimeLeft),
                BO.CallInListFields.Status => filterCalls.OrderBy(c => c.Status),
                BO.CallInListFields.TotalAssignments => filterCalls.OrderBy(c => c.TotalAssignments),
                _ => filterCalls.OrderBy(c => c.CallId)
            };
        }

        return sortCalls;

        
    }

    /// <summary>
    /// Returns the details of a specific call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call </param>
    /// <returns> the call details.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if no call with the specified ID exists.</exception>
    public BO.Call GetCallDetails(int id)
    {
        DO.Call? doCall;
        IEnumerable<DO.Assignment> doAssignments;

        lock (AdminManager.BlMutex) //stage 7
            doCall = _dal.Call.Read(id);
        if (doCall == null)
            throw new BO.BlDoesNotExistException("There is no call with this ID.");
        lock (AdminManager.BlMutex) //stage 7
            doAssignments = _dal.Assignment.ReadAll()
                   .Where(a => a.CallId == id);
        BO.Call boCall;
        lock (AdminManager.BlMutex) //stage 7
        {
            boCall = new BO.Call
            {
                Id = doCall.Id,
                CarTypeToSend = (BO.CallType)doCall.CarTypeToSend,
                Description = doCall.Description,
                Address = doCall.Address,
                Latitude = doCall.Latitude,
                Longitude = doCall.Longitude,
                OpenTime = doCall.OpenTime,
                MaxTime = doCall.MaxTime,
                Status = CallManager.Status(doCall.Id),
           
                ListAssignmentsForCalls = doAssignments.Select(a => new BO.CallAssignInList
                {

                    VolunteerId = a.VolunteerId,
                    Name = _dal.Volunteer.Read(a.VolunteerId)?.Name,
                    EnterTime = a.EnterTime,
                    EndTime = a.EndTime,
                    TypeEndOfTreatment = a.TypeEndOfTreatment.HasValue ? (BO.EndType)a.TypeEndOfTreatment : null

                }).ToList()
            };
        }

        return boCall;
    }
   
    /// <summary>
    /// Updates the details of an existing call.
    /// </summary>
    /// <param name="id">The ID of the call to update.</param>
    /// <param name="call">The updated call details.</param>
    /// <exception cref="BO.BlFormatException">Thrown if the provided call data is in an invalid format (e.g., missing address).</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call with the specified ID does not exist in the database.</exception>
    public void UpdatingCallDetails(int id, BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        if (call.Address == null)
        {
            throw new BO.BlFormatException("Invalid address.");
        }
        //var coordinates = Tools.GetAddressCoordinates(call.Address);
        //call.Latitude = coordinates.Latitude;
        //call.Longitude = coordinates.Longitude;

        CallManager.IntegrityCheck(call);

        try
        {
            DO.Call DOCall = new DO.Call(
           /*call.Id*/id,
           call.Description,
           call.Address ?? string.Empty,
           call.Latitude,
           call.Longitude,
           call.OpenTime,
           call.MaxTime,
           (DO.CallType)call.CarTypeToSend);
           lock (AdminManager.BlMutex) //stage 7
              _dal.Call.Update(DOCall);
            CallManager.Observers.NotifyItemUpdated(DOCall.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
            _=UpdateCallFieldsAsync(DOCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error:" + ex);
        }
    }

    /// <summary>
    /// Deletes a call by its ID if it meets the necessary conditions.
    /// </summary>
    /// <param name="id">The ID of the call to be deleted.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if no call exists with the specified ID.</exception>
    /// <exception cref="BO.UnauthorizedAccessException">Thrown if the call cannot be deleted due to existing assignments or the call's status.</exception>
    /// <exception cref="BO.BlNullPropertyException">Thrown if no assignments are found for the call.</exception>
    public void DeleteCall(int id)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            IEnumerable<DO.Assignment> assignments;
            DO.Call? callToDelete;
            lock (AdminManager.BlMutex) //stage 7
                   callToDelete = _dal.Call.Read(id);
            if (callToDelete == null) throw new BO.BlDoesNotExistException("There is no call with this ID.");
            lock (AdminManager.BlMutex) //stage 7
                assignments = _dal.Assignment.ReadAll();
            

            if (assignments != null)
            {
                var assignmentsOfCall = from assignment in assignments
                                        where assignment.CallId == id
                                        select assignment;
                if (!assignmentsOfCall.Any() && (CallManager.Status(id) == BO.CallStatus.Open || CallManager.Status(id) == BO.CallStatus.OpenAtRisk))
                {
                    lock (AdminManager.BlMutex) //stage 7
                       _dal.Call.Delete(id);
                    CallManager.Observers.NotifyListUpdated();  //stage 5
                }
                else
                    throw new BO.UnauthorizedAccessException("You cannot delete this call.");
            }
            else
                throw new BO.BlNullPropertyException("no assignments");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error deleting volunteer:", ex);
        }
    }
  
    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="call">The call object containing the details of the call to be added.</param>
    /// <exception cref="BO.BlFormatException">Thrown if the address is invalid.</exception>
    /// <exception cref="BO.BllAlreadyExistException">Thrown if a call with the same ID already exists.</exception>
    public void AddCall(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        if (call.Address == null)
        {
            throw new BO.BlFormatException("Invalid address.");
        }
        //var coordinates = Tools.GetAddressCoordinates(call.Address);
        //call.Latitude = coordinates.Latitude;
        //call.Longitude = coordinates.Longitude;
        Helpers.CallManager.IntegrityCheck(call);
        try
        {
            DO.Call callToAdd = new DO.Call(
                call.Id,
                call.Description,
                call.Address,
                call.Latitude,
                call.Longitude,
                call.OpenTime,
                call.MaxTime,
                (DO.CallType)call.CarTypeToSend);
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Create(callToAdd);
            CallManager.Observers.NotifyListUpdated();  //stage 5
           _= UpdateCallFieldsAsync(callToAdd);
        }
        catch (DO.DalAlreadyExistException ex)
        { throw new BO.BllAlreadyExistException("Error creating call", ex); }
    }
    
    /// <summary>
    /// Returns a list of closed calls handled by a specific volunteer, with optional filtering and sorting.
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer whose closed calls are being retrieved.</param>
    /// <param name="filter">The type of the call to filter the closed calls (RegularVehicle, Ambulance, IntensiveCareAmbulance, None).</param>
    /// <param name="sortBy">The field by which to sort the list of closed calls (e.g., ID, Address, EnterTime, etc.).</param>
    /// <returns>Sorted and filtered list of closed calls handled by the volunteer.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer with the specified ID does not exist.</exception>
    public IEnumerable<BO.ClosedCallInList> closedCallsHandledByVolunteer(int VolunteerId, BO.CallType? filter, BO.ClosedCallInListField? sortBy)
    {
        IEnumerable<DO.Assignment> assignment;
        IEnumerable<DO.Call> calls;
        DO.Volunteer? volunteer;

        lock (AdminManager.BlMutex) //stage 7
            volunteer = _dal.Volunteer.Read(VolunteerId);
        if (volunteer == null)
             throw new BO.BlDoesNotExistException($"Volunteer with {VolunteerId} not found");
        lock (AdminManager.BlMutex) //stage 7
            assignment = _dal.Assignment.ReadAll();
            //All the ID of the calls the volunteer took
            var callsVolunteer = from a in assignment
                                 where a.VolunteerId == VolunteerId
                                 select a.CallId;
        //All calls of the volunteer received as a parameter
        lock (AdminManager.BlMutex) //stage 7
            calls = _dal.Call.ReadAll(c => callsVolunteer.Contains(c.Id));
        
        var fitCalls = from call in calls
                       where (CallManager.Status(call.Id) == BO.CallStatus.Close
                                                      || CallManager.Status(call.Id) == BO.CallStatus.Expired)
                       select call;
        //filter to calls of volunteer using assignment
        var filterCalls = CallManager.Filter(fitCalls, filter);
        
         //var finalListCalls = CallManager.SortClosedCall(callsForList, sortBy);
         var sortedCall = sortBy switch
        {
            BO.ClosedCallInListField.Id => filterCalls.OrderBy(c => c.Id),
            BO.ClosedCallInListField.CallType => filterCalls.OrderBy(c => c.CarTypeToSend),
            BO.ClosedCallInListField.Address => filterCalls.OrderBy(c => c.Address),
            BO.ClosedCallInListField.OpenTime => filterCalls.OrderBy(c => c.OpenTime),
            BO.ClosedCallInListField.EnterTime => filterCalls.OrderBy(c =>
            assignment.FirstOrDefault(a => a.CallId == c.Id)?.EnterTime),
            BO.ClosedCallInListField.EndTime => filterCalls.OrderBy(c =>
            assignment.FirstOrDefault(a => a.CallId == c.Id)?.EndTime),
            BO.ClosedCallInListField.TypeEndOfTreatment => filterCalls.OrderBy(c =>
            assignment.FirstOrDefault(a => a.CallId == c.Id)?.TypeEndOfTreatment),

            _ => filterCalls.OrderBy(c => c.Id)
        };
        return sortedCall.Select(call => CallManager.ToBOClosedCall(call));
    }
    
    /// <summary>
    /// Returns a list of open calls available for selection by a specific volunteer, with optional filtering and sorting.
    /// </summary>
    /// <param name="id">The ID of the volunteer whose open calls are being retrieved.</param>
    /// <param name="filter">The type of the call to filter the open calls (RegularVehicle, Ambulance, IntensiveCareAmbulance, None).</param>
    /// <param name="sortBy">The field by which to sort the list of open calls (e.g., ID, Address, OpenTime, etc.).</param>
    /// <returns>Sorted and filtered list of open calls available for selection by the volunteer.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer with the specified ID does not exist.</exception>
    /// <exception cref="BlNullPropertyException">Thrown if the volunteer's address is null.</exception>
    public IEnumerable<BO.OpenCallInList> openCallsForSelectionByVolunteer(int id, BO.CallType? filter, BO.OpenCallInListField? sortBy)
    {
        return CallManager.openCallsForSelectionByVolunteer(id, filter, sortBy);

    }
   
    /// <summary>
    /// Updates the end of treatment details for a specific assignment handled by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer who is updating the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment to be updated.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer or the assignment with the specified ID does not exist.</exception>
    /// <exception cref="BO.UnauthorizedAccessException">Thrown if the volunteer does not have permission to update the assignment or if the assignment cannot be updated due to existing end time or treatment type.</exception>
    public void UpdateEndOfTreatmentCall(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.UpdateEndOfTreatmentCall(volunteerId,assignmentId);
    }
    
    /// <summary>
    /// Cancels the handling of a specific assignment by a volunteer or an administrator.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer attempting to cancel the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment to be canceled.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer or the assignment with the specified ID does not exist.</exception>
    /// <exception cref="BO.UnauthorizedAccessException">Thrown if the volunteer does not have permission to cancel the assignment or if the assignment cannot be canceled due to existing end time or treatment type.</exception>
    public void CancelCallHandling(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.CancelCallHandling(volunteerId,assignmentId);
    }
    
    /// <summary>
    /// Assigns a specific call to a volunteer for handling.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer choosing to handle the call.</param>
    /// <param name="callId">The ID of the call to be handled by the volunteer.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer or the call with the specified ID does not exist.</exception>
    /// <exception cref="BO.BlOperationNotAllowedException">Thrown if the volunteer is already handling another call or if the call is not available for handling.</exception>
    public void ChooseCallForHandling(int volunteerId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.ChooseCallForHandling(volunteerId, callId);
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


    private async Task UpdateCallFieldsAsync(DO.Call call)
    {
        try
        {
            var coordinate = await Helpers.Tools.GetAddressCoordinates(call.Address); // קריאה אסינכרונית
            call = call with { Latitude = coordinate.Latitude, Longitude = coordinate.Longitude };
            lock (AdminManager.BlMutex)
                _dal.Call.Update(call);
            CallManager.Observers.NotifyItemUpdated(call.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating call location for ID {call.Id}: {ex.Message}");
        }
    }
}
