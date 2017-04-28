using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            // Fill the form with existing information
            this.startDateEditOrder.SelectedDate = order.startDate;
            this.endDateEditOrder.SelectedDate = order.endDate;
            this.drivingLicenseEditOrder.Text = order.license;
            this.carListEditOrder.SelectedValue = order.regNumber;


            LoadCarComboBox();
            LoadCustomerDetails();
        }

        private void LoadCarComboBox()
        {
            this.dbContext = new CarRentModelContainer();

            DateTime fromD = (DateTime)this.startDateEditOrder.SelectedDate;
            var toD = this.endDateEditOrder.SelectedDate;

            // Get all cars available within selected dates and add them to a comboBox
            var query = (from c in this.dbContext.Cars select c.regNumber)
                .Except
                (
                    (from c in this.dbContext.Cars
                     join o in this.dbContext.Orders on c.ID equals o.carID
                     let oEndDate = SqlFunctions.DateAdd("dd", o.duration, o.startDate)
                     // Filter not vailable cars by dates + ignoring the from a current order
                     where (o.startDate <= fromD && oEndDate >= fromD && o.ID != order.ID) ||
                           (o.startDate >= fromD && oEndDate <= toD && o.ID != order.ID) ||
                           (o.startDate <= toD && oEndDate >= toD && o.ID != order.ID)
                     select c.regNumber)
                );

            this.carListEditOrder.ItemsSource = query.ToList();
        }
        // Getting a customer using his license(unique)
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
        // Delete current order
        private void DeleteBtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            var query = (from o in dbContext.Orders
                        where o.ID == order.ID
                        select o).FirstOrDefault();
            dbContext.Orders.Remove(query);
            dbContext.SaveChanges();
            this.Close();
        }

        private void CancelBtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // Preload name and surname of a customer if there is a match in driving license
        private void drivingLicenseEditOrder_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.dbContext = new CarRentModelContainer();
            var query = from cu in this.dbContext.Customers select cu;
            foreach (var customer in query)
            {
                if (this.drivingLicenseEditOrder.Text.ToUpper() == customer.drivingLicense.ToUpper())
                {
                    this.firstnameEditOrder.Text = customer.firstName;
                    this.lastnameEditOrder.Text = customer.lastName;
                    break;
                }
                else
                {
                    this.firstnameEditOrder.Text = "";
                    this.lastnameEditOrder.Text = "";
                }
            }
        }
    }
}
