using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class ShiftMap : EntityTypeConfiguration<Shift> {
        public ShiftMap() {
            // Primary Key
            HasKey(t => t.ShiftID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Shift", "HumanResources");
            Property(t => t.ShiftID).HasColumnName("ShiftID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.StartTime).HasColumnName("StartTime");
            Property(t => t.EndTime).HasColumnName("EndTime");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<Shift> builder) {
            builder.HasKey(t => t.ShiftID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Shift", "HumanResources");
            builder.Property(t => t.ShiftID).HasColumnName("ShiftID").ValueGeneratedOnAdd();
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.StartTime).HasColumnName("StartTime");
            builder.Property(t => t.EndTime).HasColumnName("EndTime");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();
        }
    }
}