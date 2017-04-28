using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRent
{
    /* Order information is expanded during the data retrieval from a database,
     * additional calculations are performed and new properties generated.
     * All that information is saved into an instance of this class and passed to 
     * edit order dialog window.
     */

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
