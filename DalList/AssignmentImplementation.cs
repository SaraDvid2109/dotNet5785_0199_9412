﻿namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementation of the IAssignment interface for managing Assignment entities in the Data Access Layer (DAL).
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new Assignment entity and adds it to the data source.
    /// The ID is automatically generated using the next available ID from the configuration.
    /// </summary>
    /// <param name="item">The Assignment object to add.</param>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
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
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        Assignment? found = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (found != null)
            DataSource.Assignments.Remove(found);
        else
            throw new DalDoesNotExistException($"Assignment with ID ={id} does Not exist");
    }

    /// <summary>
    /// Deletes all Assignment entities from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll() => DataSource.Assignments.Clear();

    /// <summary>
    /// Reads an Assignment entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to read.</param>
    /// <returns>The Assignment object if found; otherwise, null.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Assignment? Read(int id) => DataSource.Assignments.FirstOrDefault(a => a.Id == id);

    /// <summary>
    /// Returns the first Assignment matching the filter, or null.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Assignment? Read(Func<Assignment, bool> filter) => DataSource.Assignments?.FirstOrDefault(filter);

    /// <summary>
    /// Reads all Assignment entities from the data source.
    /// </summary>
    /// <returns>A list of all Assignment objects.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
     => filter != null
            ? from item in DataSource.Assignments
              where filter(item)
              select item
            : from item in DataSource.Assignments
              select item;

    /// <summary>
    /// Updates an existing Assignment entity in the data source.
    /// </summary>
    /// <param name="item">The updated Assignment object.</param>
    /// <exception cref="NotImplementedException">Thrown if the Assignment with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Assignment item)
    {
        Assignment? found = DataSource.Assignments.Find(a => a.Id == item.Id);
        if (found != null)
        {
            DataSource.Assignments.Remove(found);
            DataSource.Assignments.Add(item);
        }
        else
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist");
    }
}


