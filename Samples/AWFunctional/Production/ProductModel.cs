// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("information.png")]
    public class ProductModel  {

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        private ICollection<ProductModelIllustration> _ProductModelIllustration = new List<ProductModelIllustration>();
        private ICollection<ProductModelProductDescriptionCulture> _ProductModelProductDescriptionCulture = new List<ProductModelProductDescriptionCulture>();
        private ICollection<Product> _productVariants = new List<Product>();

        [NakedObjectsIgnore]
        public virtual int ProductModelID { get; set; }

        [Title]
        [MemberOrder(10)]
        public virtual string Name { get; set; }

        [NakedObjectsIgnore]
        public virtual string CatalogDescription { get; set; }

        [DisplayName("CatalogDescription")]
        [MemberOrder(20)]
        [MultiLine(NumberOfLines = 10)]
        [TypicalLength(500)]
        public virtual string FormattedCatalogDescription {
            get {
                var output = new StringBuilder();

                if (!string.IsNullOrEmpty(CatalogDescription)) {
                    XElement.Parse(CatalogDescription).Elements().ToList().ForEach(n => output.Append(n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n"));
                }
                return output.ToString();
            }
        }

        [MemberOrder(30)]
        public virtual string Instructions { get; set; }

        [TableView(true, "Name", "Number", "Color", "ProductInventory")]
        public virtual ICollection<Product> ProductVariants {
            get { return _productVariants; }
            set { _productVariants = value; }
        }

        [NakedObjectsIgnore]
        public virtual ICollection<ProductModelIllustration> ProductModelIllustration {
            get { return _ProductModelIllustration; }
            set { _ProductModelIllustration = value; }
        }

        [NakedObjectsIgnore]
        public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCulture {
            get { return _ProductModelProductDescriptionCulture; }
            set { _ProductModelProductDescriptionCulture = value; }
        }

        [MemberOrder(22)]
        public virtual string LocalCultureDescription {
            get {
                string usersPref = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                IEnumerable<string> query = from obj in ProductModelProductDescriptionCulture
                    where obj.Culture.CultureID.StartsWith(usersPref)
                    select obj.ProductDescription.Description;

                return query.FirstOrDefault();
            }
        }

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion
    }
}