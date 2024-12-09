using System;
using BlApi;
using BO;

namespace BlTest
{
    class Program
    {
        static readonly IBl s_bl = BlApi.Factory.Get();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Manage Volunteers");
                Console.WriteLine("2. Manage Calls");
                Console.WriteLine("3. Manage Assignments");
                Console.WriteLine("4. Manage Admin");
                Console.WriteLine("0. Exit");
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
                        ManageAssignments();
                        break;
                    case 4:
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
                Console.WriteLine("1. Read Volunteer");
                Console.WriteLine("2. Read All Volunteers");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ReadVolunteer();
                        break;
                    case 2:
                        ReadAllVolunteers();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void ReadVolunteer()
        {
            Console.Write("Enter Volunteer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }

            try
            {
                BO.Volunteer? volunteer = s_bl.volunteer.GetVolunteerDetails(volunteerId);
                Console.WriteLine(volunteer);
            }
            catch (BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ReadAllVolunteers()
        {
            try
            {
                foreach (var volunteer in s_bl.volunteer.VolunteerList(null, null))
                {
                    Console.WriteLine(volunteer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ManageCalls()
        {
            while (true)
            {
                Console.WriteLine("Call Management:");
                Console.WriteLine("1. Read Call");
                Console.WriteLine("2. Read All Calls");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ReadCall();
                        break;
                    case 2:
                        ReadAllCalls();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void ReadCall()
        {
            Console.Write("Enter Call ID: ");
            if (!int.TryParse(Console.ReadLine(), out int callId))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }

            try
            {
                BO.Call? call = s_bl.call.GetCallDetails(callId);
                Console.WriteLine(call);
            }
            catch (BlDoesNotExistException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ReadAllCalls()
        {
            try
            {
                foreach (var call in s_bl.call.CallInLists(null, null, null))
                {
                    Console.WriteLine(call);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ManageAssignments()
        {
            while (true)
            {
                Console.WriteLine("Assignment Management:");
                Console.WriteLine("1. Read Assignment");
                Console.WriteLine("2. Read All Assignments");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ReadAssignment();
                        break;
                    case 2:
                        ReadAllAssignments();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void ReadAssignment()
        {
            Console.Write("Enter Assignment ID: ");
            if (!int.TryParse(Console.ReadLine(), out int assignmentId))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }

            //try
            //{
            //    BO.CallAssignInList CallAssignInList? assignment = s_bl.assignment.GetAssignmentDetails(assignmentId);
            //    Console.WriteLine(assignment);
            //}
            //catch (BlDoesNotExistException ex)
            //{
            //    Console.WriteLine($"Error: {ex.Message}");
            //}
        }

        private static void ReadAllAssignments()
        {
            //try
            //{
            //    foreach (var assignment in s_bl.assignment.AssignmentList(null, null))
            //    {
            //        Console.WriteLine(assignment);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error: {ex.Message}");
            //}
        }

        private static void ManageAdmin()
        {
            while (true)
            {
                Console.WriteLine("Admin Management:");
                Console.WriteLine("1. Reset DB");
                Console.WriteLine("2. Initialize DB");
                Console.WriteLine("3. Forward Clock");
                Console.WriteLine("4. Get Clock");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        s_bl.Admin.ResetDB();
                        Console.WriteLine("Database reset.");
                        break;
                    case 2:
                        s_bl.Admin.InitializeDB();
                        Console.WriteLine("Database initialized.");
                        break;
                    case 3:
                        ForwardClock();
                        break;
                    case 4:
                        Console.WriteLine(s_bl.Admin.GetClock());
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void ForwardClock()
        {
            Console.Write("Enter time unit (e.g., HOUR): ");
            string? timeUnitStr = Console.ReadLine();
            if (Enum.TryParse<BO.TimeUnit>(timeUnitStr, out BO.TimeUnit timeUnit))
            {
                s_bl.Admin.ForwardClock(timeUnit);
                Console.WriteLine("Clock forwarded.");
            }
            else
            {
                Console.WriteLine("Invalid time unit.");
            }
        }
    }
}

