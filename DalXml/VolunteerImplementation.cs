namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
/// <summary>
/// Implementation of the Ivolunteer interface for managing volunteer entities in the Data Access Layer (DAL).
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Adds a new Volunteer to the data source.
    /// </summary>
    /// <param name="item">The Volunteer object to add</param>
    /// <exception cref="DalAlreadyExistException">Thrown if a Volunteer with the same ID already exists.</exception>
    public void Create(Volunteer item)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (volunteers.Any(v => v.Id == item.Id))
            throw new DalAlreadyExistException($"Student with ID={item.Id} already exists");
        else
            volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteers_xml);
    }
    /// <summary>
    /// Deletes a Volunteer from the data source by ID.
    /// </summary>
    /// <param name="id">he ID of the Volunteer to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Volunteer with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (volunteers.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteers_xml);

    }
    /// <summary>
    /// Deletes all Volunteers from the data source.
    /// </summary>
    public void DeleteAll() => XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);
    /// <summary>
    /// Retrieves a Volunteer from the data source by ID.
    /// </summary>
    /// <param name="id">The ID of the Volunteer to retrieve.</param>
    /// <returns>The Volunteer object, or null if not found.</returns>
    public Volunteer? Read(int id)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return volunteers.FirstOrDefault(it => it.Id == id);
    }
    /// <summary>
    /// Returns the first Volunteer matching the filter, or null.
    /// </summary>
    /// <param name="filter">A predicate function used to filter the call.</param>
    /// <returns>The first volunteer that matches the filter, or null if no match is found.</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return volunteers.FirstOrDefault(filter);
    }
    /// <summary>
    /// Returns all volunteers that match the specified filter, or all volunteers if no filter is provided.
    /// </summary>
    /// <param name="filter">A predicate function used to filter the call.</param>
    /// <returns>objects that satisfy the filter condition, or all volunteers if no filter is specified</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return filter == null
            ? volunteers.Select(item => item)
            :volunteers.Where(filter);
    }
    /// <summary>
    /// Updates an existing Volunteer in the data source.
    /// </summary>
    /// <param name="item">The updated Volunteer object.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the Volunteer with the specified ID does not exist.<</exception>
    public void Update(Volunteer item)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (volunteers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist");
        volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(volunteers,Config.s_volunteers_xml);
    }
}
