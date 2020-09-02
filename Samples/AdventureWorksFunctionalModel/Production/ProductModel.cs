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
using NakedFunctions;

namespace AdventureWorksModel
{
    [IconName("information.png")]
    public class ProductModel: IHasRowGuid, IHasModifiedDate
    {
        public ProductModel(
            int productModelID,
            string name,
            string catalogDescription,
            string instructions,
            ICollection<Product> productVariants,
            ICollection<ProductModelIllustration> productModelIllustration,
            ICollection<ProductModelProductDescriptionCulture>productModelProductDescriptionCulture,
            Guid rowguid,
            DateTime modifiedDate
            )
        {
            ProductModelID = productModelID;
            Name = name;
            CatalogDescription = catalogDescription;
            Instructions = instructions;
            ProductVariants = productVariants;
            ProductModelIllustration = productModelIllustration;
            ProductModelProductDescriptionCulture = productModelProductDescriptionCulture;
        }
        public ProductModel() { }
        [NakedObjectsIgnore]
        public virtual int ProductModelID { get; set; }

        [MemberOrder(10)]
        public virtual string Name { get; set; }

        [NakedObjectsIgnore]
        public virtual string CatalogDescription { get; set; }

        [MemberOrder(30)]
        public virtual string Instructions { get; set; }

        [TableView(true, "Name", "Number", "Color", "ProductInventory")]
        public virtual ICollection<Product> ProductVariants { get; set; }

        [NakedObjectsIgnore]
        public virtual ICollection<ProductModelIllustration> ProductModelIllustration { get; set; }

        [NakedObjectsIgnore]
        public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCulture { get; set; }

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

    public static class ProductModelFunctions
    {
        public static string Title(this ProductModel pm)
        {
            return pm.CreateTitle(pm.Name);
        }
        [DisplayAsProperty]
        [MemberOrder(22)]
        public static string LocalCultureDescription(ProductModel pm)
        {
            string usersPref = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return (from obj in pm.ProductModelProductDescriptionCulture
                    where obj.Culture.CultureID.StartsWith(usersPref)
                    select obj.ProductDescription.Description).FirstOrDefault();
        }

        [DisplayAsProperty]
        [DisplayName("CatalogDescription")]
        [MemberOrder(20)]
        [MultiLine(NumberOfLines = 10)]
        [TypicalLength(500)]
        public static string FormattedCatalogDescription(ProductModel pm)
        {
            var output = new StringBuilder();
            //TODO: Re-write using aggregation (reduce) pattern
            if (!string.IsNullOrEmpty(pm.CatalogDescription))
            {
                XElement.Parse(pm.CatalogDescription).Elements().ToList().ForEach(n => output.Append(n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n"));
            }
            return output.ToString();
        }

        public static ProductInventory Updating(ProductInventory a, [Injected] DateTime now)
        {
            return a.With(x => x.ModifiedDate, now);
        }
    }
}
