using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class BillOfMaterialMap : EntityTypeConfiguration<BillOfMaterial>
    {
        public BillOfMaterialMap()
        {
            // Primary Key
            this.HasKey(t => t.BillOfMaterialID);

            // Properties
            this.Property(t => t.UnitMeasureCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("BillOfMaterials", "Production");
            this.Property(t => t.BillOfMaterialID).HasColumnName("BillOfMaterialsID");
            this.Property(t => t.ProductAssemblyID).HasColumnName("ProductAssemblyID");
            this.Property(t => t.ComponentID).HasColumnName("ComponentID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            this.Property(t => t.BOMLevel).HasColumnName("BOMLevel");
            this.Property(t => t.PerAssemblyQty).HasColumnName("PerAssemblyQty");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasOptional(t => t.Product).WithMany().HasForeignKey(t => t.ProductAssemblyID);
            this.HasRequired(t => t.Product1).WithMany().HasForeignKey(t => t.ComponentID);
            this.HasRequired(t => t.UnitMeasure).WithMany().HasForeignKey(t => t.UnitMeasureCode);

        }
    }
}
