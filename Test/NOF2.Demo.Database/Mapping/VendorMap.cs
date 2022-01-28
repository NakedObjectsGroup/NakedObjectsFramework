using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Vendor> builder)
        {
            builder.HasKey(t => t.BusinessEntityID);

            // Properties
            builder.Property(t => t.mappedAccountNumber)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.mappedPurchasingWebServiceURL)
                   .HasMaxLength(1024);

            // Table & Column Mappings
            builder.ToTable("Vendor", "Purchasing");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedAccountNumber).HasColumnName("AccountNumber");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedCreditRating).HasColumnName("CreditRating");
            builder.Property(t => t.mappedPreferredVendorStatus).HasColumnName("PreferredVendorStatus");
            builder.Property(t => t.mappedActiveFlag).HasColumnName("ActiveFlag");
            builder.Property(t => t.mappedPurchasingWebServiceURL).HasColumnName("PurchasingWebServiceURL");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
