using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        // Reference to the business logic layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private int id;
        public string ButtonText { get; set; }

        /// <summary>
        /// Initializes a new instance of the CallWindow class.
        /// </summary>
        /// <param name="id">The ID of the call to load. If 0, a new call will be created.</param>
        public CallWindow(int id = 0)
        {
            this.id = id;
            ButtonText = id == 0 ? "Add" : "Update";
            DataContext = this;
            InitializeComponent();
            LoadCall(id);
        }

        /// <summary>
        /// Loads the call details based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the call to load.</param>
        private void LoadCall(int id)
        {
            try
            {
                if (id == 0)
                {
                    CurrentCall = new BO.Call
                    {
                        OpenTime = s_bl.Admin.GetClock()
                    };
                }
                else
                {
                    CurrentCall = s_bl.call.GetCallDetails(id);
                    UpdateUIAccess();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the UI access based on the call's status.
        /// </summary>
        private void UpdateUIAccess()
        {
            if (CurrentCall == null) return;

            switch (CurrentCall.Status)
            {
                case CallStatus.Close:
                case CallStatus.Expired:
                    MakeAllFieldsReadOnly();
                    break;
                case CallStatus.Treatment:
                case CallStatus.TreatmentOfRisk:
                    EnableFieldsForMaxTimeOnly();
                    break;
                case CallStatus.Open:
                case CallStatus.OpenAtRisk:
                    EnableAllFields();
                    break;
            }
        }

        /// <summary>
        /// Makes all fields read-only.
        /// </summary>
        private void MakeAllFieldsReadOnly()
        {
            IsDescriptionReadOnly = true;
            IsAddressReadOnly = true;
            IsMaxTimeReadOnly = true;
            IsCarTypeReadOnly = true;
        }

        /// <summary>
        /// Enables fields for max time only.
        /// </summary>
        private void EnableFieldsForMaxTimeOnly()
        {
            IsDescriptionReadOnly = true;
            IsAddressReadOnly = true;
            IsMaxTimeReadOnly = false;
            IsCarTypeReadOnly = true;
        }

        /// <summary>
        /// Enables all fields.
        /// </summary>
        private void EnableAllFields()
        {
            IsDescriptionReadOnly = false;
            IsAddressReadOnly = false;
            IsMaxTimeReadOnly = false;
            IsCarTypeReadOnly = false;
        }

        // Define DependencyProperty for IsDescriptionReadOnly
        public static readonly DependencyProperty IsDescriptionReadOnlyProperty =
            DependencyProperty.Register(nameof(IsDescriptionReadOnly), typeof(bool), typeof(CallWindow), new PropertyMetadata(false));

        public bool IsDescriptionReadOnly
        {
            get => (bool)GetValue(IsDescriptionReadOnlyProperty);
            set => SetValue(IsDescriptionReadOnlyProperty, value);
        }

        // Define DependencyProperty for IsAddressReadOnly
        public static readonly DependencyProperty IsAddressReadOnlyProperty =
            DependencyProperty.Register(nameof(IsAddressReadOnly), typeof(bool), typeof(CallWindow), new PropertyMetadata(false));

        public bool IsAddressReadOnly
        {
            get => (bool)GetValue(IsAddressReadOnlyProperty);
            set => SetValue(IsAddressReadOnlyProperty, value);
        }

        // Define DependencyProperty for IsMaxTimeReadOnly
        public static readonly DependencyProperty IsMaxTimeReadOnlyProperty =
            DependencyProperty.Register(nameof(IsMaxTimeReadOnly), typeof(bool), typeof(CallWindow), new PropertyMetadata(false));

        public bool IsMaxTimeReadOnly
        {
            get => (bool)GetValue(IsMaxTimeReadOnlyProperty);
            set => SetValue(IsMaxTimeReadOnlyProperty, value);
        }

        // Define DependencyProperty for IsTypeReadOnly
        public static readonly DependencyProperty IsCarTypeReadOnlyProperty =
            DependencyProperty.Register(nameof(IsCarTypeReadOnly), typeof(bool), typeof(CallWindow), new PropertyMetadata(false));

        public bool IsCarTypeReadOnly
        {
            get => (bool)GetValue(IsCarTypeReadOnlyProperty);
            set => SetValue(IsCarTypeReadOnlyProperty, value);
        }

        /// <summary>
        /// Gets or sets the current call.
        /// </summary>
        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set
            {
                SetValue(CurrentCallProperty, value);
                OnPropertyChanged(nameof(IsUpdatingAndHasAssignments)); // עדכון מאפיין המחושב
            }
        }

        /// <summary>
        /// Gets a value indicating whether the call is being updated and has assignments.
        /// </summary>
        public bool IsUpdatingAndHasAssignments
        {
            get
            {
                return id != 0 && CurrentCall?.ListAssignmentsForCalls != null && CurrentCall.ListAssignmentsForCalls.Count > 0;
            }
        }

        // Define DependencyProperty for CurrentCall
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null, OnCurrentCallChanged));

        private static void OnCurrentCallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as CallWindow;
            window?.UpdateUIAccess();
        }

        /// <summary>
        /// Handles the Add/Update button click event.
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.call.AddCall(CurrentCall!);
                    SendEmailsToVolunteers();
                    MessageBox.Show("The call was added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (ButtonText == "Update")
                {
                    s_bl.call.UpdatingCallDetails(id, CurrentCall!);
                    MessageBox.Show("The call was updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sends emails to volunteers about the new call.
        /// </summary>
        private async void SendEmailsToVolunteers()
        {
            try
            {
                BO.Call? currentCall = null;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    currentCall = CurrentCall;
                });

                if (currentCall?.Description == null)
                {
                    throw new InvalidOperationException("Current call description is null.");
                }

                var allVolunteers = await Task.Run(() =>
                {
                    return s_bl.volunteer.VolunteerList(true, null);
                });

                foreach (var volunteer in allVolunteers)
                {
                    var volunteerDetails = await Task.Run(() =>
                    {
                        return s_bl.volunteer.GetVolunteerDetails(volunteer.Id);
                    });

                    if (volunteerDetails.Address != null && volunteerDetails.MaximumDistance.HasValue)
                    {
                        bool canVolunteerAttend = await Task.Run(() =>
                        {
                            return s_bl.volunteer.CanVolunteerAttendCall(volunteerDetails, currentCall);
                        });

                        if (canVolunteerAttend)
                        {
                            string? email = volunteerDetails.Mail;
                            if (email != null)
                            {
                                string message = $"קריאה חדשה: {currentCall.Description}";
                                await SendEmailAsync(email, "קריאה חדשה", message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error sending emails: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        public static void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)) // השתמשי בפורט 587
                {
                    // שימוש בסיסמת אפליקציה (אם את ב-Gmail)
                    smtpClient.Credentials = new NetworkCredential("hadasshor10@gmail.com", "bqdd enut suql kbsh");
                    smtpClient.EnableSsl = true; // הפעלת SSL

                    // יצירת הודעת מייל
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress("hadasshor10@gmail.com"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);
                    // mailMessage.To.Add("yedidia2004@gmail.com");

                    // שליחת המייל
                    smtpClient.Send(mailMessage);
                    Console.WriteLine($"Email sent successfully to {toEmail}");
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                if (smtpEx.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {smtpEx.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="recipientEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using var smtpClient = new SmtpClient("smtp.gmail.com") // עבור Gmail
                {
                    Port = 587, // TLS
                    Credentials = new NetworkCredential("your-email@gmail.com", "your-app-password"), // סיסמת אפליקציה
                    EnableSsl = true // חיבור מאובטח
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // אם המייל שלך מכיל HTML
                };
                mailMessage.To.Add(recipientEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error sending email: {ex.Message}", ex);
            }
        }

        public ObservableCollection<CallAssignInList> Assignments { get; set; } = new ObservableCollection<CallAssignInList>();

        /// <summary>
        /// Adds the observer when the window loads.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall != null && CurrentCall.Id != 0)
            {
                s_bl.call.AddObserver(CurrentCall.Id, RefreshCall);
                s_bl.volunteer.AddObserver(CurrentCall.Id, RefreshCall);
            }

        }

        /// <summary>
        /// Removes the observer when the window is closed.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCall != null && CurrentCall.Id != 0)
            {
                s_bl.call.RemoveObserver(CurrentCall.Id, RefreshCall);
                s_bl.volunteer.RemoveObserver(CurrentCall.Id, RefreshCall);
            }
        }

        /// <summary>
        /// Refreshes the call details.
        /// </summary>
        private void RefreshCall()
        {
            if (CurrentCall == null) return;

            int id = CurrentCall.Id;
            CurrentCall = null;
            CurrentCall = s_bl.call.GetCallDetails(id);
            UpdateUIAccess();
        }

        /// <summary>
        /// Handles errors by displaying a message box.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The exception that occurred.</param>
        private void HandleError(string message, Exception ex)
        {
            Console.WriteLine($"Error: {message} - {ex.Message}");
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}

