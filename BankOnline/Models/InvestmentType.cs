using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class InvestmentType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float Percentage { get; set; }

        public virtual ICollection<Investment> Investments { get; set; }
    }
}