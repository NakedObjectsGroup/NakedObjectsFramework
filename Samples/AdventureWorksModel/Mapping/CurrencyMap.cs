using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CurrencyMap : EntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            // Primary Key
            HasKey(t => t.CurrencyCode);

            // Properties
            Property(t => t.CurrencyCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Currency", "Sales");
            Property(t => t.CurrencyCode).HasColumnName("CurrencyCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
