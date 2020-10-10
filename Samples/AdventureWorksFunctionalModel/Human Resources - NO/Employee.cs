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
using NakedObjects;

namespace AdventureWorksModel {

    public interface IEmployee : IBusinessEntity { } //Interface is for testing purposes
    [IconName("person.png")]
    public class Employee : IEmployee {
        #region Injected Services
        public EmployeeRepository EmployeeRepository { set; protected get; }
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion
        
        #region Title & Icon

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(PersonDetails);
            return t.ToString();
        }

        #endregion

        #region Properties

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

         [MemberOrder(1), Disabled]
        public virtual Person PersonDetails { get; set; }

        [MemberOrder(10)]
        public virtual string NationalIDNumber { get; set; }

        [MemberOrder(12)]
        public virtual string JobTitle { get; set; }

        [MemberOrder(13)]
        [Mask("d")]
        public virtual DateTime? DateOfBirth { get; set; }

        [MemberOrder(14)]
        [StringLength(1)]
        public virtual string MaritalStatus { get; set; }

        public IList<string> ChoicesMaritalStatus() {
            return new[] {"S", "M"};
        }

        [MemberOrder(15)]
        [StringLength(1)]
        public virtual string Gender { get; set; }

        public IList<string> ChoicesGender() {
            return new[] {"M", "F"};
        }

        [MemberOrder(16)]
        [Mask("d")]
        public virtual DateTime? HireDate { get; set; }

        [MemberOrder(17)]
        public virtual bool Salaried { get; set; }

        [MemberOrder(18)]
        public virtual short VacationHours { get; set; }

        [MemberOrder(19)]
        public virtual short SickLeaveHours { get; set; }

        [MemberOrder(20)]
        public virtual bool Current { get; set; }

        #region Manager
        [NakedObjectsIgnore]
        public virtual int? ManagerID { get; set; }

        [Optionally]
        [MemberOrder(30)]
        public virtual Employee Manager { get; set; }

        [PageSize(20)]
        public IQueryable<Employee> AutoCompleteManager([MinLength(2)] string name) {
            return EmployeeRepository.FindEmployeeByName(null, name);
        }

        #endregion

        #region LoginID

        [MemberOrder(11)]
        public virtual string LoginID { get; set; }

        [Executed(Where.Remotely)]
        public virtual bool HideLoginID() {
            if (Container.IsPersistent(this)) {
                Employee userAsEmployee = EmployeeRepository.CurrentUserAsEmployee();
                return userAsEmployee != null ? userAsEmployee.LoginID != LoginID : true;
            }
            return false;
        }

        #endregion

        [NakedObjectsIgnore]
        public virtual SalesPerson SalesPerson { get; set; }
      
        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region collections
        private ICollection<EmployeeDepartmentHistory> _departmentHistory = new List<EmployeeDepartmentHistory>();

        [TableView(true, 
            nameof(EmployeeDepartmentHistory.StartDate),
            nameof(EmployeeDepartmentHistory.EndDate),
            nameof(EmployeeDepartmentHistory.Department),
            nameof(EmployeeDepartmentHistory.Shift))]
        public virtual ICollection<EmployeeDepartmentHistory> DepartmentHistory {
            get { return _departmentHistory; }
            set { _departmentHistory = value; }
        }

        private ICollection<EmployeePayHistory> _payHistory = new List<EmployeePayHistory>();

        [TableView(true, 
            nameof(EmployeePayHistory.RateChangeDate),
            nameof(EmployeePayHistory.Rate))]
        public virtual ICollection<EmployeePayHistory> PayHistory {
            get { return _payHistory; }
            set { _payHistory = value; }
        }
        #endregion

        #region Actions

        #region ChangePayRate (Action)

        [MemberOrder(10)]
        public EmployeePayHistory ChangePayRate() {
            EmployeePayHistory current = CurrentEmployeePayHistory();
            var eph = Container.NewTransientInstance<EmployeePayHistory>();
            eph.Employee = this;
            eph.RateChangeDate = DateTime.Now.Date;
            eph.PayFrequency = current.PayFrequency;
            return eph;
        }

        private EmployeePayHistory CurrentEmployeePayHistory() {
            var query = from obj in PayHistory
                        orderby obj.RateChangeDate descending
                        select obj;
            return query.FirstOrDefault();
        }

        // Use 'hide', 'dis', 'val', 'actdef', 'actcho' shortcuts to add supporting methods here.

        #endregion

        #region ChangeDepartmentOrShift (Action)

        [MemberOrder(20)]
        public void ChangeDepartmentOrShift(Department department, [Optionally] Shift shift) {
            CurrentAssignment().EndDate = DateTime.Now;
            var newAssignment = Container.NewTransientInstance<EmployeeDepartmentHistory>();
            newAssignment.Department = department;
            newAssignment.Shift = shift;
            newAssignment.Employee = this;
            newAssignment.StartDate = DateTime.Now;
            Container.Persist(ref newAssignment);
            DepartmentHistory.Add(newAssignment);
        }

        public Department Default0ChangeDepartmentOrShift() {
            EmployeeDepartmentHistory current = CurrentAssignment();
            return current != null ? current.Department : null;
        }

        private EmployeeDepartmentHistory CurrentAssignment() {
            return DepartmentHistory.Where(n => n.EndDate == null).FirstOrDefault();
        }

        #endregion

        public void SpecifyManager(IEmployee manager)
        {
            this.ManagerID = manager.BusinessEntityID;
        }

        public IQueryable<Employee> ColleaguesInSameDept()
        {
            var allCurrent = Container.Instances<EmployeeDepartmentHistory>().Where(edh => edh.EndDate == null);
            var thisId = this.BusinessEntityID;
            var thisDeptId = allCurrent.Single(edh => edh.EmployeeID == thisId).DepartmentID;
            return allCurrent.Where(edh => edh.DepartmentID == thisDeptId).Select(edh => edh.Employee);
        }
        #endregion
    }
}