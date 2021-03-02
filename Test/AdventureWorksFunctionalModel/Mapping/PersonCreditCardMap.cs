using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
            HasRequired(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PersonCreditCard> builder)
        {
            builder.HasKey(t => new { t.PersonID, t.CreditCardID });

            // Properties
            builder.Property(t => t.PersonID)
                   .ValueGeneratedNever();

            builder.Property(t => t.CreditCardID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("PersonCreditCard", "Sales");
            builder.Property(t => t.PersonID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.CreditCardID).HasColumnName("CreditCardID");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
           //builder.HasRequired(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
           //builder.HasRequired(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID);
        }
    }
}
