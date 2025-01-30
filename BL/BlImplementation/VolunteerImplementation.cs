namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.Linq;

/// <summary>
/// Implementation of the logical service entity interface for volunteer management
/// </summary>
internal class volunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Authenticates a volunteer by username and password and returns their role.
    /// </summary>
    /// <param name="username">The username of the volunteer.</param>
    /// <param name="password">The password of the volunteer.</param>
    /// <returns>The role of the volunteer (Role).</returns>
    /// <exception cref="BO.BlNullPropertyException">Thrown if no volunteer is found with the given username or password.</exception>
    public BO.Roles Login(string username, string password)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex) //stage 7
             volunteer = _dal.Volunteer.ReadAll(v => v.Name == username && v.Password == password).FirstOrDefault();
        if (volunteer == null)
        {
            throw new BO.BlNullPropertyException("There is no volunteer with that name or password.");
        }
        //BO.Volunteer boVolunteer = VolunteerManager.ToBOVolunteer(volunteer);
        var boVolunteer = new BO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Mail = volunteer.Mail,
            Password = volunteer.Password,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = (BO.Roles)volunteer.Role,
            Active = volunteer.Active,
            MaximumDistance = volunteer.MaximumDistance,
            Type = (BO.DistanceType)volunteer.Type,
            TotalCallsHandled = 0,
            TotalCallsCanceled = 0,
            TotalCallsChosenHandleExpired = 0,
            Progress = new BO.CallInProgress(),
        };
        return boVolunteer.Role;
    }

    /// <summary>
    /// Filters and sorts the volunteers based on the given activity status and sorting field, then returns the filtered and sorted list.
    /// </summary>
    /// <param name="active">Filter for active/inactive volunteers (null for all).</param>
    /// <param name="field">The field by which to sort the volunteers.</param>
    /// <returns>A filtered and sorted list of volunteers.</returns>
    /// <exception cref="BO.BlNullPropertyException">Thrown if the volunteer data source is empty or null.</exception>
    public IEnumerable<BO.VolunteerInList> VolunteerList(bool? active, BO.VolunteerField? field)
    {
        IEnumerable<DO.Volunteer> volunteers;
        IEnumerable<IGrouping<bool, DO.Volunteer>> groupedVolunteers;
        IEnumerable<DO.Volunteer> sortVolunteers;
        List<DO.Assignment> assignments;
        lock (AdminManager.BlMutex) //stage 7
            assignments = _dal.Assignment.ReadAll().ToList();
        if (active == null)
        {
            lock (AdminManager.BlMutex) //stage 7
                volunteers = _dal.Volunteer.ReadAll();
        }
        else
        {
            lock (AdminManager.BlMutex) //stage 7
                groupedVolunteers = _dal.Volunteer.ReadAll().GroupBy(v => v.Active == true);
            if (groupedVolunteers == null || !groupedVolunteers.Any())
                throw new BO.BlNullPropertyException("Volunteer data source is empty or null.");

            volunteers = groupedVolunteers.FirstOrDefault(g => g.Key == active.Value) ?? Enumerable.Empty<DO.Volunteer>();
        }

        
        if (!volunteers.Any())
            return Enumerable.Empty<BO.VolunteerInList>();
        else
        {
            sortVolunteers = field switch
            {
                BO.VolunteerField.Id => volunteers.OrderBy(v => v.Id),
                BO.VolunteerField.Name => volunteers.OrderBy(v => v.Name),
                BO.VolunteerField.Active => volunteers.OrderBy(v => v.Active),
                _ => volunteers.OrderBy(v => v.Id)
            };
        }

        return sortVolunteers.Select(volunteer =>
        {
            var idCall = assignments.LastOrDefault(item => item.VolunteerId == volunteer.Id && (item.TypeEndOfTreatment == null || item.EndTime == null));
            DO.Call? lastCall;
            lock (AdminManager.BlMutex) //stage 7
                lastCall = idCall != null ? _dal.Call.Read(idCall.CallId) : null;
            var treated = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.Treated) ?? Enumerable.Empty<DO.Assignment>();
            var selfCancellation = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.SelfCancellation) ?? Enumerable.Empty<DO.Assignment>();
            var expiredCancellation = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.ExpiredCancellation) ?? Enumerable.Empty<DO.Assignment>();

            return new BO.VolunteerInList
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Active = volunteer.Active,
                TotalCallsHandled = treated.Count(),
                TotalCallsCanceled = selfCancellation.Count(),
                TotalCallsChosenHandleExpired = expiredCancellation.Count(),
                CallHandledId = idCall?.CallId,
                CallHandledType = lastCall != null ? (BO.CallType)lastCall.CarTypeToSend : BO.CallType.None
                //idCall?.TypeEndOfTreatment.HasValue == true ? (BO.CallType)idCall.TypeEndOfTreatment.Value : BO.CallType.None

            };
        });

       
    }
   
    /// <summary>
    /// Returns the details of a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>The volunteer details.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if no volunteer is found with the given ID.</exception>
    public BO.Volunteer GetVolunteerDetails(int id)
    {
        return VolunteerManager.GetVolunteerDetails(id);        

    }
    
    /// <summary>
    /// Updates the details of a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to update.</param>
    /// <param name="volunteer">The updated volunteer information.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if no volunteer is found with the given ID or if the volunteer to update doesn't exist.</exception>
    /// <exception cref="BO.UnauthorizedAccessException">Thrown if the user doesn't have permission to update the volunteer's details.</exception>
    public void UpdatingVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        //if (!string.IsNullOrEmpty(volunteer.Address))
        //{
        //    var coordinate = Helpers.Tools.GetAddressCoordinates(volunteer.Address);
        //    volunteer.Latitude = coordinate.Latitude;
        //    volunteer.Longitude = coordinate.Longitude;
        //}

        Helpers.VolunteerManager.IntegrityCheck(volunteer);
        try
        {
            DO.Volunteer volunteerToUpdate;
            DO.Volunteer? requester;
            lock (AdminManager.BlMutex) //stage 7
                 requester = _dal.Volunteer.Read(id);
            if (requester == null)
            {
                throw new BO.BlDoesNotExistException("There is no volunteer with this ID.");
            }
            // Check permissions
            if (!requester.Role.Equals("Manager"))
            {
                if (requester.Id != volunteer.Id)
                    throw new BO.UnauthorizedAccessException("You are not authorized to update this volunteer.");
            }
            DO.Volunteer? existingVolunteer;
            lock (AdminManager.BlMutex) //stage 7
               existingVolunteer = _dal.Volunteer.Read(volunteer.Id);
            if (existingVolunteer == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volunteer.Id} not found.");
            }

            if ((BO.Roles)requester.Role != BO.Roles.Manager && (BO.Roles)existingVolunteer.Role != volunteer.Role)
            {
                throw new BO.UnauthorizedAccessException("Only managers can update the role.");
            }
            volunteerToUpdate = VolunteerManager.ToDOVolunteer(volunteer);
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Update(volunteerToUpdate);
            

            VolunteerManager.Observers.NotifyItemUpdated(volunteerToUpdate.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5
            CallManager.Observers.NotifyItemUpdated(volunteerToUpdate.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();
            
            _ = UpdateVolunteerFieldsAsync(volunteerToUpdate);
            

        }
        catch (DO.DalDoesNotExistException ex)
        {

            throw new BO.BlDoesNotExistException("Error updating volunteer details: " + ex.Message);
        }
    }
    
    /// <summary>
    /// Deletes a volunteer by their ID if the volunteer is inactive.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if no volunteer is found with the given ID.</exception>
    /// <exception cref="BO.OperationNotAllowedException">Thrown if the volunteer is active and cannot be deleted.</exception>
    public void DeleteVolunteer(int id)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            DO.Volunteer? volunteerToDelete;
            lock (AdminManager.BlMutex) //stage 7
                volunteerToDelete = _dal.Volunteer.Read(id);
            if (volunteerToDelete == null) throw new BO.BlDoesNotExistException("There is no volunteer with this ID.");
            if (!volunteerToDelete.Active)
            {
                lock (AdminManager.BlMutex) //stage 7
                    _dal.Volunteer.Delete(id);
                VolunteerManager.Observers.NotifyListUpdated();  //stage 5
            }
            else
                throw new BO.BlOperationNotAllowedException("The volunteer cannot be deleted.");
            
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error deleting volunteer:" + ex.Message);
        }
    }
    
    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteer">The volunteer details to be added.</param>
    /// <exception cref="BO.BllAlreadyExistException">Thrown if a volunteer with the same ID already exists.</exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        //if (!string.IsNullOrEmpty(volunteer.Address))
        //{
        //    var coordinate = Helpers.Tools.GetAddressCoordinates(volunteer.Address);
        //    volunteer.Latitude = coordinate.Latitude;
        //    volunteer.Longitude = coordinate.Longitude;
        //}
        Helpers.VolunteerManager.IntegrityCheck(volunteer);
        try
        {
            DO.Volunteer volunteerToAdd = VolunteerManager.ToDOVolunteer(volunteer);
            lock (AdminManager.BlMutex) //stage 7
                 _dal.Volunteer.Create(volunteerToAdd);
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5
            
             _ = UpdateVolunteerFieldsAsync(volunteerToAdd);
            
        }
        catch (DO.DalAlreadyExistException ex) { throw new BO.BllAlreadyExistException(ex.Message); }
    }

    /// <summary>
    /// Filters the list of volunteers based on the specified call type.
    /// </summary>
    /// <param name="type">The <see cref="BO.CallType"/> used to filter the volunteers.</param>
    /// <returns>
    /// If no volunteers are found, returns an empty collection.
    /// </returns>
    public IEnumerable<BO.VolunteerInList> FilterVolunteerListByCallType(BO.CallType type)
    {
        IEnumerable<DO.Volunteer> volunteers;
        lock (AdminManager.BlMutex) //stage 7
              volunteers = _dal.Volunteer.ReadAll();

        if (!volunteers.Any())
        {
            return Enumerable.Empty<BO.VolunteerInList>();
        }
        else
        {
            List<DO.Assignment> assignments;
            lock (AdminManager.BlMutex) //stage 7
                assignments = _dal.Assignment.ReadAll().ToList();

            return volunteers.Select(volunteer =>
            {
                var idCall = assignments.FirstOrDefault(item => item.VolunteerId == volunteer.Id && (item.TypeEndOfTreatment==null || item.EndTime==null));
                DO.Call? lastCall;
                lock (AdminManager.BlMutex) //stage 7
                    lastCall = idCall != null ? _dal.Call.Read(idCall.CallId) : null;
                var treated = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.Treated) ?? Enumerable.Empty<DO.Assignment>();
                var selfCancellation = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.SelfCancellation) ?? Enumerable.Empty<DO.Assignment>();
                var expiredCancellation = Helpers.VolunteerManager.GetAssignments(assignments, volunteer, DO.EndType.ExpiredCancellation) ?? Enumerable.Empty<DO.Assignment>();

                return new BO.VolunteerInList
                {
                    Id = volunteer.Id,
                    Name = volunteer.Name,
                    Active = volunteer.Active,
                    TotalCallsHandled = treated.Count(),
                    TotalCallsCanceled = selfCancellation.Count(),
                    TotalCallsChosenHandleExpired = expiredCancellation.Count(),
                    CallHandledId = idCall?.Id,
                    CallHandledType = lastCall != null ? (BO.CallType)lastCall.CarTypeToSend : BO.CallType.None

                };

            }).Where(volunteer => volunteer.CallHandledType == type);
        }
    }

    public bool CanVolunteerAttendCall(BO.Volunteer volunteer, BO.Call call)
    {
        if (volunteer.Address == null || call.Address == null)
        {
            return false;
        }


        double distance = volunteer.Type == BO.DistanceType.Aerial
                     ? Tools.DistanceCalculator.CalculateAirDistance(call.Latitude,call.Longitude, volunteer.Latitude,volunteer.Latitude)
                     : Tools.DistanceCalculator.CalculateDistanceOSRMSync(
                         new Tools.Location { Lat = call.Latitude, Lon = call.Longitude },
                         new Tools.Location { Lat = volunteer.Latitude, Lon = volunteer.Longitude },
                         (DO.DistanceType)volunteer.Type);
        return volunteer.MaximumDistance.HasValue && distance <= volunteer.MaximumDistance.Value;
    }
    
    /// <summary>
    /// Checks if a volunteer currently has an active assignment.
    /// </summary>
    /// <param name="id">The unique ID of the volunteer.</param>
    /// <returns>True if the volunteer has an assignment that is in "Treatment" or "TreatmentOfRisk" status.
    /// False if the volunteer has no assignment or the assignment is in any other status.</returns>
    public bool VolunteerHaveCall(int id)
    {
        Assignment? assignment;
        lock (AdminManager.BlMutex) //stage 7
            assignment =_dal.Assignment.ReadAll(a => a.VolunteerId == id).LastOrDefault();
        if (assignment == null)
            return false;
        if (assignment.TypeEndOfTreatment.Equals(BO.CallStatus.Treatment)
                                  || assignment.TypeEndOfTreatment.Equals(BO.CallStatus.TreatmentOfRisk)||assignment.EndTime==null)
            return true;
        else
             return false;
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


    /// <summary>
    /// Computes missing fields asynchronously and updates the volunteer in the DAL.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to update.</param>
    /// <param name="address">The address to compute coordinates for.</param>
    private async Task UpdateVolunteerFieldsAsync(DO.Volunteer volunteer)
    {
        try
        {
            var coordinate = await Helpers.Tools.GetAddressCoordinates(volunteer.Address); // קריאה אסינכרונית
            volunteer = volunteer with { Latitude = coordinate.Latitude, Longitude = coordinate.Longitude };
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(volunteer);
            VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating volunteer location for ID {volunteer.Id}: {ex.Message}");
        }
    }

}

