namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sales.CreditCard")]
    public partial class CreditCard
    {
        public CreditCard()
        {
            ContactCreditCards = new HashSet<ContactCreditCard>();
            SalesOrderHeaders = new HashSet<SalesOrderHeader>();
        }

        public int CreditCardID { get; set; }

        [Required]
        [StringLength(50)]
        public string CardType { get; set; }

        [Required]
        [StringLength(25)]
        public string CardNumber { get; set; }

        public byte ExpMonth { get; set; }

        public short ExpYear { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ContactCreditCard> ContactCreditCards { get; set; }

        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
    }
}
