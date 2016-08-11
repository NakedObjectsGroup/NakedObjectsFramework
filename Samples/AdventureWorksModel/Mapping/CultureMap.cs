using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CultureMap : EntityTypeConfiguration<Culture>
    {
        public CultureMap()
        {
            // Primary Key
            HasKey(t => t.CultureID);

            // Properties
            Property(t => t.CultureID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Culture", "Production");
            Property(t => t.CultureID).HasColumnName("CultureID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
