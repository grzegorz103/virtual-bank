using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class Credit
    {
        public int ID { get; set; }
        public int BankAccountID { get; set; }  // wnioskodawca
        public float Balance { get; set; } // wysokosc kredytu
        public float BalancePaid { get; set; } // ilosc wplaconej gotowki
        public int InstallmentNums { get; set; }
        public DateTime StatusDate { get; set; } // data rozpatrzenia wniosku
        public CreditType? CreditType { get; set; }

        public virtual BankAccount BankAccount { get; set; }
    }
}