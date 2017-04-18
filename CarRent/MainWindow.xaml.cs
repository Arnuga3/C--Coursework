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
            DisplayAllCarsList();
            DisplayAllCustomersList();
        }

        private void DisplayAllCustomersList()
        {
            this.dbContext = new CarRentModelContainer();
            // LINQ query to fill a list with 'all customers' result
            var query = from c in this.dbContext.Customers
                        select c;
            // Bind to a list
            listViewCustomers.ItemsSource = query.ToList();
        }

        private void CustomerSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.dbContext = new CarRentModelContainer();
            var searchStr = this.CustomerSearchField.Text;
            if (searchStr != "")
            {
                var query = from c in this.dbContext.Customers
                            where c.firstName.StartsWith(searchStr) || c.lastName.StartsWith(searchStr) || c.drivingLicense.StartsWith(searchStr)
                            select c;
                // Bind to a list
                listViewCustomers.ItemsSource = query.ToList();
            }
            else
            {
                DisplayAllCustomersList();
            }
            
        }

        private void DisplayAllCarsList()
        {
            this.dbContext = new CarRentModelContainer();
            // LINQ query to fill a list with 'all cars' result
            var query = from c in this.dbContext.Cars
                        select c;
            // Bind to a list
            listViewCars.ItemsSource = query.ToList();
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
                            startDate = o.startDate,
                            endDate = d,
                            duration = o.duration,
                            customer = cu.firstName + " " + cu.lastName,
                            license = cu.drivingLicense,
                            regNumber = c.regNumber,
                            dailyRate = c.dailyRate,
                            total = o.duration * c.dailyRate,
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
                            startDate = o.startDate,
                            endDate = d,
                            duration = o.duration,
                            customer = cu.firstName + " " + cu.lastName,
                            license = cu.drivingLicense,
                            regNumber = c.regNumber,
                            dailyRate = c.dailyRate,
                            total = o.duration * c.dailyRate,
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
                            startDate = o.startDate,
                            endDate = d,
                            duration = o.duration,
                            customer = cu.firstName + " " + cu.lastName,
                            license = cu.drivingLicense,
                            regNumber = c.regNumber,
                            dailyRate = c.dailyRate,
                            total = o.duration * c.dailyRate,
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
                            startDate = o.startDate,
                            endDate = d,
                            duration = o.duration,
                            customer = cu.firstName + " " + cu.lastName,
                            license = cu.drivingLicense,
                            regNumber = c.regNumber,
                            dailyRate = c.dailyRate,
                            total = o.duration * c.dailyRate,
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
                // Getting data from a dialog form
                DateTime fromDate = (DateTime)dialog.startDateNewOrder.SelectedDate;
                DateTime toDate = (DateTime)dialog.endDateNewOrder.SelectedDate;
                // Calc the duration
                int duration = (toDate - fromDate).Days;
                // >>> carID is actually a regNumber HERE!!!
                String carID = dialog.carListNewOrder.Text;
                String custName = dialog.firstnameNewOrder.Text;
                String custLastName = dialog.lastnameNewOrder.Text;
                String drivingLicense = dialog.drivingLicenseNewOrder.Text;
                if (fromDate != null && toDate != null && carID != null &&
                    custName != null && custLastName != null && drivingLicense != null)
                {
                    // Initialize PKs for new order insertion
                    // Getting a PK of a car using the regNumber (also unique)
                    int carPK = (from c in this.dbContext.Cars where c.regNumber == carID select c.ID).FirstOrDefault();
                    // Set to 0, reassign value later in the code depending on condition
                    int customerPK= 0;
                    Boolean newCustomer = true;

                    // Checking if a customer already in DB
                    var query = from cu in this.dbContext.Customers select cu;
                    foreach (var customer in query)
                    {
                        if (drivingLicense.ToUpper() == customer.drivingLicense.ToUpper())
                        {
                            // Customer in a DB, taking his ID
                            customerPK = customer.ID;
                            newCustomer = false;
                            break;
                        }
                    }

                    // If customer is new
                    if (newCustomer)
                    {
                        // Save customer to DB
                        Customers newCu = new Customers { firstName = custName, lastName = custLastName, drivingLicense = drivingLicense };
                        this.dbContext.Customers.Add(newCu);
                        this.dbContext.SaveChanges();
                        // Fetch an just inserted customer's ID
                        customerPK = newCu.ID;
                    }

                    // Create a new order record
                    Orders newO = new Orders { carID = carPK, customerID = customerPK, startDate = fromDate, duration = duration };
                    this.dbContext.Orders.Add(newO);
                    this.dbContext.SaveChanges();

                    // Refresh the list (display all - default)
                    DisplayAllOrders();
                }
            }
        }

        private void AddNewCarBtn_Click(object sender, RoutedEventArgs e)
        {
            newCar dialog = new newCar();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                String regNum = dialog.RegNumNewCar.Text;
                String dailyRateField = dialog.DailyRateNewCar.Text;
                if (regNum != null && dailyRateField != null)
                {
                    float dailyRate = (float)Convert.ToDouble(dialog.DailyRateNewCar.Text);

                    Cars car = new Cars { regNumber = regNum, dailyRate = dailyRate };
                    this.dbContext.Cars.Add(car);
                    this.dbContext.SaveChanges();

                    DisplayAllCarsList();
                }
            }
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                var item = (dynamic)listView.SelectedItems[0];
                OrderFullDetails order = new OrderFullDetails
                {
                    ID = item.ID,
                    startDate = item.startDate,
                    endDate = item.endDate,
                    duration = item.duration,
                    customer = item.customer,
                    license = item.license,
                    regNumber = item.regNumber,
                    dailyRate = item.dailyRate,
                    total = item.total
                };
                EditOrderDialog dialog = new EditOrderDialog(order);
                bool? result = dialog.ShowDialog();
            }
        }
    }
}

