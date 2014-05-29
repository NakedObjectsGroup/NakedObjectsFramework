using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class DatabaseLog
    {
    
        #region Primitive Properties
        #region DatabaseLogID (Int32)
    [MemberOrder(100)]
        public virtual int  DatabaseLogID {get; set;}

        #endregion

        #region PostTime (DateTime)
    [MemberOrder(110), Mask("d")]
        public virtual System.DateTime  PostTime {get; set;}

        #endregion

        #region DatabaseUser (String)
    [MemberOrder(120), StringLength(128)]
        public virtual string  DatabaseUser {get; set;}

        #endregion

        #region Event (String)
    [MemberOrder(130), StringLength(128)]
        public virtual string  Event {get; set;}

        #endregion

        #region Schema (String)
    [MemberOrder(140), Optionally, StringLength(128)]
        public virtual string  Schema {get; set;}

        #endregion

        #region Object (String)
    [MemberOrder(150), Optionally, StringLength(128)]
        public virtual string  Object {get; set;}

        #endregion

        #region TSQL (String)
    [MemberOrder(160)]
        public virtual string  TSQL {get; set;}

        #endregion

        #region XmlEvent (String)
    [MemberOrder(170)]
        public virtual string  XmlEvent {get; set;}

        #endregion


        #endregion

    }
}
