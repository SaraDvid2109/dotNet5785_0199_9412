namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (DataSource.Volunteers.Contains(item))
        { 
            throw new NotImplementedException($"Student with ID={item.Id} already exists");
        }

        DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        Volunteer? found = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (found != null)
        {
            DataSource.Volunteers.Remove(found);
        }
        throw new NotImplementedException($"Student with ID={id} does Not exist");
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        Volunteer? found = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        return found;
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);

    }

    public void Update(Volunteer item)
    {
        Volunteer? found = DataSource.Volunteers.FirstOrDefault(v => v.Id == item.Id);
        if (found != null)
        {
            DataSource.Volunteers.Remove(found);
            DataSource.Volunteers.Add(item);
        }
        throw new NotImplementedException($"Student with ID={item.Id} does Not exist");
    }
}

