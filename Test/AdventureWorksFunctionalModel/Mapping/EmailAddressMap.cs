using System.Data.Entity.ModelConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class EmailAddressMap : EntityTypeConfiguration<EmailAddress> {
        public EmailAddressMap() {
            // Primary Key
            HasKey(t => new { t.BusinessEntityID, t.EmailAddressID });

            // Table & Column Mappings
            ToTable("EmailAddress", "Person");
            Property(t => t.EmailAddress1).HasColumnName("EmailAddress");
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<EmailAddress> builder) {
            builder.HasKey(t => new { t.BusinessEntityID, t.EmailAddressID });

            // Table & Column Mappings
            builder.ToTable("EmailAddress", "Person");
            builder.Property(t => t.EmailAddress1).HasColumnName("EmailAddress");
        }
    }
}