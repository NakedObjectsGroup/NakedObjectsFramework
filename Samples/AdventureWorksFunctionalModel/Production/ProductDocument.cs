// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksModel {
    public class ProductDocument : IHasModifiedDate {
        public ProductDocument(
            int productID,
            int documentID,
            Document document,
            Product product
            )
        {
            ProductID = productID;
            DocumentID = documentID;
            Document = document;
            Product = product;
        }

        public ProductDocument() { }

        public virtual int ProductID { get; set; }
        public virtual int DocumentID { get; set; }
        public virtual Document Document { get; set; }
        public virtual Product Product { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion
    }

    public static class ProductDocumentFunctions
    {
        public static ProductDocument Updating(ProductDocument c, [Injected] DateTime now)
        {
            return c.With(x => x.ModifiedDate, now);
        }
    }
}