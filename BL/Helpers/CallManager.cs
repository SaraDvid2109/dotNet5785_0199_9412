using BlApi;
using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Net;
namespace Helpers;

/// <summary>
/// The <c>CallManager</c> class provides helper methods for managing and processing calls.
/// It interacts with the data access layer (DAL) to perform operations related to calls and assignments.
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Performs an integrity check on the provided call, validating its fields and ensuring it meets business logic constraints.
    /// </summary>
    /// <param name="call">The call to be validated.</param>
    public static void IntegrityCheck(BO.Call call)
    {
        if (!Enum.IsDefined(typeof(BO.CallType), call.CarTypeToSend))
        {
            throw new BO.BlFormatException("Invalid CallType format.");
        }

        if (call.OpenTime > DateTime.Now)
        {
            throw new BO.BlFormatException("Invalid OpenTime format.");
        }

        if (call.MaxTime.HasValue)
        {
            TimeSpan timeDifference = call.MaxTime.Value - call.OpenTime;
            if (timeDifference.TotalMinutes < 5 || timeDifference.TotalMinutes > 30)
            {
                throw new BO.BlFormatException("Invalid MaxTime format.");
            }
        }
        else
        {
            throw new BO.BlFormatException("MaxTime cannot be null.");
        }

        if (!Enum.IsDefined(typeof(BO.CallStatus), call.Status))
        {
            throw new BO.BlFormatException("Invalid CallStatus format.");
        }

        if (string.IsNullOrEmpty(call.Address))
        {
            throw new BO.BlFormatException("Address cannot be null or empty.");
        }

        //var coordinates = Helpers.Tools.GetAddressCoordinates(call.Address);
        //if (coordinates == (0, 0))
        //{
        //    throw new BO.BlFormatException("Invalid address.");
        //}
        //if (call.Latitude != coordinates.Latitude)
        //{
        //    throw new BO.BlFormatException("Invalid Latitude.");
        //}
        //if (call.Longitude != coordinates.Longitude)
        //{
        //    throw new BO.BlFormatException("Invalid Longitude.");
        //}
    }

    /// <summary>
    /// Returns the status of a specific call based on its assignments and current time.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>The status of the call as an enum value of type <see cref="BO.CallStatus"/>.</returns>
    public static BO.CallStatus Status(int callId) //stage 4
    {
        DO.Call? call;
        lock (AdminManager.BlMutex) //stage 7
            call = s_dal.Call.Read(callId);
        var assignments = GetAssignmentCall(callId);
        if (assignments == null || !assignments.Any())
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                if ((AdminManager.Now <= call!.MaxTime) && (call!.MaxTime - AdminManager.Now <= s_dal.Config.RiskRange))
                    return BO.CallStatus.OpenAtRisk;

                else
                {
                    if (AdminManager.Now <= call!.MaxTime)
                        return BO.CallStatus.Open;
                    else
                        return BO.CallStatus.Expired;
                }
            }
        }
        else
        {
            var assignment = GetLastAssignment(assignments);
            if (assignment == null)
            {
                // Handle the null case appropriately
                throw new BO.BlNullReferenceException("Assignment does not exist");
            }
            if (assignment.TypeEndOfTreatment == null || assignment.EndTime == null)
            {
                lock (AdminManager.BlMutex) //stage 7
                {
                    if ((call!.MaxTime - AdminManager.Now) <= s_dal.Config.RiskRange)
                        return BO.CallStatus.TreatmentOfRisk;
                    else
                        return BO.CallStatus.Treatment;
                }
            }
            else
            {
                if (assignment.TypeEndOfTreatment == DO.EndType.Treated)
                    return BO.CallStatus.Close;
                else
                {
                    if (assignment.TypeEndOfTreatment == DO.EndType.SelfCancellation)
                        return BO.CallStatus.Open;
                    else
                        return BO.CallStatus.Expired;
                }
            }
        }
    }

    /// <summary>
    /// Retrieves all assignments for a given call ID.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>A collection of <see cref="DO.Assignment"/> related to the call.</returns>
    public static IEnumerable<DO.Assignment>? GetAssignmentCall(int callId)
    {
        IEnumerable<DO.Assignment>? assignments;
        lock (AdminManager.BlMutex) //stage 7
            assignments = s_dal.Assignment.ReadAll();
        if (assignments != null)
            assignments = from assignment in assignments
                          where assignment.CallId == callId
                          select assignment;

        return assignments;
    }

    /// <summary>
    /// Retrieves the last assignment of a call from a list of assignments.
    /// </summary>
    /// <param name="assignments">The list of assignments to search through.</param>
    /// <returns>The last <see cref="DO.Assignment"/> or null if no assignment exists.</returns>
    public static DO.Assignment? GetLastAssignment(IEnumerable<DO.Assignment>? assignments)
    {
        if (assignments == null)
            return null;
        return assignments.OrderByDescending(a => a.Id).FirstOrDefault();
        //return assignments.OrderByDescending(a => a.TypeEndOfTreatment).FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the name of the last volunteer assigned to a specific call.
    /// </summary>
    /// <param name="call">The call for which the volunteer is being queried.</param>
    /// <returns>The name of the last volunteer assigned to the call.</returns>
    public static string getLastVolunteer(DO.Call call)
    {
        var assignments = AssignmentManager.findAssignment(call.Id);
        var last = assignments.MaxBy(a => a.EnterTime);
        if (last == null)
        {
            throw new BO.BlNullPropertyException("No assignments found for the given call.");
        }
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = s_dal.Volunteer.Read(last.VolunteerId) ?? throw new BO.BlNullPropertyException("No assignments found for the given call.");
        return volunteer.Name;
    }

    /// <summary>
    /// Filters a collection of calls by the specified <see cref="BO.CallType"/>.
    /// </summary>
    /// <param name="toFilter">The collection of calls to filter.</param>
    /// <param name="type">The <see cref="BO.CallType"/> to filter by.</param>
    /// <returns>A filtered collection of calls.</returns>
    public static IEnumerable<DO.Call> Filter(IEnumerable<DO.Call> toFilter, BO.CallType? type)
    {
        if (type != null)
        {
            toFilter = from call in toFilter
                       where call.CarTypeToSend == (DO.CallType)type
                       select call;
        }
        return toFilter;
    }

    /// <summary>
    /// Filters a collection of <see cref="BO.CallInList"/> objects based on the provided filter field and value.
    /// </summary>
    /// <param name="toFilter">The collection of <see cref="BO.CallInList"/> to filter.</param>
    /// <param name="obj">The value to filter by.</param>
    /// <param name="filter">The field to filter by.</param>
    /// <returns>A filtered collection of <see cref="BO.CallInList"/>.</returns>
    public static IEnumerable<BO.CallInList> FilterCallInList(IEnumerable<BO.CallInList> toFilter, object? obj, BO.CallInListFields? filter)
    {
        if (filter == null || obj == null)
            return toFilter;

        var calls = toFilter;
        string? value = obj.ToString();

        switch (filter)
        {
            case BO.CallInListFields.Id:
                if (!int.TryParse(value, out int id))
                    throw new BO.BlInvalidData("Invalid number format");
                calls = from call in calls
                        where call.Id == id
                        select call;
                break;
            case BO.CallInListFields.CallId:
                if (!int.TryParse(value, out int callId))
                    throw new BO.BlInvalidData("Invalid number format");
                calls = from call in calls
                        where call.CallId == callId
                        select call;
                break;
            case BO.CallInListFields.CallType:
                if (!Enum.TryParse(typeof(BO.CallType), value, true, out var type))
                    throw new BO.BlInvalidData("Invalid CallType value");
                calls = from call in calls
                        where call.CallType == (BO.CallType)type
                        select call;
                break;
            case BO.CallInListFields.OpenTime:
                if (!DateTime.TryParse(value, out DateTime openTime))
                    throw new BO.BlInvalidData("Invalid open time format");
                calls = from call in calls
                        where call.OpenTime == openTime
                        select call;
                break;
            case BO.CallInListFields.TimeLeftToFinish:
                if (!TimeSpan.TryParse(value, out TimeSpan timeLeft))
                    throw new BO.BlInvalidData("Invalid time left format");
                calls = from call in calls
                        where call.TimeLeftToFinish == timeLeft
                        select call;
                break;
            case BO.CallInListFields.LastVolunteer:
                calls = from call in calls
                        where call.LastVolunteer == value
                        select call;
                break;
            case BO.CallInListFields.TreatmentTimeLeft:
                if (!TimeSpan.TryParse(value, out TimeSpan timeOfTreatment))
                    throw new BO.BlInvalidData("Invalid time of treatment format");
                calls = from call in calls
                        where call.TreatmentTimeLeft == timeOfTreatment
                        select call;
                break;
            case BO.CallInListFields.Status:
                if (!Enum.TryParse(typeof(BO.CallStatus), value, true, out var callStatus))
                    throw new BO.BlInvalidData($"Invalid CallStatus value");
                calls = from call in calls
                        where call.Status == (BO.CallStatus)callStatus
                        select call;
                break;
            case BO.CallInListFields.TotalAssignments:
                if (!int.TryParse(value, out int numAssignments))
                    throw new BO.BlInvalidData("Invalid number format");
                calls = from call in calls
                        where call.TotalAssignments == numAssignments
                        select call;
                break;
            default:
                calls = toFilter;
                break;
        }

        return calls;
    }

    /// <summary>
    /// Converts a <see cref="DO.Call"/> to a <see cref="BO.ClosedCallInList"/> object.
    /// </summary>
    /// <param name="call">The call to be converted.</param>
    /// <returns>A <see cref="BO.ClosedCallInList"/> object representing the call.</returns>
    public static BO.ClosedCallInList ToBOClosedCall(DO.Call call)
    {
        DO.Assignment? assignment;
        lock (AdminManager.BlMutex) //stage 7
        {
            var assignments = s_dal.Assignment.ReadAll();
            assignment = s_dal.Assignment.Read(a => a.CallId == call.Id);
        }

        if (assignment == null)
        {
            throw new BO.BlNullPropertyException("No assignment found for the given call.");
        }

        return new BO.ClosedCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CarTypeToSend,
            Address = call.Address,
            OpenTime = call.OpenTime,
            EnterTime = assignment.EnterTime,
            EndTime = assignment.EndTime,
            TypeEndOfTreatment = (BO.EndType?)assignment.TypeEndOfTreatment
        };
    }

    /// <summary>
    /// Converts a <see cref="DO.Call"/> and a <see cref="DO.Volunteer"/> to a <see cref="BO.OpenCallInList"/> object.
    /// </summary>
    /// <param name="call">The call to be converted.</param>
    /// <param name="volunteer">The volunteer associated with the call.</param>
    public static BO.OpenCallInList ToBOOpenCall(DO.Call call, DO.Volunteer volunteer)
    {
        if (call.Address == null)
        {
            throw new BO.BlNullPropertyException("Call address cannot be null.");
        }

        if (volunteer.Address == null)
        {
            throw new BO.BlNullPropertyException("Volunteer address cannot be null.");
        }
        return new BO.OpenCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CarTypeToSend,
            Destination = call.Description,
            Address = call.Address,
            OpenTime = call.OpenTime,
            MaxTime = call.MaxTime,
            Distance =
                     volunteer.Type == DO.DistanceType.Aerial
                     ? Tools.DistanceCalculator.CalculateAirDistance(call.Latitude,call.Longitude, volunteer.Latitude,volunteer.Longitude)
                     : Tools.DistanceCalculator.CalculateDistanceOSRMSync(
                         new Tools.Location { Lat = call.Latitude, Lon = call.Longitude },
                         new Tools.Location { Lat = volunteer.Latitude, Lon = volunteer.Longitude },
                         volunteer.Type),
        };
    }

    /// <summary>
    /// Converts a <see cref="BO.Call"/> object to a <see cref="DO.Call"/> object.
    /// </summary>
    /// <param name="call">The <see cref="BO.Call"/> object to be converted.</param>
    /// <returns>A <see cref="DO.Call"/> object representing the call.</returns>
    public static DO.Call ToDOCall(BO.Call call)
    {
        return new DO.Call(
                call.Id,
                call.Description,
                call.Address ?? throw new BO.BlNullPropertyException("No address entered!"),
                call.Latitude,
                call.Longitude,
                call.OpenTime,
                call.MaxTime,
                (DO.CallType)call.CarTypeToSend);
    }

    /// <summary>
    /// Method to perform periodic updates on students based on the clock update.
    /// </summary>
    /// <param name="oldClock">The previous clock value.</param>
    /// <param name="newClock">The updated clock value.</param>
    /// 
    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {

        List<DO.Call>? Calls;
        lock (AdminManager.BlMutex) //stage 7
            Calls = s_dal.Call.ReadAll().ToList();
        List<int> ids = new List<int> { };
        List<DO.Call> noAssignments = (from Call in Calls
                                let assignments = GetAssignmentCall(Call.Id)
                                where AdminManager.Now >= Call.MaxTime && assignments == null
                                select Call).ToList();

        List<DO.Call> haveAssignments = (from Call in Calls
                              let assignments = GetAssignmentCall(Call.Id)
                              where AdminManager.Now >= Call.MaxTime && assignments != null
                              let lastAssignment = GetLastAssignment(assignments)
                              where lastAssignment != null && lastAssignment.EndTime == null
                              select Call).ToList();

        foreach (var Call in noAssignments)
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                s_dal.Assignment.Create(new Assignment
                {
                    CallId = Call.Id,
                    EnterTime = AdminManager.Now,
                    EndTime = AdminManager.Now,
                    TypeEndOfTreatment = DO.EndType.ExpiredCancellation
                });
            }
            ids.Add(Call.Id);
            foreach (var item in ids)
            {
                Observers.NotifyItemUpdated(item); //stage 5
            }
            ids.Clear();
            Observers.NotifyListUpdated(); //stage 5
        }

        var convertHaveAssignments = haveAssignments.Select(c => GetLastAssignment(GetAssignmentCall(c.Id)!)!);
        var updatedHaveAssignments = convertHaveAssignments.Select(a => a = a with { EndTime = AdminManager.Now, TypeEndOfTreatment = DO.EndType.ExpiredCancellation });
        
        foreach (var assignment in updatedHaveAssignments)
        {
            lock (AdminManager.BlMutex) //stage 7
                s_dal.Assignment.Update(assignment);
            ids.Add(assignment.CallId);

            Observers.NotifyItemUpdated(assignment.Id); //stage 5
        }
        foreach (var item in ids)
        {
            Observers.NotifyItemUpdated(item); //stage 5
        }
        Observers.NotifyListUpdated(); //stage 5
    }

    /// <summary>
    /// Converts a <see cref="DO.Call"/> and its assignments to a <see cref="BO.CallInList"/> object.
    /// </summary>
    /// <param name="call">The <see cref="DO.Call"/> to be converted.</param>
    /// <param name="assignments">The assignments related to the call.</param>
    /// <returns>A <see cref="BO.CallInList"/> object representing the call.</returns>
    public static BO.CallInList ConvertFromDOToCallInList(DO.Call call, IEnumerable<DO.Assignment> assignments)
    {
        var assignmentOfCall = assignments.FirstOrDefault(a => a.CallId == call.Id);
        if (assignmentOfCall == null)
        {
            return new BO.CallInList
            {
                Id = null,
                CallId = call.Id,
                CallType = (BO.CallType)call.CarTypeToSend,
                OpenTime = call.OpenTime,
                TimeLeftToFinish = null,
                LastVolunteer = null,
                TreatmentTimeLeft = null,
                Status = Status(call.Id),
                TotalAssignments = 0
            };
        }
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
            volunteer = s_dal.Volunteer.Read(assignmentOfCall.VolunteerId);
        TimeSpan? time = null;
        if (assignmentOfCall.EndTime != null)
        {
            time = assignmentOfCall.EndTime - assignmentOfCall.EnterTime;
        }
        return new BO.CallInList
        {
            Id = assignmentOfCall.Id,
            CallId = call.Id,
            CallType = (BO.CallType)call.CarTypeToSend,
            OpenTime = call.OpenTime,
            TimeLeftToFinish = AdminManager.Now - call.MaxTime > TimeSpan.Zero ? AdminManager.Now - call.MaxTime : TimeSpan.Zero,
            LastVolunteer = volunteer?.Name, // Add null check here
            TreatmentTimeLeft = time,
            Status = Status(call.Id),
            TotalAssignments = assignments.Count()
        };
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
    public static IEnumerable<BO.OpenCallInList> openCallsForSelectionByVolunteer(int id, BO.CallType? filter, BO.OpenCallInListField? sortBy)
    {
        IEnumerable<DO.Call> calls;
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
        {
            volunteer = s_dal.Volunteer.Read(id);
            if (volunteer == null)
                throw new BO.BlDoesNotExistException($"Volunteer with {id} not found");
            if (volunteer.Address == null)
            {
                throw new BlNullPropertyException("Volunteer address cannot be null.");
            }

            calls = s_dal.Call.ReadAll();
        }
        IEnumerable<DO.Call> sortedCall;
        IEnumerable<DO.Call> filterCalls;

        var openCalls = from call in calls
                        let status = CallManager.Status(call.Id)
                        where status == BO.CallStatus.Open || status == BO.CallStatus.OpenAtRisk
                        select call;

        filterCalls = CallManager.Filter(openCalls, filter);

        if (sortBy == null)
            return filterCalls.OrderBy(c => c.Id).Select(c => CallManager.ToBOOpenCall(c, volunteer))
                               .Where(c => volunteer.MaximumDistance.HasValue && c.Distance <= volunteer.MaximumDistance.Value);
        else
        {
            sortedCall = sortBy switch
            {
                BO.OpenCallInListField.Id => filterCalls.OrderBy(c => c.Id),
                BO.OpenCallInListField.CallType => filterCalls.OrderBy(c => c.CarTypeToSend),/////
                BO.OpenCallInListField.Destination => filterCalls.OrderBy(c => c.Description),
                BO.OpenCallInListField.Address => filterCalls.OrderBy(c => c.Address),
                BO.OpenCallInListField.OpenTime => filterCalls.OrderBy(c => c.OpenTime),
                BO.OpenCallInListField.MaxTime => filterCalls.OrderBy(c => c.MaxTime),
                BO.OpenCallInListField.Distance => filterCalls.OrderBy(c =>
                    volunteer.Type == DO.DistanceType.Aerial
                    ? Tools.DistanceCalculator.CalculateAirDistance(c.Latitude,c.Longitude ,volunteer.Latitude,volunteer.Longitude)
                    : Tools.DistanceCalculator.CalculateDistanceOSRMSync(
                        new Tools.Location { Lat = c.Latitude, Lon = c.Longitude },
                        new Tools.Location { Lat = volunteer.Latitude, Lon = volunteer.Longitude },
                        volunteer.Type)),
                _ => filterCalls.OrderBy(c => c.Id)
            };
        }
        return sortedCall.Select(c => CallManager.ToBOOpenCall(c, volunteer))
                        .Where(c => volunteer.MaximumDistance.HasValue && c.Distance <= volunteer.MaximumDistance.Value);


    }

    /// <summary>
    /// Assigns a specific call to a volunteer for handling.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer choosing to handle the call.</param>
    /// <param name="callId">The ID of the call to be handled by the volunteer.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer or the call with the specified ID does not exist.</exception>
    /// <exception cref="BO.BlOperationNotAllowedException">Thrown if the volunteer is already handling another call or if the call is not available for handling.</exception>
    public static void ChooseCallForHandling(int volunteerId, int callId)
    {
        
        IEnumerable<DO.Assignment> assignments;
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
        {
            volunteer = s_dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
                throw new BO.BlDoesNotExistException($"Volunteer with {volunteerId} not found");
            DO.Call? call = s_dal.Call.Read(callId);
            if (call == null)
                throw new BO.BlDoesNotExistException($"Call with {callId} not found");

            assignments = s_dal.Assignment.ReadAll();
        }
        if (assignments != null)
        {
            var assignmentVolunteer = assignments.FirstOrDefault(a => a.VolunteerId == volunteerId && a.TypeEndOfTreatment == null);
            if (assignmentVolunteer != null)
                throw new BlOperationNotAllowedException($"Volunteer with {volunteerId} is already treating a call");
        }
        if (CallManager.Status(callId) != BO.CallStatus.Open && CallManager.Status(callId) != BO.CallStatus.OpenAtRisk)
            throw new BlOperationNotAllowedException($"The call is already being handled by another volunteer.");

        DO.Assignment assignmentToAdd = new DO.Assignment(0, callId, volunteerId, AdminManager.Now, null, null);
        lock (AdminManager.BlMutex) //stage 7
        {
            s_dal.Volunteer.Update(volunteer with { Active = true });//////
            s_dal.Assignment.Create(assignmentToAdd);
        }

        CallManager.Observers.NotifyListUpdated();  //stage 5
        CallManager.Observers.NotifyItemUpdated(volunteerId);

    }

    /// <summary>
    /// Updates the end of treatment details for a specific assignment handled by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer who is updating the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment to be updated.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer or the assignment with the specified ID does not exist.</exception>
    /// <exception cref="BO.UnauthorizedAccessException">Thrown if the volunteer does not have permission to update the assignment or if the assignment cannot be updated due to existing end time or treatment type.</exception>
    public static void UpdateEndOfTreatmentCall(int volunteerId, int assignmentId)
    {
        
        DO.Assignment? assignment;
        lock (AdminManager.BlMutex) //stage 7
        {
            DO.Volunteer? volunteer = s_dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
                throw new BO.BlDoesNotExistException($"Volunteer with {volunteerId} not found");
            assignment = s_dal.Assignment.Read(assignmentId);
            if (assignment == null)
                throw new BO.BlDoesNotExistException($"Assignment with {assignmentId} not found");
        }
        if (assignment.VolunteerId != volunteerId)
            throw new BO.UnauthorizedAccessException("You do not have access permission to update the assignment");
        if (assignment.TypeEndOfTreatment != null && assignment.EndTime != null)
            throw new BO.UnauthorizedAccessException("You cannot update this assignment");

        DO.Assignment assignmentToUpdate = assignment with { EndTime = AdminManager.Now, TypeEndOfTreatment = DO.EndType.Treated };
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                s_dal.Assignment.Update(assignmentToUpdate);
            CallManager.Observers.NotifyItemUpdated(volunteerId);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5


        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error attempting to update call handling completion:" + ex);
        }
    }
    
    /// <summary>
    /// Cancels the handling of a specific assignment by a volunteer or an administrator.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer attempting to cancel the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment to be canceled.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer or the assignment with the specified ID does not exist.</exception>
    /// <exception cref="BO.UnauthorizedAccessException">Thrown if the volunteer does not have permission to cancel the assignment or if the assignment cannot be canceled due to existing end time or treatment type.</exception>
    public static void CancelCallHandling(int volunteerId, int assignmentId)
    {
        
        DO.Volunteer? volunteer;
        DO.Assignment? assignment;
        lock (AdminManager.BlMutex) //stage 7
        {
            volunteer = s_dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
                throw new BO.BlDoesNotExistException($"Volunteer with {volunteerId} not found");
            assignment = s_dal.Assignment.Read(assignmentId);
            if (assignment == null)
                throw new BO.BlDoesNotExistException($"Assignment with {assignmentId} not found");
        }
        if (volunteer.Role == DO.Roles.Volunteer && assignment.VolunteerId != volunteerId)
            throw new BO.UnauthorizedAccessException("Sorry! You do not have access permission to revoke the assignment");
        if (assignment.TypeEndOfTreatment != null && assignment.EndTime != null)
            throw new BO.UnauthorizedAccessException("You cannot cancel this assignment");

        DO.Assignment assignmentToUpdate;
        if (assignment.VolunteerId == volunteerId)
            assignmentToUpdate = assignment with { EndTime = AdminManager.Now, TypeEndOfTreatment = DO.EndType.SelfCancellation };
        else
            assignmentToUpdate = assignment with { EndTime = AdminManager.Now, TypeEndOfTreatment = DO.EndType.AdminCancellation };
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                s_dal.Assignment.Update(assignmentToUpdate);
            CallManager.Observers.NotifyItemUpdated(volunteerId);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5


        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error trying to update call cancellation :" + ex);
        }
    }
   
    internal static void AddCallsSimulation()
    {
        //We asked the GPT chat: create an array for me that has 50 cases that call the MDA
        string?[] descriptionsArr = new string?[]
        {
            "Unconscious person", "Road accident with injuries", "Gunshot wound", "Severe burns",
            "Fall from height", "Infant not breathing", "Stroke symptoms", "Acute heart attack",
            "Severe shortness of breath", "Heavy bleeding", "Head injury", "Deep cut on hand",
            "Fall from bicycle", "Food poisoning", "Severe allergic reaction", "Chest pain",
            "Drowning", "Snake bite", "Workplace injury", "Minor burn from gas explosion",
            "Stab wound in abdomen", "Dehydration symptoms", "High fever and seizures",
            "Fall on stairs", "Sports injury", "Broken leg in accident", "Assault with sharp object",
            "Motorcycle accident injury", "Frostbite while outdoors", "Minor electric shock",
            "Sudden sharp back pain", "Scheduled check-up visit", "Blood pressure measurement request",
            "Routine blood test", "Medical transport request", "Assistance with medical equipment setup",
            "Consultation for ongoing symptoms", "Heart rate monitoring device setup",
            "Request for flu vaccination", "Consultation for mild allergy", "Routine blood sugar test",
            "Request for tetanus shot", "Follow-up on previous treatment", "Minor wound dressing change",
            "Routine elderly health assessment", "Work clearance health screening", "Pregnancy check-up",
            "Chronic pain management", "Diabetes management support", "Request for mobility aid assistance",
            "Health education session", "Guidance on post-surgery care", "Prescription refill assistance",
            "Dietitian consultation request", "Physical fitness assessment", "Medication side effect inquiry",
            "Physiotherapy session request", "Home safety evaluation", "Wellness check for remote patient",
            "Routine child vaccination"
        };

        //We asked the GPT chat: can you create for us an array of 50 addresses in Israel
        string[] addresses = new string[]
        {
            "Herzl St 10, Tel Aviv", "Ben Gurion St 5, Ramat Gan", "Dizengoff St 25, Tel Aviv",
            "Allenby St 40, Haifa", "Jaffa St 60, Jerusalem", "Rothschild Blvd 16, Tel Aviv",
            "Weizmann St 12, Kfar Saba", "HaNasi St 8, Herzliya", "Sokolov St 30, Holon",
            "Ben Yehuda St 100, Tel Aviv", "Ehad HaAm St 50, Beersheba", "Herzliya St 15, Netanya",
            "Keren HaYesod St 22, Ashdod", "Herzl St 45, Rishon LeZion", "Moshe Dayan St 3, Ashkelon",
            "Ben Tsvi St 10, Bat Yam", "Yitzhak Rabin St 20, Lod", "King George St 45, Tel Aviv",
            "Arlozorov St 100, Tel Aviv", "Aluf David St 5, Petah Tikva", "Habanim St 12, Hadera",
            "Shabazi St 18, Ramat Hasharon", "Levi Eshkol St 40, Ashkelon", "Weizmann St 6, Rehovot",
            "Jabotinsky St 15, Bnei Brak", "HaGalil St 10, Kiryat Shmona", "HaNasi Weizmann St 35, Haifa",
            "Moshe Dayan St 1, Ashdod", "Menachem Begin Blvd 55, Tel Aviv", "Hashalom Rd 10, Tel Aviv",
            "Shderot Chen St 45, Eilat", "Ayalon St 5, Rishon LeZion", "King Solomon St 20, Tiberias",
            "Rothschild Blvd 80, Tel Aviv", "Yigal Allon St 55, Ramat Gan", "Neve Shaanan St 3, Haifa",
            "Einstein St 12, Haifa", "Bar Ilan St 4, Givat Shmuel", "Yehuda Halevi St 40, Tel Aviv",
            "Haifa Rd 10, Acre", "Nahum St 1, Holon", "Eliezer Kaplan St 5, Herzliya",
            "Dov Hoz St 20, Be'er Sheva", "Moshe Sharet St 15, Ashkelon", "Haneviim St 60, Jerusalem",
            "Emek Refaim St 12, Jerusalem", "HaSolel St 1, Nazareth", "Hanamal St 4, Haifa",
            "HaKibbutz HaMeuhad St 6, Kfar Yona"
        };

        //We asked the GPT chat: can you create for us an array of longitude lines and an array
        //of latitude lines corresponding to the above array respectively
        double[] latitudes = new double[]
        {
            32.066158, 32.082271, 32.080480, 32.818409, 31.784217, 32.063922, 32.175034, 32.166313, 32.014046,
            32.089771, 31.251810, 32.328516, 31.802418, 31.969633, 31.669258, 32.018748, 31.951569, 32.073253,
            32.087601, 32.090678, 32.440987, 32.145339, 31.661712, 31.894756, 32.089611, 33.207333, 32.796785,
            31.803742, 32.071457, 32.061399, 29.55805, 31.973001, 32.785539, 32.070054, 32.788712, 32.110003,
            32.083762, 32.055893, 32.926099, 32.019313, 32.166313, 31.249872, 31.661712, 32.083307, 31.784217,
            31.765365, 32.696947, 32.823115, 32.317325, 32.392867
        };

        double[] longitudes = new double[]
        {
            34.779808, 34.812548, 34.774989, 34.988507, 35.223391, 34.771805, 34.906552, 34.842972, 34.772101,
            34.773922, 34.791460, 34.853196, 34.641665, 34.804390, 34.574262, 34.747685, 34.899520, 34.774281,
            34.791522, 34.887761, 34.923137, 34.838293, 34.571489, 34.812223, 34.834804, 35.570961, 35.003008,
            34.656998, 34.791613, 34.789561, 34.934200, 34.771497, 34.779572, 34.804868, 35.021502, 35.053653,
            34.824040, 34.773058, 35.066441, 34.767654, 34.842972, 34.771687, 34.571489, 34.799839, 35.223391,
            35.219762, 35.308230, 35.002729, 34.919138, 34.876413
        };
        var random = new Random();
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                int place = random.Next(0, addresses.Length);
                var newCall = new DO.Call
                {
                    Description = descriptionsArr[random.Next(0, descriptionsArr.Length)],
                    Address = addresses[place],
                    Latitude = latitudes[place],
                    Longitude = longitudes[place],
                    OpenTime = AdminManager.Now,
                    MaxTime = AdminManager.Now.AddMinutes(random.Next(10, 30)),
                    CarTypeToSend = (DO.CallType)random.Next(0, 3)
                };
                lock (AdminManager.BlMutex)
                    s_dal.Call.Create(newCall);
            }
            Task.Delay(TimeSpan.FromMinutes(5));
        }
    }


}


