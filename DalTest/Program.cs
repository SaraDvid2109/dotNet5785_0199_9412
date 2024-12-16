using Dal;
using DalApi;
using DO;
namespace DalTest;

/// <summary>
/// Main program for manually testing the data layer functionality
/// </summary>
internal class Program
{

    //private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    //private static ICall? s_dalCall = new CallImplementation(); //stage 1
    //private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    //private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1

    //static readonly IDal s_dal = new DalList(); //stage 2
    //static readonly IDal s_dal = new DalXml(); //stage 3
    static readonly IDal s_dal = Factory.Get; //stage 4

    static void Main(string[] args)
    {

        bool exit = false;
        while (!exit)
        {
            DisplayMainMenu();
            MainMenuOptions choice = (MainMenuOptions)GetUserInput(typeof(MainMenuOptions));

            switch (choice)
            {
                case MainMenuOptions.Exit:
                    exit = true;
                    break;
                case MainMenuOptions.VolunteerMenu:
                    DisplayEntityMenu("Volunteer");
                    break;
                case MainMenuOptions.CallMenu:
                    DisplayEntityMenu("Call");
                    break;
                case MainMenuOptions.AssignmentMenu:
                    DisplayEntityMenu("Assignment");
                    break;
                case MainMenuOptions.InitializeData:
                    InitializeData();
                    break;
                case MainMenuOptions.DisplayAllData:
                    DisplayAllData();
                    break;
                case MainMenuOptions.ConfigMenu:
                    DisplayConfigMenu();
                    break;
                case MainMenuOptions.ResetDatabase:
                    ResetDatabase();
                    break;
            }
        }
    }

    /// <summary>
    /// Show main menu
    /// </summary>
    private static void DisplayMainMenu()
    {
        Console.WriteLine("Main Menu:");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Display Submenu for Volunteer");
        Console.WriteLine("2. Display Submenu for Call");
        Console.WriteLine("3. Display Submenu for Assignment");
        Console.WriteLine("4. Initialize Data");
        Console.WriteLine("5. Display All Data");
        Console.WriteLine("6. Display Configuration Menu");
        Console.WriteLine("7. Reset Database");
    }

    /// <summary>
    /// Show submenu for entity
    /// </summary>
    /// <param name="entityName">The name of the entity</param>
    private static void DisplayEntityMenu(string entityName)
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine($"{entityName} Menu:");
            Console.WriteLine("0. Exit");
            Console.WriteLine($"1. Add New {entityName}");
            Console.WriteLine($"2. View {entityName} by ID");
            Console.WriteLine($"3. View All {entityName}s");
            Console.WriteLine($"4. Update {entityName}");
            Console.WriteLine($"5. Delete {entityName}");
            Console.WriteLine($"6. Delete All {entityName}s");
            //ChatGPT- How to get enum type input from the user.
            EntityMenuOptions choice = (EntityMenuOptions)GetUserInput(typeof(EntityMenuOptions));
            try
            {

                switch (choice)
                {
                    case EntityMenuOptions.Exit:
                        exit = true;
                        break;
                    case EntityMenuOptions.Create:
                        CreateEntity(entityName);
                        break;
                    case EntityMenuOptions.Read:
                        ReadEntity(entityName);
                        break;
                    case EntityMenuOptions.ReadAll:
                        ReadAllEntities(entityName);
                        break;
                    case EntityMenuOptions.Update:
                        UpdateEntity(entityName);
                        break;
                    case EntityMenuOptions.Delete:
                        DeleteEntity(entityName);
                        break;
                    case EntityMenuOptions.DeleteAll:
                        DeleteAllEntities(entityName);
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
    }

    /// <summary>
    /// Show submenu for configuration entity
    /// </summary>
    private static void DisplayConfigMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Configuration Menu:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Advance Clock by Minute");
            Console.WriteLine("2. Advance Clock by Hour");
            Console.WriteLine("3. Display Current Clock Value");
            Console.WriteLine("4. Set Configuration Value");
            Console.WriteLine("5. Display Configuration Value");
            Console.WriteLine("6. Reset All Configuration Values");

            ConfigMenuOptions choice = (ConfigMenuOptions)GetUserInput(typeof(ConfigMenuOptions));

            switch (choice)
            {
                case ConfigMenuOptions.Exit:
                    exit = true;
                    break;
                case ConfigMenuOptions.AdvanceClockMinute:
                    if (s_dal!= null) s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                    break;
                case ConfigMenuOptions.AdvanceClockHour:
                    if (s_dal != null) s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                    break;
                case ConfigMenuOptions.DisplayCurrentClock:
                    Console.WriteLine($"Current Clock Time: {s_dal!.Config.Clock}");
                    break;
                case ConfigMenuOptions.SetConfigValue:
                    SetConfigValue();
                    break;
                case ConfigMenuOptions.DisplayConfigValue:
                    DisplayConfigValue();
                    break;
                case ConfigMenuOptions.ResetConfigValues:
                    ResetConfigValues();
                    break;
            }
        }
    }

    /// <summary>
    /// Receiving a number from the user and checking that it is a valid value in an Enum
    /// </summary>
    /// <param name="enumType">Enum type value</param>
    /// <returns>The number entered by the user</returns>
    private static int GetUserInput(Type enumType)
    {
        //ChatGPT-How to check that the number the user entered is valid and also exists in the Enum
        int choice;
        if (!int.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(enumType, choice))
        {
            throw new DalFormatException("Invalid input, please try again.");
        }
        return choice;
    }

    private static void InitializeData()//לשאול אם האיתחול אמור להיות בתחילת המיין
    {
        Console.WriteLine("Initializing data.");
        //Initialization.Do(s_dal); //stage 2
        Initialization.Do(); //stage 4

    }

    /// <summary>
    /// Displaying all data in the database
    /// </summary>
    private static void DisplayAllData()
    {
        Console.WriteLine("Displaying all data.");
        try
        {
            if (s_dal.Volunteer.ReadAll() == null || !s_dal.Volunteer.ReadAll().Any() || s_dal.Assignment.ReadAll() == null || !s_dal.Assignment.ReadAll().Any()
                || s_dal.Call.ReadAll() == null || !s_dal.Call.ReadAll().Any())
                throw new DalNullReferenceException("Volunteer/ Assignment/ Call service is null");

            foreach (Volunteer volunteer in s_dal.Volunteer.ReadAll())
            {
                if (volunteer == null)
                    throw new DalNullReferenceException("Volunteer is null");
                Console.WriteLine(volunteer);
            }

            foreach (Assignment assignment in s_dal.Assignment.ReadAll())
            {
                if (assignment == null)
                    throw new DalNullReferenceException("Assignment is null");
                Console.WriteLine(assignment);
            }

            foreach (Call call in s_dal.Call.ReadAll())
            {
                if (call == null)
                    throw new DalNullReferenceException("Call is null");
                Console.WriteLine(call);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }


    }

    /// <summary>
    /// Database reset and configuration data reset
    /// </summary>
    private static void ResetDatabase()
    {
        Console.WriteLine("Resetting database.");
        //if (s_dalVolunteer != null) s_dalVolunteer.DeleteAll(); //stage 1
        //if (s_dalCall != null) s_dalCall.DeleteAll();
        //if (s_dalAssignment != null) s_dalAssignment.DeleteAll();
        //if (s_dalConfig != null) s_dalConfig.Reset(); //stage 1
        s_dal.ResetDB();
    }

    /// <summary>
    /// Adding a new object of the entity type to the list
    /// </summary>
    /// <param name="entityName">The name of the entity</param>
    private static void CreateEntity(string entityName)
    {
        try
        {

            switch (entityName)
            {
                case "Volunteer":
                    Volunteer volunteer = CreateVolunteer();
                    s_dal.Volunteer?.Create(volunteer);
                    break;
                case "Call":
                    Call call = CreateCall();
                    s_dal.Call?.Create(call);
                    break;
                case "Assignment":
                    Assignment assignment = CreateAssignment();
                    s_dal.Assignment?.Create(assignment);
                    break;
                default:
                    Console.WriteLine("Invalid entity name");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Object display by ID
    /// </summary>
    /// <param name="entityName">The name of the entity</param>
    /// <exception cref="FormatException">Throws if the ID number is invalid.</exception>
    private static void ReadEntity(string entityName)
    {
        Console.WriteLine($"Reading object for {entityName}.");

        Console.WriteLine($"Please enter the ID of the {entityName}:");
        if (!int.TryParse(Console.ReadLine(), out int idInput))
            throw new DalFormatException("Invalid ID. Please enter a numeric value.");

        switch (entityName)
        {
            case "Volunteer":
                Volunteer? volunteer = s_dal.Volunteer?.Read(idInput);
                if (volunteer == null)
                    throw new DalFormatException("Volunteer not found.");
                Console.WriteLine(volunteer);
                break;
            case "Call":
                Call? call = s_dal.Call?.Read(idInput);
                if (call == null)
                    throw new DalFormatException("Call not found.");
                Console.WriteLine(call);
                break;
            case "Assignment":
                Assignment? assignment = s_dal.Assignment?.Read(idInput);
                if (assignment == null)
                    throw new DalFormatException("Assignment not found.");
                Console.WriteLine(assignment);
                break;
            default:
                Console.WriteLine("Unknown entity type.");
                break;
        }

    }

    /// <summary>
    /// List view of all objects of the entity type
    /// </summary>
    /// <param name="entityName">The name of the entity</param>
    private static void ReadAllEntities(string entityName)
    {
        Console.WriteLine($"Reading all objects for {entityName}.");
        try
        {
            switch (entityName)
            {
                case "Volunteer":

                    IEnumerable<Volunteer>? volunteers = s_dal.Volunteer?.ReadAll();
                    if (volunteers != null && volunteers.Any())
                    {
                        foreach (Volunteer volunteer in volunteers)
                        {
                            Console.WriteLine(volunteer);
                        }
                    }
                    else
                    {
                        throw new DalFormatException("No volunteers");
                    }
                    break;

                case "Call":
                    IEnumerable<Call>? calls = s_dal.Call?.ReadAll();
                    if (calls != null && calls.Any())
                    {
                        foreach (Call call in calls)
                        {
                            Console.WriteLine(call);
                        }
                    }
                    else
                    {
                        throw new DalFormatException("No calls");
                    }
                    break;

                case "Assignment":
                    IEnumerable<Assignment>? assignments = s_dal.Assignment?.ReadAll();
                    if (assignments != null && assignments.Any())
                    {
                        foreach (Assignment assignment in assignments)
                        {
                            Console.WriteLine(assignment);
                        }
                    }
                    else
                    {
                        throw new DalFormatException("No assignments");
                    }
                    break;

                default:
                    Console.WriteLine("Unknown entity type.");
                    break;
            }
        }
        catch (DalFormatException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }

    /// <summary>
    /// Updating existing object data
    /// </summary>
    /// <param name="entityName">The name of the entity</param>
    private static void UpdateEntity(string entityName)
    {
        Console.WriteLine($"Updating object for {entityName}...");
        try
        {
            switch (entityName)
            {
                case "Volunteer":
                    Volunteer volunteer = CreateVolunteer();
                    s_dal.Volunteer?.Update(volunteer);
                    break;
                case "Call":
                    Call call = CreateCall();
                    s_dal.Call?.Update(call);
                    break;
                case "Assignment":
                    Assignment assignment = CreateAssignment();
                    s_dal.Assignment?.Update(assignment);
                    break;
                default:
                    Console.WriteLine("Invalid entity name");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deleting an existing object from the list
    /// </summary>
    /// <param name="entityName">The name of the entity</param>
    private static void DeleteEntity(string entityName)
    {
        Console.WriteLine($"Deleting object for {entityName}...");
        Console.WriteLine($"write {entityName} ID");
        if (!int.TryParse(Console.ReadLine(), out int inputId))
            throw new DalFormatException("Invalid ID. Please enter a numeric value.");
        try
        {
            switch (entityName)
            {
                case "Volunteer":
                    s_dal!.Volunteer.Delete(inputId);
                    break;
                case "Call":
                    s_dal!.Call.Delete(inputId);
                    break;
                case "Assignment":
                    s_dal!.Assignment.Delete(inputId);
                    break;
                default:
                    Console.WriteLine("Invalid entity name");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes all objects in a list of a specific entity
    /// </summary>
    /// <param name="entityName">The name of the entity</param>
    private static void DeleteAllEntities(string entityName)
    {
        Console.WriteLine($"Deleting all objects for {entityName}.");
        try
        {
            switch (entityName)
            {
                case "Volunteer":
                    s_dal!.Volunteer.DeleteAll();
                    break;
                case "Call":
                    s_dal!.Call.DeleteAll();
                    break;
                case "Assignment":
                    s_dal!.Assignment.DeleteAll();
                    break;
                default:
                    Console.WriteLine("Invalid entity name");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Setting a configuration value
    /// </summary>
    /// <exception cref="FormatException">Thrown if the input is invalid</exception>
    private static void SetConfigValue()
    {
        Console.WriteLine("Setting a configuration value.");
        Console.WriteLine("Select:\n 0 - Clock\n 1 - RiskRange");
        if (!int.TryParse(Console.ReadLine(), out int choice))
            throw new DalFormatException("invalid input!");
        switch (choice)
        {
            case 0:
                Console.WriteLine("Enter new Clock value in the format 'dd/MM/YYYY HH:mm':");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime clockValue))
                    throw new DalFormatException(" clock Value is invalid!");
                if (s_dal != null)
                    s_dal.Config.Clock = clockValue;
                break;
            case 1:
                Console.WriteLine("Enter new RiskRange value in the format 'hh:mm:ss':");
                if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan RiskRangeValue))
                    throw new DalFormatException(" Risk Range Value is invalid!");
                if (s_dal!= null)
                    s_dal.Config.RiskRange = RiskRangeValue;
                break;
            default:
                Console.WriteLine("Invalid choice. Please select 0 or 1.");
                break;

        }
    }

    /// <summary>
    /// Displaying a configuration value
    /// </summary>
    /// <exception cref="FormatException">Thrown if the input is invalid</exception>
    private static void DisplayConfigValue()
    {
        Console.WriteLine("Displaying a configuration value...");
        Console.WriteLine("Select:\n 0 - Clock\n 1 - RiskRange");
        if (!int.TryParse(Console.ReadLine(), out int choice))
            throw new DalFormatException("invalid input!");
        switch (choice)
        {
            case 0:
                Console.WriteLine($"Clock: {s_dal.Config.Clock}");
                break;
            case 1:
                Console.WriteLine($"RiskRange: {s_dal.Config.RiskRange}");
                break;
            default:
                Console.WriteLine("Invalid choice. Please select 0 or 1.");
                break;
        }
    }

    /// <summary>
    /// Reset all configuration values
    /// </summary>
    private static void ResetConfigValues()
    {
        Console.WriteLine("Resetting configuration values.");
        s_dal.Config.Reset();
    }

    /// <summary>
    /// Create a new volunteer with the user input 
    /// </summary>
    /// <returns>New volunteer</returns>
    /// <exception cref="FormatException">Thrown if the input is invalid</exception>
    private static Volunteer CreateVolunteer()
    {

        Console.WriteLine("Enter Volunteer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int Id))
            throw new DalFormatException("ID is invalid!");

        Console.WriteLine("Enter Volunteer Name:");
        string Name = Console.ReadLine() ?? "";
        Console.WriteLine("Enter Volunteer Phone: ");
        string Phone = Console.ReadLine() ?? "";
        Console.WriteLine("Enter Volunteer Mail: ");
        string Mail = Console.ReadLine() ?? "";
        Console.WriteLine("Enter Volunteer Password: ");
        string? Password = Console.ReadLine();
        Console.WriteLine("Enter Volunteer Address: ");
        string? Address = Console.ReadLine();

        Console.WriteLine("Enter Volunteer Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double Latitude))
            throw new DalFormatException("Latitude is invalid!");

        Console.WriteLine("Enter Volunteer Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double Longitude))
            throw new DalFormatException("Longitude is invalid!");

        Console.WriteLine("Enter Volunteer Active (true/false): ");
        bool Active = bool.TryParse(Console.ReadLine(), out bool active) && active;

        Console.WriteLine("Enter Volunteer MaximumDistance: ");
        if (!double.TryParse(Console.ReadLine(), out double MaximumDistance))
            throw new DalFormatException("Maximum Distance is invalid!");

        Console.WriteLine("Enter Volunteer Role: ");
        //ChatGPT- How to get enum type input from the user.
        Roles Role = Enum.TryParse(Console.ReadLine(), out Roles parsedRole) ? parsedRole : 0;

        Console.WriteLine("Enter Volunteer Type: ");
        DistanceType Type = Enum.TryParse(Console.ReadLine(), out DistanceType parsedType) ? parsedType : 0;

        Volunteer volunteer = new Volunteer(
            Id: Id,
            Name: Name,
            Phone: Phone,
            Mail: Mail,
            Password: Password,
            Address: Address,
            Latitude: Latitude,
            Longitude: Longitude,
            Active: Active,
            MaximumDistance: MaximumDistance,
            Role: Role,
            Type: Type
        );
        return volunteer;
    }

    /// <summary>
    /// Create a new call with the user input 
    /// </summary>
    /// <returns>New call</returns>
    /// <exception cref="FormatException">Thrown if the input is invalid</exception>
    /// 
    private static Call CreateCall()
    {
        Console.WriteLine("Enter Call Description:");
        string description = Console.ReadLine() ?? "";

        Console.WriteLine("Enter Call Address: ");
        string address = Console.ReadLine() ?? "";

        Console.WriteLine("Enter Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
            throw new DalFormatException("Latitude is invalid!");

        Console.WriteLine("Enter Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
            throw new DalFormatException("Latitude is invalid!");

        Console.WriteLine("Enter Open Time (format: YYYY-MM-dd HH:mm:ss): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime openTime))
            throw new DalFormatException("Open Time is invalid!");

        Console.WriteLine("Enter Max Time (format: YYYY-MM-dd HH:mm:ss): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime maxTime))
            throw new DalFormatException("Open Time is invalid!");

        Console.WriteLine("Enter Call Type (0 = Regular Vehicle, 1 = Ambulance, 3=Intensive Care Ambulance): ");
        CallType type = (CallType)Enum.Parse(typeof(CallType), Console.ReadLine() ?? "");

        Call call = new Call(
            Id: 0,
            Description: description,
            Address: address,
            Latitude: latitude,
            Longitude: longitude,
            OpenTime: openTime,
            MaxTime: maxTime,
            CarTypeToSend: type
        );
        return call;
    }

    /// <summary>
    /// Create a new assignment with the user input 
    /// </summary>
    /// <returns>New assignment</returns> 
    /// <exception cref="FormatException">Thrown if the input is invalid</exception>
    private static Assignment CreateAssignment()
    {
        Console.WriteLine("Enter Call ID: ");
        if (!int.TryParse(Console.ReadLine(), out int callId))
            throw new DalFormatException("Call ID is invalid!");

        Console.WriteLine("Enter Volunteer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int volunteerId))
            throw new DalFormatException("Volunteer ID is invalid!");

        Console.WriteLine("Enter Enter Time (format: YYYY-MM-dd HH:mm:ss): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime enterTime))
            throw new DalFormatException("Enter Time is invalid!");

        Console.WriteLine("Enter End Time (format: YYYY-MM-dd HH:mm:ss): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime))
            throw new DalFormatException("End Time is invalid!");

        Console.WriteLine("Enter End Type (0 - Processed, 1 - Admin Cancellation, 3 - Self Cancellation,4 - Expired Cancellation): ");

        EndType? typeEndOfTreatment = Enum.TryParse(typeof(EndType), Console.ReadLine(), out var parsedEndType)
            ? (EndType)parsedEndType : null;

        Assignment assignment = new Assignment(
            Id: 0,
            CallId: callId,
            VolunteerId: volunteerId,
            EnterTime: enterTime,
            EndTime: endTime,
            TypeEndOfTreatment: typeEndOfTreatment
        );
        return assignment;
    }
}


