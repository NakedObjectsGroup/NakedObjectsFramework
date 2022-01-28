using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(t => t.AddressID);

            //builder.Ignore(t => t.AddressType);
            //builder.Ignore(t => t.AddressFor);
            //builder.Ignore(t => t.CountryRegion);



            builder.Property(t => t.mappedAddressLine1)
                   .IsRequired()
                   .HasMaxLength(60);

            builder.Property(t => t.mappedAddressLine2)
                   .HasMaxLength(60);

            builder.Property(t => t.mappedCity)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(t => t.mappedPostalCode)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.ToTable("Address", "Person");
            builder.Property(t => t.AddressID).HasColumnName("AddressID");
            builder.Property(t => t.mappedAddressLine1).HasColumnName("AddressLine1");
            builder.Property(t => t.mappedAddressLine2).HasColumnName("AddressLine2");
            builder.Property(t => t.mappedCity).HasColumnName("City");
            builder.Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            builder.Property(t => t.mappedPostalCode).HasColumnName("PostalCode");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.StateProvince).WithMany().HasForeignKey(t => t.StateProvinceID);
            builder.HasOne(t => t.StateProvince).WithMany().HasForeignKey(t => t.StateProvinceID);
        }
    }
}
