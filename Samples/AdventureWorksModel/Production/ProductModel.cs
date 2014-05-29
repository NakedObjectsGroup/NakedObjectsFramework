// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("information.png")]
    public class ProductModel : AWDomainObject {

        private ICollection<ProductModelIllustration> _ProductModelIllustration = new List<ProductModelIllustration>();
        private ICollection<ProductModelProductDescriptionCulture> _ProductModelProductDescriptionCulture = new List<ProductModelProductDescriptionCulture>();
        private ICollection<Product> _productVariants = new List<Product>();

        [Hidden]
        public virtual int ProductModelID { get; set; }

        [Title]
        [MemberOrder(10)]
        public virtual string Name { get; set; }

        [Hidden]
        public virtual string CatalogDescription { get; set; }

        [DisplayName("CatalogDescription")]
        [MemberOrder(20)]
        [MultiLine(NumberOfLines = 10)]
        [TypicalLength(500)]
        public virtual string FormattedCatalogDescription {
            get
            {
                var output = new StringBuilder();

                if (!string.IsNullOrEmpty(CatalogDescription))
                {
                    XElement.Parse(CatalogDescription).Elements().ToList().ForEach(n => output.Append(n.Name.ToString().Substring(n.Name.ToString().IndexOf("}") + 1) + ": " + n.Value + "\n"));
                }
                return output.ToString();
            }
        }

        [MemberOrder(30)]
        public virtual string Instructions { get; set; }

        [TableView(true, "Name", "Number", "Color")]
        public virtual ICollection<Product> ProductVariants {
            get { return _productVariants; }
            set { _productVariants = value; }
        }

        [Hidden]
        public virtual ICollection<ProductModelIllustration> ProductModelIllustration {
            get { return _ProductModelIllustration; }
            set { _ProductModelIllustration = value; }
        }

        [Hidden]
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

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #endregion
    }
}