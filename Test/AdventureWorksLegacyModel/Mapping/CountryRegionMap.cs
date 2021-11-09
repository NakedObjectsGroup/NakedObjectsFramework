using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<CountryRegion> builder)
        {
            builder.HasKey(t => t.mappedCountryRegionCode);

            builder.Ignore(t => t.CountryRegionCode).Ignore(t => t.Name);
                  

            // Table & Column Mappings
            builder.ToTable("CountryRegion", "Person");
            builder.Property(t => t.mappedCountryRegionCode).HasColumnName("CountryRegionCode");
            builder.Property(t => t.mappedName).HasColumnName("Name").IsRequired().HasMaxLength(50); ;
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
