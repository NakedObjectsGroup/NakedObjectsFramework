using System.Data.Entity.ModelConfiguration;
using AdventureWorksModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping {
    public class EmailAddressMap : EntityTypeConfiguration<EmailAddress> {
        public EmailAddressMap() {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.EmailAddressID});

            // Table & Column Mappings
            this.ToTable("EmailAddress", "Person");
            Property(t => t.EmailAddress1).HasColumnName("EmailAddress");
        }
    }
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<EmailAddress> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.EmailAddressID });

            // Table & Column Mappings
            builder.ToTable("EmailAddress", "Person");
            builder.Property(t => t.EmailAddress1).HasColumnName("EmailAddress");
        }
    }
}