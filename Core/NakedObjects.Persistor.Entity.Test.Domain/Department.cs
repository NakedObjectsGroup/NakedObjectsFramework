using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Department
    {
    
        #region Primitive Properties
        #region DepartmentID (Int16)
    [MemberOrder(100)]
        public virtual short  DepartmentID {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region GroupName (String)
    [MemberOrder(120), StringLength(50)]
        public virtual string  GroupName {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region EmployeeDepartmentHistories (Collection of EmployeeDepartmentHistory)
    		
    	    private ICollection<EmployeeDepartmentHistory> _employeeDepartmentHistories = new List<EmployeeDepartmentHistory>();
    		
    		[MemberOrder(140), Disabled]
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


        #endregion

    }
}
