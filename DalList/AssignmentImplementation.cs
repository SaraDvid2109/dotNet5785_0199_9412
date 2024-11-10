namespace Dal;
using DalApi;
using DO;
using System.Collections;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int temp = Config.NextAssignmentId;
        Assignment copyItem = item with { Id = temp };
        DataSource.Assignments.Add(copyItem);
    }

    public void Delete(int id)
    {
        Assignment? found = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (found != null)
        {
            DataSource.Assignments.Remove(found);
        }
        throw new NotImplementedException($"Student with ID ={ id } does Not exist");
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        Assignment? found = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        return found;
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);

    }

    public void Update(Assignment item)
    {
        Assignment? found = DataSource.Assignments.FirstOrDefault(a => a.Id == item.Id);
        if (found != null)
        {
            DataSource.Assignments.Remove(found);
            DataSource.Assignments.Add(item);
        }
        throw new NotImplementedException($"Student with ID={item.Id} does Not exist");
    }
}


