using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class vProductModelCatalogDescription
    {
        public int ProductModelID { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Manufacturer { get; set; }
        public string Copyright { get; set; }
        public string ProductURL { get; set; }
        public string WarrantyPeriod { get; set; }
        public string WarrantyDescription { get; set; }
        public string NoOfYears { get; set; }
        public string MaintenanceDescription { get; set; }
        public string Wheel { get; set; }
        public string Saddle { get; set; }
        public string Pedal { get; set; }
        public string BikeFrame { get; set; }
        public string Crankset { get; set; }
        public string PictureAngle { get; set; }
        public string PictureSize { get; set; }
        public string ProductPhotoID { get; set; }
        public string Material { get; set; }
        public string Color { get; set; }
        public string ProductLine { get; set; }
        public string Style { get; set; }
        public string RiderExperience { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
    }
}
