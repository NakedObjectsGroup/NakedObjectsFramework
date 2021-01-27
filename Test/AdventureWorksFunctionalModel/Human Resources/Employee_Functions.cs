// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFunctions;
using System.Linq;

using AW.Types;
using System.Collections.Generic;
using System;

namespace AW.Functions
{
    public static class Employee_Functions
    {
        public static bool HideLoginID(Employee e, IContext context)
        {
            var userAsEmployee = Employee_MenuFunctions.CurrentUserAsEmployee(context);
            return userAsEmployee != null ? userAsEmployee.LoginID != e.LoginID : true;
        }

        public static IQueryable<Employee> ColleaguesInSameDept(
            this Employee e, IContext context)
        {
            var allCurrent = context.Instances<EmployeeDepartmentHistory>().Where(edh => edh.EndDate == null);
            var thisId = e.BusinessEntityID;
            var thisDeptId = allCurrent.Single(edh => edh.EmployeeID == thisId).DepartmentID;
            return allCurrent.Where(edh => edh.DepartmentID == thisDeptId).Select(edh => edh.Employee);
        }

        [MemberOrder(10)]
        public static (EmployeePayHistory, IContext) ChangePayRate(Employee e, IContext context)
        {
            EmployeePayHistory current = CurrentEmployeePayHistory(e);
            var eph = new EmployeePayHistory() { Employee = e, RateChangeDate = context.Now(), PayFrequency = current.PayFrequency };
            return context.SaveAndDisplay(eph);
        }

        public static EmployeePayHistory CurrentEmployeePayHistory(Employee e) => e.PayHistory.OrderByDescending(x => x.RateChangeDate).FirstOrDefault();

        #region ChangeDepartmentOrShift
        [MemberOrder(20)]
        public static (Employee, IContext) ChangeDepartmentOrShift(
           this Employee e, Department department, Shift shift, IContext context)
        {
            var edh = CurrentAssignment(e) with { 
                EndDate = context.Now(), 
                ModifiedDate = context.Now()
            };
            var newAssignment = new EmployeeDepartmentHistory(){
                EmployeeID = e.BusinessEntityID,
                DepartmentID = department.DepartmentID,
                ShiftID = shift.ShiftID, 
                StartDate = context.Today(),
                ModifiedDate = context.Today()
            };
            return (e, context.WithPendingSave(edh, newAssignment));
        }

        public static Department Default1ChangeDepartmentOrShift(this Employee e)
        {
            EmployeeDepartmentHistory current = CurrentAssignment(e);
            return current != null ? current.Department : null;
        }

        public static Shift Default2ChangeDepartmentOrShift(this Employee e)
        {
            EmployeeDepartmentHistory current = CurrentAssignment(e);
            return current != null ? current.Shift : null;
        }

        private static EmployeeDepartmentHistory CurrentAssignment(Employee e)
        {
            return e.DepartmentHistory.Where(n => n.EndDate == null).FirstOrDefault();
        }

        #endregion

        #region Edit Properties
        internal static (Employee, IContext) UpdateEmployee(Employee e, IContext context) =>
            context.SaveAndDisplay(e with { ModifiedDate = context.Now() });

        [Edit]
        public static (Employee, IContext) UpdateNationalIDNumber(this Employee e, 
            [MaxLength(15)] string nationalIdNumber, IContext context) =>
                UpdateEmployee(e with { NationalIDNumber = nationalIdNumber }, context);

        [Edit]
        public static (Employee, IContext) UpdateLoginID(this Employee e,
             [MaxLength(256)] string loginID, IContext context) =>
                UpdateEmployee(e with { LoginID = loginID }, context);

        [Edit]
        public static (Employee, IContext) UpdateJobTitle(this Employee e,
            [MaxLength(50)] string jobTitle, IContext context) =>
                UpdateEmployee(e with { JobTitle = jobTitle }, context);

        [Edit]
        public static (Employee, IContext) UpdateDateOfBirth(this Employee e,
             DateTime dateOfBirth, IContext context) =>
                UpdateEmployee(e with { DateOfBirth = dateOfBirth }, context);

        public static string ValidateUpdateDateOfBirth(this Employee e,
            DateTime dob, IContext context) => ValidateDateOfBirth(dob, context);

        internal static string ValidateDateOfBirth(DateTime dob, IContext context) =>
            (dob > context.Today().AddYears(-16)) || (dob < context.Today().AddYears(-100)) ? "Invalid Date Of Birth" : null;

        [Edit]
        public static (Employee, IContext) UpdateMaritalStatus(this Employee e, 
            string maritalStatus, IContext context) =>
                UpdateEmployee(e with { MaritalStatus = maritalStatus }, context);

        public static IList<string> Choices1UpdateMaritalStatus(this Employee e) => MaritalStatuses;

        internal static string[] MaritalStatuses = new[] { "S", "M" };

        [Edit]
        public static (Employee, IContext) UpdateGender(
            this Employee e, string gender, IContext context) =>
                UpdateEmployee(e with { Gender = gender }, context);

        public static IList<string> Choices1UpdateGender(this Employee e) => Genders;
            
        internal static string[] Genders = new[] { "M", "F" };

        [Edit]
        public static (Employee, IContext) UpdateHireDate(this Employee e,
             DateTime hireDate, IContext context) =>
                UpdateEmployee(e with { HireDate = hireDate }, context);

        [Edit]
        public static (Employee, IContext) UpdateSalaried(this Employee e,
             bool salaried, IContext context) =>
                UpdateEmployee(e with { Salaried = salaried }, context);

        [Edit]
        public static (Employee, IContext) UpdateVacationHours(this Employee e,
            short vacationHours, IContext context) =>
                UpdateEmployee(e with { VacationHours = vacationHours}, context);

        [Edit]
        public static (Employee, IContext) UpdateSickLeaveHours(this Employee e,
            short sickLeaveHours, IContext context) =>
                UpdateEmployee(e with { SickLeaveHours = sickLeaveHours}, context);


        [Edit]
        public static (Employee, IContext) UpdateCurrent(this Employee e,
             bool current, IContext context) =>
                UpdateEmployee(e with { Current = current}, context);

        [Edit]
        public static (Employee, IContext) UpdateManager(this Employee e, Employee manager, IContext context) =>
         UpdateEmployee(e with { Manager = manager }, context);

        [PageSize(20)]
        public static IQueryable<Employee> AutoComplete1UpdateManager(
             this Employee e, [MinLength(2)] string name, IContext context) =>
             Employee_MenuFunctions.FindEmployeeByName(null, name, context);

        #endregion
    }
}