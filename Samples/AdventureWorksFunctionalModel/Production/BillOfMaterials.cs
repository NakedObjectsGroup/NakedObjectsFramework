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

namespace AdventureWorksModel
{
    public class BillOfMaterial : IHasModifiedDate
    {
        public BillOfMaterial(
            int billOfMaterialID,
            DateTime startDate,
            DateTime? endDate,
            short bOMLevel,
            decimal perAssemblyQty,
            int? productAssemblyID,
            Product product,
            int componentID,
            Product product1,
            string unitMeasureCode,
            UnitMeasure unitMeasure,
            DateTime modifiedDate
            )
        {
            BillOfMaterialID = billOfMaterialID;
            StartDate = startDate;
            EndDate = endDate;
            BOMLevel = bOMLevel;
            PerAssemblyQty = perAssemblyQty;
            ProductAssemblyID = ProductAssemblyID;
            Product = product;
            ComponentID = componentID;
            Product1 = product1;
            UnitMeasureCode = unitMeasureCode;
            UnitMeasure = unitMeasure;
            ModifiedDate = modifiedDate;
        }
        public BillOfMaterial() { }

        [NakedObjectsIgnore]
        public virtual int BillOfMaterialID { get; set; }

        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual short BOMLevel { get; set; }
        public virtual decimal PerAssemblyQty { get; set; }

        [NakedObjectsIgnore]
        public virtual int? ProductAssemblyID { get; set; }
        public virtual Product Product { get; set; }

        [NakedObjectsIgnore]
        public virtual int ComponentID { get; set; }
        public virtual Product Product1 { get; set; }

        [NakedObjectsIgnore]
        public string UnitMeasureCode { get; set; }
        public virtual UnitMeasure UnitMeasure { get; set; }

       [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
    }
    public static class BillOfMaterialFunctions
    {
        public static BillOfMaterial Updating(BillOfMaterial bom, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(bom, now);

        }
    }
}