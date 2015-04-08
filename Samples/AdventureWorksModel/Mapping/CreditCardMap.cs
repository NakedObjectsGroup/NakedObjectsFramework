using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CreditCardMap : EntityTypeConfiguration<CreditCard>
    {
        public CreditCardMap()
        {
            // Primary Key
            this.HasKey(t => t.CreditCardID);

            //Ignores
            this.Ignore(t => t.Creator);
            this.Ignore(t => t.ForContact);
            this.Ignore(t => t.ContactCreditCard);
            this.Ignore(t => t.ObfuscatedNumber);

            // Properties
            this.Property(t => t.CardType)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CardNumber)
                .IsRequired()
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("CreditCard", "Sales");
            this.Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            this.Property(t => t.CardType).HasColumnName("CardType");
            this.Property(t => t.CardNumber).HasColumnName("CardNumber");
            this.Property(t => t.ExpMonth).HasColumnName("ExpMonth");
            this.Property(t => t.ExpYear).HasColumnName("ExpYear");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
          
        }
    }
}
