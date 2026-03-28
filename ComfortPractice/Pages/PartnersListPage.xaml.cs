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
    /// Логика взаимодействия для PartnersListPage.xaml
    /// </summary>
    public partial class PartnersListPage : Page
    {
        List<Partners> partnersList;
        public PartnersListPage()
        {
            InitializeComponent();        
            PartnerLW.ItemsSource =Connection.connect.Partners.ToList();
            LoadPartners();
        }
        private void LoadPartners()
        {
            try
            {
                partnersList = Connection.connect.Partners.ToList();
                ApplyFilterAndSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ApplyFilterAndSort()
        {
            if (partnersList == null) return;
            var filteredList = partnersList;
            string searchText = SearchBox?.Text?.ToLower() ?? "";
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredList = filteredList.Where(p =>
                    (p.NamePartner != null && p.NamePartner.ToLower().Contains(searchText)) ||
                    (p.SurnameDirector != null && p.SurnameDirector.ToLower().Contains(searchText)) ||
                    (p.NameDirector != null && p.NameDirector.ToLower().Contains(searchText)) ||
                    (p.Phone != null && p.Phone.Contains(searchText))).ToList();
            }
            if (SortCombo != null && SortCombo.SelectedItem != null)
            {
                string sortOption = (SortCombo.SelectedItem as ComboBoxItem).Content.ToString();
                switch (sortOption)
                {
                    case "По наименованию (А-Я)":
                        filteredList = filteredList.OrderBy(p => p.NamePartner).ToList();
                        break;
                    case "По наименованию (Я-А)":
                        filteredList = filteredList.OrderByDescending(p => p.NamePartner).ToList();
                        break;
                    case "По рейтингу (возрастание)":
                        filteredList = filteredList.OrderBy(p => p.Raiting).ToList();
                        break;
                    case "По рейтингу (убывание)":
                        filteredList = filteredList.OrderByDescending(p => p.Raiting).ToList();
                        break;
                }
            }
            if (PartnerLW != null)
            {
                PartnerLW.ItemsSource = filteredList;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilterAndSort();
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartnersAddEdit(new Partners()));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PartnerLW.SelectedItem as Partners;
            if (selPartner != null)
            {
                NavigationService.Navigate(new PartnersAddEdit(selPartner));
            }
            else
            {
                MessageBox.Show("Не выбран партнер для редактирования!");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PartnerLW.SelectedItem as Partners;
            if (selPartner == null)
            {
                MessageBox.Show("Выберите партнера для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Вы уверены, что хотите удалить партнера \"{selPartner.NamePartner}\"?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Connection.connect.Partners.Remove(selPartner);
                    Connection.connect.SaveChanges();
                    MessageBox.Show("Партнер успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPartners();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void HistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PartnerLW.SelectedItem as Partners;
            if (selPartner == null)
            {
                MessageBox.Show("Выберите партнера для просмотра истории продаж!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NavigationService.Navigate(new SalesHistoryPage(selPartner));
        }
    }
}