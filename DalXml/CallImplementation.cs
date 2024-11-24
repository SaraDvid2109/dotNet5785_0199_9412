namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        int temp = Config.NextCallId;
        Call copyItem = item with { Id = temp };
        calls.Add(copyItem);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_volunteer_xml); 
    }

    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_call_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_call_xml);
    }

    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        return calls.FirstOrDefault(it => it.Id == id);
    }

    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        return calls.FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        return filter == null
            ? calls.Select(item => item)
            : calls.Where(filter);
    }

    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_call_xml);
        if (calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_call_xml);
    }
}
