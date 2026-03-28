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
    public partial class SalesHistoryPage : Page
    {
        Partners currentPartner;
        List<SaleHistoryItem> salesHistory;
        public SalesHistoryPage(Partners partner)
        {
            InitializeComponent();
            currentPartner = partner;
            PartnerNameTb.Text = $"История продаж: {partner.NamePartner}";
            LoadSalesHistory();
        }

        private void LoadSalesHistory()
        {
            try
            {
                var requests = Connection.connect.Request .Where(r => r.Id_partner == currentPartner.Id_partner && r.Id_status == 5).OrderBy(r => r.RequestDate).ToList();
                salesHistory = new List<SaleHistoryItem>();
                foreach (var request in requests)
                {
                    var requestDetails = Connection.connect.RequestDetails.Where(rd => rd.Id_request == request.Id_request).ToList();
                    foreach (var rd in requestDetails)
                    {
                        var product = Connection.connect.Products.FirstOrDefault(p => p.Id_product == rd.Id_product);
                        if (product != null && product.MinPrice.HasValue && rd.Quantity.HasValue)
                        {
                            salesHistory.Add(new SaleHistoryItem
                            {
                                RequestDate = request.RequestDate ?? DateTime.Now,
                                ProductName = product.NameProduct ?? "Неизвестно",
                                Quantity = rd.Quantity.Value,
                                TotalAmount = rd.Quantity.Value * product.MinPrice.Value,
                                Status = "Выполнена"
                            });
                        }
                    }
                }
                salesHistory = salesHistory.OrderBy(s => s.RequestDate).ToList();
                decimal totalSum = salesHistory.Sum(s => s.TotalAmount);
                TotalSalesTb.Text = $"Общая сумма продаж (всего): {totalSum:N2} руб.";
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            if (salesHistory == null || salesHistory.Count == 0)
            {
                SalesLW.ItemsSource = null;
                FilteredTotalTb.Text = "Сумма за выбранный период: 0,00 руб.";
                return;
            }
            var filteredList = salesHistory.ToList();
            if (StartDatePicker.SelectedDate.HasValue)
            {
                DateTime startDate = StartDatePicker.SelectedDate.Value.Date;
                filteredList = filteredList.Where(s => s.RequestDate.Date >= startDate).ToList();
            }
            if (EndDatePicker.SelectedDate.HasValue)
            {
                DateTime endDate = EndDatePicker.SelectedDate.Value.Date;
                filteredList = filteredList.Where(s => s.RequestDate.Date <= endDate).ToList();
            }
            SalesLW.ItemsSource = filteredList;
            decimal filteredSum = filteredList.Sum(s => s.TotalAmount);
            FilteredTotalTb.Text = $"Сумма за выбранный период: {filteredSum:N2} руб.";
        }

        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            ApplyFilter();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartnersListPage());
        }
    }

    public class SaleHistoryItem
    {
        public DateTime RequestDate { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}