using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasOptional(t => t.Product).WithMany().HasForeignKey(t => t.ProductAssemblyID);
            HasRequired(t => t.Product1).WithMany().HasForeignKey(t => t.ComponentID);
            HasRequired(t => t.UnitMeasure).WithMany().HasForeignKey(t => t.UnitMeasureCode);

        }
    }
}
