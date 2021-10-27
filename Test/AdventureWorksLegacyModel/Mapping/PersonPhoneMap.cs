using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Person;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping {
    public class PersonPhoneMap : EntityTypeConfiguration<PersonPhone> {
        public PersonPhoneMap() {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.PhoneNumber, t.PhoneNumberTypeID});

            // Table & Column Mappings
            this.ToTable("PersonPhone", "Person");
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PersonPhone> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.PhoneNumber, t.PhoneNumberTypeID });

            // Table & Column Mappings
            builder.ToTable("PersonPhone", "Person");


            builder.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            builder.Property(t => t.PhoneNumberTypeID).HasColumnName("PhoneNumberTypeID");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            builder.Ignore(t => t.Person);
            //builder.HasOne(t => t.Person).WithMany().HasForeignKey(t => t.BusinessEntityID);
        }
    }
}