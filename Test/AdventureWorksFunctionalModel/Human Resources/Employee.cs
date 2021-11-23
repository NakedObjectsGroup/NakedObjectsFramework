






using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
    public interface IEmployee : IBusinessEntity { } //Interface is for testing purposes

    public class Employee : IEmployee, IHasRowGuid, IHasModifiedDate {

        public Employee() { }

        public Employee(Employee cloneFrom)
        {
            BusinessEntityID = cloneFrom.BusinessEntityID;
            PersonDetails = cloneFrom.PersonDetails;
            NationalIDNumber = cloneFrom.NationalIDNumber;
            JobTitle = cloneFrom.JobTitle;
            DateOfBirth = cloneFrom.DateOfBirth;
            MaritalStatus = cloneFrom.MaritalStatus;
            Gender = cloneFrom.Gender;
            HireDate = cloneFrom.HireDate;
            Salaried = cloneFrom.Salaried;
            VacationHours = cloneFrom.VacationHours;
            SickLeaveHours = cloneFrom.SickLeaveHours;
            Current =   cloneFrom.Current;
            Manager = cloneFrom.Manager;
            LoginID = cloneFrom.LoginID;    
            SalesPerson = cloneFrom.SalesPerson;
            ModifiedDate = cloneFrom.ModifiedDate;
            rowguid = cloneFrom.rowguid;
            DepartmentHistory = cloneFrom.DepartmentHistory;
            PayHistory = cloneFrom.PayHistory;
        }

        [Hidden]
        public int BusinessEntityID { get; init; }

        [MemberOrder(1)]
        public virtual Person PersonDetails { get; init; }

        [MemberOrder(10)]
        public string NationalIDNumber { get; init; } = "";

        [MemberOrder(12)]
        public string JobTitle { get; init; } = "";

        [MemberOrder(13)] [Mask("d")]
        public DateTime? DateOfBirth { get; init; }

        [MemberOrder(14)]
        public string MaritalStatus { get; init; } = "";

        [MemberOrder(15)]
        public string Gender { get; init; } = "";

        [MemberOrder(16)] [Mask("d")]
        public DateTime? HireDate { get; init; }

        [MemberOrder(17)]
        public bool Salaried { get; init; }

        [MemberOrder(18)]
        public short VacationHours { get; init; }

        [MemberOrder(19)]
        public short SickLeaveHours { get; init; }

        [MemberOrder(20)]
        public bool Current { get; init; }

        [MemberOrder(30)]
        public virtual Employee? Manager { get; init; }

        [MemberOrder(11)]
        public string LoginID { get; init; } = "";

        [Hidden]
        public virtual SalesPerson? SalesPerson { get; init; }

        [TableView(true,
                   nameof(EmployeeDepartmentHistory.StartDate),
                   nameof(EmployeeDepartmentHistory.EndDate),
                   nameof(EmployeeDepartmentHistory.Department),
                   nameof(EmployeeDepartmentHistory.Shift))]
        public virtual ICollection<EmployeeDepartmentHistory> DepartmentHistory { get; init; } = new List<EmployeeDepartmentHistory>();

        [TableView(true,
                   nameof(EmployeePayHistory.RateChangeDate),
                   nameof(EmployeePayHistory.Rate))]
        public virtual ICollection<EmployeePayHistory> PayHistory { get; init; } = new List<EmployeePayHistory>();


        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => $"{PersonDetails}";


    }
}