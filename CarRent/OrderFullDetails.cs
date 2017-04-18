using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRent
{
    public partial class OrderFullDetails
    {
        public int ID { get; set; }
        public System.DateTime startDate { get; set; }
        public System.DateTime endDate { get; set; }
        public int duration { get; set; }
        public string customer { get; set; }
        public string license { get; set; }
        public string regNumber { get; set; }
        public double dailyRate { get; set; }
        public double total { get; set; }
    }
}
