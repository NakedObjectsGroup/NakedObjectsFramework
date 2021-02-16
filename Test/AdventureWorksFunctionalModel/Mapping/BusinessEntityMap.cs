using System.Data.Entity.ModelConfiguration;
using AW.Types;

namespace AW.Mapping {
    public class BusinessEntityMap : EntityTypeConfiguration<BusinessEntity>
    {
        public BusinessEntityMap()
        {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            ToTable("BusinessEntity", "Person");
            Property(t => t.BusinessEntityRowguid).HasColumnName("rowguid");
            Property(t => t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            HasMany(t => t.Addresses).WithRequired(t => t.BusinessEntity);
        }
    }
}