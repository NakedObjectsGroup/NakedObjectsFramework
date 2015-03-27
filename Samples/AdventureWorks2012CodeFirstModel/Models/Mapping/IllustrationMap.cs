using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class IllustrationMap : EntityTypeConfiguration<Illustration>
    {
        public IllustrationMap()
        {
            // Primary Key
            this.HasKey(t => t.IllustrationID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Illustration", "Production");
            this.Property(t => t.IllustrationID).HasColumnName("IllustrationID");
            this.Property(t => t.Diagram).HasColumnName("Diagram");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
