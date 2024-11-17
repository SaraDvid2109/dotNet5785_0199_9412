﻿namespace DalTest;
using DalApi;
using DO;
using Dal;
using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Reflection;

/// <summary>
/// Provides initialization logic for creating volunteers and other related entities.
/// </summary>
/// <remarks>
/// Some documentation was created using Chat GPT with manual adjustments & Some of the code too.
/// </remarks>
public static class Initialization
{
    //private static IAssignment? s_dalAssignment; // Data Access Layer (DAL) for assignments.
    //private static ICall? s_dalCall; // Data Access Layer (DAL) for calls.
    //private static IVolunteer? s_dalVolunteer; // Data Access Layer (DAL) for volunteers.
    //private static IConfig? s_dalConfig; // Data Access Layer (DAL) for configuration settings.
    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new(); // Random number generator for generating random data.
    //public static object DataSource { get; private set; } // next stage

    /// <summary>
    /// Initializes volunteers with random data and adds them to the database.
    /// </summary>
    /// <remarks>
    /// Some documentation was created using Chat GPT with manual adjustments & Some of the code too.
    /// </remarks>
    private static void CreateVolunteers()
    {
        //We asked the GPT chat: can you create for us an array of 15 names
        string[] VolunteerNames =
        { 
            "Amit Cohen", "Noa Levi", "Yonatan Mizrahi", "Maya Azulay", "Daniel Gold",
            "Hila Shalom", "Tomer Kaplan", "Yael Ben-David", "Eitan Sade", "Shira Hadad",
            "Lior Oren", "Roni Peretz", "Yaakov Azulay", "Eden Dayan", "Talia Mor"
        };

        //We asked the GPT chat: can you create for us an array of the prefixes on Israel's phone
        string[] prefixes = { "050", "052", "053", "054", "055", "058" };

        //We asked the GPT chat: can you create for us an array of 15 addresses in Israel
        string[] addresses =
        {
           "Herzl St 10, Tel Aviv, 6523701", "Ben Gurion St 5, Ramat Gan, 5222002",
           "Dizengoff St 25, Tel Aviv, 6311701", "Allenby St 40, Haifa, 3313302",
           "Jaffa St 60, Jerusalem, 9434101", "Rothschild Blvd 16, Tel Aviv, 6688112",
           "Weizmann St 12, Kfar Saba, 4428104", "HaNasi St 8, Herzliya, 4650403",
           "Sokolov St 30, Holon, 5831801", "Ben Yehuda St 100, Tel Aviv, 6347511",
           "Ehad HaAm St 50, Beersheba, 8455802", "Herzliya St 15, Netanya, 4241102",
           "Keren HaYesod St 22, Ashdod, 7728203", "Herzl St 45, Rishon LeZion, 7522001",
           "Moshe Dayan St 3, Ashkelon, 7878203"
        };

        //We asked the GPT chat: can you create for us an array of longitude lines and an array
        //of latitude lines corresponding to the above array respectively
        double[]? latitudes =
        {
           32.066158, 32.082271, 32.080480, 32.818409, 31.784217,
           32.063922, 32.175034, 32.166313, 32.014046, 32.089771,
           31.251810, 32.328516, 31.802418, 31.969633, 31.669258
        };
        
        double[]? longitudes =
        {
            34.779808, 34.812548, 34.774989, 34.988507, 35.223391,
            34.771805, 34.906552, 34.842972, 34.772101, 34.773922,
            34.791460, 34.853196, 34.641665, 34.804390, 34.574262
        };

        // Allowed characters for generating passwords.
        string allCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        int id;
        string? password = null;
        string email;
        string prefix;
        string phoneNumber;
        Roles role;

        int i = 0; 
        foreach (string volunteerName in VolunteerNames)
        {
            id = s_rand.Next(200000000, 400000000);

            //We asked the GPT chat: how to remove spaces and convert names to lowercase letters
            email = $"{VolunteerNames[i].Replace(" ", "").ToLower()}@gmail.com";

            prefix = prefixes[s_rand.Next(prefixes.Length)];
            phoneNumber = prefix;
            for (int j = 0; j < 7; j++) { phoneNumber += s_rand.Next(0, 10); }

            // We asked the GPT chat: How to build a random password using a StringBuilder object
            StringBuilder passwordBuilder = new StringBuilder();
            for (int j = 0; j < 7; j++) { passwordBuilder.Append(allCharacters[s_rand.Next(allCharacters.Length)]); }// chat gpt
            password = passwordBuilder.ToString();

            double MaximumDistance = s_rand.Next(0, 10);
            role = (i == 0) ? Roles.Management : Roles.Volunteer;

            try
            {
                s_dal!.Volunteer.Create(new(id, volunteerName, phoneNumber, email, password, addresses[i],
                          latitudes[i], longitudes[i], false, MaximumDistance, role, DistanceType.Aerial));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            i++;
        }  

    }

    /// <summary>
    /// Static class responsible for initialization of data for the application.
    /// </summary>
    /// <remarks>
    /// Some documentation was created using Chat GPT with manual adjustments & Some of the code too.
    /// </remarks>
    private static void CreateCalls()
    {

        //We asked the GPT chat: create an array for me that has 50 cases that call the MDA
        string?[] descriptionsArr = new string?[]
        {
            "Unconscious person", "Road accident with injuries", "Gunshot wound", "Severe burns",
            "Fall from height", "Infant not breathing", "Stroke symptoms", "Acute heart attack",
            "Severe shortness of breath", "Heavy bleeding", "Head injury", "Deep cut on hand",
            "Fall from bicycle", "Food poisoning", "Severe allergic reaction", "Chest pain",
            "Drowning", "Snake bite", "Workplace injury", "Minor burn from gas explosion",
            "Stab wound in abdomen", "Dehydration symptoms", "High fever and seizures",
            "Fall on stairs", "Sports injury", "Broken leg in accident", "Assault with sharp object",
            "Motorcycle accident injury", "Frostbite while outdoors", "Minor electric shock",
            "Sudden sharp back pain", "Scheduled check-up visit", "Blood pressure measurement request",
            "Routine blood test", "Medical transport request", "Assistance with medical equipment setup",
            "Consultation for ongoing symptoms", "Heart rate monitoring device setup",
            "Request for flu vaccination", "Consultation for mild allergy", "Routine blood sugar test",
            "Request for tetanus shot", "Follow-up on previous treatment", "Minor wound dressing change",
            "Routine elderly health assessment", "Work clearance health screening", "Pregnancy check-up",
            "Chronic pain management", "Diabetes management support", "Request for mobility aid assistance",
            "Health education session", "Guidance on post-surgery care", "Prescription refill assistance",
            "Dietitian consultation request", "Physical fitness assessment", "Medication side effect inquiry",
            "Physiotherapy session request", "Home safety evaluation", "Wellness check for remote patient",
            "Routine child vaccination"
        };

        //We asked the GPT chat: can you create for us an array of 50 addresses in Israel
        string[] addresses = new string[]
        {
            "Herzl St 10, Tel Aviv", "Ben Gurion St 5, Ramat Gan", "Dizengoff St 25, Tel Aviv",
            "Allenby St 40, Haifa", "Jaffa St 60, Jerusalem", "Rothschild Blvd 16, Tel Aviv",
            "Weizmann St 12, Kfar Saba", "HaNasi St 8, Herzliya", "Sokolov St 30, Holon",
            "Ben Yehuda St 100, Tel Aviv", "Ehad HaAm St 50, Beersheba", "Herzliya St 15, Netanya",
            "Keren HaYesod St 22, Ashdod", "Herzl St 45, Rishon LeZion", "Moshe Dayan St 3, Ashkelon",
            "Ben Tsvi St 10, Bat Yam", "Yitzhak Rabin St 20, Lod", "King George St 45, Tel Aviv",
            "Arlozorov St 100, Tel Aviv", "Aluf David St 5, Petah Tikva", "Habanim St 12, Hadera",
            "Shabazi St 18, Ramat Hasharon", "Levi Eshkol St 40, Ashkelon", "Weizmann St 6, Rehovot",
            "Jabotinsky St 15, Bnei Brak", "HaGalil St 10, Kiryat Shmona", "HaNasi Weizmann St 35, Haifa",
            "Moshe Dayan St 1, Ashdod", "Menachem Begin Blvd 55, Tel Aviv", "Hashalom Rd 10, Tel Aviv",
            "Shderot Chen St 45, Eilat", "Ayalon St 5, Rishon LeZion", "King Solomon St 20, Tiberias",
            "Rothschild Blvd 80, Tel Aviv", "Yigal Allon St 55, Ramat Gan", "Neve Shaanan St 3, Haifa",
            "Einstein St 12, Haifa", "Bar Ilan St 4, Givat Shmuel", "Yehuda Halevi St 40, Tel Aviv",
            "Haifa Rd 10, Acre", "Nahum St 1, Holon", "Eliezer Kaplan St 5, Herzliya",
            "Dov Hoz St 20, Be'er Sheva", "Moshe Sharet St 15, Ashkelon", "Haneviim St 60, Jerusalem",
            "Emek Refaim St 12, Jerusalem", "HaSolel St 1, Nazareth", "Hanamal St 4, Haifa",
            "HaKibbutz HaMeuhad St 6, Kfar Yona"
        };

        //We asked the GPT chat: can you create for us an array of longitude lines and an array
        //of latitude lines corresponding to the above array respectively
        double[] latitudes = new double[]
        {
            32.066158, 32.082271, 32.080480, 32.818409, 31.784217, 32.063922, 32.175034, 32.166313, 32.014046,
            32.089771, 31.251810, 32.328516, 31.802418, 31.969633, 31.669258, 32.018748, 31.951569, 32.073253,
            32.087601, 32.090678, 32.440987, 32.145339, 31.661712, 31.894756, 32.089611, 33.207333, 32.796785,
            31.803742, 32.071457, 32.061399, 29.55805, 31.973001, 32.785539, 32.070054, 32.788712, 32.110003,
            32.083762, 32.055893, 32.926099, 32.019313, 32.166313, 31.249872, 31.661712, 32.083307, 31.784217,
            31.765365, 32.696947, 32.823115, 32.317325, 32.392867
        };

        double[] longitudes = new double[]
        {
            34.779808, 34.812548, 34.774989, 34.988507, 35.223391, 34.771805, 34.906552, 34.842972, 34.772101,
            34.773922, 34.791460, 34.853196, 34.641665, 34.804390, 34.574262, 34.747685, 34.899520, 34.774281,
            34.791522, 34.887761, 34.923137, 34.838293, 34.571489, 34.812223, 34.834804, 35.570961, 35.003008,
            34.656998, 34.791613, 34.789561, 34.934200, 34.771497, 34.779572, 34.804868, 35.021502, 35.053653,
            34.824040, 34.773058, 35.066441, 34.767654, 34.842972, 34.771687, 34.571489, 34.799839, 35.223391,
            35.219762, 35.308230, 35.002729, 34.919138, 34.876413
        };

        string? Description;
        string Address;
        double? Latitude;
        double? Longitude;
        DateTime OpenTime;
        DateTime? MaxTime;
        CallType type;

        DateTime start = new DateTime(s_dal!.Config.Clock.Year - 0, 1, 1); //stage 1
        int range = (s_dal!.Config.Clock - start).Days; //stage 1

        int i = 0;
        foreach (string address in addresses)
        {
            Description = descriptionsArr[s_rand.Next(0, descriptionsArr.Length)];
            Address = address;
            Latitude = latitudes[i];
            Longitude = longitudes[i];

            OpenTime = start.AddDays(s_rand.Next(range));
            MaxTime = OpenTime.AddMinutes(s_rand.Next(5, 30));

            //
            type = (CallType)s_rand.Next(0, 2);

            try
            {
                s_dal!.Call.Create(new(0, Description, Address, Latitude, Longitude, OpenTime, MaxTime, type));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            
            i++;
        }

    }

    /// <summary>
    /// Initializes and creates a list of assignments for calls.
    /// </summary>
    /// <remarks>
    /// This method reads all existing calls and volunteers, assigns volunteers to calls,
    /// calculates the entry and end times, and determines the treatment end type. The 
    /// assignments are then added to the data source.
    /// Some documentation was created using Chat GPT with manual adjustments & Some of the code too.
    /// </remarks>
    private static void CreateAssignments()
    {

        //We asked the GPT chat: create an array for me that has 50 cases that call the MDA, the same arr as CreateCalls
        string?[] descriptionsArr = new string?[]
        {
            "Unconscious person", "Road accident with injuries", "Gunshot wound", "Severe burns",
            "Fall from height", "Infant not breathing", "Stroke symptoms", "Acute heart attack",
            "Severe shortness of breath", "Heavy bleeding", "Head injury", "Deep cut on hand",
            "Fall from bicycle", "Food poisoning", "Severe allergic reaction", "Chest pain",
            "Drowning", "Snake bite", "Workplace injury", "Minor burn from gas explosion",
            "Stab wound in abdomen", "Dehydration symptoms", "High fever and seizures",
            "Fall on stairs", "Sports injury", "Broken leg in accident", "Assault with sharp object",
            "Motorcycle accident injury", "Frostbite while outdoors", "Minor electric shock",
            "Sudden sharp back pain", "Scheduled check-up visit", "Blood pressure measurement request",
            "Routine blood test", "Medical transport request", "Assistance with medical equipment setup",
            "Consultation for ongoing symptoms", "Heart rate monitoring device setup",
            "Request for flu vaccination", "Consultation for mild allergy", "Routine blood sugar test",
            "Request for tetanus shot", "Follow-up on previous treatment", "Minor wound dressing change",
            "Routine elderly health assessment", "Work clearance health screening", "Pregnancy check-up",
            "Chronic pain management", "Diabetes management support", "Request for mobility aid assistance",
            "Health education session", "Guidance on post-surgery care", "Prescription refill assistance",
            "Dietitian consultation request", "Physical fitness assessment", "Medication side effect inquiry",
            "Physiotherapy session request", "Home safety evaluation", "Wellness check for remote patient",
            "Routine child vaccination"
        };

        //We asked the GPT chat: can you create for us an array of 50 addresses in Israel, the same arr as CreateCalls
        string[] addresses = new string[]
        {
            "Herzl St 10, Tel Aviv", "Ben Gurion St 5, Ramat Gan", "Dizengoff St 25, Tel Aviv",
            "Allenby St 40, Haifa", "Jaffa St 60, Jerusalem", "Rothschild Blvd 16, Tel Aviv",
            "Weizmann St 12, Kfar Saba", "HaNasi St 8, Herzliya", "Sokolov St 30, Holon",
            "Ben Yehuda St 100, Tel Aviv", "Ehad HaAm St 50, Beersheba", "Herzliya St 15, Netanya",
            "Keren HaYesod St 22, Ashdod", "Herzl St 45, Rishon LeZion", "Moshe Dayan St 3, Ashkelon",
            "Ben Tsvi St 10, Bat Yam", "Yitzhak Rabin St 20, Lod", "King George St 45, Tel Aviv",
            "Arlozorov St 100, Tel Aviv", "Aluf David St 5, Petah Tikva", "Habanim St 12, Hadera",
            "Shabazi St 18, Ramat Hasharon", "Levi Eshkol St 40, Ashkelon", "Weizmann St 6, Rehovot",
            "Jabotinsky St 15, Bnei Brak", "HaGalil St 10, Kiryat Shmona", "HaNasi Weizmann St 35, Haifa",
            "Moshe Dayan St 1, Ashdod", "Menachem Begin Blvd 55, Tel Aviv", "Hashalom Rd 10, Tel Aviv",
            "Shderot Chen St 45, Eilat", "Ayalon St 5, Rishon LeZion", "King Solomon St 20, Tiberias",
            "Rothschild Blvd 80, Tel Aviv", "Yigal Allon St 55, Ramat Gan", "Neve Shaanan St 3, Haifa",
            "Einstein St 12, Haifa", "Bar Ilan St 4, Givat Shmuel", "Yehuda Halevi St 40, Tel Aviv",
            "Haifa Rd 10, Acre", "Nahum St 1, Holon", "Eliezer Kaplan St 5, Herzliya",
            "Dov Hoz St 20, Be'er Sheva", "Moshe Sharet St 15, Ashkelon", "Haneviim St 60, Jerusalem",
            "Emek Refaim St 12, Jerusalem", "HaSolel St 1, Nazareth", "Hanamal St 4, Haifa",
            "HaKibbutz HaMeuhad St 6, Kfar Yona"
        };

        //We asked the GPT chat: can you create for us an array of longitude lines and an array
        //of latitude lines corresponding to the above array respectively, the same arrs as CreateCalls
        double[] latitudes = new double[]
        {
            32.066158, 32.082271, 32.080480, 32.818409, 31.784217, 32.063922, 32.175034, 32.166313, 32.014046,
            32.089771, 31.251810, 32.328516, 31.802418, 31.969633, 31.669258, 32.018748, 31.951569, 32.073253,
            32.087601, 32.090678, 32.440987, 32.145339, 31.661712, 31.894756, 32.089611, 33.207333, 32.796785,
            31.803742, 32.071457, 32.061399, 29.55805, 31.973001, 32.785539, 32.070054, 32.788712, 32.110003,
            32.083762, 32.055893, 32.926099, 32.019313, 32.166313, 31.249872, 31.661712, 32.083307, 31.784217,
            31.765365, 32.696947, 32.823115, 32.317325, 32.392867
        };

        double[] longitudes = new double[]
        {
            34.779808, 34.812548, 34.774989, 34.988507, 35.223391, 34.771805, 34.906552, 34.842972, 34.772101,
            34.773922, 34.791460, 34.853196, 34.641665, 34.804390, 34.574262, 34.747685, 34.899520, 34.774281,
            34.791522, 34.887761, 34.923137, 34.838293, 34.571489, 34.812223, 34.834804, 35.570961, 35.003008,
            34.656998, 34.791613, 34.789561, 34.934200, 34.771497, 34.779572, 34.804868, 35.021502, 35.053653,
            34.824040, 34.773058, 35.066441, 34.767654, 34.842972, 34.771687, 34.571489, 34.799839, 35.223391,
            35.219762, 35.308230, 35.002729, 34.919138, 34.876413
        };

        int CallId;
        int VolunteerId = 0;
        DateTime? EnterTime;
        DateTime? EndTime;
        EndType? TypeEndOfTreatment;

        IEnumerable<Call> CopyCalls = s_dal!.Call.ReadAll();
        IEnumerable<Volunteer> CopyVolunteers = s_dal!.Volunteer.ReadAll();

        DateTime start = new DateTime(s_dal!.Config.Clock.Year - 0, 1, 1); //stage 1
        int range = (s_dal!.Config.Clock - start).Days; //stage 1

        int i = 0;
        foreach (var call in CopyCalls)
        {
            if (i > 50) break;

            CallId = call.Id;
            if (CopyVolunteers.Any())
            {
                int randomIndex = s_rand.Next(CopyVolunteers.Count());
                VolunteerId = CopyVolunteers.ElementAt(randomIndex).Id;
            }
            //if (CopyVolunteers.Count > 0)
            //{
            //    int randomIndex = s_rand.Next(CopyVolunteers.Count);
            //    VolunteerId = CopyVolunteers[randomIndex].Id;
            //}

            //We asked the GPT chat: how to calculate the TimeSpan between the opening time
            //of a call (OpenTime) and the maximum time (MaxTime) with treatment in the case that MaxTime is not defined
            DateTime maxTime = call.MaxTime ?? DateTime.MaxValue;
            TimeSpan timeSpan = maxTime - call.OpenTime;

            //We asked the GPT chat: how to add random minutes to a certain time
            //(like OpenTime) and create an end time (EndTime)
            int randomMinutes = s_rand.Next(0, (int)timeSpan.TotalMinutes);
            EnterTime = call.OpenTime.AddMinutes(randomMinutes);
            EndTime = EnterTime.Value.AddMinutes(s_rand.Next(0, 60));

            if (EndTime <= maxTime)
                TypeEndOfTreatment = (EndType)s_rand.Next(0, 2);
            else
                TypeEndOfTreatment = EndType.ExpiredCancellation;

            try
            {
                s_dal!.Assignment.Create(new(0, CallId, VolunteerId, EnterTime, EndTime, TypeEndOfTreatment));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            i++;
        }

    }

    /// <summary>
    /// Handles the initialization of volunteers, calls, and assignments, resetting configuration and data lists.
    /// </summary>
    /// <param name="dalAssignment">Data access layer object for assignments. Cannot be null.</param>
    /// <param name="dalCall">Data access layer object for calls. Cannot be null.</param>
    /// <param name="dalVolunteer">Data access layer object for volunteers. Cannot be null.</param>
    /// <param name="dalConfig">Data access layer object for configuration. Cannot be null.</param>
    /// <exception cref="NullReferenceException">
    /// Thrown when any of the provided data access layer objects are null.
    /// </exception>
    /// <remarks>
    /// This method resets the data source, initializes volunteers, calls, and assignments, 
    /// and is designed to be the main setup function for creating the initial state of the system. 
    /// Some documentation was created using Chat GPT with manual adjustments & Some of the code too.
    /// </remarks>
    public static void Do(IDal dal) //stage 1
    {

        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");// check if we need it
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2

        Console.WriteLine("Reset Configuration values and List values.");
        //s_dalConfig.Reset(); //stage 1
        //s_dalVolunteer.DeleteAll(); //stage 1
        //s_dalAssignment.DeleteAll();
        //s_dalCall.DeleteAll();
        s_dal.ResetDB();//stage 2


        Console.WriteLine("Initializing Volunteers list.");
        CreateVolunteers();

        Console.WriteLine("Initializing Calls list.");
        CreateCalls();

        Console.WriteLine("Initializing Assignments list.");
        CreateAssignments();

    }

}

