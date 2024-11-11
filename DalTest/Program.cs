using Dal;
using DalApi;

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
                    case MainMenuOptions.Entity1Menu:
                        DisplayEntityMenu("Entity 1");
                        break;
                    case MainMenuOptions.Entity2Menu:
                        DisplayEntityMenu("Entity 2");
                        break;
                    case MainMenuOptions.Entity3Menu:
                        DisplayEntityMenu("Entity 3");
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
            Console.WriteLine("1. Display Submenu for Entity 1");
            Console.WriteLine("2. Display Submenu for Entity 2");
            Console.WriteLine("3. Display Submenu for Entity 3");
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
            // קריאה למתודה Initialization.Do
        }

        private static void DisplayAllData()
        {
            Console.WriteLine("Displaying all data...");
            // הצגת כל הנתונים
        }

        private static void ResetDatabase()
        {
            Console.WriteLine("Resetting database...");
            // איפוס נתוני בסיס נתונים ונתוני תצורה
        }

        private static void CreateEntity(string entityName)
        {
            Console.WriteLine($"Creating new object for {entityName}...");
            // קלט ויצירת אובייקט חדש
        }

        private static void ReadEntity(string entityName)
        {
            Console.WriteLine($"Reading object for {entityName}...");
            // קלט מזהה והצגת אובייקט
        }

        private static void ReadAllEntities(string entityName)
        {
            Console.WriteLine($"Reading all objects for {entityName}...");
            // הצגת כל האובייקטים מטיפוס הישות
        }

        private static void UpdateEntity(string entityName)
        {
            Console.WriteLine($"Updating object for {entityName}...");
            // עדכון אובייקט
        }

        private static void DeleteEntity(string entityName)
        {
            Console.WriteLine($"Deleting object for {entityName}...");
            // מחיקת אובייקט
        }

        private static void DeleteAllEntities(string entityName)
        {
            Console.WriteLine($"Deleting all objects for {entityName}...");
            // מחיקת כל האובייקטים
        }

        private static void AdvanceClockMinute()
        {
            Console.WriteLine("Advancing clock by one minute...");
            // קידום שעון בדקה
        }

        private static void AdvanceClockHour()
        {
            Console.WriteLine("Advancing clock by one hour...");
            // קידום שעון בשעה
        }

        private static void DisplayCurrentClock()
        {
            Console.WriteLine("Displaying current clock value...");
            // הצגת ערך השעון
        }

        private static void SetConfigValue()
        {
            Console.WriteLine("Setting a configuration value...");
            // קביעת ערך חדש למשתנה תצורה
        }

        private static void DisplayConfigValue()
        {
            Console.WriteLine("Displaying a configuration value...");
            // הצגת ערך משתנה תצורה
        }

        private static void ResetConfigValues()
        {
            Console.WriteLine("Resetting configuration values...");
            // איפוס כל ערכי התצורה
        }
    }521


