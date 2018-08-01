using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel {
    public class BusinessEntityAddressMap : EntityTypeConfiguration<BusinessEntityAddress>
    {
        public BusinessEntityAddressMap()
        {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.AddressTypeID, t.AddressID });

            // Table & Column Mappings
            ToTable("BusinessEntityAddress", "Person");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            //Relationships
            HasRequired(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);
            HasRequired(t => t.AddressType).WithMany().HasForeignKey(t => t.AddressTypeID);
            HasRequired(t => t.BusinessEntity).WithMany().HasForeignKey(t => t.BusinessEntityID);
        }
    }
}