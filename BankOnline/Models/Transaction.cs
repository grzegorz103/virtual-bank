using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; }
        public int? FromID { get; set; }
        public int? ToID { get; set; }

        [DataType(DataType.Currency)]
        public float Amount { get; set; }
        public virtual BankAccount From { get; set; }
        public virtual BankAccount To { get; set; }
    }
}