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
    /// Interaction logic for newCar.xaml
    /// </summary>
    public partial class newCar : Window
    {

        private CarRentModelContainer dbContext;
        private Boolean allowSaving = true;

        public newCar()
        {
            InitializeComponent();
        }

        private void SaveNewCarBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = allowSaving ? true : false;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // Checking if the car registration number is already in db
        private void RegNumNewCar_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.dbContext = new CarRentModelContainer();
            var query = from c in this.dbContext.Cars select c;
            foreach (var car in query)
            {
                if (this.RegNumNewCar.Text.ToUpper() == car.regNumber.ToUpper())
                {
                    this.errorLabel.Content = "That car is already registered.";
                    allowSaving = false;
                    break;
                }
                else
                {
                    allowSaving = true;
                    this.errorLabel.Content = "";
                }
            }
        }
    }
}
