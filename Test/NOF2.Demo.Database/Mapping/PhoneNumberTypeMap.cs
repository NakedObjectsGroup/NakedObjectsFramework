using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model {

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PhoneNumberType> builder)
        {
            builder.HasKey(t => t.PhoneNumberTypeID);

            // Table & Column Mappings
            builder.ToTable("PhoneNumberType", "Person");


            builder.Property(t => t.PhoneNumberTypeID).HasColumnName("PhoneNumberTypeID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}