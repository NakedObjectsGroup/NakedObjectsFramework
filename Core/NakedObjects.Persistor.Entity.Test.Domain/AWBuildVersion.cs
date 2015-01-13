using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class AWBuildVersion
    {
    
        #region Primitive Properties
        #region SystemInformationID (Byte)
    [MemberOrder(100)]
        public virtual byte  SystemInformationID {get; set;}

        #endregion

        #region Database_Version (String)
    [MemberOrder(110), StringLength(25)]
        public virtual string  Database_Version {get; set;}

        #endregion

        #region VersionDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  VersionDate {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

    }
}
