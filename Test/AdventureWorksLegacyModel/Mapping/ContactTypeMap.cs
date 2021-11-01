using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ContactType> builder)
        {
            builder.HasKey(t => t.ContactTypeID);

            // Ignores
            builder.Ignore(t => t.Name);


            // Table & Column Mappings
            builder.ToTable("ContactType", "Person");
            builder.Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            builder.Property(t => t.mappedName).HasColumnName("Name").IsRequired().HasMaxLength(50);
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
