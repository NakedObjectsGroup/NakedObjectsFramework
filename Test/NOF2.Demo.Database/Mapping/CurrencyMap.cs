using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(t => t.CurrencyCode);

            // Properties
            builder.Property(t => t.CurrencyCode)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(3);

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Currency", "Sales");
            builder.Property(t => t.CurrencyCode).HasColumnName("CurrencyCode");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
