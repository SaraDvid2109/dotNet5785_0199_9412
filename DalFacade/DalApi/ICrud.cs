using DO;
namespace DalApi;

/// <summary>
/// Interface for generic CRUD (Create, Read, Update, Delete) operations.
/// Provides a contract for managing entities in the Data Access Layer (DAL).
/// </summary>
/// <typeparam name="T">The type of the entity managed by this interface. Must be a reference type.</typeparam>
public interface ICrud<T> where T : class
{
    void Create(T item); //Creates new entity object in DAL
    T? Read(int id); //Reads entity object by its ID 
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2
    void Update(T item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
    T? Read(Func<T, bool> filter); // stage 2
}
