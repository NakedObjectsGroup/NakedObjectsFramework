// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFramework;
using NakedLegacy.Types;
using NakedObjects;

namespace AdventureWorksModel
{
    public interface IEmployee : IBusinessEntity { } //Interface is for testing purposes

    [LegacyType]
    public class Employee : IEmployee, TitledObject
    {
        #region Injected Services
        public EmployeeRepository EmployeeRepository { set; protected get; }
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting()
        {
            rowguid = Guid.NewGuid();
            ModifiedDate.DateTime = DateTime.Now;
        }

        public virtual void Updating() => ModifiedDate.DateTime = DateTime.Now;
        #endregion

        #region Title

        public Title Title() => new Title(PersonDetails);

        #endregion

        #region Properties

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        [MemberOrder(1), Disabled]
        public virtual Person PersonDetails { get; set; }

        #region NationalIDNumber (Legacy Property)
        internal string mappedNationalIDNumber;
        private TextString myNationalIDNumber;

        [MemberOrder(10)]
        public virtual TextString NationalIDNumber => myNationalIDNumber ??= new TextString(mappedNationalIDNumber, v => mappedNationalIDNumber = v);
        #endregion

        #region JobTitle (Legacy Property)
        internal string mappedJobTitle;
        private TextString myJobTitle;

        [MemberOrder(12)]
        public virtual TextString JobTitle => myJobTitle ??= new TextString(mappedJobTitle, v => mappedJobTitle = v);
        #endregion

        #region DateOfBirth (Legacy Property)
        internal DateTime? mappedDateOfBirth;
        private Date myDateOfBirth;

        [MemberOrder(13)]
        public virtual Date DateOfBirth => myDateOfBirth ??= new Date(mappedDateOfBirth.GetValueOrDefault(), v => mappedDateOfBirth = v);
        #endregion

        #region MaritalStatus (Legacy Property)
        internal string mappedMaritalStatus;
        private TextString myMaritalStatus;

        [MemberOrder(14)]
        [StringLength(1)]
        public virtual TextString MaritalStatus => myMaritalStatus ??= new TextString(mappedMaritalStatus, v => mappedMaritalStatus = v);

        public void AboutMaritalStatus(FieldAbout a, TextString newMaritalStatus)
        {
            switch (a.TypeCode)
            {
                case AboutTypeCodes.Name:
                    break;
                case AboutTypeCodes.Parameters:
                    a.Options = (object[])ChoicesMaritalStatus();
                    break;
                case AboutTypeCodes.Usable:
                    break;
                case AboutTypeCodes.Valid:
                    //var msg = newMaritalStatus is null || newMaritalStatus.Text.Length != 1 ? "Invalid" : "";
                    //a.InvalidReason += msg;
                    //a.inv = a.InvalidReason | (msg != "");
                    break;
                case AboutTypeCodes.Visible:
                    break;
                default:
                    break;
            }
        }

        public IList<string> ChoicesMaritalStatus()
        {
            return new[] { "S", "M" };
        }
        #endregion

        #region Gender (Legacy Property)
        internal string mappedGender;
        private TextString myGender;

        [MemberOrder(15)]
        public virtual TextString Gender => myGender ??= new TextString(mappedGender, v => mappedGender = v);

        public void AboutGender(FieldAbout a, TextString newGender)
        {
            switch (a.TypeCode)
            {
                case AboutTypeCodes.Name:
                    break;
                case AboutTypeCodes.Parameters:
                    a.Options = (object[])ChoicesGender();
                    break;
                case AboutTypeCodes.Usable:
                    break;
                case AboutTypeCodes.Valid:
                    break;
                case AboutTypeCodes.Visible:
                    break;
                default:
                    break;
            }
        }

        public IList<string> ChoicesGender()
        {
            return new[] { "M", "F" };
        }
        #endregion

        #region HireDate (Legacy Property)
        internal DateTime? mappedHireDate;
        private Date myHireDate;

        [MemberOrder(16)]
        public virtual Date HireDate => myHireDate ??= new Date(mappedHireDate.GetValueOrDefault(), v => mappedHireDate = v);
        #endregion

        #region Salaried (Legacy Property)
        internal bool mappedSalaried;
        private Logical mySalaried;

        //[MemberOrder(17)] //TODO uncomment when Logical supported
        //public virtual Logical Salaried => mySalaried ??= new Logical(mappedSalaried, v => mappedSalaried = v);
        #endregion

        #region VacationHours (Legacy Property)
        internal short mappedVacationHours;
        private WholeNumber myVacationHours;

        [MemberOrder(18)]
        public virtual WholeNumber VacationHours => myVacationHours ??= new WholeNumber(mappedVacationHours, v => mappedVacationHours = (short)v);
        #endregion

        #region SickLeaveHours (Legacy Property)
        internal short mappedSickLeaveHours;
        private WholeNumber mySickLeaveHours;

        [MemberOrder(19)]
        public virtual WholeNumber SickLeaveHours => mySickLeaveHours ??= new WholeNumber(mappedSickLeaveHours, v => mappedSickLeaveHours = (short) v);
        #endregion

        #region Current (Legacy Property)
        internal bool mappedCurrent;
        private Logical myCurrent;

        //[MemberOrder(20)] //TODO uncomment when Logical supported
        //public virtual Logical Current => myCurrent ??= new Logical(mappedCurrent, v => mappedCurrent = v);
        #endregion

        #region Manager
        [NakedObjectsIgnore]
        public virtual int? ManagerID { get; set; }

        [Optionally]
        [MemberOrder(30)]
        public virtual Employee Manager { get; set; }

        [PageSize(20)]
        public IQueryable<Employee> AutoCompleteManager([MinLength(2)] string name)
        {
            return EmployeeRepository.FindEmployeeByName(null, name);
        }

        #endregion

        #region LoginID (Legacy Property)
        internal string mappedLoginID;
        private TextString myLoginID;

        [MemberOrder(11)]
        public virtual TextString LoginID => myLoginID ??= new TextString(mappedLoginID, v => mappedLoginID = v);

        public void AboutLoginID(FieldAbout a, TextString newLoginID)
        {
            switch (a.TypeCode)
            {
                case AboutTypeCodes.Name:
                    break;
                case AboutTypeCodes.Parameters:
                    break;
                case AboutTypeCodes.Usable:
                    break;
                case AboutTypeCodes.Valid:
                    break;
                case AboutTypeCodes.Visible:
                    //a.Visible = !HideLoginID(); But logic needs converting
                    break;
                default:
                    break;
            }
        }

        public virtual bool HideLoginID()
        {
            if (Container.IsPersistent(this))
            {
                Employee userAsEmployee = EmployeeRepository.CurrentUserAsEmployee();
                return userAsEmployee != null ? userAsEmployee.LoginID != LoginID : true;
            }
            return false;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual SalesPerson SalesPerson { get; set; }

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate
        internal DateTime mappedModifiedDate;
        private TimeStamp myModifiedDate;

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual TimeStamp ModifiedDate => myModifiedDate ??= new TimeStamp(mappedModifiedDate, s => mappedModifiedDate = s);
        #endregion

        #endregion

        #region DepartmentHistory (Legacy Collection)
        public virtual ICollection<EmployeeDepartmentHistory> mappedDepartmentHistory { get; } = new List<EmployeeDepartmentHistory>();
        private InternalCollection myDepartmentHistory;

        [TableView(true,
    nameof(EmployeeDepartmentHistory.StartDate),
    nameof(EmployeeDepartmentHistory.EndDate),
    nameof(EmployeeDepartmentHistory.Department),
    nameof(EmployeeDepartmentHistory.Shift))]
        [MemberOrder(1)]
        public InternalCollection DepartmentHistory => myDepartmentHistory ??= new InternalCollection<EmployeeDepartmentHistory>(mappedDepartmentHistory);
        #endregion

        #region PayHistory (Legacy Collection)
        public virtual ICollection<EmployeePayHistory> mappedPayHistory { get; } = new List<EmployeePayHistory>();
        private InternalCollection myPayHistory;

        [TableView(true,
    nameof(EmployeePayHistory.RateChangeDate),
    nameof(EmployeePayHistory.Rate))]
        [MemberOrder(1)]
        public InternalCollection PayHistory => myPayHistory ??= new InternalCollection<EmployeePayHistory>(mappedPayHistory);
        #endregion

        #region Actions

        #region ChangePayRate (Action)

        [MemberOrder(10)]
        public EmployeePayHistory ActionChangePayRate()
        {
            EmployeePayHistory current = CurrentEmployeePayHistory();
            var eph = Container.NewTransientInstance<EmployeePayHistory>();
            eph.Employee = this;
            eph.RateChangeDate = DateTime.Now.Date;
            eph.PayFrequency = current.PayFrequency;
            return eph;
        }

        private EmployeePayHistory CurrentEmployeePayHistory()
        {
            var query = from obj in mappedPayHistory
                        orderby obj.RateChangeDate descending
                        select obj;
            return query.FirstOrDefault();
        }

        // Use 'hide', 'dis', 'val', 'actdef', 'actcho' shortcuts to add supporting methods here.

        #endregion

        #region ChangeDepartmentOrShift (Action)

        [MemberOrder(20)]
        public void ActionChangeDepartmentOrShift(Department department, [Optionally] Shift shift)
        {
            CurrentAssignment().EndDate = DateTime.Now;
            var newAssignment = Container.NewTransientInstance<EmployeeDepartmentHistory>();
            newAssignment.Department = department;
            newAssignment.Shift = shift;
            newAssignment.Employee = this;
            newAssignment.StartDate = DateTime.Now;
            Container.Persist(ref newAssignment);
            DepartmentHistory.Add(newAssignment);
        }

        public Department Default0ChangeDepartmentOrShift()
        {
            EmployeeDepartmentHistory current = CurrentAssignment();
            return current != null ? current.Department : null;
        }

        private EmployeeDepartmentHistory CurrentAssignment() => mappedDepartmentHistory.Where(n => n.EndDate == null).FirstOrDefault();
         #endregion

        public void SpecifyManagerAction(IEmployee manager)
        {
            this.ManagerID = manager.BusinessEntityID;
        }

        public IQueryable<Employee> ActionColleaguesInSameDept()
        {
            var allCurrent = Container.Instances<EmployeeDepartmentHistory>().Where(edh => edh.EndDate == null);
            var thisId = this.BusinessEntityID;
            var thisDeptId = allCurrent.Single(edh => edh.EmployeeID == thisId).DepartmentID;
            return allCurrent.Where(edh => edh.DepartmentID == thisDeptId).Select(edh => edh.Employee);
        }
        #endregion
    }
}