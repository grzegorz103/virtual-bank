using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class DefinedTransfer
    {
        public int ID { get; set; }

        public int? FromID { get; set; }
        public int? ToID { get; set; }

        [DataType(DataType.Currency)]
        public float Amount { get; set; }
        public virtual BankAccount From { get; set; }
        public virtual BankAccount To { get; set; }

        [MinLength(3)]
        public string Title { get; set; }
        public int ProfileID { get; set; }
        public virtual Profile Profile { get; set; }


    }
}