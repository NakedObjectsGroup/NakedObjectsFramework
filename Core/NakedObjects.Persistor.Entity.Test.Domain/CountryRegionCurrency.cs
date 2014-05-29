using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class CountryRegionCurrency
    {
    
        #region Primitive Properties
        #region CountryRegionCode (String)
    [MemberOrder(100), StringLength(3)]
        public virtual string  CountryRegionCode {get; set;}

        #endregion

        #region CurrencyCode (String)
    [MemberOrder(110), StringLength(3)]
        public virtual string  CurrencyCode {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region CountryRegion (CountryRegion)
    		
    [MemberOrder(130)]
    	public virtual CountryRegion CountryRegion {get; set;}

        #endregion

        #region Currency (Currency)
    		
    [MemberOrder(140)]
    	public virtual Currency Currency {get; set;}

        #endregion


        #endregion

    }
}
