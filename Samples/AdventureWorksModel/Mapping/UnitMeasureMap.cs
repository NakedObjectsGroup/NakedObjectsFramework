using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class UnitMeasureMap : EntityTypeConfiguration<UnitMeasure>
    {
        public UnitMeasureMap()
        {
            // Primary Key
            HasKey(t => t.UnitMeasureCode);

            // Properties
            Property(t => t.UnitMeasureCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("UnitMeasure", "Production");
            Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
