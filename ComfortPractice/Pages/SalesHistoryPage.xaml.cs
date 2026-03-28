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
    /// Логика взаимодействия для SalesHistoryPage.xaml
    /// </summary>
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
            var requests = Connection.connect.Request.Where(r => r.Id_partner == currentPartner.Id_partner && r.Id_status == 5).ToList();
            salesHistory = new List<SaleHistoryItem>();
            foreach (var request in requests)
            {
                var requestDetails = Connection.connect.RequestDetails.Where(rd => rd.Id_request == request.Id_request).ToList();
                foreach (var rd in requestDetails)
                {
                    var product = Connection.connect.Products.FirstOrDefault(p => p.Id_product == rd.Id_product);
                    if (product != null && product.MinPrice.HasValue && rd.Quantity.HasValue)
                    {
                        var statusName = Connection.connect.Status.FirstOrDefault(s => s.Id_status == request.Id_status)?.NameStatus ?? "Неизвестно";
                        salesHistory.Add(new SaleHistoryItem
                        {
                            RequestDate = request.RequestDate ?? DateTime.Now,
                            ProductName = product.NameProduct ?? "Неизвестно",
                            Quantity = rd.Quantity.Value,
                            TotalAmount = rd.Quantity.Value * product.MinPrice.Value,
                            Status = statusName
                        });
                    }
                }
            }
            decimal totalSum = salesHistory.Sum(s => s.TotalAmount);
            TotalSalesTb.Text = $"Общая сумма продаж: {totalSum:N2} руб.";
            ApplyFilter();
        }



        private void ApplyFilter()
        {
            var filteredList = salesHistory;
            if (StartDatePicker.SelectedDate.HasValue)
            {
                filteredList = filteredList.Where(s => s.RequestDate >= StartDatePicker.SelectedDate.Value).ToList();
            }
            if (EndDatePicker.SelectedDate.HasValue)
            {
                filteredList = filteredList.Where(s => s.RequestDate <= EndDatePicker.SelectedDate.Value).ToList();
            }
            SalesLW.ItemsSource = filteredList;
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
