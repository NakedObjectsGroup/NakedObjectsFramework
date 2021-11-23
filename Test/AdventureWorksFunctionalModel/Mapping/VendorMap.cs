using System.Data.Entity.ModelConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class VendorMap : EntityTypeConfiguration<Vendor> {
        public VendorMap() {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Properties
            Property(t => t.AccountNumber)
                .IsRequired()
                .HasMaxLength(15);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.PurchasingWebServiceURL)
                .HasMaxLength(1024);

            // Table & Column Mappings
            ToTable("Vendor", "Purchasing");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CreditRating).HasColumnName("CreditRating");
            Property(t => t.PreferredVendorStatus).HasColumnName("PreferredVendorStatus");
            Property(t => t.ActiveFlag).HasColumnName("ActiveFlag");
            Property(t => t.PurchasingWebServiceURL).HasColumnName("PurchasingWebServiceURL");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<Vendor> builder) {
            builder.HasKey(t => t.BusinessEntityID);

            // Properties
            builder.Property(t => t.AccountNumber)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.PurchasingWebServiceURL)
                   .HasMaxLength(1024);

            // Table & Column Mappings
            builder.ToTable("Vendor", "Purchasing");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.CreditRating).HasColumnName("CreditRating");
            builder.Property(t => t.PreferredVendorStatus).HasColumnName("PreferredVendorStatus");
            builder.Property(t => t.ActiveFlag).HasColumnName("ActiveFlag");
            builder.Property(t => t.PurchasingWebServiceURL).HasColumnName("PurchasingWebServiceURL");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
        }
    }
}