using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class Investment
    {
        public int ID { get; set; }
        public int? BankAccountID { get; set; }
        public DateTime VisitDate { get; set; }
        public float Balance { get; set; }
        public int InvestmentTypeID { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual InvestmentType InvestmentType { get; set; }
    }
}