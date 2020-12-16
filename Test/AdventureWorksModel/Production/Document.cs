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
    public class Document  {

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

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
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion
    }
}