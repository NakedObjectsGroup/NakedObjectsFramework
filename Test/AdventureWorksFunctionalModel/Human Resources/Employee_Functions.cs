






using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class Employee_Functions {
        public static bool HideLoginID(Employee e, IContext context) {
            var userAsEmployee = Employee_MenuFunctions.CurrentUserAsEmployee(context);
            return userAsEmployee != null ? userAsEmployee.LoginID != e.LoginID : true;
        }

        public static IQueryable<Employee> ColleaguesInSameDept(
            this Employee e, IContext context) {
            var allCurrent = context.Instances<EmployeeDepartmentHistory>().Where(edh => edh.EndDate == null);
            var thisId = e.BusinessEntityID;
            var thisDeptId = allCurrent.Single(edh => edh.EmployeeID == thisId).DepartmentID;
            return allCurrent.Where(edh => edh.DepartmentID == thisDeptId).Select(edh => edh.Employee);
        }

        [MemberOrder(10)]
        public static (EmployeePayHistory, IContext) ChangePayRate(Employee e, IContext context) {
            var current = CurrentEmployeePayHistory(e);
            var eph = new EmployeePayHistory { Employee = e, RateChangeDate = context.Now(), PayFrequency = current.PayFrequency };
            return (eph, context.WithNew(eph));
        }

        public static EmployeePayHistory CurrentEmployeePayHistory(Employee e) => e.PayHistory.OrderByDescending(x => x.RateChangeDate).First();

        #region ChangeDepartmentOrShift

        [MemberOrder(20)]
        public static IContext ChangeDepartmentOrShift(
            this Employee e, Department department, Shift shift, IContext context) {
            var currentAssignment = CurrentAssignment(e);
            EmployeeDepartmentHistory updatedCA = new(currentAssignment) {
                EndDate = context.Now(),
                ModifiedDate = context.Now()
            };
            var newAssignment = new EmployeeDepartmentHistory {
                EmployeeID = e.BusinessEntityID,
                DepartmentID = department.DepartmentID,
                ShiftID = shift.ShiftID,
                StartDate = context.Today(),
                ModifiedDate = context.Today()
            };
            return context.WithNew(newAssignment).WithUpdated(currentAssignment, updatedCA);
        }

        public static Department? Default1ChangeDepartmentOrShift(this Employee e) => CurrentAssignment(e)?.Department;

        public static Shift? Default2ChangeDepartmentOrShift(this Employee e) => CurrentAssignment(e)?.Shift;

        private static EmployeeDepartmentHistory CurrentAssignment(Employee e) =>
            e.DepartmentHistory.First(n => n.EndDate == null);

        #endregion

        #region Edit Properties

        internal static IContext UpdateEmployee(
            Employee original, Employee updated, IContext context) =>
            context.WithUpdated(original, new(updated) { ModifiedDate = context.Now() });

        [Edit]
        public static IContext UpdateNationalIDNumber(this Employee e,
                                                      [MaxLength(15)] string nationalIdNumber, IContext context) =>
            UpdateEmployee(e, new(e) { NationalIDNumber = nationalIdNumber }, context);

        [Edit]
        public static IContext UpdateLoginID(this Employee e,
                                             [MaxLength(256)] string loginID, IContext context) =>
            UpdateEmployee(e, new(e) { LoginID = loginID }, context);

        [Edit]
        public static IContext UpdateJobTitle(this Employee e,
                                              [MaxLength(50)] string jobTitle, IContext context) =>
            UpdateEmployee(e, new(e) { JobTitle = jobTitle }, context);

        [Edit]
        public static IContext UpdateDateOfBirth(this Employee e,
                                                 DateTime? dateOfBirth, IContext context) =>
            UpdateEmployee(e, new(e) { DateOfBirth = dateOfBirth }, context);

        public static string? ValidateUpdateDateOfBirth(this Employee e,
                                                        DateTime? dateOfBirth, IContext context) =>
            ValidateDateOfBirth(dateOfBirth, context);

        internal static string? ValidateDateOfBirth(DateTime? dateOfBirth, IContext context) =>
            dateOfBirth > context.Today().AddYears(-16) || dateOfBirth < context.Today().AddYears(-100) ? "Invalid Date Of Birth" : null;

        public static IContext UpdateMaritalStatus(this Employee e,
                                                   string maritalStatus, IContext context) =>
            UpdateEmployee(e, new(e) { MaritalStatus = maritalStatus }, context);

        public static IList<string> Choices1UpdateMaritalStatus(this Employee e) => MaritalStatuses;

        internal static string[] MaritalStatuses = { "S", "M" };

        [Edit]
        public static IContext UpdateGender(
            this Employee e, string gender, IContext context) =>
            UpdateEmployee(e, new(e) { Gender = gender }, context);

        public static IList<string> Choices1UpdateGender(this Employee e) => Genders;

        internal static string[] Genders = { "M", "F" };

        [Edit]
        public static IContext UpdateHireDate(this Employee e,
                                              DateTime? hireDate, IContext context) =>
            UpdateEmployee(e, new(e) { HireDate = hireDate }, context);

        [Edit]
        public static IContext UpdateSalaried(this Employee e,
                                              bool salaried, IContext context) =>
            UpdateEmployee(e, new(e) { Salaried = salaried }, context);

        [Edit]
        public static IContext UpdateVacationHours(this Employee e,
                                                   short vacationHours, IContext context) =>
            UpdateEmployee(e, new(e) { VacationHours = vacationHours }, context);

        [Edit]
        public static IContext UpdateSickLeaveHours(this Employee e,
                                                    short sickLeaveHours, IContext context) =>
            UpdateEmployee(e, new(e) { SickLeaveHours = sickLeaveHours }, context);

        [Edit]
        public static IContext UpdateCurrent(this Employee e,
                                             bool current, IContext context) =>
            UpdateEmployee(e, new(e) { Current = current }, context);

        [Edit]
        public static IContext UpdateManager(this Employee e, Employee manager, IContext context) =>
            UpdateEmployee(e, new(e) { Manager = manager }, context);

        [PageSize(20)]
        public static IQueryable<Employee> AutoComplete1UpdateManager(
            this Employee e, [MinLength(2)] string name, IContext context) =>
            Employee_MenuFunctions.FindEmployeeByName(null, name, context);

        #endregion
    }
}