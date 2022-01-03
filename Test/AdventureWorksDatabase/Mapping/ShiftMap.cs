using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{


    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Shift> builder)
        {
            builder.HasKey(t => t.ShiftID);

            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Shift", "HumanResources");
            builder.Property(t => t.ShiftID).HasColumnName("ShiftID").ValueGeneratedOnAdd();
            builder.Property(t => t.mappedName).HasColumnName("Name");

            ////builder.OwnsOne(t => t.mappedTimes, tp => {
            ////    tp.Property(a => a.StartTime).HasColumnName("StartTime");
            ////    tp.Property(a => a.EndTime).HasColumnName("EndTime");

            ////});

            //builder.Property(t => t.Times.StartTime).HasColumnName("StartTime");
            //builder.Property(t => t.Times.EndTime).HasColumnName("EndTime");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
