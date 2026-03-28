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
    /// <summary>
    /// Логика взаимодействия для PartnersAddEdit.xaml
    /// </summary>
    public partial class PartnersAddEdit : Page
    {
        Partners partner;
        public PartnersAddEdit(Partners _partner)
        {
            InitializeComponent();
            partner = _partner;
            this.DataContext = partner;
            TypeCB.ItemsSource = Connection.connect.TypeOfBusiness.ToList();
            TypeCB.DisplayMemberPath = "NameBusiness";
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(partner.NamePartner) ||string.IsNullOrEmpty(partner.SurnameDirector) || string.IsNullOrEmpty(partner.NameDirector))
            {
                MessageBox.Show("Заполните обязательные поля (Наименование, ФИО директора)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if (TypeCB.SelectedItem != null)
                {
                    partner.Id_type = (TypeCB.SelectedItem as TypeOfBusiness).Id_type;
                }
                if (partner.Id_partner == 0)
                {
                    Connection.connect.Partners.Add(partner);
                }
                Connection.connect.SaveChanges();
                MessageBox.Show("Операция прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new PartnersListPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartnersListPage());
        }
    }
}