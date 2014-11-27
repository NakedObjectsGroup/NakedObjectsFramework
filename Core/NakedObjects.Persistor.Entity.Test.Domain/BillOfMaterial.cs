using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class BillOfMaterial
    {
    
        #region Primitive Properties
        #region BillOfMaterialsID (Int32)
    [MemberOrder(100)]
        public virtual int  BillOfMaterialsID {get; set;}

        #endregion

        #region StartDate (DateTime)
    [MemberOrder(110), Mask("d")]
        public virtual System.DateTime  StartDate {get; set;}

        #endregion

        #region EndDate (DateTime)
    [MemberOrder(120), Optionally, Mask("d")]
        public virtual Nullable<System.DateTime>  EndDate {get; set;}

        #endregion

        #region BOMLevel (Int16)
    [MemberOrder(130)]
        public virtual short  BOMLevel {get; set;}

        #endregion

        #region PerAssemblyQty (Decimal)
    [MemberOrder(140)]
        public virtual decimal  PerAssemblyQty {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(150), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Product (Product)
    		
    [MemberOrder(160)]
    	public virtual Product Product {get; set;}

        #endregion

        #region Product1 (Product)
    		
    [MemberOrder(170)]
    	public virtual Product Product1 {get; set;}

        #endregion

        #region UnitMeasure (UnitMeasure)
    		
    [MemberOrder(180)]
    	public virtual UnitMeasure UnitMeasure {get; set;}

        #endregion


        #endregion

    }
}
