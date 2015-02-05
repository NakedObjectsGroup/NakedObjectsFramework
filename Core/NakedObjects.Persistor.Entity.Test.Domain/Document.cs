// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class Document {
        #region Primitive Properties

        #region DocumentID (Int32)

        [MemberOrder(100)]
        public virtual int DocumentID { get; set; }

        #endregion

        #region Title (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string Title { get; set; }

        #endregion

        #region FileName (String)

        [MemberOrder(120), StringLength(400)]
        public virtual string FileName { get; set; }

        #endregion

        #region FileExtension (String)

        [MemberOrder(130), StringLength(8)]
        public virtual string FileExtension { get; set; }

        #endregion

        #region Revision (String)

        [MemberOrder(140), StringLength(5)]
        public virtual string Revision { get; set; }

        #endregion

        #region ChangeNumber (Int32)

        [MemberOrder(150)]
        public virtual int ChangeNumber { get; set; }

        #endregion

        #region Status (Byte)

        [MemberOrder(160)]
        public virtual byte Status { get; set; }

        #endregion

        #region DocumentSummary (String)

        [MemberOrder(170), Optionally]
        public virtual string DocumentSummary { get; set; }

        #endregion

        #region Document1 (Binary)

        [MemberOrder(180), Optionally]
        public virtual byte[] Document1 { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(190), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region ProductDocuments (Collection of ProductDocument)

        private ICollection<ProductDocument> _productDocuments = new List<ProductDocument>();

        [MemberOrder(200), Disabled]
        public virtual ICollection<ProductDocument> ProductDocuments {
            get { return _productDocuments; }
            set { _productDocuments = value; }
        }

        #endregion

        #endregion
    }
}