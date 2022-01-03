using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel {
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PersonPhone> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.mappedPhoneNumber, t.PhoneNumberTypeID });

            // Table & Column Mappings
            builder.ToTable("PersonPhone", "Person");


            builder.Property(t => t.mappedPhoneNumber).HasColumnName("PhoneNumber");
            builder.Property(t => t.PhoneNumberTypeID).HasColumnName("PhoneNumberTypeID");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            //builder.Ignore(t => t.mappedPerson);
            //builder.HasOne(t => t.Person).WithMany().HasForeignKey(t => t.BusinessEntityID);
        }
    }
}