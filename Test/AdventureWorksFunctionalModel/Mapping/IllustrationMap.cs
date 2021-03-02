using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Illustration> builder)
        {
            builder.HasKey(t => t.IllustrationID);

            // Properties
            // Table & Column Mappings
            builder.ToTable("Illustration", "Production");
            builder.Property(t => t.IllustrationID).HasColumnName("IllustrationID");
            builder.Property(t => t.Diagram).HasColumnName("Diagram");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
