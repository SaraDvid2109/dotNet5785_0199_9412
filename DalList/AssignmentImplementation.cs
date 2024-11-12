namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <summary>
/// Implementation of the IAssignment interface for managing Assignment entities in the Data Access Layer (DAL).
/// </summary>
public class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new Assignment entity and adds it to the data source.
    /// The ID is automatically generated using the next available ID from the configuration.
    /// </summary>
    /// <param name="item">The Assignment object to add.</param>
    public void Create(Assignment item)
    {
        int temp = Config.NextAssignmentId;
        Assignment copyItem = item with { Id = temp };
        DataSource.Assignments.Add(copyItem);
    }

    /// <summary>
    /// Deletes an Assignment entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to delete.</param>
    /// <exception cref="NotImplementedException">Thrown if the Assignment with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        Assignment? found = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (found != null)
            DataSource.Assignments.Remove(found);
        else
            throw new NotImplementedException($"Assignment with ID ={id} does Not exist");
    }

    /// <summary>
    /// Deletes all Assignment entities from the data source.
    /// </summary>
    public void DeleteAll() => DataSource.Assignments.Clear();

    /// <summary>
    /// Reads an Assignment entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to read.</param>
    /// <returns>The Assignment object if found; otherwise, null.</returns>
    public Assignment? Read(int id) => DataSource.Assignments.FirstOrDefault(a => a.Id == id);

    /// <summary>
    /// Reads all Assignment entities from the data source.
    /// </summary>
    /// <returns>A list of all Assignment objects.</returns>
    public List<Assignment> ReadAll() => new List<Assignment>(DataSource.Assignments);

    /// <summary>
    /// Updates an existing Assignment entity in the data source.
    /// </summary>
    /// <param name="item">The updated Assignment object.</param>
    /// <exception cref="NotImplementedException">Thrown if the Assignment with the specified ID does not exist.</exception>
    public void Update(Assignment item)
    {
        Assignment? found = DataSource.Assignments.FirstOrDefault(a => a.Id == item.Id);
        if (found != null)
        {
            DataSource.Assignments.Remove(found);
            DataSource.Assignments.Add(item);
        }
        else
            throw new NotImplementedException($"Assignment with ID={item.Id} does Not exist");
    }
}


