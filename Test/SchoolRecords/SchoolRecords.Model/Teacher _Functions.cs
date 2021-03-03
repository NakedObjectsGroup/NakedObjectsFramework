using NakedFunctions;
using System.Collections.Generic;
using System.Linq;

namespace SchoolRecords.Model
{
    public static class Teacher_Functions
    {
        [DisplayAsProperty, MemberOrder(5)]
        [RenderEagerly]
        [TableView(false, "Subject", "YearGroup", "SetName")]
        public static ICollection<TeachingSet> SetsTaught(this Teacher t, IContext context)
        {
            int id = t.Id;
            return context.Instances<TeachingSet>().Where(s => s.Teacher.Id == id).OrderBy(s => s.Subject.Name).ThenBy(s => s.YearGroup).ToList();
        }
    }
}
