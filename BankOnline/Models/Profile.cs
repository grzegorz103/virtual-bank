using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public class Profile
    {
        public int ID { get; set; }

        [EmailAddress]
        public string UserName { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        public bool EnableMail { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<DefinedTransfer> DefinedTransfers { get; set; }
    }
}