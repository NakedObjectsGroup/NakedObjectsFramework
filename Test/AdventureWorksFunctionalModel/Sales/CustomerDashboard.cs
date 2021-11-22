// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using AW.Functions;
using NakedFunctions;

namespace AW.Types {
    /// <summary>
    /// </summary>
    [ViewModel(typeof(CustomerDashboard_Functions))]
    public record CustomerDashboard {
        [Hidden]
#pragma warning disable 8618
        public virtual Customer Root { get; init; }
#pragma warning restore 8618

        public string Name => $"{(Root.IsIndividual() ? Root.Person : Root.Store?.Name)}";

        //Empty field, not - to test that fields are not editable in a VM
        public string Comments { get; init; } = "";

        [TableView(true, "OrderDate", "TotalDue", "Status")]
        public IList<SalesOrderHeader> RecentOrders(IContext context) =>
            Root.RecentOrders(context).Take(5).ToList();

        public override string ToString() => $"{Name} - Dashboard";
    }
}