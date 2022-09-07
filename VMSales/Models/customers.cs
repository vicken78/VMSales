using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSales.Models
{
    public class Customer
    { 
        public string n { get; set; }
        public int i
        { get; set; }
        public Customer()
        {
        }
    }
    public class Customers : List<Customer>
    {
        public Customers(string value, int numvalue)
        {
            this.Add(new Customer() { n = value, i = numvalue});
        }
    }
}
 
