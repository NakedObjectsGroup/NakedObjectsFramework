using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel {

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<BusinessEntity> builder)
        {
            builder.HasKey(t => t.BusinessEntityID);

            // Table & Column Mappings
            builder.ToTable("BusinessEntity", "Person");
            builder.Property(t => t.BusinessEntityRowguid).HasColumnName("rowguid");
            builder.Property(t => t.BusinessEntityModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            builder.HasMany(t => t.mappedAddresses).WithOne(t => t.BusinessEntity).HasForeignKey(t => t.BusinessEntityID);
            builder.HasMany(t => t.mappedContacts).WithOne(t => t.BusinessEntity).HasForeignKey(t => t.BusinessEntityID);
        }
    }
}