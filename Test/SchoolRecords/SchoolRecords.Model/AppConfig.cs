using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SchoolRecords.Model
{
    //TODO: Make this look
    public class AppConfig
    {
        public static Type[] Services()
        {
            return new[] {
                    typeof(Student_MenuFunctions),
                    typeof(Teacher_MenuFunctions),
                    typeof(Subject_MenuFunctions),
                    typeof(TeachingSet_MenuFunctions),
                };
        }

        public static IDictionary<string, Type> MainMenus()
        {
            return new Dictionary<string, Type>()
            {
                ["Students"] = typeof(Student_MenuFunctions),
                ["Staff"] = typeof(Teacher_MenuFunctions),
                ["Subjects"] = typeof(Subject_MenuFunctions),
                ["Sets"] = typeof(TeachingSet_MenuFunctions),
            };
        }

        public static DbContext CreateDbContext()
        {
            return new DatabaseContext("OOPRecords");
        }
    }
}
