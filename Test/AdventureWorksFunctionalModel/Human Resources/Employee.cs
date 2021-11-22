// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
    public interface IEmployee : IBusinessEntity { } //Interface is for testing purposes

    public record Employee : IEmployee, IHasRowGuid, IHasModifiedDate {
        [MemberOrder(1)]
#pragma warning disable 8618
        public virtual Person PersonDetails { get; init; }
#pragma warning restore 8618

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

        [Hidden]
        public int BusinessEntityID { get; init; }

        public virtual bool Equals(Employee? other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => $"{PersonDetails}";

        public override int GetHashCode() => base.GetHashCode();
    }
}