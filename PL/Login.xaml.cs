using PL.Volunteer;
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
            InitializeComponent();
            DataContext = this;
           

        }

        public int VolunteerId
        {
            get { return (int)GetValue(VolunteerIdProperty); }
            set { SetValue(VolunteerIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerIdProperty =
            DependencyProperty.Register("VolunteerId", typeof(int), typeof(Login), new PropertyMetadata(0));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(Login), new PropertyMetadata(""));


        public bool IsManager
        {
            get { return (bool)GetValue(IsManagerProperty); }
            set { SetValue(IsManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsManagerProperty =
            DependencyProperty.Register("IsManager", typeof(bool), typeof(Login), new PropertyMetadata(false));



        public bool IsPasswordVisible
        {
            get { return (bool)GetValue(IsPasswordVisibleProperty); }
            set { SetValue(IsPasswordVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPasswordVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPasswordVisibleProperty =
            DependencyProperty.Register("IsPasswordVisible", typeof(bool), typeof(Login), new PropertyMetadata(false));


        public bool IsPasswordVisible1
        {
            get { return (bool)GetValue(IsPasswordVisible1Property); }
            set { SetValue(IsPasswordVisible1Property, value); }
        }

        // Using a DependencyProperty as the backing store for IsPasswordVisible1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPasswordVisible1Property =
            DependencyProperty.Register("IsPasswordVisible1", typeof(bool), typeof(Login), new PropertyMetadata(true));


       
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsManager == true)
                {
                    MessageBox.Show("Administrator already logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
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

        private void VolunteerScreen_Click(object sender, RoutedEventArgs e)
        {
            VolunteerProfile volunteerWindow = new VolunteerProfile(VolunteerId);
            volunteerWindow.Show();
        }
        private void MainManagementScreen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        //private void LoginObserver()
        //{
          

        //}
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IsManager = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void EyeButton_Click(object sender, RoutedEventArgs e)
        {
            //IsPasswordVisible = !IsPasswordVisible;
            MessageBox.Show($"The password entered is: {Password}");

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Password = passwordBox.Password; ;
            }
        }
    }

}
