using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model {
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<EmailAddress> builder)
        {
            builder.HasKey(t => new { t.BusinessEntityID, t.EmailAddressID });

            // Table & Column Mappings
            builder.ToTable("EmailAddress", "Person");
            builder.Property(t => t.mappedEmailAddress1).HasColumnName("EmailAddress");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}