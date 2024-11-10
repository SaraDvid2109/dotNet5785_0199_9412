namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{

    public void Create(Call item)
    {
        int temp = Config.NextCallId;
        Call copyItem = item with { Id = temp};
        DataSource.Calls.Add(copyItem);
    }

    public void Delete(int id)
    {
        Call? found = DataSource.Calls.FirstOrDefault(c => c.Id == id);
        if (found != null)
        {
            DataSource.Calls.Remove(found);
        }
        throw new NotImplementedException($"Student with ID={ id } does Not exist");
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        Call? found = DataSource.Calls.FirstOrDefault(c => c.Id == id);
        return found;
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);

    }

    public void Update(Call item)
    {
        Call? found = DataSource.Calls.FirstOrDefault(c => c.Id == item.Id);
        if (found != null)
        {
            DataSource.Calls.Remove(found);
            DataSource.Calls.Add(item);
        }
        throw new NotImplementedException($"Student with ID={item.Id} does Not exist");
    }
}

