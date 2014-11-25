using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ErrorLog
    {
    
        #region Primitive Properties
        #region ErrorLogID (Int32)
    [MemberOrder(100)]
        public virtual int  ErrorLogID {get; set;}

        #endregion

        #region ErrorTime (DateTime)
    [MemberOrder(110), Mask("d")]
        public virtual System.DateTime  ErrorTime {get; set;}

        #endregion

        #region UserName (String)
    [MemberOrder(120), StringLength(128)]
        public virtual string  UserName {get; set;}

        #endregion

        #region ErrorNumber (Int32)
    [MemberOrder(130)]
        public virtual int  ErrorNumber {get; set;}

        #endregion

        #region ErrorSeverity (Int32)
    [MemberOrder(140), Optionally]
        public virtual Nullable<int>  ErrorSeverity {get; set;}

        #endregion

        #region ErrorState (Int32)
    [MemberOrder(150), Optionally]
        public virtual Nullable<int>  ErrorState {get; set;}

        #endregion

        #region ErrorProcedure (String)
    [MemberOrder(160), Optionally, StringLength(126)]
        public virtual string  ErrorProcedure {get; set;}

        #endregion

        #region ErrorLine (Int32)
    [MemberOrder(170), Optionally]
        public virtual Nullable<int>  ErrorLine {get; set;}

        #endregion

        #region ErrorMessage (String)
    [MemberOrder(180), StringLength(4000)]
        public virtual string  ErrorMessage {get; set;}

        #endregion


        #endregion

    }
}
