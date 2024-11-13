namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <summary>
/// Implementation of the IVolunteer interface, managing Volunteer entities in memory.
/// </summary>
public class VolunteerImplementation : IVolunteer
{

    /// <summary>
    /// Adds a new Volunteer to the data source.
    /// </summary>
    /// <param name="item">The Volunteer object to add.</param>
    /// <exception cref="NotImplementedException">Thrown if a Volunteer with the same ID already exists.</exception>
    public void Create(Volunteer item)
    {
        if (DataSource.Volunteers.Contains(item))
            throw new NotImplementedException($"Volunteer with ID={item.Id} already exists");
        else
            DataSource.Volunteers.Add(item);
    }

    /// <summary>
    /// Deletes a Volunteer from the data source by ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to delete.</param>
    /// <exception cref="NotImplementedException">Thrown if the Volunteer with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        Volunteer? found = DataSource.Volunteers.Find(v => v.Id == id);/*FirstOrDefault(v => v.Id == id);*/
        if (found != null)
        {
            DataSource.Volunteers.Remove(found);
            Console.WriteLine("Deleted");
        }
        else
            throw new NotImplementedException($"Volunteer with ID={id} does Not exist");
    }

    /// <summary>
    /// Deletes all Volunteers from the data source.
    /// </summary>
    public void DeleteAll() => DataSource.Volunteers.Clear();

    /// <summary>
    /// Retrieves a Volunteer from the data source by ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to retrieve.</param>
    /// <returns>The Volunteer object, or null if not found.</returns>
    public Volunteer? Read(int id) => DataSource.Volunteers.Find(v => v.Id == id);

    /// <summary>
    /// Retrieves all Volunteers from the data source.
    /// </summary>
    /// <returns>A list of all Volunteer objects.</returns>
    public List<Volunteer> ReadAll() => new List<Volunteer>(DataSource.Volunteers);

    /// <summary>
    /// Updates an existing Volunteer in the data source.
    /// </summary>
    /// <param name="item">The updated Volunteer object.</param>
    /// <exception cref="NotImplementedException">Thrown if the Volunteer with the specified ID does not exist.</exception>
    public void Update(Volunteer item)
    {
        Volunteer? found = DataSource.Volunteers.Find(v => v.Id == item.Id);
        if (found != null)
        {
            DataSource.Volunteers.Remove(found);
            DataSource.Volunteers.Add(item);
        }
        else
            throw new NotImplementedException($"Volunteer with ID={item.Id} does Not exist");
    }
}

