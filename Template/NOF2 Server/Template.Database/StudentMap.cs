
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Model;

namespace Template.Database
{

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(t => t.Id);

            // Ignores
            builder
                .Ignore(t => t.Container);
            builder.Property(t => t.mappedFullName);
            builder.Property(t => t.mappedCurrentYearGroup);
            builder.HasOne(t => t.PersonalTutor).WithMany().HasForeignKey(t => t.PersonalTutorId);

        }
    }
}
