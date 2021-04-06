using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class ShiftMap : EntityTypeConfiguration<Shift>
    {
        public ShiftMap()
        {
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
            Property(t => t.Times.StartTime).HasColumnName("StartTime");
            Property(t => t.Times.EndTime).HasColumnName("EndTime");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Shift> builder)
        {
            builder.HasKey(t => t.ShiftID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Shift", "HumanResources");
            builder.Property(t => t.ShiftID).HasColumnName("ShiftID").ValueGeneratedOnAdd();
            builder.Property(t => t.Name).HasColumnName("Name");

            builder.OwnsOne(t => t.Times, tp => {
                tp.Property(a => a.StartTime).HasColumnName("StartTime");
                tp.Property(a => a.EndTime).HasColumnName("EndTime");

            });

            //builder.Property(t => t.Times.StartTime).HasColumnName("StartTime");
            //builder.Property(t => t.Times.EndTime).HasColumnName("EndTime");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
