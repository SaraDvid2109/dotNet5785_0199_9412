namespace BlImplementation;
using BlApi;
using Helpers;

internal class volunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DO.Roles Login(string username, string password)
    {
        if (password.Length < 8 || password.Length > 12)
            throw new Exception("The password must be 8–12 characters long.");
        //chat GPT-How to check if a password contains all the required characters
        bool hasLower = password.Any(char.IsLower); //Lowercase letters
        bool hasUpper = password.Any(char.IsUpper); //Uppercase letters
        bool hasDigit = password.Any(char.IsDigit); //Numbers
        bool hasSpecial = password.Any(ch => "!@#$%^&*(),.?\"{}|<>".Contains(ch)); //Special characters
        bool isStrongPassword = hasLower && hasUpper && hasDigit && hasSpecial;

        if (!isStrongPassword)
            throw new ArgumentException("The password must include letters, numbers, and special characters.");

        var volunteer = _dal.Volunteer.ReadAll(v => v.Name == username && v.Password == password).FirstOrDefault();

        if (volunteer == null)
        {
            throw new UnauthorizedAccessException("There is no volunteer with that name or password.");
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
            groupedVolunteers = _dal.Volunteer.ReadAll().GroupBy(v => v.Active);
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
        try
        {
            DO.Volunteer? volunteer = _dal.Volunteer.Read(id);
            if (volunteer == null)
            {
                throw new Exception("There is no volunteer with this ID.");
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
        catch (Exception ex)
        {
            throw new Exception("There is no volunteer with this ID.", ex);
        }

    }
    public void UpdatingVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        Helpers.Tools.IntegrityCheck(volunteer);
        try
        {
            // בדיקת הרשאות
            var requester = _dal.Volunteer.Read(id);
            if (requester == null || (!requester.Role.Equals("Manager")) || requester.Id != volunteer.Id)
            {
                throw new Exception("You are not authorized to update this volunteer.");
            }
            var existingVolunteer = _dal.Volunteer.Read(volunteer.Id);
            if (existingVolunteer == null)
            {
                throw new Exception($"Volunteer with ID {volunteer.Id} not found.");
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
        catch (Exception ex)
        {

            throw new Exception("Error updating volunteer details: " + ex.Message);
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
                throw new Exception("The volunteer cannot be deleted.");
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting volunteer:" + ex.Message);
        }
    }
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        Helpers.Tools.IntegrityCheck(volunteer);
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
        catch (Exception ex) { throw new Exception(ex.Message); }
    }

}

   