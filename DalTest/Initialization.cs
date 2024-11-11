namespace DalTest;
using DalApi;
using DO;
using Dal;
using System;
using System.Data;
using System.Data.Common;
using System.Text;

public static class Initialization
{
    private static IAssignment? s_dalAssignment; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();

    //public static object DataSource { get; private set; }

    private static void CreateVolunteers()
    {

        string[] VolunteerNames =
        { "Amit Cohen",
        "Noa Levi",
        "Yonatan Mizrahi",
        "Maya Azulay",
        "Daniel Gold",
        "Hila Shalom",
        "Tomer Kaplan",
        "Yael Ben-David",
        "Eitan Sade",
        "Shira Hadad",
        "Lior Oren",
        "Roni Peretz",
        "Yaakov Azulay",
        "Eden Dayan",
        "Talia Mor" };

        string[] prefixes = { "050", "052", "053", "054", "055", "058" };//

        string[] addresses = new string[]
        {
         "Herzl St 10, Tel Aviv, 6523701",
         "Ben Gurion St 5, Ramat Gan, 5222002",
         "Dizengoff St 25, Tel Aviv, 6311701",
         "Allenby St 40, Haifa, 3313302",
         "Jaffa St 60, Jerusalem, 9434101",
         "Rothschild Blvd 16, Tel Aviv, 6688112",
         "Weizmann St 12, Kfar Saba, 4428104",
         "HaNasi St 8, Herzliya, 4650403",
         "Sokolov St 30, Holon, 5831801",
         "Ben Yehuda St 100, Tel Aviv, 6347511",
         "Ehad HaAm St 50, Beersheba, 8455802",
         "Herzliya St 15, Netanya, 4241102",
         "Keren HaYesod St 22, Ashdod, 7728203",
         "Herzl St 45, Rishon LeZion, 7522001",
         "Moshe Dayan St 3, Ashkelon, 7878203"
        };

        double[]? latitudes = new double[]
        {
            32.066158, // Herzl St 10, Tel Aviv
            32.082271, // Ben Gurion St 5, Ramat Gan
            32.080480, // Dizengoff St 25, Tel Aviv
            32.818409, // Allenby St 40, Haifa
            31.784217, // Jaffa St 60, Jerusalem
            32.063922, // Rothschild Blvd 16, Tel Aviv
            32.175034, // Weizmann St 12, Kfar Saba
            32.166313, // HaNasi St 8, Herzliya
            32.014046, // Sokolov St 30, Holon
            32.089771, // Ben Yehuda St 100, Tel Aviv
            31.251810, // Ehad HaAm St 50, Beersheba
            32.328516, // Herzliya St 15, Netanya
            31.802418, // Keren HaYesod St 22, Ashdod
            31.969633, // Herzl St 45, Rishon LeZion
            31.669258  // Moshe Dayan St 3, Ashkelon
        };

        double[]? longitudes = new double[]
        {
          34.779808, // Herzl St 10, Tel Aviv
          34.812548, // Ben Gurion St 5, Ramat Gan
          34.774989, // Dizengoff St 25, Tel Aviv
          34.988507, // Allenby St 40, Haifa
          35.223391, // Jaffa St 60, Jerusalem
          34.771805, // Rothschild Blvd 16, Tel Aviv
          34.906552, // Weizmann St 12, Kfar Saba
          34.842972, // HaNasi St 8, Herzliya
          34.772101, // Sokolov St 30, Holon
          34.773922, // Ben Yehuda St 100, Tel Aviv
          34.791460, // Ehad HaAm St 50, Beersheba
          34.853196, // Herzliya St 15, Netanya
          34.641665, // Keren HaYesod St 22, Ashdod
          34.804390, // Herzl St 45, Rishon LeZion
          34.574262  // Moshe Dayan St 3, Ashkelon
        };

        string allCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        int id;
        string? password = null;
        string email;
        string prefix;
        string phoneNumber;
        Roles role;
        //do

        //while (s_dalVolunteer!.Read(id) != null);
        for (int i = 0; i != 15; i++)
        {
            id = s_rand.Next(200000000, 400000000);
            email = $"{VolunteerNames[i].Replace(" ", "").ToLower()}@gmail.com";
            prefix = prefixes[s_rand.Next(prefixes.Length)];//
            phoneNumber = prefix;

            for (int j = 0; j < 11; j++)
            {
                phoneNumber += s_rand.Next(0, 10);
            }// chat gpt

            StringBuilder passwordBuilder = new StringBuilder();
            for (int j = 0; j < 7; j++)
            {
                passwordBuilder.Append(allCharacters[s_rand.Next(allCharacters.Length)]);
            }// chat gpt
            password = passwordBuilder.ToString();

            double MaximumDistance = s_rand.Next(0, 20);
            if (i == 0) { role = Roles.Management; }
            else { role = Roles.Volunteer; }
            //Volunteer temp = new Volunteer(id, phoneNumber, VolunteerNames[i], password, addresses[i], latitudes[i], longitudes[i], false, MaximumDistance, role, 0);
            try
            {
                s_dalVolunteer!.Create(new(id, phoneNumber, VolunteerNames[i], password, addresses[i],
                                       null, latitudes[i], longitudes[i], false, MaximumDistance, role, DistanceType.Aerial));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        //DateTime start = new DateTime(1995, 1, 1);
        //DateTime bdt = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

    }

    private static void CreateCalls()
    {
        string?[] descriptionsArr = new string?[]
    {
        "Unconscious person",
        "Road accident with injuries",
        "Gunshot wound",
        "Severe burns",
        "Fall from height",
        "Infant not breathing",
        "Stroke symptoms",
        "Acute heart attack",
        "Severe shortness of breath",
        "Heavy bleeding",
        "Head injury",
        "Deep cut on hand",
        "Fall from bicycle",
        "Food poisoning",
        "Severe allergic reaction",
        "Chest pain",
        "Drowning",
        "Snake bite",
        "Workplace injury",
        "Minor burn from gas explosion",
        "Stab wound in abdomen",
        "Dehydration symptoms",
        "High fever and seizures",
        "Fall on stairs",
        "Sports injury",
        "Broken leg in accident",
        "Assault with sharp object",
        "Motorcycle accident injury",
        "Frostbite while outdoors",
        "Minor electric shock",
        "Sudden sharp back pain",
        "Scheduled check-up visit",
        "Blood pressure measurement request",
        "Routine blood test",
        "Medical transport request",
        "Assistance with medical equipment setup",
        "Consultation for ongoing symptoms",
        "Heart rate monitoring device setup",
        "Request for flu vaccination",
        "Consultation for mild allergy",
        "Routine blood sugar test",
        "Request for tetanus shot",
        "Follow-up on previous treatment",
        "Minor wound dressing change",
        "Routine elderly health assessment",
        "Work clearance health screening",
        "Pregnancy check-up",
        "Chronic pain management",
        "Diabetes management support",
        "Request for mobility aid assistance",
        "Health education session",
        "Guidance on post-surgery care",
        "Prescription refill assistance",
        "Dietitian consultation request",
        "Physical fitness assessment",
        "Medication side effect inquiry",
        "Physiotherapy session request",
        "Home safety evaluation",
        "Wellness check for remote patient",
        "Routine child vaccination"
    };

        string[] addresses = new string[]
    {
        "Herzl St 10, Tel Aviv",
        "Ben Gurion St 5, Ramat Gan",
        "Dizengoff St 25, Tel Aviv",
        "Allenby St 40, Haifa",
        "Jaffa St 60, Jerusalem",
        "Rothschild Blvd 16, Tel Aviv",
        "Weizmann St 12, Kfar Saba",
        "HaNasi St 8, Herzliya",
        "Sokolov St 30, Holon",
        "Ben Yehuda St 100, Tel Aviv",
        "Ehad HaAm St 50, Beersheba",
        "Herzliya St 15, Netanya",
        "Keren HaYesod St 22, Ashdod",
        "Herzl St 45, Rishon LeZion",
        "Moshe Dayan St 3, Ashkelon",
        "Ben Tsvi St 10, Bat Yam",
        "Yitzhak Rabin St 20, Lod",
        "King George St 45, Tel Aviv",
        "Arlozorov St 100, Tel Aviv",
        "Aluf David St 5, Petah Tikva",
        "Habanim St 12, Hadera",
        "Shabazi St 18, Ramat Hasharon",
        "Levi Eshkol St 40, Ashkelon",
        "Weizmann St 6, Rehovot",
        "Jabotinsky St 15, Bnei Brak",
        "HaGalil St 10, Kiryat Shmona",
        "HaNasi Weizmann St 35, Haifa",
        "Moshe Dayan St 1, Ashdod",
        "Menachem Begin Blvd 55, Tel Aviv",
        "Hashalom Rd 10, Tel Aviv",
        "Shderot Chen St 45, Eilat",
        "Ayalon St 5, Rishon LeZion",
        "King Solomon St 20, Tiberias",
        "Rothschild Blvd 80, Tel Aviv",
        "Yigal Allon St 55, Ramat Gan",
        "Neve Shaanan St 3, Haifa",
        "Einstein St 12, Haifa",
        "Bar Ilan St 4, Givat Shmuel",
        "Yehuda Halevi St 40, Tel Aviv",
        "Haifa Rd 10, Acre",
        "Nahum St 1, Holon",
        "Eliezer Kaplan St 5, Herzliya",
        "Dov Hoz St 20, Be'er Sheva",
        "Moshe Sharet St 15, Ashkelon",
        "Haneviim St 60, Jerusalem",
        "Emek Refaim St 12, Jerusalem",
        "HaSolel St 1, Nazareth",
        "Hanamal St 4, Haifa",
        "HaKibbutz HaMeuhad St 6, Kfar Yona"
    };

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

        DateTime start = new DateTime(s_dalConfig!.Clock.Year - 0, 1, 1); //stage 1
        int range = (s_dalConfig.Clock - start).Days; //stage 1

        for (int i = 0; i != 50; i++)
        {
            Description = descriptionsArr[s_rand.Next(0, 59)];
            Address = addresses[i];
            Latitude = latitudes[i];
            Longitude = longitudes[i];

            OpenTime = start.AddDays(s_rand.Next(range));
            MaxTime = OpenTime.AddMinutes(s_rand.Next(5, 30));

            type = (CallType)s_rand.Next(0, 2);


            try
            {
                s_dalCall!.Create(new(0, Description, Address, Latitude, Longitude, OpenTime, MaxTime, type));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }


    }

    private static void CreateAssignments()
    {
        string?[] descriptionsArr = new string?[]
    {
        "Unconscious person",
        "Road accident with injuries",
        "Gunshot wound",
        "Severe burns",
        "Fall from height",
        "Infant not breathing",
        "Stroke symptoms",
        "Acute heart attack",
        "Severe shortness of breath",
        "Heavy bleeding",
        "Head injury",
        "Deep cut on hand",
        "Fall from bicycle",
        "Food poisoning",
        "Severe allergic reaction",
        "Chest pain",
        "Drowning",
        "Snake bite",
        "Workplace injury",
        "Minor burn from gas explosion",
        "Stab wound in abdomen",
        "Dehydration symptoms",
        "High fever and seizures",
        "Fall on stairs",
        "Sports injury",
        "Broken leg in accident",
        "Assault with sharp object",
        "Motorcycle accident injury",
        "Frostbite while outdoors",
        "Minor electric shock",
        "Sudden sharp back pain",
        "Scheduled check-up visit",
        "Blood pressure measurement request",
        "Routine blood test",
        "Medical transport request",
        "Assistance with medical equipment setup",
        "Consultation for ongoing symptoms",
        "Heart rate monitoring device setup",
        "Request for flu vaccination",
        "Consultation for mild allergy",
        "Routine blood sugar test",
        "Request for tetanus shot",
        "Follow-up on previous treatment",
        "Minor wound dressing change",
        "Routine elderly health assessment",
        "Work clearance health screening",
        "Pregnancy check-up",
        "Chronic pain management",
        "Diabetes management support",
        "Request for mobility aid assistance",
        "Health education session",
        "Guidance on post-surgery care",
        "Prescription refill assistance",
        "Dietitian consultation request",
        "Physical fitness assessment",
        "Medication side effect inquiry",
        "Physiotherapy session request",
        "Home safety evaluation",
        "Wellness check for remote patient",
        "Routine child vaccination"
    };

        string[] addresses = new string[]
    {
        "Herzl St 10, Tel Aviv",
        "Ben Gurion St 5, Ramat Gan",
        "Dizengoff St 25, Tel Aviv",
        "Allenby St 40, Haifa",
        "Jaffa St 60, Jerusalem",
        "Rothschild Blvd 16, Tel Aviv",
        "Weizmann St 12, Kfar Saba",
        "HaNasi St 8, Herzliya",
        "Sokolov St 30, Holon",
        "Ben Yehuda St 100, Tel Aviv",
        "Ehad HaAm St 50, Beersheba",
        "Herzliya St 15, Netanya",
        "Keren HaYesod St 22, Ashdod",
        "Herzl St 45, Rishon LeZion",
        "Moshe Dayan St 3, Ashkelon",
        "Ben Tsvi St 10, Bat Yam",
        "Yitzhak Rabin St 20, Lod",
        "King George St 45, Tel Aviv",
        "Arlozorov St 100, Tel Aviv",
        "Aluf David St 5, Petah Tikva",
        "Habanim St 12, Hadera",
        "Shabazi St 18, Ramat Hasharon",
        "Levi Eshkol St 40, Ashkelon",
        "Weizmann St 6, Rehovot",
        "Jabotinsky St 15, Bnei Brak",
        "HaGalil St 10, Kiryat Shmona",
        "HaNasi Weizmann St 35, Haifa",
        "Moshe Dayan St 1, Ashdod",
        "Menachem Begin Blvd 55, Tel Aviv",
        "Hashalom Rd 10, Tel Aviv",
        "Shderot Chen St 45, Eilat",
        "Ayalon St 5, Rishon LeZion",
        "King Solomon St 20, Tiberias",
        "Rothschild Blvd 80, Tel Aviv",
        "Yigal Allon St 55, Ramat Gan",
        "Neve Shaanan St 3, Haifa",
        "Einstein St 12, Haifa",
        "Bar Ilan St 4, Givat Shmuel",
        "Yehuda Halevi St 40, Tel Aviv",
        "Haifa Rd 10, Acre",
        "Nahum St 1, Holon",
        "Eliezer Kaplan St 5, Herzliya",
        "Dov Hoz St 20, Be'er Sheva",
        "Moshe Sharet St 15, Ashkelon",
        "Haneviim St 60, Jerusalem",
        "Emek Refaim St 12, Jerusalem",
        "HaSolel St 1, Nazareth",
        "Hanamal St 4, Haifa",
        "HaKibbutz HaMeuhad St 6, Kfar Yona"
    };

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


        List<Call> CopyCalls = s_dalCall!.ReadAll();
        List<Volunteer> CopyVolunteers = s_dalVolunteer!.ReadAll();

        DateTime start = new DateTime(s_dalConfig!.Clock.Year - 0, 1, 1); //stage 1
        int range = (s_dalConfig.Clock - start).Days; //stage 1

        for (int i = 0; i != 50; i++)
        {
            CallId = CopyCalls[i].Id;
            if (CopyVolunteers.Count > 0)
            {
                int randomIndex = s_rand.Next(CopyVolunteers.Count);
                VolunteerId = CopyVolunteers[randomIndex].Id;
            }

            DateTime maxTime = CopyCalls[i].MaxTime ?? DateTime.MaxValue;
            TimeSpan timeSpan = maxTime - CopyCalls[i].OpenTime;
            int randomMinutes = s_rand.Next(0, (int)timeSpan.TotalMinutes);
            EnterTime = CopyCalls[i].OpenTime.AddMinutes(randomMinutes);
            EndTime = EnterTime.Value.AddMinutes(s_rand.Next(0, 60));

            if (EndTime <= maxTime)
            {
                TypeEndOfTreatment = (EndType)s_rand.Next(0, 2);
            }
            else
            {
                TypeEndOfTreatment = EndType.ExpiredCancellation;
            }


            try
            {
                s_dalAssignment!.Create(new(0, CallId, VolunteerId, EnterTime, EndTime, TypeEndOfTreatment));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }


    }

    public static void Do(IAssignment? dalAssignment, ICall? dalCall, IVolunteer? dalVolunteer, IConfig? dalConfig) //stage 1
    {
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");

        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");// check if we need it

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset(); //stage 1
        s_dalVolunteer.DeleteAll(); //stage 1
        s_dalAssignment.DeleteAll();
        s_dalCall.DeleteAll();



        Console.WriteLine("Initializing Volunteers list ...");
        CreateVolunteers();

        Console.WriteLine("Initializing Calls list ...");
        CreateCalls();

        Console.WriteLine("Initializing Assignments list ...");
        CreateAssignments();


    }

}

