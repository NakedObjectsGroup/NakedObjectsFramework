using System.Data.Entity.ModelConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class CurrencyRateMap : EntityTypeConfiguration<CurrencyRate> {
        public CurrencyRateMap() {
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.Currency).WithMany().HasForeignKey(t => t.FromCurrencyCode);
            HasRequired(t => t.Currency1).WithMany().HasForeignKey(t => t.ToCurrencyCode);
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<CurrencyRate> builder) {
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
            builder.Property(t => t.CurrencyRateDate).HasColumnName("CurrencyRateDate");
            builder.Property(t => t.FromCurrencyCode).HasColumnName("FromCurrencyCode");
            builder.Property(t => t.ToCurrencyCode).HasColumnName("ToCurrencyCode");
            builder.Property(t => t.AverageRate).HasColumnName("AverageRate");
            builder.Property(t => t.EndOfDayRate).HasColumnName("EndOfDayRate");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            builder.HasOne(t => t.Currency).WithMany().HasForeignKey(t => t.FromCurrencyCode);
            builder.HasOne(t => t.Currency1).WithMany().HasForeignKey(t => t.ToCurrencyCode);
        }
    }
}