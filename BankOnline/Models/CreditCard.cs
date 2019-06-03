using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class CreditCard
    {
        public int ID { get; set; }
        public string Image { get; set; }

        public virtual ICollection<BankAccount> BankAccounts{ get; set; }
    }
}