using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Illustration> builder)
        {
            builder.HasKey(t => t.IllustrationID);

            // Properties
            // Table & Column Mappings
            builder.ToTable("Illustration", "Production");
            builder.Property(t => t.IllustrationID).HasColumnName("IllustrationID");
            builder.Property(t => t.mappedDiagram).HasColumnName("Diagram");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
