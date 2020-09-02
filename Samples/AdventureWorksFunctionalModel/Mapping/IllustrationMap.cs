using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class IllustrationMap : EntityTypeConfiguration<Illustration>
    {
        public IllustrationMap()
        {
            // Primary Key
            HasKey(t => t.IllustrationID);

            // Properties
            // Table & Column Mappings
            ToTable("Illustration", "Production");
            Property(t => t.IllustrationID).HasColumnName("IllustrationID");
            Property(t => t.Diagram).HasColumnName("Diagram");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
