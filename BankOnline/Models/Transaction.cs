using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? FromID { get; set; }
        public int? ToID { get; set; }

        public virtual BankAccount From { get; set; }
        public virtual BankAccount To { get; set; }
    }
}