using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<BillOfMaterial> builder)
        {
            builder.HasKey(t => t.BillOfMaterialID);

            // Properties
            builder.Property(t => t.UnitMeasureCode)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(3);

            // Table & Column Mappings
            builder.ToTable("BillOfMaterials", "Production");
            builder.Property(t => t.BillOfMaterialID).HasColumnName("BillOfMaterialsID");
            builder.Property(t => t.ProductAssemblyID).HasColumnName("ProductAssemblyID");
            builder.Property(t => t.ComponentID).HasColumnName("ComponentID");
            builder.Property(t => t.mappedStartDate).HasColumnName("StartDate");
            builder.Property(t => t.mappedEndDate).HasColumnName("EndDate");
            builder.Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            builder.Property(t => t.mappedBOMLevel).HasColumnName("BOMLevel");
            builder.Property(t => t.mappedPerAssemblyQty).HasColumnName("PerAssemblyQty");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductAssemblyID);
            builder.HasOne(t => t.Product1).WithMany().HasForeignKey(t => t.ComponentID);
            builder.HasOne(t => t.UnitMeasure).WithMany().HasForeignKey(t => t.UnitMeasureCode);
        }
    }
}
