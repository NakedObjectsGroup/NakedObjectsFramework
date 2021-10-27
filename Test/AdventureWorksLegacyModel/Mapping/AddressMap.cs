using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            HasKey(t => t.AddressID);

            //Ignores
            Ignore(t => t.AddressType);
            Ignore(t => t.AddressFor);
            Ignore(t => t.CountryRegion);

            // Properties
            Property(t => t.AddressLine1)
                .IsRequired()
                .HasMaxLength(60);

            Property(t => t.AddressLine2)
                .HasMaxLength(60);

            Property(t => t.City)
                .IsRequired()
                .HasMaxLength(30);

            Property(t => t.PostalCode)
                .IsRequired()
                .HasMaxLength(15);

            // Table & Column Mappings
            ToTable("Address", "Person");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.AddressLine1).HasColumnName("AddressLine1");
            Property(t => t.AddressLine2).HasColumnName("AddressLine2");
            Property(t => t.City).HasColumnName("City");
            Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            Property(t => t.PostalCode).HasColumnName("PostalCode");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.StateProvince).WithMany().HasForeignKey(t =>t.StateProvinceID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(t => t.AddressID);

            builder.Ignore(t => t.AddressType);
            builder.Ignore(t => t.AddressFor);
            builder.Ignore(t => t.CountryRegion);



            builder.Property(t => t.AddressLine1)
                   .IsRequired()
                   .HasMaxLength(60);

            builder.Property(t => t.AddressLine2)
                   .HasMaxLength(60);

            builder.Property(t => t.City)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(t => t.PostalCode)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.ToTable("Address", "Person");
            builder.Property(t => t.AddressID).HasColumnName("AddressID");
            builder.Property(t => t.AddressLine1).HasColumnName("AddressLine1");
            builder.Property(t => t.AddressLine2).HasColumnName("AddressLine2");
            builder.Property(t => t.City).HasColumnName("City");
            builder.Property(t => t.StateProvinceID).HasColumnName("StateProvinceID");
            builder.Property(t => t.PostalCode).HasColumnName("PostalCode");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.StateProvince).WithMany().HasForeignKey(t => t.StateProvinceID);
            builder.HasOne(t => t.StateProvince).WithMany().HasForeignKey(t => t.StateProvinceID);
        }
    }
}
