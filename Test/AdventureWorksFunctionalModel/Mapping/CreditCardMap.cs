using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping
{
    public class CreditCardMap : EntityTypeConfiguration<CreditCard>
    {
        public CreditCardMap()
        {
            // Primary Key
            HasKey(t => t.CreditCardID);

            //Ignores

            // Properties
            Property(t => t.CardType)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.CardNumber)
                .IsRequired()
                .HasMaxLength(25);

            // Table & Column Mappings
            ToTable("CreditCard", "Sales");
            Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            Property(t => t.CardType).HasColumnName("CardType");
            Property(t => t.CardNumber).HasColumnName("CardNumber");
            Property(t => t.ExpMonth).HasColumnName("ExpMonth");
            Property(t => t.ExpYear).HasColumnName("ExpYear");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken();

            HasMany(t => t.PersonLinks).WithRequired(t => t.CreditCard);
        }
    }
}
