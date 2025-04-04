﻿using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public Login()
        {
            Password = "pA9^A$pX";
            VolunteerId = 123456782;
            
            InitializeComponent();
            DataContext = this;

        }
       
        /// <summary>
        ///Dependency property for binding volunteer's ID to the UI
        /// </summary>
        public int VolunteerId
        {
            get { return (int)GetValue(VolunteerIdProperty); }
            set { SetValue(VolunteerIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerIdProperty =
            DependencyProperty.Register("VolunteerId", typeof(int), typeof(Login), new PropertyMetadata(0));

        /// <summary>
        ///Dependency property for binding volunteer's password to the UI
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(Login), new PropertyMetadata(""));

        /// <summary>
        /// Dependency property for indicating whether the user is a manager.
        /// </summary>
        public bool IsManager
        {
            get { return (bool)GetValue(IsManagerProperty); }
            set { SetValue(IsManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsManagerProperty =
            DependencyProperty.Register("IsManager", typeof(bool), typeof(Login), new PropertyMetadata(false));

        /// <summary>
        /// Handles the login process. If the user is a manager, provides the option to proceed. 
        /// Otherwise, logs the user directly into the volunteer screen.
        /// </summary>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow) 
                    {
                        MessageBox.Show("Administrator already logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if(window is VolunteerProfile volunteerWindow && volunteerWindow.CurrentVolunteer!=null)
                    {
                        if (volunteerWindow.CurrentVolunteer.Role==BO.Roles.Manager && volunteerWindow.CurrentVolunteer.Id != this.VolunteerId)
                        {
                            MessageBox.Show("Administrator already logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                if (VolunteerId == 0 || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Please enter both ID and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var volunteerDetails = s_bl.volunteer.GetVolunteerDetails(VolunteerId);

                if (volunteerDetails == null)
                {
                    MessageBox.Show("Volunteer with the provided ID does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string name = volunteerDetails.Name ?? "";
                BO.Roles role = s_bl.volunteer.Login(name, Password);

                if (role == BO.Roles.Volunteer)
                {
                    IsManager = false;
                    VolunteerProfile volunteerWindow = new VolunteerProfile(VolunteerId);
                    volunteerWindow.Show();
                }
                else if (role == BO.Roles.Manager)
                {
                    IsManager = true;
                }
                else
                {
                    MessageBox.Show("Invalid password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        /// <summary>
        /// Opens the volunteer profile screen for the currently logged-in volunteer.
        /// </summary>
        private void VolunteerScreen_Click(object sender, RoutedEventArgs e)
        {
            VolunteerProfile volunteerWindow = new VolunteerProfile(VolunteerId) { Owner = this };
                volunteerWindow.Show();
        }
        
        /// <summary>
        /// Opens the main management screen for administrative tasks.
        /// </summary>
        private void MainManagementScreen_Click(object sender, RoutedEventArgs e)
        {
            if (this.OwnedWindows.Count == 0) // אם אין חלון כזה בבעלות החלון הראשי
            {
                MainWindow mainWindow = new MainWindow { Owner = this };
                mainWindow.Show();
            }
            else
            {
                this.OwnedWindows[0].Activate(); // מביא את החלון הפתוח לקדמת המסך
            }
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        /// <summary>
        /// Observes the login status and updates the manager flag based on the volunteer's role.
        /// </summary>
        private void LoginObserver() 
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (s_bl.volunteer.GetVolunteerDetails(VolunteerId).Role == BO.Roles.Manager)
                        IsManager = true;
                    else
                        IsManager = false;
                });
        }
       
        /// <summary>
        /// Adds the LoginObserver to monitor changes when the window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.volunteer.AddObserver(VolunteerId,LoginObserver);
        }
        
        /// <summary>
        /// Removes the LoginObserver when the window is closed.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
           s_bl.volunteer.RemoveObserver(VolunteerId, LoginObserver);
        }
       
        /// <summary>
        /// Displays the entered password when the Eye button is clicked.
        /// </summary>
        private void EyeButton_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBox.Show($"The password entered is: {Password}");

        }
        
        /// <summary>
        /// Updates the Password property when the password in the PasswordBox changes.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Password = passwordBox.Password; ;
            }
        }
    }

}
