using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class Illustration
    {
        public Illustration()
        {
            this.ProductModelIllustrations = new List<ProductModelIllustration>();
        }

        public int IllustrationID { get; set; }
        public string Diagram { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<ProductModelIllustration> ProductModelIllustrations { get; set; }
    }
}
