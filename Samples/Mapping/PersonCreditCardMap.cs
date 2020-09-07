using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class PersonCreditCardMap : EntityTypeConfiguration<PersonCreditCard>
    {
        public PersonCreditCardMap()
        {
            // Primary Key
            HasKey(t => new { t.PersonID, t.CreditCardID });

            // Properties
            Property(t => t.PersonID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.CreditCardID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("PersonCreditCard", "Sales");
            Property(t => t.PersonID).HasColumnName("BusinessEntityID");
            Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
            HasRequired(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID);

        }
    }
}
