using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class Credit
    {
        public int ID { get; set; }
        public int BankAccountID { get; set; }  // wnioskodawca
        [DataType(DataType.Currency)]
        public float Balance { get; set; } // wysokosc kredytu
        [DataType(DataType.Currency)]
        public float BalancePaid { get; set; } // ilosc wplaconej gotowki
        public DateTime StatusDate { get; set; } // data rozpatrzenia wniosku
        public CreditType? CreditType { get; set; }
        [DataType(DataType.Currency)]
        public float BaseBalance { get; set; }

        public virtual BankAccount BankAccount { get; set; }
    }
}