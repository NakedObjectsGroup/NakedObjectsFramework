using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class CreditCard
    {
        public CreditCard()
        {
            this.ContactCreditCards = new List<ContactCreditCard>();
            this.SalesOrderHeaders = new List<SalesOrderHeader>();
        }

        public int CreditCardID { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public byte ExpMonth { get; set; }
        public short ExpYear { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<ContactCreditCard> ContactCreditCards { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
    }
}
