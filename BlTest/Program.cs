using System;
using System.Collections.Generic;
using System.Text.Json;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;

namespace BlTest
{
    class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Manage Volunteers");
                Console.WriteLine("2. Manage Calls");
                Console.WriteLine("3. Manage Admin");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ManageVolunteers();
                        break;
                    case 2:
                        ManageCalls();
                        break;
                    case 3:
                        ManageAdmin();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
     

        private static void ManageVolunteers()
        {
            while (true)
            {
                Console.WriteLine("Volunteer Management:");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Display volunteer list (sorted and filtered)");
                Console.WriteLine("3. Display volunteer details");
                Console.WriteLine("4. Update volunteer");
                Console.WriteLine("5. Delete volunteer");
                Console.WriteLine("6. Add volunteer");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            Console.Write("Enter Username: ");
                            string username = Console.ReadLine()??"";
                            if (string.IsNullOrWhiteSpace(username))
                                throw new BO.BlFormatException("Invalid input: Username cannot be empty.");
                           
                            Console.Write("Enter Password: ");
                            string password = Console.ReadLine()??"";
                            if (string.IsNullOrWhiteSpace(password))
                                throw new BO.BlFormatException("Invalid input: Password cannot be empty.");
                            try
                            {
                                BO.Roles role = s_bl.volunteer.Login(username, password);
                                Console.WriteLine($"Your name: {username}  Your role: {role}");
                            }
                            catch (BlNullPropertyException ex)
                            {
                                Console.WriteLine($"Login failed: {ex.Message}");
                            }
                            break;


                        case 2:
                            Console.Write("Please enter true or false for active status: ");
                            //bool.TryParse(Console.ReadLine(), out bool status);
                            //if (!bool.TryParse(Console.ReadLine(), out bool status))
                            //    throw new BO.BlFormatException("Invalid input. Please try again.");
                            string? input = Console.ReadLine(); 
                            bool? status = string.IsNullOrWhiteSpace(input)
                                ? null
    :                           bool.TryParse(input, out bool parsedStatus)
                                ? parsedStatus
        :                       throw new BO.BlFormatException("Invalid input. Please enter true, false, or leave empty.");
                            
                            Console.Write("Sort by field (0:Id 1:Name 2:Active ): ");
                            if (!Enum.TryParse(Console.ReadLine(), true, out BO.VolunteerField sortField))
                                sortField = VolunteerField.Id;
                            //throw new BO.BlFormatException("Invalid Field.");
                            try
                            {
                                //Console.WriteLine($"the field:{sortField} and active:{status}");
                                var volunteerList = s_bl.volunteer.VolunteerList(status, sortField);
                                //Console.WriteLine($"the field:{sortField} and active:{status}");
                                //if (volunteerList != null && volunteerList.Any())
                                //{
                                //    foreach (BO.VolunteerInList vol in volunteerList)
                                //    {
                                //        Console.WriteLine(vol);
                                //    }
                                //}
                                //else
                                //{
                                //    throw new DalFormatException("No volunteers");
                                //}
                                //if (volunteerList == null|| !volunteerList.Any())////////////////////////////////
                                //    throw new BO.BlFormatException("the list is empty");
                                foreach (var v in volunteerList)
                                {
                                    Console.WriteLine(v);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in VolunteerList: {ex.Message}");
                            }
                            break;

                        case 3:
                            Console.Write("Enter Volunteer ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                               throw new BO.BlFormatException("Invalid input. Please enter a valid numeric ID.");
                            try
                            {
                                var volunteerDetails = s_bl.volunteer.GetVolunteerDetails(volunteerId);
                                Console.WriteLine(volunteerDetails);
                            }
                            catch (BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;
                        case 4:
                            Console.Write("Enter your ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int id))
                                throw new BO.BlFormatException("ID is invalid!");
                            BO.Volunteer updatedVolunteer=CreateVolunteer();
                            try
                            {
                                s_bl.volunteer.UpdatingVolunteerDetails(id, updatedVolunteer);
                                Console.WriteLine("Volunteer updated successfully.");
                            }
                            catch (BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;

                        case 5:
                            Console.Write("Enter Volunteer ID to delete: ");
                            if (!int.TryParse(Console.ReadLine(), out int deleteId))
                                throw new BO.BlFormatException("Invalid input. Please enter a valid numeric ID.");
                            try
                            {
                                s_bl.volunteer.DeleteVolunteer(deleteId);
                                Console.WriteLine("Volunteer deleted successfully.");
                            }
                            catch (BlDoesNotExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;
                        case 6:
                            BO.Volunteer volunteer = CreateVolunteer();
                            try
                            {
                                s_bl.volunteer.AddVolunteer(volunteer);
                                Console.WriteLine("Volunteer added successfully.");
                            }
                            catch (BllAlreadyExistException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            break;

                        case 0:
                            return;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private static void ManageCalls()
        {
            while (true)
            {
                Console.WriteLine("Call Management:");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("1. Call list by status");
                Console.WriteLine("2. Call list");
                Console.WriteLine("3. Call details");
                Console.WriteLine("4. Update call");
                Console.WriteLine("5. Delete call");
                Console.WriteLine("6. Add call");
                Console.WriteLine("7. List of closed calls handled by a volunteer");
                Console.WriteLine("8. List of open calls available for a volunteer");
                Console.WriteLine("9. End treatment on a call");
                Console.WriteLine("10.Cancel treatment on a call");
                Console.WriteLine("11.Select a call for treatment");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1":


                        IEnumerable<int> calls;
                        try
                        {
                        calls = s_bl.call.CallQuantities();                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            throw new BO.BlDoesNotExistException("ERRORE: " + ex);
                        }
                        foreach (var call in calls)
                            Console.WriteLine(call);
                        
                        
                        //Console.WriteLine(calls);
                        break;
                    case "2":
                        try
                        {
                            Console.Write("Enter filter field (or press Enter for none): ");
                            BO.CallField? filterField = Enum.TryParse(Console.ReadLine(), true, out BO.CallField filter) ? filter : (BO.CallField?)null;

                            Console.Write("Enter filter value (or press Enter for none): ");
                            object? filterValue = string.IsNullOrWhiteSpace(Console.ReadLine()) ? null : Console.ReadLine();

                            Console.Write("Enter sort field (or press Enter for none): ");
                            BO.CallField? sortField = Enum.TryParse(Console.ReadLine(), true, out BO.CallField sort) ? sort : (BO.CallField?)null;

                            var sortCalls = s_bl.call.CallInLists(filterField, filterValue, sortField);

                            foreach (var call in sortCalls)
                            {
                                Console.WriteLine(call);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "3":
                        try
                        {
                            Console.Write("Enter Call ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int callId))
                                throw new BlFormatException("invalid ID");

                            BO.Call callDetails = s_bl.call.GetCallDetails(callId);
                            Console.WriteLine(callDetails);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "4":
                        try
                        {
                            Console.Write("Enter Call ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int callId))
                                throw new BlFormatException("invalid ID");
                            BO.Call callToUpdate = CreateCall();
                            s_bl.call.UpdatingCallDetails(callToUpdate.Id,callToUpdate);
                            Console.WriteLine("Call updated successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "5":
                        try
                        {
                            Console.Write("Enter Call ID to delete: ");
                            if (!int.TryParse(Console.ReadLine(), out int deleteId))
                                throw new BlFormatException("invalid ID");

                            s_bl.call.DeleteCall(deleteId);
                            Console.WriteLine("Call deleted successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "6":
                        try
                        {
                            BO.Call newCall = CreateCall();

                            s_bl.call.AddCall(newCall);
                            Console.WriteLine("Call added successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "7":
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int volunteerIdToclosed))
                                throw new BlFormatException("invalid ID");

                            Console.Write("Enter call type (or press Enter for none): ");
                            BO.CallType? callType = Enum.TryParse(Console.ReadLine(), true, out BO.CallType type) 
                                ? type : (BO.CallType?)null;//Can be null

                            Console.Write("Enter sort field (or press Enter for none): ");
                            BO.ClosedCallInListField? sortBy = Enum.TryParse(Console.ReadLine(), true, out BO.ClosedCallInListField sortField)
                                ? sortField : (BO.ClosedCallInListField?)null;//Can be null

                            var closedCalls = s_bl.call.closedCallsHandledByVolunteer(volunteerIdToclosed, callType, sortBy);

                            foreach (var closedCall in closedCalls)
                            {
                                Console.WriteLine(closedCall);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "8":
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (!int.TryParse(Console.ReadLine(), out int volunteerIdToOpen))
                                throw new BlFormatException("invalid ID");

                            Console.Write("Enter call type (or press Enter for none): ");
                            BO.CallType? callType = Enum.TryParse(Console.ReadLine(), true, out BO.CallType type)
                                ? type : (BO.CallType?)null;//Can be null

                            Console.Write("Enter sort field (or press Enter for none): ");
                            BO.OpenCallInListField? sortBy = Enum.TryParse(Console.ReadLine(), true, out BO.OpenCallInListField sortField) 
                                ? sortField : (BO.OpenCallInListField?)null;//Can be null

                            var closedCalls = s_bl.call.openCallsForSelectionByVolunteer(volunteerIdToOpen, callType, sortBy);

                            foreach (var closedCall in closedCalls)
                            {
                                Console.WriteLine(closedCall);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    case "9":
                        // עדכון סיום טיפול בקריאה
                        Console.WriteLine("Enter volunteer ID:");
                        if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                            throw new BlFormatException("invalid ID");
                        
                        Console.WriteLine("Enter assignment ID:");
                        if (!int.TryParse(Console.ReadLine(), out int assignmentId))
                            throw new BlFormatException("invalid ID");
                       
                        try
                        {
                            s_bl.call.UpdateEndOfTreatmentCall(volunteerId, assignmentId);
                            Console.WriteLine("Treatment completion updated successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;


                    case "10":
                        // ביטול טיפול בקריאה
                        
                        Console.WriteLine("Enter volunteer ID:");
                        if (!int.TryParse(Console.ReadLine(), out int volunteerIdCancel))
                            throw new BlFormatException("invalid ID");
                        
                        Console.WriteLine("Enter assignment ID:");
                        if (!int.TryParse(Console.ReadLine(), out int assignmentIdCancel))
                            throw new BlFormatException("invalid ID");
                        try
                        {
                           s_bl.call. CancelCallHandling(volunteerIdCancel, assignmentIdCancel);
                            Console.WriteLine("Call handling canceled successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;

                    case "11":
                        // בחירת קריאה לטיפול
                        Console.WriteLine("Enter volunteer ID:");
                        if (!int.TryParse(Console.ReadLine(), out int volunteerIdChoose))
                            throw new BlFormatException("invalid volunteer ID");
                        Console.WriteLine("Enter call ID:");
                        if (!int.TryParse(Console.ReadLine(), out int callIdChoose))
                            throw new BlFormatException("invalid call ID");
                        try
                        {
                            s_bl.call.ChooseCallForHandling(volunteerIdChoose, callIdChoose);
                            Console.WriteLine("Call assigned successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                        break;


                    case "0":
                        Console.WriteLine("Exiting Call Management.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        private static void ManageAdmin()
        {
            while (true)
            {
                Console.WriteLine("Admin Management:");
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("1. Get Clock");
                Console.WriteLine("2. Forward Clock");
                Console.WriteLine("3. Get max range");
                Console.WriteLine("4. set max range");
                Console.WriteLine("5. Reset DB");
                Console.WriteLine("6. Initialize DB");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        DateTime clock= s_bl.Admin.GetClock();
                        Console.WriteLine(clock);
                        break;
                    case 2:
                        Console.Write("Enter Time unit (Minute/Hour/Day/Month/Year )");
                        if (!Enum.TryParse(Console.ReadLine(), true, out BO.TimeUnit timeUnit))
                            throw new BO.BlFormatException("time unit is invalid!");
                        try
                        {
                            s_bl.Admin.ForwardClock(timeUnit);
                            Console.Write($"The clock advanced by one unit:{timeUnit}");
                        } 
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case 3:
                        TimeSpan maxRange = s_bl.Admin.GetMaxRange();
                        Console.WriteLine($"The current maximum time range is {maxRange}");
                        break;
                    case 4:
                        Console.Write("Enter the maximum time range (in the format HH:MM:SS):");
                        if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan maxRangeToSet))
                            throw new BO.BlFormatException("maximum time range  invalid!");
                        s_bl.Admin.SetMaxRange(maxRangeToSet);
                        Console.WriteLine($"The maximum time range has been successfully set to {maxRangeToSet}.");

                        break;
                    case 5:
                        s_bl.Admin.ResetDB();
                        Console.WriteLine("Database reset.");
                        break;
                    case 6:
                        s_bl.Admin.InitializeDB();
                        Console.WriteLine("Database initialized.");
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        private static BO.Volunteer CreateVolunteer()
        {
            Console.Write("Enter Volunteer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                throw new BO.BlFormatException("ID is invalid!");

            Console.Write("Enter Volunteer Name: ");
            string name = Console.ReadLine() ??"";

            Console.Write("Enter Volunteer Phone: ");
            string phone = Console.ReadLine() ?? "";

            Console.Write("Enter Volunteer Email: ");
            string mail = Console.ReadLine() ??"";

            Console.Write("Enter Volunteer Password: ");
            string password = Console.ReadLine() ??"";

            Console.Write("Enter Volunteer Address: ");
            string address = Console.ReadLine() ??"";

            //Console.Write("Enter Volunteer Latitude (leave empty for default 0.0): ");
            //string? latitudeInput = Console.ReadLine();
            //double latitude = string.IsNullOrWhiteSpace(latitudeInput) ? 0.0 ://Empty input: will be set to 0.0.
            //                  double.TryParse(latitudeInput, out double lat) ? lat ://Valid input (number): the value will be converted to double.
            //                  throw new BO.BlFormatException("Latitude is invalid!");//Invalid input (for example: "abc"): an error will be thrown.

            //Console.Write("Enter Volunteer Longitude (leave empty for default 0.0): ");
            //string? longitudeInput = Console.ReadLine();
            //double longitude = string.IsNullOrWhiteSpace(longitudeInput) ? 0.0 ://Empty input: will be set to 0.0.
            //                   double.TryParse(longitudeInput, out double lon) ? lon ://Valid input (number): the value will be converted to double.
            //                   throw new BO.BlFormatException("Longitude is invalid!");//Invalid input (for example: "abc"): an error will be thrown.

            Console.Write("Enter Volunteer Role (0:Volunteer 1:Manager): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BO.Roles role))
                throw new BO.BlFormatException("Role is invalid!");

            Console.Write("Is the Volunteer Active? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool active))
                throw new BO.BlFormatException("Active status is invalid!");

            Console.Write("Enter Maximum Distance the Volunteer Can Travel: ");
            if (!double.TryParse(Console.ReadLine(), out double maxDistance))
                throw new BO.BlFormatException("Maximum distance is invalid!");

            Console.Write("Enter Volunteer Type (0:Aerial 1:Car 2:Walking): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BO.DistanceType type))
                throw new BO.BlFormatException("Type is invalid!");

            BO.Volunteer volunteer = new BO.Volunteer
            {
                Id = volunteerId,
                Name = name,
                Phone = phone,
                Mail = mail,
                Password = password,
                Address = address,
                //Latitude = latitude,
                //Longitude = longitude,
                Type = type,
                Role = role,
                Active = active,
                MaximumDistance = maxDistance

                //TotalCallsHandled = totalCallsHandled,
                //TotalCallsCanceled = totalCallsCanceled,
                //TotalCallsChosenHandleExpired = totalCallsExpired
            };


            return volunteer;
        }

        private static BO.Call CreateCall()
        {
           
            Console.Write("Enter Type (RegularVehicle, Ambulance, IntensiveCareAmbulance, None): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BO.CallType type))
                throw new BO.BlFormatException("type is invalid!");

            Console.Write("Enter call Description : ");
            string description = Console.ReadLine() ?? "";

            Console.Write("Enter call Address : ");
            string address = Console.ReadLine()??"";
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new BO.BlFormatException("Must enter an address!");
            }
            //Console.Write("Enter call Address : ");
            //string address = Console.ReadLine() ?? throw new BO.BlFormatException("Must enter an address!");

            //Console.Write("Enter call Latitude : ");
            //if (!double.TryParse(Console.ReadLine(),out double latitude))
            //    throw new BO.BlFormatException("latitude is invalid!");

            //Console.Write("Enter call Longitude : ");
            //if (!double.TryParse(Console.ReadLine(), out double longitude))
            //    throw new BO.BlFormatException("longitude is invalid!");

            //Console.WriteLine("Enter Open Time (format: yyyy-MM-dd HH:mm:ss): ");
            //if (!DateTime.TryParse(Console.ReadLine(), out DateTime openTime))
            //    throw new DalFormatException("Open Time is invalid!");

            Console.WriteLine("Enter Max Time 5 to 30 minutes(format: yyyy-MM-dd HH:mm:ss): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime maxTime))
                throw new DalFormatException("Max Time is invalid!");

            Console.Write("Enter status (Open, Treatment, OpenAtRisk, TreatmentOfRisk, Expired, Close): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BO.CallStatus status))
                throw new BO.BlFormatException("type is invalid!");
            List<BO.CallAssignInList>? listAssignmentsForCalls =new List<BO.CallAssignInList>();////////////

            BO.Call call = new BO.Call
            {

                CarTypeToSend = type,
                Description = description,
                Address = address,
                //Latitude = latitude,
                //Longitude =longitude,
                OpenTime = DateTime.Now,
                MaxTime = maxTime,
                Status = status,
                ListAssignmentsForCalls = listAssignmentsForCalls,
            };
            return call;
        }

    }

}





