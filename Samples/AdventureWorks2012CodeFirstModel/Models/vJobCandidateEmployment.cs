using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class vJobCandidateEmployment
    {
        public int JobCandidateID { get; set; }
        public Nullable<System.DateTime> Emp_StartDate { get; set; }
        public Nullable<System.DateTime> Emp_EndDate { get; set; }
        public string Emp_OrgName { get; set; }
        public string Emp_JobTitle { get; set; }
        public string Emp_Responsibility { get; set; }
        public string Emp_FunctionCategory { get; set; }
        public string Emp_IndustryCategory { get; set; }
        public string Emp_Loc_CountryRegion { get; set; }
        public string Emp_Loc_State { get; set; }
        public string Emp_Loc_City { get; set; }
    }
}
