﻿using BL.Helpers;
using BlApi;
using DO;
using Helpers;
using System;
using System.Net;
using System.Xml.Linq;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public Array CallQuantities()
    {
        var calls = _dal.Call.ReadAll();
        var groupedCalls = calls.GroupBy(c=> CallManager.Status(c.Id)).ToArray();
        return groupedCalls;
    }

    public IEnumerable<BO.CallInList> CallInLists(BO.CallField? filter, object? value, BO.CallField? sort)
    {
        IEnumerable<DO.Call> calls;
        IEnumerable<IGrouping<object, DO.Call>> groupedCalls;
        IEnumerable<DO.Call> sotrtCalls;
        calls = _dal.Call.ReadAll();
        if (filter == null)
            sotrtCalls = _dal.Call.ReadAll();
        else
        {
            groupedCalls = filter switch
            {
                BO.CallField.Address => calls.GroupBy(c => c.Address),
                BO.CallField.CarTaypeToSend => calls.GroupBy(c => (object)c.CarTaypeToSend),
                BO.CallField.Id => calls.GroupBy(c => (object)c.Id),
                _ => calls.GroupBy(c => (object)c.Id)
            };
            calls = groupedCalls.FirstOrDefault(c => c.Key == value) ?? Enumerable.Empty<DO.Call>();
        }
        if (sort == null)
            sotrtCalls = calls.OrderBy(c => c.Id);
        else
        {
            sotrtCalls = sort switch
            {
                BO.CallField.Address => calls.OrderBy(c => c.Address).Distinct(),
                BO.CallField.CarTaypeToSend => calls.OrderBy(c => c.CarTaypeToSend).Distinct(),
                BO.CallField.Id => calls.OrderBy(c => c.Id).Distinct(),
                _ => calls.OrderBy(c => c.Id).Distinct()
            };
        }

        return sotrtCalls.Select(call => new BO.CallInList()
        {
            CallId = call.Id,
            CallType = (BO.CallType)call.CarTaypeToSend,
            OpenTime = call.OpenTime,
            LastVolunteer = CallManager.getLastVolunteer(call),
            Status = CallManager.Status(call.Id)
            
        });
    }

    public BO.Call GetCallDetails(int id)
    {
        //try and catch for Read?
         var doCall = _dal.Call.Read(id);
        if (doCall == null)
            throw new Exception("");
         var doAssignments = _dal.Assignment.ReadAll()
                .Where(a => a.CallId == id);

        var boCall = new BO.Call
        {
            Id = doCall.Id,
            CarTaypeToSend = (BO.CallType)doCall.CarTaypeToSend,
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
                Name = "",
                EnterTime = a.EnterTime ?? DateTime.MinValue,
                EndTime = a.EndTime,
                EndType = 0,
                TypeEndOfTreatment = a.TypeEndOfTreatment.HasValue ? (BO.EndType)a.TypeEndOfTreatment.Value :null

            }).ToList()
        };

        return boCall;
    }
   
    public void UpdatingCallDetails(BO.Call call)
    {
        Tools.IntegrityCheck(call);

        DO.Call DOcall = new DO.Call(
           call.Id,
           call.Description,
           call.Address,
           call.Latitude,
           call.Longitude,
           call.OpenTime,
           call.MaxTime,
           (DO.CallType)call.CarTaypeToSend);
        try
        {
            _dal.Call.Update(DOcall);
        }
        catch (Exception ex)
        {
            throw new Exception("Error:" + ex);
        }
    }

    public void DeleteCall(int id)
    {
        try
        {
            // not sure about this --- I changed a little
            DO.Call? callToDelete = _dal.Call.Read(id);
            if (callToDelete == null) throw new Exception("");
            var assignments = _dal.Assignment.ReadAll();
            if (assignments != null)
            {
                var assignmentsOfCall = from assignment in assignments
                                        where assignment.CallId == id
                                        select assignment;
                if (assignmentsOfCall == null && CallManager.Status(id) == BO.CallStatus.Open)
                    _dal.Volunteer.Delete(id);
            }
            else
                throw new Exception($"Cannot delete call with {id}");
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting volunteer:" + ex.Message);
        }
    }

    public void AddCall(BO.Call call)
    {
        Helpers.Tools.IntegrityCheck(call);
        try
        {
            DO.Call callToAdd = new DO.Call(
                call.Id,
                call.Description,
                call.Address ?? string.Empty,
                call.Latitude,
                call.Longitude,
                call.OpenTime,
                call.MaxTime,
                (DO.CallType)call.CarTaypeToSend);

            _dal.Call.Create(callToAdd);
        }
        catch (Exception ex) { throw new Exception(ex.Message); }
    }

    public IEnumerable<BO.ClosedCallInList> closedCallsHandledByVolunteer(int VolunteerId, BO.CallType? filter, BO.ClosedCallInListField? sortBy)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(VolunteerId);
        if (volunteer == null)
            throw new Exception($"Volunteer with {VolunteerId} not found");

        var assignment = _dal.Assignment.ReadAll();
        //All the ID of the calls the volunteer took
        var callsVolunteer =from a in assignment
               where a.VolunteerId==VolunteerId
               select a.CallId;
        //All calls of the volunteer received as a parameter
        var calls = _dal.Call.ReadAll(c => callsVolunteer.Contains(c.Id));
       
        var fitCalls = from call in calls
                       where (CallManager.Status(call.Id) == BO.CallStatus.Close 
                                                      || CallManager.Status(call.Id) == BO.CallStatus.Expired)
                       select call;
        //filter to calls of volunteer using assignment
        var filterCalls = CallManager.Filter(fitCalls, filter);
        
         //var finalListCalls = CallManager.SortClosedCall(callsForList, sortBy);
         var sortedcall = sortBy switch
        {
            BO.ClosedCallInListField.Id => filterCalls.OrderBy(c => c.Id),
            BO.ClosedCallInListField.CallType => filterCalls.OrderBy(c => c.CarTaypeToSend),////לא בטוח
            BO.ClosedCallInListField.Address => filterCalls.OrderBy(c => c.Address),
            BO.ClosedCallInListField.OpenTime => filterCalls.OrderBy(c => c.OpenTime),
            //BO.ClosedCallInListField.EnterTime => filterCalls.OrderBy(c => c.EnterTime),
            //BO.ClosedCallInListField.EndTime => filterCalls.OrderBy(c => c.EndTime),
            //BO.ClosedCallInListField.TypeEndOfTreatment => filterCalls.OrderBy(c => c.TypeEndOfTreatment),

            _ => filterCalls.OrderBy(c => c.Id)
        };
        return sortedcall.Select(call => CallManager.ToBOClosedCall(call));
    }


    public IEnumerable<BO.OpenCallInList> openCallsForSelectionByVolunteer(int id, BO.CallType? filter, BO.OpenCallInListField? sortBy)
    {
       
        var calls = _dal.Call.ReadAll();
        IEnumerable<DO.Call> sortedcall;
        IEnumerable<DO.Call> filterCalls;

        var openCalls = from call in calls
                        let status = CallManager.Status(call.Id)
                        where status == BO.CallStatus.Open || status == BO.CallStatus.OpenAtRisk
                        select call;

        filterCalls = CallManager.Filter(openCalls, filter);
            
        if (sortBy == null)
            return filterCalls.OrderBy(c => c.Id).Select(c => CallManager.ToBOOpenCall(c));
        else
        {
            sortedcall = sortBy switch
            {
                BO.OpenCallInListField.Id => filterCalls.OrderBy(c => c.Id),
                BO.OpenCallInListField.CallType => filterCalls.OrderBy(c => c.CarTaypeToSend),/////
                BO.OpenCallInListField.Address => filterCalls.OrderBy(c => c.Address),
                //BO.OpenCallInListField.Destination => filterCalls.OrderBy(c => c.Destination),
                //BO.OpenCallInListField.Distance => filterCalls.OrderBy(c => c.d),
                BO.OpenCallInListField.OpenTime => filterCalls.OrderBy(c => c.OpenTime),
                BO.OpenCallInListField.MaxTime => filterCalls.OrderBy(c => c.MaxTime),
                _ => filterCalls.OrderBy(c => c.Id)
            };
        }
       
      return sortedcall.Select(c => CallManager.ToBOOpenCall(c));

    }

    public void UpdateEndOfTreatmentCall(int volunteerId, int assignmentId)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new Exception($"Volunteer with {volunteerId} not found");
        DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);
        if (assignment == null)
            throw new Exception($"Assignment with {assignmentId} not found");

        if (assignment.VolunteerId != volunteerId)
            throw new Exception("You do not have access permission to update the assignment");
        if (assignment.TypeEndOfTreatment != null || assignment.EndTime != null)
            throw new Exception("You cannot update this assignment");

        DO.Assignment assignmentToUpdate = assignment with { EndTime = ClockManager.Now, TypeEndOfTreatment = DO.EndType.Treated };
        try
        {
            _dal.Assignment.Update(assignmentToUpdate);
        }
        catch (Exception ex)
        {
            throw new Exception("Error attempting to update call handling completion:" + ex);
        }
    }
    public void CancelCallHandling(int volunteerId, int assignmentId)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new Exception($"Volunteer with {volunteerId} not found");
        DO.Assignment? assignment = _dal.Assignment.Read(assignmentId);
        if (assignment == null)
            throw new Exception($"Assignment with {assignmentId} not found");

        if (volunteer.Role == DO.Roles.Volunteer && assignment.VolunteerId != volunteerId)
            throw new Exception("Sorry! You do not have access permission to revoke the assignment");
        if (assignment.TypeEndOfTreatment != null || assignment.EndTime != null)
            throw new Exception("You cannot cancel this assignment");

        DO.Assignment assignmentToUpdate;
        if (assignment.VolunteerId == volunteerId)
            assignmentToUpdate = assignment with { EndTime = ClockManager.Now, TypeEndOfTreatment = DO.EndType.SelfCancellation };
        else
            assignmentToUpdate = assignment with { EndTime = ClockManager.Now, TypeEndOfTreatment = DO.EndType.AdminCancellation };
        try
        {
            _dal.Assignment.Update(assignmentToUpdate);
        }
        catch (Exception ex)
        {
            throw new Exception("Error trying to update call cancellation :" + ex);
        }
    }

    public void ChooseCallForHandling(int volunteerId, int callId)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new Exception($"Volunteer with {volunteerId} not found");
        DO.Call? call = _dal.Call.Read(callId);
        if (call == null)
            throw new Exception($"Call with {callId} not found");

        var assignments = _dal.Assignment.ReadAll();
        if (assignments != null)
        {
            var assignmentVolunteer = assignments.FirstOrDefault(a => a.VolunteerId == volunteerId && a.TypeEndOfTreatment == null);
            if (assignmentVolunteer != null)
                throw new Exception($"Volunteer with {volunteerId} is already treating a call");
        }
        if (CallManager.Status(callId) != BO.CallStatus.Open && CallManager.Status(callId) != BO.CallStatus.OpenAtRisk)
            throw new Exception($"Volunteer with {volunteerId} is already treating a call");

        DO.Assignment assignmentToAdd = new DO.Assignment(0, callId, volunteerId, ClockManager.Now, null, null);
        _dal.Assignment.Create(assignmentToAdd);
    }
                  
}
