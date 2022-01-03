using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
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
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Person).WithMany().HasForeignKey(t => t.PersonID);
            builder.HasOne(t => t.CreditCard).WithMany().HasForeignKey(t => t.CreditCardID);
        }
    }
}
