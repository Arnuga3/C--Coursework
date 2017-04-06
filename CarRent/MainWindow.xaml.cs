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
using System.Data.Entity.SqlServer;

namespace CarRent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            /*
             * Some hard-coded data to start with
             * 
            var car = new Cars { regNumber = "FE77ADE", dailyRate = 12.5 };
            var customer = new Customers { firstName = "John", lastName = "Fox" };
            var order = new Orders { carID = 1, customerID = 1, startDate = DateTime.Now, duration = 5 };
            db.Cars.Add(car);
            db.Customers.Add(customer);
            db.Orders.Add(order);
            db.SaveChanges();
            */

            DisplayAllOrders();
        }

        private void DisplayAllOrders()
        {
            using (var db = new CarRentModelContainer())
            {
                // LINQ query to fill a list with 'all orders' result
                //Join 3 tables (Orders, Cars, Customers)
                var query = from o in db.Orders
                            join c in db.Cars on o.carID equals c.ID
                            join cu in db.Customers on o.customerID equals cu.ID
                            // Add number of days to a 'start date' to get an 'end date'
                            let d = SqlFunctions.DateAdd("dd", o.duration, o.startDate)
                            // Write to new objects with properties used in a binding
                            select new
                            {
                                ID = o.ID,
                                regNumber = c.regNumber,
                                dailyRate = c.dailyRate,
                                duration = o.duration,
                                startDate = o.startDate,
                                endDate = d,
                                total = o.duration * c.dailyRate,
                                customer = cu.firstName + " " + cu.lastName
                            };
                // Bind to a list
                listView.ItemsSource = query.ToList();
            }
        }

        private void DisplayCurrentOrders()
        {
            using (var db = new CarRentModelContainer())
            {
                // LINQ query to fill a list with 'all orders' result
                //Join 3 tables (Orders, Cars, Customers)
                var query = from o in db.Orders
                            join c in db.Cars on o.carID equals c.ID
                            join cu in db.Customers on o.customerID equals cu.ID
                            // Add number of days to a 'start date' to get an 'end date'
                            let d = SqlFunctions.DateAdd("dd", o.duration, o.startDate)
                            // Write to new objects with properties used in a binding
                            where o.startDate <= DateTime.Now && d >= DateTime.Now
                            select new
                            {
                                ID = o.ID,
                                regNumber = c.regNumber,
                                dailyRate = c.dailyRate,
                                duration = o.duration,
                                startDate = o.startDate,
                                endDate = d,
                                total = o.duration * c.dailyRate,
                                customer = cu.firstName + " " + cu.lastName
                            };
                // Bind to a list
                listView.ItemsSource = query.ToList();
            }
        }

        private void DisplayPendingOrders()
        {
            using (var db = new CarRentModelContainer())
            {
                // LINQ query to fill a list with 'all orders' result
                //Join 3 tables (Orders, Cars, Customers)
                var query = from o in db.Orders
                            join c in db.Cars on o.carID equals c.ID
                            join cu in db.Customers on o.customerID equals cu.ID
                            // Add number of days to a 'start date' to get an 'end date'
                            let d = SqlFunctions.DateAdd("dd", o.duration, o.startDate)
                            // Write to new objects with properties used in a binding
                            where o.startDate > DateTime.Now
                            select new
                            {
                                ID = o.ID,
                                regNumber = c.regNumber,
                                dailyRate = c.dailyRate,
                                duration = o.duration,
                                startDate = o.startDate,
                                endDate = d,
                                total = o.duration * c.dailyRate,
                                customer = cu.firstName + " " + cu.lastName
                            };
                // Bind to a list
                listView.ItemsSource = query.ToList();
            }
        }


        private void ordersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AllOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            DisplayAllOrders();
        }

        private void CurrentOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            DisplayCurrentOrders();
        }

        private void PendingOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            DisplayPendingOrders();
        }
    }
}
