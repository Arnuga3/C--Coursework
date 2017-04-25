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

namespace CarRent
{
    /// <summary>
    /// Interaction logic for EditCustomerDialog.xaml
    /// </summary>
    public partial class EditCustomerDialog : Window
    {
        private Customers customer;
        private CarRentModelContainer dbContext;

        public EditCustomerDialog(Customers customer)
        {
            InitializeComponent();
            this.customer = customer;
            this.firstNameEdit.Text = customer.firstName;
            this.lastNameEdit.Text = customer.lastName;
            this.drivingLicenseEdit.Text = customer.drivingLicense;
        }

        private void SaveEditCarBtn_Click(object sender, RoutedEventArgs e)
        {
            this.dbContext = new CarRentModelContainer();
            var query = (from cu in this.dbContext.Customers
                          where cu.ID == customer.ID
                          select cu).FirstOrDefault();

            query.firstName = this.firstNameEdit.Text;
            query.lastName = this.lastNameEdit.Text;
            query.drivingLicense = this.drivingLicenseEdit.Text;

            this.dbContext.SaveChanges();
            this.Close();
        }

        private void DeleteCarBtn_Click(object sender, RoutedEventArgs e)
        {
            this.dbContext = new CarRentModelContainer();
            var query = (from o in this.dbContext.Orders
                         where o.customerID == customer.ID
                         select o).FirstOrDefault();
            // Not in orders
            if (query == null)
            {
                // Delete this car
                var query2 = (from c in this.dbContext.Customers
                              where c.ID == customer.ID
                              select c).FirstOrDefault();
                this.dbContext.Customers.Remove(query2);
                this.dbContext.SaveChanges();
            }
            else
            {
                MessageBox.Show("Customer's name is in the order(s). Cannot delete.");
            }
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
