using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Shift
    {
    
        #region Primitive Properties
        #region ShiftID (Byte)
    [MemberOrder(100)]
        public virtual byte  ShiftID {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region StartTime (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  StartTime {get; set;}

        #endregion

        #region EndTime (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual DateTime  EndTime {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region EmployeeDepartmentHistories (Collection of EmployeeDepartmentHistory)
    		
    	    private ICollection<EmployeeDepartmentHistory> _employeeDepartmentHistories = new List<EmployeeDepartmentHistory>();
    		
    		[MemberOrder(150), Disabled]
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
