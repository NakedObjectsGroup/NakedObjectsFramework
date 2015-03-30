using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class vProductAndDescription
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string ProductModel { get; set; }
        public string CultureID { get; set; }
        public string Description { get; set; }
    }
}
