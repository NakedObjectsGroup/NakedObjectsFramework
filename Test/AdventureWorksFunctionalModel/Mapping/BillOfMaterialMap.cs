using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class BillOfMaterialMap : EntityTypeConfiguration<BillOfMaterial>
    {
        public BillOfMaterialMap()
        {
            // Primary Key
            HasKey(t => t.BillOfMaterialID);

            // Properties
            Property(t => t.UnitMeasureCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            ToTable("BillOfMaterials", "Production");
            Property(t => t.BillOfMaterialID).HasColumnName("BillOfMaterialsID");
            Property(t => t.ProductAssemblyID).HasColumnName("ProductAssemblyID");
            Property(t => t.ComponentID).HasColumnName("ComponentID");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            Property(t => t.BOMLevel).HasColumnName("BOMLevel");
            Property(t => t.PerAssemblyQty).HasColumnName("PerAssemblyQty");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasOptional(t => t.Product).WithMany().HasForeignKey(t => t.ProductAssemblyID);
            HasRequired(t => t.Product1).WithMany().HasForeignKey(t => t.ComponentID);
            HasRequired(t => t.UnitMeasure).WithMany().HasForeignKey(t => t.UnitMeasureCode);

        }
    }

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
            builder.Property(t => t.StartDate).HasColumnName("StartDate");
            builder.Property(t => t.EndDate).HasColumnName("EndDate");
            builder.Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            builder.Property(t => t.BOMLevel).HasColumnName("BOMLevel");
            builder.Property(t => t.PerAssemblyQty).HasColumnName("PerAssemblyQty");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductAssemblyID);
            builder.HasOne(t => t.Product1).WithMany().HasForeignKey(t => t.ComponentID);
            builder.HasOne(t => t.UnitMeasure).WithMany().HasForeignKey(t => t.UnitMeasureCode);
        }
    }
}
