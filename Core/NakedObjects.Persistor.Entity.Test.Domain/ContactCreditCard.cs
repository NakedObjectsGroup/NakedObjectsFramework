using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ContactCreditCard
    {
    
        #region Primitive Properties
        #region ContactID (Int32)
    [MemberOrder(100)]
        public virtual int  ContactID {get; set;}

        #endregion

        #region CreditCardID (Int32)
    [MemberOrder(110)]
        public virtual int  CreditCardID {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Contact (Contact)
    		
    [MemberOrder(130)]
    	public virtual Contact Contact {get; set;}

        #endregion

        #region CreditCard (CreditCard)
    		
    [MemberOrder(140)]
    	public virtual CreditCard CreditCard {get; set;}

        #endregion


        #endregion

    }
}
