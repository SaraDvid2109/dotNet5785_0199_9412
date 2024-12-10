namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;

internal class volunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DO.Roles Login(string username, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll(v => v.Name == username && v.Password == password).FirstOrDefault();

        if (volunteer == null)
        {
            throw new BlNullPropertyException("There is no volunteer with that name or password.");
        }
        return volunteer.Role;
    }

    public IEnumerable<BO.VolunteerInList> VolunteerList(bool? active, BO.VolunteerField? field)
    {
        IEnumerable<DO.Volunteer> volunteers;
        IEnumerable<IGrouping<bool, DO.Volunteer>> groupedVolunteers;
        IEnumerable<DO.Volunteer> sotrtVolunteers;
        if (active == null)
            volunteers = _dal.Volunteer.ReadAll();
        else
        {
            groupedVolunteers = _dal.Volunteer.ReadAll().GroupBy(v => v.Active == true);
            volunteers = groupedVolunteers.FirstOrDefault(g => g.Key == active.Value) ?? Enumerable.Empty<DO.Volunteer>();
        }
        if (field == null)
            sotrtVolunteers = volunteers.OrderBy(v => v.Id);
        else
        {
            sotrtVolunteers = field switch
            {
                BO.VolunteerField.Id => volunteers.OrderBy(v => v.Id),
                BO.VolunteerField.Name => volunteers.OrderBy(v => v.Name),
                //BO.VolunteerField.Active => volunteers.OrderBy(v => v.Active),
                BO.VolunteerField.Phone => volunteers.OrderBy(v => v.Phone),
                BO.VolunteerField.Mail => volunteers.OrderBy(v => v.Mail),
                BO.VolunteerField.Address => volunteers.OrderBy(v => v.Address),
                BO.VolunteerField.MaximumDistance => volunteers.OrderBy(v => v.MaximumDistance),
                BO.VolunteerField.Role => volunteers.OrderBy(v => v.Role),
                BO.VolunteerField.Type => volunteers.OrderBy(v => v.Type),
                _ => volunteers.OrderBy(v => v.Id)
            };
        }

        return sotrtVolunteers.Select(volunteer => new BO.VolunteerInList
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Active = volunteer.Active,
            TotalCallsHandled = 0,
            TotalCallsCanceled = 0,
            TotalCallsChosenHandleExpired = 0,
            CallHandledId = 0
        });

    }

    public BO.Volunteer GetVolunteerDetails(int id)
    {
        DO.Volunteer? volunteer = _dal.Volunteer.Read(id);
        if (volunteer == null)
        {
            throw new BO.BlDoesNotExistException("There is no volunteer with this ID.");
        }
        return new BO.Volunteer
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
    }
    public void UpdatingVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        Helpers.VolunteerManager.IntegrityCheck(volunteer);
        try
        {
            // בדיקת הרשאות
            var requester = _dal.Volunteer.Read(id);
            if (requester == null || (!requester.Role.Equals("Manager")) || requester.Id != volunteer.Id)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this volunteer.");
            }
            var existingVolunteer = _dal.Volunteer.Read(volunteer.Id);
            if (existingVolunteer == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volunteer.Id} not found.");
            }
            // בדיקת אילו שדות השתנו
            if (!requester.Role.Equals("Manager") && !existingVolunteer.Role.Equals(volunteer.Role))
            {
                throw new UnauthorizedAccessException("Only managers can update the role.");
            }
            DO.Volunteer volunteerToUpdate = new DO.Volunteer(
                volunteer.Id,
                volunteer.Name ?? string.Empty,
                volunteer.Phone ?? string.Empty,
                volunteer.Mail ?? string.Empty,
                volunteer.Password ?? string.Empty,
                volunteer.Address ?? string.Empty,
                volunteer.Latitude,
                volunteer.Longitude,
                volunteer.Active,
                volunteer.MaximumDistance,
                (DO.Roles)volunteer.Role,
                (DO.DistanceType)volunteer.Type);
            _dal.Volunteer.Update(volunteerToUpdate);
        }
        catch (DalDoesNotExistException ex)
        {

            throw new BlDoesNotExistException("Error updating volunteer details: " + ex.Message);
        }
    }
    public void DeleteVolunteer(int id)
    {
        try
        {
            DO.Volunteer? volunteerToDelete = _dal.Volunteer.Read(id);
            if (volunteerToDelete == null) throw new Exception("");
            if (!volunteerToDelete.Active)
                _dal.Volunteer.Delete(id);
            else
                throw new BO.OperationNotAllowedException("The volunteer cannot be deleted.");
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException("Error deleting volunteer:" + ex.Message);
        }
    }
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        Helpers.VolunteerManager.IntegrityCheck(volunteer);
        try
        {
            DO.Volunteer volunteerToAdd = new DO.Volunteer(
                volunteer.Id,
                volunteer.Name ?? string.Empty,
                volunteer.Phone ?? string.Empty,
                volunteer.Mail ?? string.Empty,
                volunteer.Password ?? string.Empty,
                volunteer.Address ?? string.Empty,
                volunteer.Latitude,
                volunteer.Longitude,
                volunteer.Active,
                volunteer.MaximumDistance,
                (DO.Roles)volunteer.Role,
                (DO.DistanceType)volunteer.Type);

            _dal.Volunteer.Create(volunteerToAdd);
        }
        catch (DalAlreadyExistException ex) { throw new BllAlreadyExistException(ex.Message); }
    }

}

   