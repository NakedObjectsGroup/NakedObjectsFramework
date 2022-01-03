using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel {

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<PhoneNumberType> builder)
        {
            builder.HasKey(t => t.PhoneNumberTypeID);

            // Table & Column Mappings
            builder.ToTable("PhoneNumberType", "Person");
        }
    }
}