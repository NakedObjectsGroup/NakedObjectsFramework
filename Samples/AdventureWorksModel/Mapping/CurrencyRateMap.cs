using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CurrencyRateMap : EntityTypeConfiguration<CurrencyRate>
    {
        public CurrencyRateMap()
        {
            // Primary Key
            HasKey(t => t.CurrencyRateID);

            // Properties
            Property(t => t.FromCurrencyCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            Property(t => t.ToCurrencyCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            ToTable("CurrencyRate", "Sales");
            Property(t => t.CurrencyRateID).HasColumnName("CurrencyRateID");
            Property(t => t.CurrencyRateDate).HasColumnName("CurrencyRateDate");
            Property(t => t.FromCurrencyCode).HasColumnName("FromCurrencyCode");
            Property(t => t.ToCurrencyCode).HasColumnName("ToCurrencyCode");
            Property(t => t.AverageRate).HasColumnName("AverageRate");
            Property(t => t.EndOfDayRate).HasColumnName("EndOfDayRate");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Currency).WithMany().HasForeignKey(t => t.FromCurrencyCode);
            HasRequired(t => t.Currency1).WithMany().HasForeignKey(t => t.ToCurrencyCode);

        }
    }
}
