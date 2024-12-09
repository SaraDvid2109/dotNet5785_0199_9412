namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

/// <summary>
/// Implementation of the IAssignment interface for managing Assignment entities in the Data Access Layer (DAL).
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new Assignment entity and adds it to the data source.
    /// The ID is automatically generated using the next available ID from the configuration.
    /// </summary>
    /// <param name="item">The Assignment object to add.</param>
    public void Create(Assignment item)
    {
        XElement? assignmentRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        int temp = Config.NextAssignmentId;
        Assignment copyItem = item with { Id = temp };
        assignmentRoot.Add(createAssignmentElement(copyItem));
        XMLTools.SaveListToXMLElement(assignmentRoot, Config.s_assignments_xml);
    }
    /// <summary>
    /// Deletes an Assignment entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to delete</param>
    /// <exception cref="DO.DalDoesNotExistException">Thrown if the Assignment with the specified ID does not exist.</exception>
    public void Delete(int id)
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={id} does Not exist"))
        .Remove();
        XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    }
    /// <summary>
    /// Deletes all Assignment entities from the data source.
    /// </summary>
    public void DeleteAll()
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        assignmentRootElem.Elements().Remove();
        XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    }
    /// <summary>
    /// Reads an Assignment entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the Assignment to read.</param>
    /// <returns>The Assignment object if found; otherwise, null.</returns>
    public Assignment? Read(int id)
    {
        XElement? assignmentElem =
        XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return assignmentElem is null ? null : getAssignment(assignmentElem);

    }
    /// <summary>
    /// Returns the first Assignment matching the filter, or null.
    /// </summary>
    /// <param name="filter">A function to filter the assignments.</param>
    /// <returns>The first Assignment that matches the filter, or null if no match is found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter) =>
         XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => getAssignment(a)).FirstOrDefault(filter);
    /// <summary>
    /// Returns all assignments that match the specified filter, or all assignments if no filter is provided.
    /// </summary>
    /// <param name="filter">A predicate function used to filter the assignments. </param>
    /// <returns> objects that satisfy the filter condition, or all assignments if no filter is specified.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        IEnumerable<Assignment> assignments = assignmentsRootElem.Elements().Select(getAssignment);
        if (filter == null)
            return assignments;
        else
            return assignments.Where(filter);
    }
    /// <summary>
    /// Updates an existing Assignment entity in the data source.
    /// </summary>
    /// <param name="item">The updated Assignment object.</param>
    /// <exception cref="DO.DalDoesNotExistException">Thrown if the Assignment with the specified ID does not exist.</exception>
    public void Update(Assignment item)
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        (assignmentRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
                .Remove();

        assignmentRootElem.Add(new XElement("assignment", createAssignmentElement(item)));

        XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    }
    /// <summary>
    /// Converts a XElement into an Assignment object.
    /// </summary>
    /// <param name="a">The XElement containing the data to convert into an Assignment</param>
    /// <returns>A new Assignment object populated with data from the provided XElement</returns>
    /// <exception cref="FormatException">Thrown if mandatory fields ("Id", "CallId", or "VolunteerId") cannot be converted to the appropriate types.</exception>
    static Assignment getAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            CallId = a.ToIntNullable("CallId") ?? throw new FormatException("can't convert CallId"),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("can't convert VolunteerId"),
            EnterTime = a.ToDateTimeNullable("EnterTime"),
            EndTime= a.ToDateTimeNullable("EndTime"),
            TypeEndOfTreatment = a.ToEnumNullable<EndType>("TypeEndOfTreatment") ?? EndType.Treated,

        };
       
    }
    /// <summary>
    /// Converts a Assignment into an XElement object.
    /// </summary>
    /// <param name="a">The Assignment containing the data to convert into an XElement</param>
    /// <returns>A new XElement object populated with data from the provided Assignment</returns>
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
