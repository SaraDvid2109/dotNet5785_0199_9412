namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        XElement? assignmentRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        int temp = Config.NextAssignmentId;
        Assignment copyItem = item with { Id = temp };
        assignmentRoot.Add(createAssignmentElement(copyItem));
        //assignmentRoot.Add(copyItem);
        XMLTools.SaveListToXMLElement(assignmentRoot, Config.s_assignments_xml);
    }

    public void Delete(int id)
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={id} does Not exist"))
        .Remove();
        XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    }

    public void DeleteAll()
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        assignmentRootElem.Elements().Remove();
        XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    }

    public Assignment? Read(int id)
    {
        XElement? assignmentElem =
        XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return assignmentElem is null ? null : getAssignment(assignmentElem);

    }

    public Assignment? Read(Func<Assignment, bool> filter) =>
         XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => getAssignment(a)).FirstOrDefault(filter);

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        IEnumerable<Assignment> assignments = assignmentsRootElem.Elements().Select(getAssignment);
        if (filter == null)
            return assignments;
        else
            return assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
                .Remove();

        assignmentRootElem.Add(new XElement("assignment", createAssignmentElement(item)));

        XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    }

    static Assignment getAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallId = a.ToIntNullable("CallId") ?? throw new FormatException("can't convert CallId"),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("can't convert VolunteerId"),
            EnterTime = a.ToDateTimeNullable("EnterTime"),
            EndTime= a.ToDateTimeNullable("EndTime"),
            TypeEndOfTreatment = a.ToEnumNullable<EndType>("TypeEndOfTreatment") ?? EndType.Processed,

        };
       
    }

    static XElement createAssignmentElement(Assignment a)
    {
        XElement assignment = new XElement("assignment",new XElement("Id",a.Id),
                                                        new XElement("CallId", a.CallId),
                                                        new XElement("VolunteerId", a.VolunteerId),
                                                        new XElement("EnterTime", a.EnterTime),
                                                        new XElement("EndTime", a.EndTime),
                                                        new XElement("TypeEndOfTreatment", a.TypeEndOfTreatment));

        return assignment;
    }

}
