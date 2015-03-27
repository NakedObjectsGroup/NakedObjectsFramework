using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class Employee
    {
        public Employee()
        {
            this.Employee1 = new List<Employee>();
            this.EmployeeAddresses = new List<EmployeeAddress>();
            this.EmployeeDepartmentHistories = new List<EmployeeDepartmentHistory>();
            this.EmployeePayHistories = new List<EmployeePayHistory>();
            this.JobCandidates = new List<JobCandidate>();
            this.PurchaseOrderHeaders = new List<PurchaseOrderHeader>();
        }

        public int EmployeeID { get; set; }
        public string NationalIDNumber { get; set; }
        public int ContactID { get; set; }
        public string LoginID { get; set; }
        public Nullable<int> ManagerID { get; set; }
        public string Title { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string MaritalStatus { get; set; }
        public string Gender { get; set; }
        public System.DateTime HireDate { get; set; }
        public bool SalariedFlag { get; set; }
        public short VacationHours { get; set; }
        public short SickLeaveHours { get; set; }
        public bool CurrentFlag { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ICollection<Employee> Employee1 { get; set; }
        public virtual Employee Employee2 { get; set; }
        public virtual ICollection<EmployeeAddress> EmployeeAddresses { get; set; }
        public virtual ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistories { get; set; }
        public virtual ICollection<EmployeePayHistory> EmployeePayHistories { get; set; }
        public virtual ICollection<JobCandidate> JobCandidates { get; set; }
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public virtual SalesPerson SalesPerson { get; set; }
    }
}
