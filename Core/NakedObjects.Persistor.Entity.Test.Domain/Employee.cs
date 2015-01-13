using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Employee
    {
    
        #region Primitive Properties
        #region EmployeeID (Int32)
    [MemberOrder(100)]
        public virtual int  EmployeeID {get; set;}

        #endregion

        #region NationalIDNumber (String)
    [MemberOrder(110), StringLength(15)]
        public virtual string  NationalIDNumber {get; set;}

        #endregion

        #region LoginID (String)
    [MemberOrder(120), StringLength(256)]
        public virtual string  LoginID {get; set;}

        #endregion

        #region Title (String)
    [MemberOrder(130), StringLength(50)]
        public virtual string  Title {get; set;}

        #endregion

        #region BirthDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual DateTime  BirthDate {get; set;}

        #endregion

        #region MaritalStatus (String)
    [MemberOrder(150), StringLength(1)]
        public virtual string  MaritalStatus {get; set;}

        #endregion

        #region Gender (String)
    [MemberOrder(160), StringLength(1)]
        public virtual string  Gender {get; set;}

        #endregion

        #region HireDate (DateTime)
    [MemberOrder(170), Mask("d")]
        public virtual DateTime  HireDate {get; set;}

        #endregion

        #region SalariedFlag (Boolean)
    [MemberOrder(180)]
        public virtual bool  SalariedFlag {get; set;}

        #endregion

        #region VacationHours (Int16)
    [MemberOrder(190)]
        public virtual short  VacationHours {get; set;}

        #endregion

        #region SickLeaveHours (Int16)
    [MemberOrder(200)]
        public virtual short  SickLeaveHours {get; set;}

        #endregion

        #region CurrentFlag (Boolean)
    [MemberOrder(210)]
        public virtual bool  CurrentFlag {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(220)]
        public virtual Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(230), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Contact (Contact)
    		
    [MemberOrder(240)]
    	public virtual Contact Contact {get; set;}

        #endregion

        #region Employee1 (Collection of Employee)
    		
    	    private ICollection<Employee> _employee1 = new List<Employee>();
    		
    		[MemberOrder(250), Disabled]
        public virtual ICollection<Employee> Employee1
        {
            get
            {
                return _employee1;
            }
    		set
    		{
    		    _employee1 = value;
    		}
        }

        #endregion

        #region Employee2 (Employee)
    		
    [MemberOrder(260)]
    	public virtual Employee Employee2 {get; set;}

        #endregion

        #region EmployeeAddresses (Collection of EmployeeAddress)
    		
    	    private ICollection<EmployeeAddress> _employeeAddresses = new List<EmployeeAddress>();
    		
    		[MemberOrder(270), Disabled]
        public virtual ICollection<EmployeeAddress> EmployeeAddresses
        {
            get
            {
                return _employeeAddresses;
            }
    		set
    		{
    		    _employeeAddresses = value;
    		}
        }

        #endregion

        #region EmployeeDepartmentHistories (Collection of EmployeeDepartmentHistory)
    		
    	    private ICollection<EmployeeDepartmentHistory> _employeeDepartmentHistories = new List<EmployeeDepartmentHistory>();
    		
    		[MemberOrder(280), Disabled]
        public virtual ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistories
        {
            get
            {
                return _employeeDepartmentHistories;
            }
    		set
    		{
    		    _employeeDepartmentHistories = value;
    		}
        }

        #endregion

        #region EmployeePayHistories (Collection of EmployeePayHistory)
    		
    	    private ICollection<EmployeePayHistory> _employeePayHistories = new List<EmployeePayHistory>();
    		
    		[MemberOrder(290), Disabled]
        public virtual ICollection<EmployeePayHistory> EmployeePayHistories
        {
            get
            {
                return _employeePayHistories;
            }
    		set
    		{
    		    _employeePayHistories = value;
    		}
        }

        #endregion

        #region JobCandidates (Collection of JobCandidate)
    		
    	    private ICollection<JobCandidate> _jobCandidates = new List<JobCandidate>();
    		
    		[MemberOrder(300), Disabled]
        public virtual ICollection<JobCandidate> JobCandidates
        {
            get
            {
                return _jobCandidates;
            }
    		set
    		{
    		    _jobCandidates = value;
    		}
        }

        #endregion

        #region PurchaseOrderHeaders (Collection of PurchaseOrderHeader)
    		
    	    private ICollection<PurchaseOrderHeader> _purchaseOrderHeaders = new List<PurchaseOrderHeader>();
    		
    		[MemberOrder(310), Disabled]
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders
        {
            get
            {
                return _purchaseOrderHeaders;
            }
    		set
    		{
    		    _purchaseOrderHeaders = value;
    		}
        }

        #endregion

        #region SalesPerson (SalesPerson)
    		
    [MemberOrder(320)]
    	public virtual SalesPerson SalesPerson {get; set;}

        #endregion


        #endregion

    }
}
