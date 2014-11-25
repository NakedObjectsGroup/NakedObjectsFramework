using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class SalesTaxRate
    {
    
        #region Primitive Properties
        #region SalesTaxRateID (Int32)
    [MemberOrder(100)]
        public virtual int  SalesTaxRateID {get; set;}

        #endregion

        #region TaxType (Byte)
    [MemberOrder(110)]
        public virtual byte  TaxType {get; set;}

        #endregion

        #region TaxRate (Decimal)
    [MemberOrder(120)]
        public virtual decimal  TaxRate {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(130), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(140)]
        public virtual System.Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(150), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region StateProvince (StateProvince)
    		
    [MemberOrder(160)]
    	public virtual StateProvince StateProvince {get; set;}

        #endregion


        #endregion

    }
}
