using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class ErrorLogMap : EntityTypeConfiguration<ErrorLog>
    {
        public ErrorLogMap()
        {
            // Primary Key
            this.HasKey(t => t.ErrorLogID);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.ErrorProcedure)
                .HasMaxLength(126);

            this.Property(t => t.ErrorMessage)
                .IsRequired()
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("ErrorLog");
            this.Property(t => t.ErrorLogID).HasColumnName("ErrorLogID");
            this.Property(t => t.ErrorTime).HasColumnName("ErrorTime");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.ErrorNumber).HasColumnName("ErrorNumber");
            this.Property(t => t.ErrorSeverity).HasColumnName("ErrorSeverity");
            this.Property(t => t.ErrorState).HasColumnName("ErrorState");
            this.Property(t => t.ErrorProcedure).HasColumnName("ErrorProcedure");
            this.Property(t => t.ErrorLine).HasColumnName("ErrorLine");
            this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");
        }
    }
}
