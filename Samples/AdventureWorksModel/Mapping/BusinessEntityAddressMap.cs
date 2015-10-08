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
        }
    }
}