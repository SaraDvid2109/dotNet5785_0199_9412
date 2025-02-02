namespace DalTest;
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
        string[] addresses = {
    "15 HaNevi'im St, Jerusalem, Israel",
    "100 Dizengoff St, Tel Aviv, Israel",
    "50 Herzl St, Haifa, Israel",
    "5 Ben Gurion St, Herzliya, Israel",
    "20 Rothschild Blvd, Tel Aviv, Israel",
    "30 Hillel St, Jerusalem, Israel",
    "10 Weizmann St, Kfar Saba, Israel",
    "45 Allenby St, Tel Aviv, Israel",
    "1 Rothschild Blvd, Tel Aviv, Israel",
    "22 Derech HaShalom, Givatayim, Israel",
    "8 HaMelacha St, Netanya, Israel",
    "55 HaAtzmaut Blvd, Bat Yam, Israel",
    "12 Shaul HaMelech Blvd, Tel Aviv, Israel",
    "7 Keren Hayesod St, Jerusalem, Israel",
    "3 Begin Rd, Ramat Gan, Israel",
    "14 Ibn Gabirol St, Tel Aviv, Israel",
    "11 Jabotinsky St, Petah Tikva, Israel",
    "9 Nordau Blvd, Haifa, Israel",
    "6 King George St, Jerusalem, Israel",
    "4 Eilat St, Tel Aviv, Israel",
    "13 HaPalmach St, Beersheba, Israel",
    "27 HaYarkon St, Tel Aviv, Israel",
    "17 Ahad Ha'am St, Tel Aviv, Israel",
    "29 Sokolov St, Herzliya, Israel",
    "39 Rambam St, Bnei Brak, Israel",
    "33 HaHagana St, Ashdod, Israel",
    "21 Arlozorov St, Rishon LeZion, Israel",
    "47 David Remez St, Jerusalem, Israel",
    "19 Yehuda Halevi St, Tel Aviv, Israel",
    "25 HaHistadrut St, Haifa, Israel",
    "36 HaAliya St, Netanya, Israel",
    "42 HaRav Kook St, Tiberias, Israel",
    "18 Bialik St, Ramat Gan, Israel",
    "32 Kaplan St, Tel Aviv, Israel",
    "44 Menachem Begin Blvd, Ramat Gan, Israel",
    "48 Yigal Alon St, Tel Aviv, Israel",
    "53 Jaffa St, Jerusalem, Israel",
    "60 HaNasi St, Beersheba, Israel",
    "37 Moriah Blvd, Haifa, Israel",
    "8 Herzl St, Rehovot, Israel",
    "5 Hahashmonaim St, Modi'in, Israel",
    "22 HaTayasim Blvd, Holon, Israel",
    "31 Yehuda St, Petah Tikva, Israel",
    "26 Kdoshei HaShoa St, Ashkelon, Israel",
    "14 HaTzoran St, Kiryat Ata, Israel",
    "28 Sheshet HaYamim St, Rosh HaAyin, Israel",
    "9 HaShalom Rd, Herzliya, Israel",
    "34 Eshkol Blvd, Ashdod, Israel",
    "50 Ramat Yam St, Herzliya, Israel",
    "43 Hativat Golani St, Eilat, Israel"
};

        double[] longitudes = {
    35.2223, 34.7745, 34.9896, 34.8337, 34.7732, 35.2207, 34.9065, 34.7706, 34.7719, 34.8106,
    34.8576, 34.7518, 34.7822, 35.2201, 34.8123, 34.7827, 34.8832, 34.9891, 35.2209, 34.7624,
    34.7915, 34.7664, 34.7731, 34.8412, 34.8307, 34.6508, 34.7702, 35.2253, 34.7715, 34.9906,
    34.8589, 35.5432, 34.8108, 34.7813, 34.8016, 34.8109, 35.2203, 34.7922, 34.9897, 34.8102,
    34.9098, 34.7819, 34.8675, 35.0721, 34.9564, 34.8290, 34.7824, 34.8293, 34.9893, 34.9547
};

        double[] latitudes = {
    31.7801, 32.0869, 32.8153, 32.1613, 32.0650, 31.7798, 32.1773, 32.0614, 32.0624, 32.0697,
    32.3220, 32.0171, 32.0736, 31.7768, 32.0732, 32.0863, 32.0912, 32.8175, 31.7779, 32.0631,
    31.2521, 32.0800, 32.0663, 32.1646, 32.0847, 31.7953, 31.9616, 31.7645, 32.0629, 32.7953,
    32.3394, 32.7897, 32.0824, 32.0682, 32.0849, 32.0675, 31.7785, 32.0739, 31.2525, 32.0725,
    31.8954, 32.0072, 31.6622, 32.8421, 32.0998, 31.8192, 32.1754, 32.1896, 29.5577, 29.5554
};


        // Allowed characters for generating passwords.
        //string allCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
        string lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
        string upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string digits = "0123456789";
        string specialCharacters = "!@#$%^*(),.?\"{}|";
        string allCharacters = lowerCaseLetters + upperCaseLetters + digits + specialCharacters;
        int id;
        string? password = null;
        string email;
        string prefix;
        string phoneNumber;
        Roles role;
        bool Active = true;

        int i = 0;
        foreach (string volunteerName in VolunteerNames)
        {

            //We asked the GPT chat: how to remove spaces and convert names to lowercase letters
            email = $"{VolunteerNames[i].Replace(" ", "").ToLower()}@gmail.com";

            prefix = prefixes[s_rand.Next(prefixes.Length)];
            phoneNumber = prefix;
            for (int j = 0; j < 7; j++) { phoneNumber += s_rand.Next(0, 10); }
            if (i > 2) 
            { 
                // We asked the GPT chat: How to build a random password using a StringBuilder object
                StringBuilder passwordBuilder = new StringBuilder();
                passwordBuilder.Append(lowerCaseLetters[s_rand.Next(lowerCaseLetters.Length)]);
                passwordBuilder.Append(upperCaseLetters[s_rand.Next(upperCaseLetters.Length)]);
                passwordBuilder.Append(digits[s_rand.Next(digits.Length)]);
                passwordBuilder.Append(specialCharacters[s_rand.Next(specialCharacters.Length)]);
                for (int j = 0; j < 4; j++) { passwordBuilder.Append(allCharacters[s_rand.Next(allCharacters.Length)]); }// chat gpt
                    password = passwordBuilder.ToString();

                //id = s_rand.Next(200000000, 400000000);
                id = GenerateValidId();
            }
            else
            {
                if (i == 0)
                {
                    password = "12!@MAnager";
                    id = 123456782;
                }
                else if (i == 1)
                {
                    password = "vK2^*m!L";
                    id = 218311280;
                }
                else
                {
                    password = "dZ5!K!)}";
                    id = 289163735;
                }
            }

            double MaximumDistance = s_rand.Next(0, 10);
            role = (i == 0) ? Roles.Manager : Roles.Volunteer;
            if (i >= 10)
                Active = false;

            s_dal!.Volunteer.Create(new(id, volunteerName, phoneNumber, email, password, addresses[i],
                        latitudes[i], longitudes[i], Active, MaximumDistance, role, DistanceType.Aerial));

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
        string[] addresses = {
    "15 HaNevi'im St, Jerusalem, Israel",
    "100 Dizengoff St, Tel Aviv, Israel",
    "50 Herzl St, Haifa, Israel",
    "5 Ben Gurion St, Herzliya, Israel",
    "20 Rothschild Blvd, Tel Aviv, Israel",
    "30 Hillel St, Jerusalem, Israel",
    "10 Weizmann St, Kfar Saba, Israel",
    "45 Allenby St, Tel Aviv, Israel",
    "1 Rothschild Blvd, Tel Aviv, Israel",
    "22 Derech HaShalom, Givatayim, Israel",
    "8 HaMelacha St, Netanya, Israel",
    "55 HaAtzmaut Blvd, Bat Yam, Israel",
    "12 Shaul HaMelech Blvd, Tel Aviv, Israel",
    "7 Keren Hayesod St, Jerusalem, Israel",
    "3 Begin Rd, Ramat Gan, Israel",
    "14 Ibn Gabirol St, Tel Aviv, Israel",
    "11 Jabotinsky St, Petah Tikva, Israel",
    "9 Nordau Blvd, Haifa, Israel",
    "6 King George St, Jerusalem, Israel",
    "4 Eilat St, Tel Aviv, Israel",
    "13 HaPalmach St, Beersheba, Israel",
    "27 HaYarkon St, Tel Aviv, Israel",
    "17 Ahad Ha'am St, Tel Aviv, Israel",
    "29 Sokolov St, Herzliya, Israel",
    "39 Rambam St, Bnei Brak, Israel",
    "33 HaHagana St, Ashdod, Israel",
    "21 Arlozorov St, Rishon LeZion, Israel",
    "47 David Remez St, Jerusalem, Israel",
    "19 Yehuda Halevi St, Tel Aviv, Israel",
    "25 HaHistadrut St, Haifa, Israel",
    "36 HaAliya St, Netanya, Israel",
    "42 HaRav Kook St, Tiberias, Israel",
    "18 Bialik St, Ramat Gan, Israel",
    "32 Kaplan St, Tel Aviv, Israel",
    "44 Menachem Begin Blvd, Ramat Gan, Israel",
    "48 Yigal Alon St, Tel Aviv, Israel",
    "53 Jaffa St, Jerusalem, Israel",
    "60 HaNasi St, Beersheba, Israel",
    "37 Moriah Blvd, Haifa, Israel",
    "8 Herzl St, Rehovot, Israel",
    "5 Hahashmonaim St, Modi'in, Israel",
    "22 HaTayasim Blvd, Holon, Israel",
    "31 Yehuda St, Petah Tikva, Israel",
    "26 Kdoshei HaShoa St, Ashkelon, Israel",
    "14 HaTzoran St, Kiryat Ata, Israel",
    "28 Sheshet HaYamim St, Rosh HaAyin, Israel",
    "9 HaShalom Rd, Herzliya, Israel",
    "34 Eshkol Blvd, Ashdod, Israel",
    "50 Ramat Yam St, Herzliya, Israel",
    "43 Hativat Golani St, Eilat, Israel"
};

        double[] longitudes = {
    35.2223, 34.7745, 34.9896, 34.8337, 34.7732, 35.2207, 34.9065, 34.7706, 34.7719, 34.8106,
    34.8576, 34.7518, 34.7822, 35.2201, 34.8123, 34.7827, 34.8832, 34.9891, 35.2209, 34.7624,
    34.7915, 34.7664, 34.7731, 34.8412, 34.8307, 34.6508, 34.7702, 35.2253, 34.7715, 34.9906,
    34.8589, 35.5432, 34.8108, 34.7813, 34.8016, 34.8109, 35.2203, 34.7922, 34.9897, 34.8102,
    34.9098, 34.7819, 34.8675, 35.0721, 34.9564, 34.8290, 34.7824, 34.8293, 34.9893, 34.9547
};

        double[] latitudes = {
    31.7801, 32.0869, 32.8153, 32.1613, 32.0650, 31.7798, 32.1773, 32.0614, 32.0624, 32.0697,
    32.3220, 32.0171, 32.0736, 31.7768, 32.0732, 32.0863, 32.0912, 32.8175, 31.7779, 32.0631,
    31.2521, 32.0800, 32.0663, 32.1646, 32.0847, 31.7953, 31.9616, 31.7645, 32.0629, 32.7953,
    32.3394, 32.7897, 32.0824, 32.0682, 32.0849, 32.0675, 31.7785, 32.0739, 31.2525, 32.0725,
    31.8954, 32.0072, 31.6622, 32.8421, 32.0998, 31.8192, 32.1754, 32.1896, 29.5577, 29.5554
};


        string? Description;
        string Address;
        double? Latitude;
        double? Longitude;
        DateTime OpenTime;
        DateTime? MaxTime;
        CallType type;


        int i = 0;
        foreach (string address in addresses)
        {
            Description = descriptionsArr[s_rand.Next(0, descriptionsArr.Length)];
            Address = address;
            Latitude = latitudes[i];
            Longitude = longitudes[i];

            if ((i >= 5 && i < 20) || (i >= 35 && i < 40))
            {
                OpenTime = s_dal!.Config.Clock.AddMinutes(s_rand.Next(-7, -2));
                MaxTime = s_dal!.Config.Clock.AddMinutes(s_rand.Next(5, 40));
            }
            else
            {
                OpenTime = s_dal!.Config.Clock.AddDays(s_rand.Next(-50, -8)).AddMinutes(s_rand.Next(0, 60));
                MaxTime = OpenTime.AddMinutes(s_rand.Next(5, 40));
            }

            type = (CallType)s_rand.Next(0, 3);

            s_dal!.Call.Create(new(0, Description, Address, Latitude, Longitude, OpenTime, MaxTime, type));

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
        string[] addresses = {
    "15 HaNevi'im St, Jerusalem, Israel",
    "100 Dizengoff St, Tel Aviv, Israel",
    "50 Herzl St, Haifa, Israel",
    "5 Ben Gurion St, Herzliya, Israel",
    "20 Rothschild Blvd, Tel Aviv, Israel",
    "30 Hillel St, Jerusalem, Israel",
    "10 Weizmann St, Kfar Saba, Israel",
    "45 Allenby St, Tel Aviv, Israel",
    "1 Rothschild Blvd, Tel Aviv, Israel",
    "22 Derech HaShalom, Givatayim, Israel",
    "8 HaMelacha St, Netanya, Israel",
    "55 HaAtzmaut Blvd, Bat Yam, Israel",
    "12 Shaul HaMelech Blvd, Tel Aviv, Israel",
    "7 Keren Hayesod St, Jerusalem, Israel",
    "3 Begin Rd, Ramat Gan, Israel",
    "14 Ibn Gabirol St, Tel Aviv, Israel",
    "11 Jabotinsky St, Petah Tikva, Israel",
    "9 Nordau Blvd, Haifa, Israel",
    "6 King George St, Jerusalem, Israel",
    "4 Eilat St, Tel Aviv, Israel",
    "13 HaPalmach St, Beersheba, Israel",
    "27 HaYarkon St, Tel Aviv, Israel",
    "17 Ahad Ha'am St, Tel Aviv, Israel",
    "29 Sokolov St, Herzliya, Israel",
    "39 Rambam St, Bnei Brak, Israel",
    "33 HaHagana St, Ashdod, Israel",
    "21 Arlozorov St, Rishon LeZion, Israel",
    "47 David Remez St, Jerusalem, Israel",
    "19 Yehuda Halevi St, Tel Aviv, Israel",
    "25 HaHistadrut St, Haifa, Israel",
    "36 HaAliya St, Netanya, Israel",
    "42 HaRav Kook St, Tiberias, Israel",
    "18 Bialik St, Ramat Gan, Israel",
    "32 Kaplan St, Tel Aviv, Israel",
    "44 Menachem Begin Blvd, Ramat Gan, Israel",
    "48 Yigal Alon St, Tel Aviv, Israel",
    "53 Jaffa St, Jerusalem, Israel",
    "60 HaNasi St, Beersheba, Israel",
    "37 Moriah Blvd, Haifa, Israel",
    "8 Herzl St, Rehovot, Israel",
    "5 Hahashmonaim St, Modi'in, Israel",
    "22 HaTayasim Blvd, Holon, Israel",
    "31 Yehuda St, Petah Tikva, Israel",
    "26 Kdoshei HaShoa St, Ashkelon, Israel",
    "14 HaTzoran St, Kiryat Ata, Israel",
    "28 Sheshet HaYamim St, Rosh HaAyin, Israel",
    "9 HaShalom Rd, Herzliya, Israel",
    "34 Eshkol Blvd, Ashdod, Israel",
    "50 Ramat Yam St, Herzliya, Israel",
    "43 Hativat Golani St, Eilat, Israel"
};

        double[] longitudes = {
    35.2223, 34.7745, 34.9896, 34.8337, 34.7732, 35.2207, 34.9065, 34.7706, 34.7719, 34.8106,
    34.8576, 34.7518, 34.7822, 35.2201, 34.8123, 34.7827, 34.8832, 34.9891, 35.2209, 34.7624,
    34.7915, 34.7664, 34.7731, 34.8412, 34.8307, 34.6508, 34.7702, 35.2253, 34.7715, 34.9906,
    34.8589, 35.5432, 34.8108, 34.7813, 34.8016, 34.8109, 35.2203, 34.7922, 34.9897, 34.8102,
    34.9098, 34.7819, 34.8675, 35.0721, 34.9564, 34.8290, 34.7824, 34.8293, 34.9893, 34.9547
};

        double[] latitudes = {
    31.7801, 32.0869, 32.8153, 32.1613, 32.0650, 31.7798, 32.1773, 32.0614, 32.0624, 32.0697,
    32.3220, 32.0171, 32.0736, 31.7768, 32.0732, 32.0863, 32.0912, 32.8175, 31.7779, 32.0631,
    31.2521, 32.0800, 32.0663, 32.1646, 32.0847, 31.7953, 31.9616, 31.7645, 32.0629, 32.7953,
    32.3394, 32.7897, 32.0824, 32.0682, 32.0849, 32.0675, 31.7785, 32.0739, 31.2525, 32.0725,
    31.8954, 32.0072, 31.6622, 32.8421, 32.0998, 31.8192, 32.1754, 32.1896, 29.5577, 29.5554
};


        IEnumerable<Call> CopyCalls = s_dal!.Call.ReadAll();
        IEnumerable<Volunteer> CopyVolunteers = s_dal!.Volunteer.ReadAll();

        for (int i = 0; i < 5; i++)
        {
            int callId = CopyCalls.ElementAt(i).Id;
            int volunteerId = CopyVolunteers.ElementAt(i).Id;
            DateTime entryTime = s_dal!.Call.Read(i)!.OpenTime.AddMinutes(9 * i) <= s_dal!.Call.Read(i)!.MaxTime ? s_dal!.Call.Read(i)!.OpenTime.AddMinutes(9 * i) : s_dal!.Call.Read(i)!.MaxTime ?? DateTime.MaxValue;
            DateTime closeTime = s_dal!.Call.Read(i)!.MaxTime ?? DateTime.MaxValue;
            EndType endType = EndType.ExpiredCancellation;

            s_dal!.Assignment.Create(new Assignment(0, callId, volunteerId, entryTime, closeTime, endType));
        }

        for (int i = 8; i < 20; i++)
        {
            int callId = CopyCalls.ElementAt(i - 3).Id;
            int volunteerId = CopyVolunteers.ElementAt(i - 8).Id;
            DateTime systemClock = s_dal.Config.Clock;
            DateTime entryTime = s_dal!.Call.Read(i - 3)!.OpenTime.AddMinutes(i / 3) < s_dal!.Call.Read(i - 3)!.MaxTime ? s_dal!.Call.Read(i - 3)!.OpenTime.AddMinutes(i / 3) : s_dal!.Call.Read(i - 3)!.OpenTime.AddMinutes(5);
            if (entryTime >= systemClock) entryTime = systemClock;

            s_dal!.Assignment.Create(new Assignment(0, callId, volunteerId, entryTime, null, null));
        }

        for (int i = 20; i < 30; i++)
        {
            int callId = CopyCalls.ElementAt(i).Id;
            int volunteerId = CopyVolunteers.ElementAt(i - 20).Id;
            DateTime entryTime = s_dal!.Call.Read(i)!.OpenTime.AddMinutes(i) <= s_dal!.Call.Read(i)!.MaxTime ? s_dal!.Call.Read(i)!.OpenTime.AddMinutes(i) : s_dal!.Call.Read(i)!.MaxTime ?? DateTime.MaxValue;
            DateTime closeTime = (s_dal!.Call.Read(i)!.MaxTime?.AddMinutes(-i) > entryTime ? s_dal!.Call.Read(i)!.MaxTime?.AddMinutes(-i) : s_dal!.Call.Read(i)!.MaxTime) ?? DateTime.MaxValue;
            EndType endType = EndType.Treated;

            s_dal!.Assignment.Create(new Assignment(0, callId, volunteerId, entryTime, closeTime, endType));
        }

        for (int i = 30; i < 40; i++)
        {
            int callId = CopyCalls.ElementAt(i).Id;
            int volunteerId = CopyVolunteers.ElementAt(i - 30).Id;
            DateTime systemClock = s_dal.Config.Clock;
            DateTime entryTime = s_dal!.Call.Read(i)!.OpenTime.AddMinutes(i / 10) <= s_dal!.Call.Read(i)!.MaxTime ? s_dal!.Call.Read(i)!.OpenTime.AddMinutes(i / 10) : s_dal!.Call.Read(i)!.MaxTime ?? DateTime.MaxValue;
            DateTime closeTime = (s_dal!.Call.Read(i)!.MaxTime?.AddMinutes(-i / 10) > entryTime? s_dal!.Call.Read(i)!.MaxTime?.AddMinutes(-i / 10) : s_dal!.Call.Read(i)!.MaxTime) ?? DateTime.MaxValue;
            if (entryTime >= systemClock) entryTime = systemClock;
            if (closeTime >= systemClock) closeTime = systemClock;
            EndType endType = EndType.SelfCancellation;

            s_dal!.Assignment.Create(new Assignment(0, callId, volunteerId, entryTime, closeTime, endType));
        }

        for (int i = 40; i < 48; i++)
        {
            int callId = CopyCalls.ElementAt(i).Id;
            int volunteerId = CopyVolunteers.ElementAt(i - 40).Id;
            var call = s_dal!.Call.Read(i);
            if (call != null)
            {
                DateTime entryTime = call.OpenTime.AddMinutes(1) <= call.MaxTime  ? s_dal!.Call.Read(i)!.OpenTime.AddMinutes(1) : call.MaxTime ?? DateTime.MaxValue;
                DateTime closeTime = (call.MaxTime?.AddMinutes(-i / 4) > entryTime ? call.MaxTime?.AddMinutes(-i / 4) : call.MaxTime) ?? DateTime.MaxValue;
                EndType endType = EndType.AdminCancellation;

                s_dal!.Assignment.Create(new Assignment(0, callId, volunteerId, entryTime, closeTime, endType));
            }
            else
            {
                // Handle the case where call is null
                Console.WriteLine($"Call with index {i} not found.");
            }
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
    //public static void Do(IDal dal) //stage 1
    public static void Do() //stage 4
    {

        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");// check if we need it

        //s_dal = dal ?? throw new DalNullReferenceException("DAL object can not be null!"); // stage 2
        s_dal = DalApi.Factory.Get; //stage 4

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
    //We asked the GPT chat how to create a valid ID number.
    static int GenerateValidId()
    {
        int baseId = s_rand.Next(10000000, 100000000);

        // Calculate the check digit
        int checkDigit = CalculateCheckDigit(baseId);

        // append the check digit to the end of the number
        return baseId * 10 + checkDigit;
    }
    static int CalculateCheckDigit(int baseId)
    {
        int sum = 0;
        string idWithoutCheckDigit = baseId.ToString();

        for (int i = 0; i < idWithoutCheckDigit.Length; i++)
        {
            int digit = idWithoutCheckDigit[i] - '0';
            int weight = (i % 2 == 0) ? 1 : 2; // Digits in odd places are multiplied by 1, even places by 2
            int product = digit * weight;

            // If the result is 2 digits, add them together (e.g., 14 becomes 5)
            sum += product > 9 ? product - 9 : product;
        }

        // The check digit is the 10's complement of the sum
        int checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit;
    }

}

