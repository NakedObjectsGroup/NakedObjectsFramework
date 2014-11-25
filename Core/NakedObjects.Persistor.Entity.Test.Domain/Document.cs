using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Document
    {
    
        #region Primitive Properties
        #region DocumentID (Int32)
    [MemberOrder(100)]
        public virtual int  DocumentID {get; set;}

        #endregion

        #region Title (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Title {get; set;}

        #endregion

        #region FileName (String)
    [MemberOrder(120), StringLength(400)]
        public virtual string  FileName {get; set;}

        #endregion

        #region FileExtension (String)
    [MemberOrder(130), StringLength(8)]
        public virtual string  FileExtension {get; set;}

        #endregion

        #region Revision (String)
    [MemberOrder(140), StringLength(5)]
        public virtual string  Revision {get; set;}

        #endregion

        #region ChangeNumber (Int32)
    [MemberOrder(150)]
        public virtual int  ChangeNumber {get; set;}

        #endregion

        #region Status (Byte)
    [MemberOrder(160)]
        public virtual byte  Status {get; set;}

        #endregion

        #region DocumentSummary (String)
    [MemberOrder(170), Optionally]
        public virtual string  DocumentSummary {get; set;}

        #endregion

        #region Document1 (Binary)
    [MemberOrder(180), Optionally]
        public virtual byte[]  Document1 {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(190), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region ProductDocuments (Collection of ProductDocument)
    		
    	    private ICollection<ProductDocument> _productDocuments = new List<ProductDocument>();
    		
    		[MemberOrder(200), Disabled]
        public virtual ICollection<ProductDocument> ProductDocuments
        {
            get
            {
                return _productDocuments;
            }
    		set
    		{
    		    _productDocuments = value;
    		}
        }

        #endregion


        #endregion

    }
}
