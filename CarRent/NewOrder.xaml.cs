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
    /// Interaction logic for NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
        private CarRentModelContainer dbContext;
        DateTime initialDate;

        public NewOrder()
        {
            InitializeComponent();
            this.startDateNewOrder.SelectedDate = DateTime.Now;
            this.endDateNewOrder.SelectedDate = DateTime.Now;
            initialDate = (DateTime)this.endDateNewOrder.SelectedDate;
            LoadCarComboBox();
        }

        private void LoadCarComboBox()
        {
            this.dbContext = new CarRentModelContainer();

            DateTime fromD = (DateTime)this.startDateNewOrder.SelectedDate;
            var toD = this.endDateNewOrder.SelectedDate;

            // Get all cars available within selected dates and add them to a comboBox
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

            this.carListNewOrder.ItemsSource = query.ToList();
        }

        private void handleDayPickerDates(object sender, RoutedEventArgs e)
        {
            DateTime fromD = (DateTime)this.startDateNewOrder.SelectedDate;
            DateTime max30 = fromD.AddDays(30);

            if (this.endDateNewOrder.SelectedDate > max30)
            {
                MessageBox.Show("The maximum duration is 30 Days.");
                this.endDateNewOrder.SelectedDate = initialDate;
            } else LoadCarComboBox();
        }

        private void carListNewOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaveBtnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelBtnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void drivingLicenseNewOrder_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.dbContext = new CarRentModelContainer();
            var query = from cu in this.dbContext.Customers select cu;
            foreach (var customer in query)
            {
                if (this.drivingLicenseNewOrder.Text.ToUpper() == customer.drivingLicense.ToUpper())
                {
                    this.firstnameNewOrder.Text = customer.firstName;
                    this.lastnameNewOrder.Text = customer.lastName;
                    break;
                }
                else
                {
                    this.firstnameNewOrder.Text = "";
                    this.lastnameNewOrder.Text = "";
                }
            }
        }
    }
}
