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
    /// Interaction logic for EditCarDialog.xaml
    /// </summary>
    public partial class EditCarDialog : Window
    {
        private Cars car;
        private CarRentModelContainer dbContext;

        public EditCarDialog(Cars car)
        {
            InitializeComponent();
            this.car = car;
            this.RegNumEditCar.Text = car.regNumber;
            this.DailyRateEditCar.Text = car.dailyRate.ToString();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveEditCarBtn_Click(object sender, RoutedEventArgs e)
        {
            this.dbContext = new CarRentModelContainer();
            var query3 = (from c in this.dbContext.Cars
                          where c.ID == car.ID
                          select c).FirstOrDefault();

            query3.regNumber = this.RegNumEditCar.Text;
            query3.dailyRate = Double.Parse(this.DailyRateEditCar.Text);

            this.dbContext.SaveChanges();
            this.Close();
        }

        private void DeleteCarBtn_Click(object sender, RoutedEventArgs e)
        {
            this.dbContext = new CarRentModelContainer();
            // Checking if that car is in any of the orders
            var query = (from o in this.dbContext.Orders
                        where o.carID == car.ID
                        select o).FirstOrDefault();
            // Not in orders
            if (query == null)
            {
                // Delete this car
                var query2 = (from c in this.dbContext.Cars
                             where c.ID == car.ID
                             select c).FirstOrDefault();
                this.dbContext.Cars.Remove(query2);
                this.dbContext.SaveChanges();
            }
            else
            {
                MessageBox.Show("Car is used in order(s). Cannot delete.");
            }
            this.Close();
        }
    }
}
