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
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private int id;
        public string ButtonText { get; set; }

        public CallWindow(int id = 0)
        {
            this.id = id;
            ButtonText = id == 0 ? "Add" : "Update";
            DataContext = this;
            InitializeComponent();
            LoadCall(id);
        }

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

        private void MakeAllFieldsReadOnly()
        {
            IsDescriptionReadOnly = true;
            IsAddressReadOnly = true;
            IsMaxTimeReadOnly = true;
            IsCarTypeReadOnly = true;
        }

        private void EnableFieldsForMaxTimeOnly()
        {
            IsDescriptionReadOnly = true;
            IsAddressReadOnly = true;
            IsMaxTimeReadOnly = false;
            IsCarTypeReadOnly = true;
        }

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

        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set
            {
                SetValue(CurrentCallProperty, value);
                OnPropertyChanged(nameof(IsUpdatingAndHasAssignments)); // עדכון מאפיין המחושב
            }
        }

        public bool IsUpdatingAndHasAssignments
        {
            get
            {
                return id != 0 && CurrentCall?.ListAssignmentsForCalls != null && CurrentCall.ListAssignmentsForCalls.Count > 0;
            }
        }

        //public static readonly DependencyProperty CurrentCallProperty =
        //    DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null, OnCurrentCallChanged));

        private static void OnCurrentCallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as CallWindow;
            window?.UpdateUIAccess();
        }

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall != null && CurrentCall.Id != 0)
                s_bl.call.AddObserver(CurrentCall.Id, RefreshCall);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCall != null && CurrentCall.Id != 0)
                s_bl.call.RemoveObserver(CurrentCall.Id, RefreshCall);
        }

        private void RefreshCall()
        {
            if (CurrentCall == null) return;

            int id = CurrentCall.Id;
            CurrentCall = null;
            CurrentCall = s_bl.call.GetCallDetails(id);
            UpdateUIAccess();
        }

        private void HandleError(string message, Exception ex)
        {
            Console.WriteLine($"Error: {message} - {ex.Message}");
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


}

