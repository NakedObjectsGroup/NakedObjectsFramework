using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<CurrencyRate> builder)
        {
            builder.HasKey(t => t.CurrencyRateID);

            // Properties
            builder.Property(t => t.FromCurrencyCode)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(3);

            builder.Property(t => t.ToCurrencyCode)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(3);

            // Table & Column Mappings
            builder.ToTable("CurrencyRate", "Sales");
            builder.Property(t => t.CurrencyRateID).HasColumnName("CurrencyRateID");
            builder.Property(t => t.mappedCurrencyRateDate).HasColumnName("CurrencyRateDate");
            builder.Property(t => t.FromCurrencyCode).HasColumnName("FromCurrencyCode");
            builder.Property(t => t.ToCurrencyCode).HasColumnName("ToCurrencyCode");
            builder.Property(t => t.mappedAverageRate).HasColumnName("AverageRate");
            builder.Property(t => t.mappedEndOfDayRate).HasColumnName("EndOfDayRate");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Currency).WithMany().HasForeignKey(t => t.FromCurrencyCode);
            builder.HasOne(t => t.Currency1).WithMany().HasForeignKey(t => t.ToCurrencyCode);
        }
    }
}
