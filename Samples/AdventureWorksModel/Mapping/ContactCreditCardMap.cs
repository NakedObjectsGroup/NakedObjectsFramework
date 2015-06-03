using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ContactCreditCardMap : EntityTypeConfiguration<ContactCreditCard>
    {
        public ContactCreditCardMap()
        {
            // Primary Key
            HasKey(t => new { t.ContactID, t.CreditCardID });

            // Properties
            Property(t => t.ContactID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.CreditCardID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ContactCreditCard", "Sales");
            Property(t => t.ContactID).HasColumnName("ContactID");
            Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.Contact).WithMany().HasForeignKey(t => t.ContactID); ;
            HasRequired(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID); ;

        }
    }
}
