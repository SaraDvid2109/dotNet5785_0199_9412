namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementation of the ICall interface for managing Call entities in the Data Access Layer (DAL).
/// </summary>
internal class CallImplementation : ICall
{

    /// <summary>
    /// Creates a new Call entity and adds it to the data source.
    /// The ID is automatically generated using the next available ID from the configuration.
    /// </summary>
    /// <param name="item">The Call object to add.</param>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Create(Call item)
    {
        int temp = Config.NextCallId;
        Call copyItem = item with { Id = temp};
        DataSource.Calls.Add(copyItem);
    }

    /// <summary>
    /// Deletes a Call entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to delete.</param>
    /// <exception cref="NotImplementedException">Thrown if the Call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        Call? found = DataSource.Calls.FirstOrDefault(c => c.Id == id);
        if (found != null)
            DataSource.Calls.Remove(found);
        else
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
    }

    /// <summary>
    /// Deletes all Call entities from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll() => DataSource.Calls.Clear();

    /// <summary>
    /// Reads a Call entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to read.</param>
    /// <returns>The Call object if found; otherwise, null.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(int id) => DataSource.Calls.FirstOrDefault(c => c.Id == id);

    /// <summary>
    /// Returns the first Call matching the filter, or null.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(Func<Call, bool> filter) => DataSource.Calls?.FirstOrDefault(filter);

    /// <summary>
    /// Reads all Call entities from the data source.
    /// </summary>
    /// <returns>A list of all Call objects.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
         => filter != null 
           ? from item in DataSource.Calls
             where filter(item)
             select item
           : from item in DataSource.Calls
             select item;

    /// <summary>
    /// Updates an existing Call entity in the data source.
    /// </summary>
    /// <param name="item">The updated Call object.</param>
    /// <exception cref="NotImplementedException">Thrown if the Call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Call item)
    {
        Call? found = DataSource.Calls.Find(c => c.Id == item.Id);
        if (found != null)
        {
            DataSource.Calls.Remove(found);
            DataSource.Calls.Add(item);
        }
        else
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
    }
}

