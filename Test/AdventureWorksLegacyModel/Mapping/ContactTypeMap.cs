using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class ContactTypeMap : EntityTypeConfiguration<ContactType>
    {
        public ContactTypeMap()
        {
            // Primary Key
            HasKey(t => t.ContactTypeID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ContactType", "Person");
            Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ContactType> builder)
        {
            builder.HasKey(t => t.ContactTypeID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ContactType", "Person");
            builder.Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
