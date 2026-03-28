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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComfortPractice.Model;

namespace ComfortPractice.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Login.Text) || string.IsNullOrEmpty(password.Password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var userObj = Connection.connect.Logins.FirstOrDefault(x => x.Login == Login.Text.Trim() && x.Password == password.Password.Trim());
                if (userObj == null)
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    CurrentUser.User = userObj;
                    NavigationService.Navigate(new PartnersListPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к бд! : {ex.Message}", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void registration_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}