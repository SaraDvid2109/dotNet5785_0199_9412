namespace DalApi;
using DO;

/// <summary>
///Interface for managing "Assignment" entities in the Data Access Layer (DAL).
/// Provides methods for creating, reading, updating, and deleting assignments 
/// </summary>
public interface IAssignment
{
    void Create(Assignment item); //Creates new entity object in DAL
    Assignment? Read(int id); //Reads entity object by its ID 
    List<Assignment> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Assignment item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}
