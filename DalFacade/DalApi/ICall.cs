namespace DalApi;
using DO;

/// <summary>
/// Interface for managing "Call" entities in the Data Access Layer (DAL).
/// Provides methods for creating, reading, updating, and deleting calls.
/// </summary>
public interface ICall
{
    void Create(Call item); //Creates new entity object in DAL
    Call? Read(int id); //Reads entity object by its ID 
    List<Call> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Call item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}