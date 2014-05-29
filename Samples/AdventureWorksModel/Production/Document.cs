// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    public class Document : AWDomainObject {
        private ICollection<ProductDocument> _ProductDocument = new List<ProductDocument>();

        public virtual int DocumentID { get; set; }

        public virtual string Title { get; set; }

        public virtual string FileName { get; set; }

        public virtual string FileExtension { get; set; }

        public virtual string Revision { get; set; }

        public virtual int ChangeNumber { get; set; }

        public virtual byte Status { get; set; }

        public virtual string DocumentSummary { get; set; }

        public byte[] Document1 { get; set; }

        public ICollection<ProductDocument> ProductDocument {
            get { return _ProductDocument; }
            set { _ProductDocument = value; }
        }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}