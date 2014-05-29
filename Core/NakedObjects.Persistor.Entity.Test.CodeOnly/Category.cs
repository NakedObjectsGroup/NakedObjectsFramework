// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;

namespace TestCodeOnly {
    public class Category {
        private ICollection<Product> _products;
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }

        public virtual ICollection<Product> Products {
            get {
                if (_products == null) _products = new List<Product>();
                return _products;
            }
            set { _products = value; }
        }
    }
}