using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;

namespace AdventureWorksModel
{
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

            //Relationships
            //HasRequired(t => t.Address).WithMany();
            //HasRequired(t => t.AddressType).WithMany();
            //HasRequired(t => t.BusinessEntity).WithMany();
        }
    }
}