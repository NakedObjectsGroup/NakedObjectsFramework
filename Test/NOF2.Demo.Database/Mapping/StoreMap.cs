using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Store> builder)
        {
            //builder.HasKey(t => t.BusinessEntityID);

            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Store", "Sales");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.SalesPersonID).HasColumnName("SalesPersonID");
            builder.Property(t => t.Demographics).HasColumnName("Demographics");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.SalesPerson).WithMany().HasForeignKey(t => t.SalesPersonID);
        }
    }
}
