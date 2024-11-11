using Dal;
using DalApi;
using DO;///////

namespace DalTest;

internal class Program
{
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1

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

        private static void DisplayEntityMenu(string entityName)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine($"{entityName} Menu:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Add New Object");
                Console.WriteLine("2. View Object by ID");
                Console.WriteLine("3. View All Objects");
                Console.WriteLine("4. Update Object");
                Console.WriteLine("5. Delete Object");
                Console.WriteLine("6. Delete All Objects");

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
                        AdvanceClockMinute();
                        break;
                    case ConfigMenuOptions.AdvanceClockHour:
                        AdvanceClockHour();
                        break;
                    case ConfigMenuOptions.DisplayCurrentClock:
                        DisplayCurrentClock();
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

        private static int GetUserInput(Type enumType)
        {
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || !Enum.IsDefined(enumType, choice))
            {
                Console.WriteLine("Invalid input, please try again.");
            }
            return choice;
        }

        private static void InitializeData()
        {
            Console.WriteLine("Initializing data...");
            Initialization.Do(s_dalAssignment, s_dalCall,s_dalVolunteer,s_dalConfig); //stage 1)

    }

        private static void DisplayAllData()///////////////לשאול
        {
           Console.WriteLine("Displaying all data...");
          // הצגת כל הנתונים
          foreach (Volunteer volunteer in s_dalVolunteer?.ReadAll() ?? Enumerable.Empty<Volunteer>())
          {
            Console.WriteLine(volunteer);
          }

          foreach (Assignment assignment in s_dalAssignment?.ReadAll() ?? Enumerable.Empty<Assignment>())
          {
            Console.WriteLine(assignment);
          }

          foreach (Call call in s_dalCall?.ReadAll() ?? Enumerable.Empty<Call>())
          {
            Console.WriteLine(call);
          }
       
        }

        private static void ResetDatabase() 
        {
            Console.WriteLine("Resetting database...");
        // איפוס נתוני בסיס נתונים ונתוני תצורה
        if (s_dalVolunteer != null)
            s_dalVolunteer.DeleteAll(); //stage 1
        if (s_dalCall != null)
            s_dalCall.DeleteAll();
        if (s_dalAssignment != null)
            s_dalAssignment.DeleteAll();
        if (s_dalConfig != null)
            s_dalConfig.Reset(); //stage 1

        }

    private static void CreateEntity(string entityName)
    {
        try
        {
            //Action action = entityName switch
            //{
            //    "Volunteer" => CreateVolunteer,
            //    "Call" => CreateCall,
            //    "Assignment" => CreateAssignment,
            //    _ => () => Console.WriteLine("Invalid entity name") 
            //};

            switch (entityName)
            {
                case "Volunteer":
                    Volunteer volunteer=CreateVolunteer();
                    s_dalVolunteer?.Create(volunteer) ;
                    break;
                case "Call":
                    Call call = CreateCall();
                    s_dalCall?.Create(call);
                    break;
                case "Assignment":
                    Assignment assignment=CreateAssignment();
                    s_dalAssignment?.Create(assignment);
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

    // פונקציות עזר לקליטת קלט מהמשתמש
    //private static int GetIntInput(string prompt)
    //{
    //    Console.Write(prompt);
    //    while (!int.TryParse(Console.ReadLine(), out int result))
    //    {
    //        Console.WriteLine("Invalid input, please enter a valid number.");
    //        Console.Write(prompt);
    //    }
    //    return result;
    //}

    private static void ReadEntity(string entityName)
        {
            Console.WriteLine($"Reading object for {entityName}...");

            Console.WriteLine("Please enter the ID of the object:");
            int idInput = Console.Read();
            //if (!int.TryParse(idInput, out int id))
            //{
            //    Console.WriteLine("Invalid ID. Please enter a numeric value.");
            //    return;
            //}

        // בהתאם לשם הישות, מבצעים פעולה מתאימה
        switch (entityName)
        {
            case "Volunteer":
                Volunteer? volunteer = s_dalVolunteer?.Read(idInput);
                Console.WriteLine(volunteer);
                break;

            case "Call":
                Call? call = s_dalCall?.Read(idInput);
                Console.WriteLine(call);
                break;

            case "Assignment":
                Assignment? assignment = s_dalAssignment?.Read(idInput);
                Console.WriteLine(assignment);
                break;

            default:
                Console.WriteLine("Unknown entity type.");
                break;
        }
    
}

        private static void ReadAllEntities(string entityName)
        {
         Console.WriteLine($"Reading all objects for {entityName}...");
        switch (entityName)
        {
            case "Volunteer":
                List<Volunteer> ? volunteer = s_dalVolunteer?.ReadAll();
                Console.WriteLine(volunteer);
                break;

            case "Call":
                List<Call> ? call = s_dalCall?.ReadAll();
                Console.WriteLine(call);
                break;

            case "Assignment":
                List<Assignment>? assignment = s_dalAssignment?.ReadAll();
                Console.WriteLine(assignment);
                break;

            default:
                Console.WriteLine("Unknown entity type.");
                break;
        }
    }

    private static void UpdateEntity(string entityName)
    {
        Console.WriteLine($"Updating object for {entityName}...");
        try
        {
            switch (entityName)
            {
                case "Volunteer":
                    Volunteer volunteer = CreateVolunteer();
                    s_dalVolunteer?.Update(volunteer);
                    break;
                case "Call":
                    Call call=CreateCall();
                    s_dalCall?.Update(call);
                    break;
                case "Assignment":
                    Assignment assignment=CreateAssignment();
                    s_dalAssignment?.Update(assignment);
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

        private static void DeleteEntity(string entityName)
        {
            Console.WriteLine($"Deleting object for {entityName}...");
            Console.WriteLine("write {entityName} ID");
            int inputId=Console.Read();
        try
        {
            switch (entityName)
            {
                case "Volunteer":
                    s_dalVolunteer!.Delete(inputId);
                    break;
                case "Call":
                    s_dalCall!.Delete(inputId);
                    break;
                case "Assignment":
                    s_dalAssignment!.Delete(inputId);
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

        private static void DeleteAllEntities(string entityName)
        {
            Console.WriteLine($"Deleting all objects for {entityName}...");
        try
        {
            switch (entityName)
            {
                case "Volunteer":
                    s_dalVolunteer!.DeleteAll();
                    break;
                case "Call":
                    s_dalCall!.DeleteAll();
                    break;
                case "Assignment":
                    s_dalAssignment!.DeleteAll();
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

        private static void AdvanceClockMinute()
        {
            Console.WriteLine("Advancing clock by one minute...");
        // קידום שעון בדקה
        if (s_dalConfig != null)
            s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
        }

        private static void AdvanceClockHour()
        {
            Console.WriteLine("Advancing clock by one hour...");

        // קידום שעון בשעה
        if (s_dalConfig != null)
            s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
        }

        private static void DisplayCurrentClock()
        {
            Console.WriteLine("Displaying current clock value...");
          // הצגת ערך השעון
          Console.WriteLine($"Current Clock Time: {s_dalConfig?.Clock}");
        }

    private static void SetConfigValue()////////////////
        {
            Console.WriteLine("Setting a configuration value...");
        // קביעת ערך חדש למשתנה תצורה
        Console.WriteLine("Select:\n 0 - Clock\n 1 - RiskRange");
        int choice = Console.Read();
        switch (choice)
        {
            case 0:
                Console.WriteLine("Enter new Clock value in the format 'dd/MM/yyyy HH:mm':");
                DateTime clockValue = DateTime.Parse(Console.ReadLine() ?? "");
                if (s_dalConfig != null)
                    s_dalConfig.Clock = clockValue;
                break;
            case 1:
                Console.WriteLine("Enter new RiskRange value in the format 'hh:mm:ss':");
                TimeSpan RiskRangeValue = TimeSpan.Parse(Console.ReadLine() ?? "");
                if (s_dalConfig != null)
                    s_dalConfig.RiskRange = RiskRangeValue;
                break;
               
        }


    }

    private static void DisplayConfigValue()
        {
            Console.WriteLine("Displaying a configuration value...");
        // הצגת ערך משתנה תצורה

        Console.WriteLine("Select:\n 0 - Clock\n 1 - RiskRange");
        int choice = Console.Read();
        switch (choice)
        {
            case 0:
               Console.WriteLine($"Clock: {s_dalConfig?.Clock}");
               break;
            case 1:
                Console.WriteLine($"RiskRange: {s_dalConfig?.RiskRange}");
                break;
        }
        
        
    }

        private static void ResetConfigValues()
        {
            Console.WriteLine("Resetting configuration values...");
            // איפוס כל ערכי התצורה
           s_dalConfig?.Reset();//////////////////////////////////////////////////לבדוק
        }

    private static Volunteer CreateVolunteer()
{
    int Id;
    string Name;
    string Phone;
    string Mail;
    string? Password;
    string? Address;
    double? Latitude;
    double? Longitude;
    bool Active;
    double? MaximumDistance;
    Roles Role;
    DistanceType Type;

    Console.WriteLine("Enter Volunteer ID: ");
    int.TryParse(Console.ReadLine(), out Id);  // שגיאה אם לא ניתן להמיר את הקלט ל-int
    Console.WriteLine("Enter Volunteer Name:");
    Name = Console.ReadLine() ?? ""; 

    Console.WriteLine("Enter Volunteer Phone: ");
    Phone = Console.ReadLine() ?? "";
    Console.WriteLine("Enter Volunteer Mail: ");
    Mail = Console.ReadLine() ?? "";
    Console.WriteLine("Enter Volunteer Password: ");
    Password = Console.ReadLine();
    Console.WriteLine("Enter Volunteer Address: ");
    Address = Console.ReadLine();
    
    Console.WriteLine("Enter Volunteer Latitude: ");
    Latitude = double.TryParse(Console.ReadLine(), out double latitude) ? latitude : (double?)null;

    Console.WriteLine("Enter Volunteer Longitude: ");
    Longitude = double.TryParse(Console.ReadLine(), out double longitude) ? longitude : (double?)null;

    Console.WriteLine("Enter Volunteer Active (true/false): ");
    Active = bool.TryParse(Console.ReadLine(), out bool active) && active;

    Console.WriteLine("Enter Volunteer MaximumDistance: ");
    MaximumDistance = double.TryParse(Console.ReadLine(), out double maxDist) ? maxDist : (double?)null;

    Console.WriteLine("Enter Volunteer Role: ");
    string? roleInput = Console.ReadLine();
    Role = Enum.TryParse(roleInput, out Roles parsedRole) ? parsedRole : 0;

    Console.WriteLine("Enter Volunteer Type: ");
    string? typeInput = Console.ReadLine();
    Type = Enum.TryParse(typeInput, out DistanceType parsedType) ? parsedType : 0;

    // יצירת האובייקט
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

    private static Call CreateCall()
    {
        Console.WriteLine("Enter Call ID: ");
        int id = Console.Read();

        Console.WriteLine("Enter Call Description: ");
        string? description = Console.ReadLine();

        Console.WriteLine("Enter Call Address: ");
        string address = Console.ReadLine() ?? "";

        Console.WriteLine("Enter Latitude: ");
        double? latitude = double.TryParse(Console.ReadLine(), out double parsedLatitude) ? parsedLatitude : null;

        Console.WriteLine("Enter Longitude: ");
        double? longitude = double.TryParse(Console.ReadLine(), out double parsedLongitude) ? parsedLongitude : null;

        Console.WriteLine("Enter Open Time (format: yyyy-MM-dd HH:mm:ss): ");
        DateTime openTime = DateTime.Parse(Console.ReadLine()??"");

        Console.WriteLine("Enter Max Time (format: yyyy-MM-dd HH:mm:ss): ");
        DateTime? maxTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedMaxTime) ? parsedMaxTime : null;

        Console.WriteLine("Enter Call Type (0 = Type1, 1 = Type2, etc.): ");
        CallType type = (CallType)Enum.Parse(typeof(CallType), Console.ReadLine()??"");

        // יצירת אובייקט Call עם הנתונים שנקלטו
        Call call = new Call(
            Id: id,
            Description: description,
            Address: address,
            Latitude: latitude,
            Longitude: longitude,
            OpenTime: openTime,
            MaxTime: maxTime,
            type: type
        );
        return call;
    }
    private static Assignment CreateAssignment()
    {
        Console.WriteLine("Enter Assignment ID: ");
        int id = Console.Read();

        Console.WriteLine("Enter Call ID: ");
        int callId = Console.Read();

        Console.WriteLine("Enter Volunteer ID: ");
        int volunteerId = Console.Read();

        Console.WriteLine("Enter Enter Time (format: yyyy-MM-dd HH:mm:ss): ");
        DateTime? enterTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEnterTime) ? parsedEnterTime : null;

        Console.WriteLine("Enter End Time (format: yyyy-MM-dd HH:mm:ss): ");
        DateTime? endTime = DateTime.TryParse(Console.ReadLine(), out DateTime parsedEndTime) ? parsedEndTime : null;

        Console.WriteLine("Enter End Type (0 = Type1, 1 = Type2, etc.): ");
        EndType? typeEndOfTreatment = Enum.TryParse(typeof(EndType), Console.ReadLine(), out var parsedEndType)
            ? (EndType)parsedEndType
            : null;

        // יצירת אובייקט Assignment עם הנתונים שנקלטו
        Assignment assignment = new Assignment(
            Id: id,
            CallId: callId,
            VolunteerId: volunteerId,
            EnterTime: enterTime,
            EndTime: endTime,
            TypeEndOfTreatment: typeEndOfTreatment
        );
     return assignment;
    }
}


