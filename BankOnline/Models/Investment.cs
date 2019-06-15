using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class Investment
    {
        public int ID { get; set; }
        public int? BankAccountID { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; }

        [DataType(DataType.Currency)]
        public float Balance { get; set; }

        [DataType(DataType.Currency)]
        public float BaseBalance { get; set; }
        public int InvestmentTypeID { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual InvestmentType InvestmentType { get; set; }
    }
}