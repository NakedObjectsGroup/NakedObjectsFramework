using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class StoreContact
    {
    
        #region Primitive Properties
        #region CustomerID (Int32)
    [MemberOrder(100)]
        public virtual int  CustomerID {get; set;}

        #endregion

        #region ContactID (Int32)
    [MemberOrder(110)]
        public virtual int  ContactID {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(120)]
        public virtual Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Contact (Contact)
    		
    [MemberOrder(140)]
    	public virtual Contact Contact {get; set;}

        #endregion

        #region ContactType (ContactType)
    		
    [MemberOrder(150)]
    	public virtual ContactType ContactType {get; set;}

        #endregion

        #region Store (Store)
    		
    [MemberOrder(160)]
    	public virtual Store Store {get; set;}

        #endregion


        #endregion

    }
}
