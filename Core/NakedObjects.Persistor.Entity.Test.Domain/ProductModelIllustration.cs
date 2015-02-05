// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class ProductModelIllustration {
        #region Primitive Properties

        #region ProductModelID (Int32)

        [MemberOrder(100)]
        public virtual int ProductModelID { get; set; }

        #endregion

        #region IllustrationID (Int32)

        [MemberOrder(110)]
        public virtual int IllustrationID { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(120), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Illustration (Illustration)

        [MemberOrder(130)]
        public virtual Illustration Illustration { get; set; }

        #endregion

        #region ProductModel (ProductModel)

        [MemberOrder(140)]
        public virtual ProductModel ProductModel { get; set; }

        #endregion

        #endregion
    }
}