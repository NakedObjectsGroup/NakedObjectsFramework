using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class DocumentMap : EntityTypeConfiguration<Document>
    {
        public DocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentID);

            // Properties
            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.FileExtension)
                .IsRequired()
                .HasMaxLength(8);

            this.Property(t => t.Revision)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("Document", "Production");
            this.Property(t => t.DocumentID).HasColumnName("DocumentID");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.FileExtension).HasColumnName("FileExtension");
            this.Property(t => t.Revision).HasColumnName("Revision");
            this.Property(t => t.ChangeNumber).HasColumnName("ChangeNumber");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.DocumentSummary).HasColumnName("DocumentSummary");
            this.Property(t => t.Document1).HasColumnName("Document");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
