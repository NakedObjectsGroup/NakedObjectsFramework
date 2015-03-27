using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class vProductModelInstructionMap : EntityTypeConfiguration<vProductModelInstruction>
    {
        public vProductModelInstructionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ProductModelID, t.Name, t.rowguid, t.ModifiedDate });

            // Properties
            this.Property(t => t.ProductModelID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Step)
                .HasMaxLength(1024);

            // Table & Column Mappings
            this.ToTable("vProductModelInstructions", "Production");
            this.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Instructions).HasColumnName("Instructions");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.SetupHours).HasColumnName("SetupHours");
            this.Property(t => t.MachineHours).HasColumnName("MachineHours");
            this.Property(t => t.LaborHours).HasColumnName("LaborHours");
            this.Property(t => t.LotSize).HasColumnName("LotSize");
            this.Property(t => t.Step).HasColumnName("Step");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
