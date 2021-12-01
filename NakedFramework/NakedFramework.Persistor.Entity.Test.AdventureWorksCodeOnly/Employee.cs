// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly; 

[Table("HumanResources.Employee")]
public class Employee {
    public int EmployeeID { get; set; }

    [Required]
    [StringLength(15)]
    public string NationalIDNumber { get; set; }

    public int ContactID { get; set; }

    [Required]
    [StringLength(256)]
    public string LoginID { get; set; }

    public int? ManagerID { get; set; }

    [Required]
    [StringLength(50)]
    public string Title { get; set; }

    public DateTime BirthDate { get; set; }

    [Required]
    [StringLength(1)]
    public string MaritalStatus { get; set; }

    [Required]
    [StringLength(1)]
    public string Gender { get; set; }

    public DateTime HireDate { get; set; }

    public bool SalariedFlag { get; set; }

    public short VacationHours { get; set; }

    public short SickLeaveHours { get; set; }

    public bool CurrentFlag { get; set; }

    public Guid rowguid { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual Contact Contact { get; set; }

    public virtual ICollection<Employee> Employee1 { get; set; } = new HashSet<Employee>();

    public virtual Employee Employee2 { get; set; }

    public virtual ICollection<EmployeeAddress> EmployeeAddresses { get; set; } = new HashSet<EmployeeAddress>();

    public virtual ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistories { get; set; } = new HashSet<EmployeeDepartmentHistory>();

    public virtual ICollection<EmployeePayHistory> EmployeePayHistories { get; set; } = new HashSet<EmployeePayHistory>();

    public virtual ICollection<JobCandidate> JobCandidates { get; set; } = new HashSet<JobCandidate>();

    public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; } = new HashSet<PurchaseOrderHeader>();

    public virtual SalesPerson SalesPerson { get; set; }
}