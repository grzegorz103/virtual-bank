using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankOnline.Models
{
    public enum TransactionType
    {
        TRANSFER,   // przelew
        PAYMENT_IN, //wplata
        PAYMENT_OUT //wypłata
    }
}