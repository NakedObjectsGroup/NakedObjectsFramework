using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class VendorMap : EntityTypeConfiguration<Vendor>
    {
        public VendorMap()
        {
            // Primary Key
            this.HasKey(t => t.VendorID);

            // Properties
            this.Property(t => t.AccountNumber)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PurchasingWebServiceURL)
                .HasMaxLength(1024);

            // Table & Column Mappings
            this.ToTable("Vendor", "Purchasing");
            this.Property(t => t.VendorID).HasColumnName("VendorID");
            this.Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.CreditRating).HasColumnName("CreditRating");
            this.Property(t => t.PreferredVendorStatus).HasColumnName("PreferredVendorStatus");
            this.Property(t => t.ActiveFlag).HasColumnName("ActiveFlag");
            this.Property(t => t.PurchasingWebServiceURL).HasColumnName("PurchasingWebServiceURL");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
