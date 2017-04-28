using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private int orderListType = 0;

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

        // Refresh lists on tab change to refresh the content
        private void OnSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                DisplayAllOrders();
                DisplayAllCarsList();
                DisplayAllCustomersList();
            }
        }

        /*
         * CUSTOMERS TAB
         */


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

        private void listViewCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = this.listViewCustomers.SelectedItem;
            if (selected == null) return;
            Customers customer = (Customers)selected;
            EditCustomerDialog dialog = new EditCustomerDialog(customer);
            bool? result = dialog.ShowDialog();
            if (result == true)
            {

            }
            DisplayAllCustomersList();
        }

        /*
         * CARS TAB
         */

        // ALL cars list
        private void DisplayAllCarsList()
        {
            this.dbContext = new CarRentModelContainer();
            // LINQ query to fill a list with 'all cars' result
            var query = from c in this.dbContext.Cars
                        select c;
            // Bind to a list
            listViewCars.ItemsSource = query.ToList();
        }

        // ADD NEW car
        private void AddNewCarBtn_Click(object sender, RoutedEventArgs e)
        {
            newCar dialog = new newCar();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                String regNum = dialog.RegNumNewCar.Text;
                String dailyRateField = dialog.DailyRateNewCar.Text;
                float dailyRate;

                if (regNum != null && dailyRateField != null)
                {
                    // Catch invalid data input from a user
                    float.TryParse(dialog.DailyRateNewCar.Text, out dailyRate);

                    Cars car = new Cars { regNumber = regNum, dailyRate = dailyRate };
                    this.dbContext.Cars.Add(car);
                    this.dbContext.SaveChanges();

                    DisplayAllCarsList();
                }
            }
        }

        private void listViewCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = this.listViewCars.SelectedItem;
            if (selected == null) return;
            Cars car = (Cars)selected;
            EditCarDialog dialog = new EditCarDialog(car);
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                
            }
            DisplayAllCarsList();
        }


        /*
         * ORDERS TAB
         */

        // ALL orders list
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
            // Labels (for summary)
            int orderNum = 0;
            double totalPrice = 0;
            foreach (var order in query)
            {
                orderNum++;
                totalPrice += order.total;
            }
            this.totalOrders.Content = "Total orders: " + orderNum;
            this.totalFinance.Content = "Total (£): " + totalPrice;

            // Bind to a list
            listView.ItemsSource = query.ToList();
        }

        // CURRENT orders list
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

        // PENDING orders list
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

        // COMPLETED orders list
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

        // ALL orders list btn action
        private void AllOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            orderListType = 0;
            DisplayAllOrders();
        }

        // CURRENT orders list btn action
        private void CurrentOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            orderListType = 1;
            DisplayCurrentOrders();
        }

        // PENDING orders list btn action
        private void PendingOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            orderListType = 2;
            DisplayPendingOrders();
        }

        // COMPLETED orders list btn action
        private void CompletedOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            orderListType = 3;
            DisplayCompletedOrders();
        }

        // ADD NEW order
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
                int duration = (toDate == fromDate) ? 1 : (toDate - fromDate).Days + 1;
                // >>> carID is actually a regNumber HERE!!!
                String carID = dialog.carListNewOrder.Text;
                String custName = dialog.firstnameNewOrder.Text;
                String custLastName = dialog.lastnameNewOrder.Text;
                String drivingLicense = dialog.drivingLicenseNewOrder.Text;
                if (fromDate != null && toDate != null && (carID != null && carID != "") &&
                    (custName != null && custName != "") && (custLastName != null && custLastName != "") 
                    && (drivingLicense != null && drivingLicense != ""))
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

                    // Refresh the list
                    switch (orderListType)
                    {
                        case 0: DisplayAllOrders();
                            break;
                        case 1: DisplayCurrentOrders();
                            break;
                        case 2: DisplayPendingOrders();
                            break;
                        case 3: DisplayCompletedOrders();
                            break;
                    }
                    
                }
                else
                {
                    MessageBox.Show("Order wasn't saved as not all inforamtion was provided.");
                }
            }
        }

        // EDIT/DELETE order
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

                if (result == true)
                {
                    // Getting data from a dialog form
                    DateTime fromDate = (DateTime)dialog.startDateEditOrder.SelectedDate;
                    DateTime toDate = (DateTime)dialog.endDateEditOrder.SelectedDate;
                    // Calc the duration
                    int duration = (toDate - fromDate).Days;
                    // >>> carID is actually a regNumber HERE!!!
                    String carID = dialog.carListEditOrder.Text;
                    String custName = dialog.firstnameEditOrder.Text;
                    String custLastName = dialog.lastnameEditOrder.Text;
                    String drivingLicense = dialog.drivingLicenseEditOrder.Text;
                    if (fromDate != null && toDate != null && carID != null &&
                        custName != null && custLastName != null && drivingLicense != null)
                    {
                        // Initialize PKs for new order insertion
                        // Getting a PK of a car using the regNumber (also unique)
                        int carPK = (from c in this.dbContext.Cars where c.regNumber == carID select c.ID).FirstOrDefault();
                        // Set to 0, reassign value later in the code depending on condition
                        int customerPK = 0;
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

                        int oUpdateID = item.ID;
                        var queryUpdate = from o in this.dbContext.Orders
                            where o.ID == oUpdateID
                            select o;

                        foreach (Orders o in queryUpdate)
                        {
                            o.carID = carPK;
                            o.customerID = customerPK;
                            o.startDate = fromDate;
                            o.duration = duration;
                        }
                        
                        this.dbContext.SaveChanges();
                    }
                }

                // Refresh the list
                switch (orderListType)
                {
                    case 0:
                        DisplayAllOrders();
                        break;
                    case 1:
                        DisplayCurrentOrders();
                        break;
                    case 2:
                        DisplayPendingOrders();
                        break;
                    case 3:
                        DisplayCompletedOrders();
                        break;
                }
            }
        }

        // Delete all orders
        private void DeleteAllOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("All orders will be permanently deleted. Delete anyway?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.dbContext = new CarRentModelContainer();
                var query = from o in this.dbContext.Orders
                            select o;
                foreach (var order in query)
                {
                    this.dbContext.Orders.Remove(order);
                }
                this.dbContext.SaveChanges();
                DisplayAllOrders();
            }
        }
    }
}