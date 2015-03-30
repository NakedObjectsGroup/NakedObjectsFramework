using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class ContactCreditCardMap : EntityTypeConfiguration<ContactCreditCard>
    {
        public ContactCreditCardMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ContactID, t.CreditCardID });

            // Properties
            this.Property(t => t.ContactID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CreditCardID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ContactCreditCard", "Sales");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Contact)
                .WithMany(t => t.ContactCreditCards)
                .HasForeignKey(d => d.ContactID);
            this.HasRequired(t => t.CreditCard)
                .WithMany(t => t.ContactCreditCards)
                .HasForeignKey(d => d.CreditCardID);

        }
    }
}
