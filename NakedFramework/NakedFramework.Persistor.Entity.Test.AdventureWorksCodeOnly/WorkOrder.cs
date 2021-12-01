// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly; 

[Table("Production.WorkOrder")]
public class WorkOrder {
    public virtual int WorkOrderID { get; set; }

    public virtual int ProductID { get; set; }

    public virtual int OrderQty { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public virtual int StockedQty { get; set; }

    public virtual short ScrappedQty { get; set; }

    public virtual DateTime StartDate { get; set; }

    public virtual DateTime? EndDate { get; set; }

    public virtual DateTime DueDate { get; set; }

    public virtual short? ScrapReasonID { get; set; }

    public virtual DateTime ModifiedDate { get; set; }

    public virtual Product Product { get; set; }

    public virtual ScrapReason ScrapReason { get; set; }

    public virtual ICollection<WorkOrderRouting> WorkOrderRoutings { get; set; }
}