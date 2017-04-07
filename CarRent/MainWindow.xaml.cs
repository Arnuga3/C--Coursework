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

        private CarRentModelContainer dbContext;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            this.dbContext = new CarRentModelContainer();
            var car = new Cars { regNumber = "FE77ADE", dailyRate = 12.5 };
            var customer = new Customers { firstName = "John", lastName = "Fox" , DrivingLicense = "LDKF66KGG"};
            var order = new Orders { carID = 1, customerID = 1, startDate = DateTime.Now, duration = 5 };
            this.dbContext.Cars.Add(car);
            this.dbContext.Customers.Add(customer);
            this.dbContext.Orders.Add(order);
            this.dbContext.SaveChanges();
            */
            

            DisplayAllOrders();
        }

        private void DisplayAllOrders()
        {
            this.dbContext = new CarRentModelContainer();
                // LINQ query to fill a list with 'all orders' result
                //Join 3 tables (Orders, Cars, Customers)
            var query = from o in this.dbContext.Orders
                        join c in this.dbContext.Cars on o.carID equals c.ID
                        join cu in this.dbContext.Customers on o.customerID equals cu.ID
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

        private void DisplayCurrentOrders()
        {
            this.dbContext = new CarRentModelContainer();
            // LINQ query to fill a list with 'all orders' result
            //Join 3 tables (Orders, Cars, Customers)
            var query = from o in this.dbContext.Orders
                        join c in this.dbContext.Cars on o.carID equals c.ID
                        join cu in this.dbContext.Customers on o.customerID equals cu.ID
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

        private void DisplayPendingOrders()
        {
            this.dbContext = new CarRentModelContainer();
            // LINQ query to fill a list with 'all orders' result
            //Join 3 tables (Orders, Cars, Customers)
            var query = from o in this.dbContext.Orders
                        join c in this.dbContext.Cars on o.carID equals c.ID
                        join cu in this.dbContext.Customers on o.customerID equals cu.ID
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

        private void DisplayCompletedOrders()
        {
            this.dbContext = new CarRentModelContainer();
            // LINQ query to fill a list with 'all orders' result
            //Join 3 tables (Orders, Cars, Customers)
            var query = from o in this.dbContext.Orders
                        join c in this.dbContext.Cars on o.carID equals c.ID
                        join cu in this.dbContext.Customers on o.customerID equals cu.ID
                        // Add number of days to a 'start date' to get an 'end date'
                        let d = SqlFunctions.DateAdd("dd", o.duration, o.startDate)
                        // Write to new objects with properties used in a binding
                        where d < DateTime.Now
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

        private void CompletedOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            DisplayCompletedOrders();
        }

        private void AddNewOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            NewOrder dialog = new NewOrder();
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                DateTime? fromDate = dialog.startDateNewOrder.SelectedDate;
                DateTime? toDate = dialog.endDateNewOrder.SelectedDate;
                String carID = dialog.carListNewOrder.Text;
                String custName = dialog.nameNewOrder.Text;
                String custLastName = dialog.lastnameNewOrder.Text;
                if (fromDate != null && toDate != null && carID != null && custName != null && custLastName != null)
                {
                    Orders newOrder = new Orders {  };
                    this.dbContext.Orders.Add(newOrder);
                    this.dbContext.SaveChanges();
                }
            }
        }

    }
}

