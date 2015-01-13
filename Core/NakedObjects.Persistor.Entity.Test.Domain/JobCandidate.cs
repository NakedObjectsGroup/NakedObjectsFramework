using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class JobCandidate
    {
    
        #region Primitive Properties
        #region JobCandidateID (Int32)
    [MemberOrder(100)]
        public virtual int  JobCandidateID {get; set;}

        #endregion

        #region Resume (String)
    [MemberOrder(110), Optionally]
        public virtual string  Resume {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Employee (Employee)
    		
    [MemberOrder(130)]
    	public virtual Employee Employee {get; set;}

        #endregion


        #endregion

    }
}
