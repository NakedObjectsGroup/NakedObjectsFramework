// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class Illustration {
        #region Primitive Properties

        #region IllustrationID (Int32)

        [MemberOrder(100)]
        public virtual int IllustrationID { get; set; }

        #endregion

        #region Diagram (String)

        [MemberOrder(110), Optionally]
        public virtual string Diagram { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(120), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region ProductModelIllustrations (Collection of ProductModelIllustration)

        private ICollection<ProductModelIllustration> _productModelIllustrations = new List<ProductModelIllustration>();

        [MemberOrder(130), Disabled]
        public virtual ICollection<ProductModelIllustration> ProductModelIllustrations {
            get { return _productModelIllustrations; }
            set { _productModelIllustrations = value; }
        }

        #endregion

        #endregion
    }
}