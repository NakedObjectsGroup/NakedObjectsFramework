using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class VendorMap : EntityTypeConfiguration<Vendor>
    {
        public VendorMap()
        {
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
