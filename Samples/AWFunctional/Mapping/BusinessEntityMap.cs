using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;

namespace AdventureWorksModel
{
    public class BusinessEntityMap : EntityTypeConfiguration<BusinessEntity>
    {
        public BusinessEntityMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            ToTable("BusinessEntity", "Person");
            Property(t => t.BusinessEntityRowguid).HasColumnName("rowguid");
            Property(t => t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            HasMany(t => t.Addresses).WithRequired(t => t.BusinessEntity);
        }
    }
}