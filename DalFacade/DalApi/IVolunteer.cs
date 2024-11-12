﻿namespace DalApi;
using DO;

/// <summary>
/// Interface for managing "Volunteer" entities in the Data Access Layer (DAL).
/// Provides methods for creating, reading, updating, and deleting volunteers.
/// </summary>
public interface IVolunteer
{
    void Create(Volunteer item); //Creates new entity object in DAL
    Volunteer? Read(int id); //Reads entity object by its ID 
    List<Volunteer> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Volunteer item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}
