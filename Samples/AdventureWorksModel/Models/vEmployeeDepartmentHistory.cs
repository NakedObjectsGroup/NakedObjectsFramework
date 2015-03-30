using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class vEmployeeDepartmentHistory
    {
        public int EmployeeID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Shift { get; set; }
        public string Department { get; set; }
        public string GroupName { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}
