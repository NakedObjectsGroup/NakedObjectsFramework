// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class CustomerDashboard_Functions { 

        //TODO: not clear what was intended functionality?
        //public (SalesOrderHeader, IContainer) NewOrder(
        //    IQueryable<BusinessEntityAddress> addresses,
        //    IQueryable<SalesOrderHeader> headers) {
        //    var order = OrderContributedActions.CreateNewOrder(Root, true, addresses, headers);
        //    return DisplayAndSave(order, context);
        //}

        public static string[] DeriveKeys(this CustomerDashboard cd) {
            return new[] {cd.Root.CustomerID.ToString() };
        }

        //TODO:
        public static CustomerDashboard PopulateUsingKeys(this CustomerDashboard dashboard, string[] keys, IContext context)
        {
            int customerId = int.Parse(keys[0]);
            return new CustomerDashboard(context.Instances<Customer>().Single(c => c.CustomerID == customerId));
        }
    }
}