    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class BankAccount
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public float Balance { get; set; }
        public int ProfileID { get; set; }
        public virtual Profile Profile { get; set; }

        public virtual ICollection<Transaction> TransactionFrom { get; set; }
        public virtual ICollection<Transaction> TransactionTo { get; set; }
        public virtual ICollection<Credit> Credits { get; set; }
    }
}