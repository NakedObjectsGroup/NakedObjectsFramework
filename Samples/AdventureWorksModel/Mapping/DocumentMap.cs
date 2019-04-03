using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class DocumentMap : EntityTypeConfiguration<Document>
    {
        public DocumentMap()
        {
            // Primary Key
            HasKey(t => t.DocumentID);

            // Properties
            Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(400);

            Property(t => t.FileExtension)
                .IsRequired()
                .HasMaxLength(8);

            Property(t => t.Revision)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(5);

            // Table & Column Mappings
            ToTable("Document", "Production");
            Property(t => t.DocumentID).HasColumnName("DocumentID");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.FileName).HasColumnName("FileName");
            Property(t => t.FileExtension).HasColumnName("FileExtension");
            Property(t => t.Revision).HasColumnName("Revision");
            Property(t => t.ChangeNumber).HasColumnName("ChangeNumber");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.DocumentSummary).HasColumnName("DocumentSummary");
            Property(t => t.Document1).HasColumnName("Document");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
