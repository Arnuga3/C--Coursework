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
using System.Data.Entity.SqlServer;

namespace CarRent
{
    /// <summary>
    /// Interaction logic for EditOrderDialog.xaml
    /// </summary> 

    public partial class EditOrderDialog : Window
    {
        private CarRentModelContainer dbContext;
        private OrderFullDetails order;

        public EditOrderDialog(OrderFullDetails order)
        {
            InitializeComponent();

            this.order = order;
            this.startDateEditOrder.SelectedDate = order.startDate;
            this.endDateEditOrder.SelectedDate = order.endDate;
            this.drivingLicenseEditOrder.Text = order.license;
            this.CurrCarLabel.Content = "Car registration number: " + order.regNumber;


            LoadCarComboBox();
            LoadCustomerDetails();
        }

        private void LoadCarComboBox()
        {
            this.dbContext = new CarRentModelContainer();

            DateTime fromD = (DateTime)this.startDateEditOrder.SelectedDate;
            var toD = this.endDateEditOrder.SelectedDate;

            // Get all cart available within selected dates and add them to a comboBox
            var query = (from c in this.dbContext.Cars select c.regNumber)
                .Except
                (
                    (from c in this.dbContext.Cars
                     join o in this.dbContext.Orders on c.ID equals o.carID
                     let oEndDate = SqlFunctions.DateAdd("dd", o.duration, o.startDate)
                     where (o.startDate <= fromD && oEndDate >= fromD) ||
                           (o.startDate >= fromD && oEndDate <= toD) ||
                           (o.startDate <= toD && oEndDate >= toD)
                     select c.regNumber)
                );

            this.carListEditOrder.ItemsSource = query.ToList();
        }

        private void LoadCustomerDetails()
        {
            this.dbContext = new CarRentModelContainer();

            var query = from c in this.dbContext.Customers
                        where c.drivingLicense == order.license
                        select c;

            this.firstnameEditOrder.Text = query.FirstOrDefault().firstName;
            this.lastnameEditOrder.Text = query.FirstOrDefault().lastName;
        }

        private void SaveBtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        private void handleDayPickerDates(object sender, RoutedEventArgs e)
        {
            LoadCarComboBox();
        }

        private void carListEditOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CancelBtnEditOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void drivingLicenseEditOrder_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
