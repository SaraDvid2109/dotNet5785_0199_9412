namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
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
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        int temp = Config.NextCallId;
        Call copyItem = item with { Id = temp };
        calls.Add(copyItem);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); 
    }

    /// <summary>
    /// Deletes a Call entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    ///  Deletes all Call entities from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void DeleteAll() => XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    
    /// <summary>
    /// Reads a Call entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call to read.</param>
    /// <returns>The Call object if found; otherwise, null.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(it => it.Id == id);
    }

    /// <summary>
    /// Returns the first Call matching the filter, or null.
    /// </summary>
    /// <param name="filter">A predicate function used to filter the call.</param>
    /// <returns>The first call that matches the filter, or null if no match is found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Returns all calls that match the specified filter, or all calls if no filter is provided.
    /// </summary>
    /// <param name="filter">A predicate function used to filter the call.</param>
    /// <returns>objects that satisfy the filter condition, or all calls if no filter is specified.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null
            ? calls.Select(item => item)
            : calls.Where(filter);
    }
   
    /// <summary>
    /// Updates an existing Call entity in the data source.
    /// </summary>
    /// <param name="item">The updated Call object.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }
}
