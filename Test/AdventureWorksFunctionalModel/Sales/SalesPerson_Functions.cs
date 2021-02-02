// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using AW.Types;
using NakedFunctions;


namespace AW.Functions
{
    public static class SalesPerson_Functions
    {
        internal static (SalesPerson, IContext) UpdateSalesPerson(
            SalesPerson original, SalesPerson updated, IContext context)
        {
            var updated2 = updated with { ModifiedDate = context.Now() };
            return (updated2, context.WithUpdated(original, updated2));
        }

        [MemberOrder(1)]
        public static (SalesPerson, IContext) RecalulateSalesYTD(this SalesPerson sp, IContext context)
        {
            var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            decimal newYTD = 0;
            IQueryable<SalesOrderHeader> query = from obj in context.Instances<SalesOrderHeader>()
                                                 where obj.SalesPerson.BusinessEntityID == sp.BusinessEntityID &&
                                                       obj.StatusByte == 5 &&
                                                       obj.OrderDate >= startOfYear
                                                 select obj;
            if (query.Count() > 0)
            {
                newYTD = query.Sum(n => n.SubTotal);
            }
            else
            {
                newYTD = 0;
            }
            return UpdateSalesPerson(sp, sp with { SalesYTD = newYTD }, context);
        }

        [MemberOrder(2)]
        public static (SalesPerson, IContext) ChangeSalesQuota(this SalesPerson sp, decimal newQuota, IContext context)
        {
            var uSp = sp with { SalesQuota = newQuota };
            var history = new SalesPersonQuotaHistory() with { SalesPerson = sp, SalesQuota = newQuota, QuotaDate = context.Now() };
            return (uSp, context.WithUpdated(sp, uSp).WithNew(history));
        }

        [MemberOrder(1)]
        public static (SalesPerson, IContext) ChangeSalesTerritory(this SalesPerson sp, SalesTerritory newTerritory, IContext context)
        {

            var newHist = new SalesTerritoryHistory() with { SalesPerson = sp, SalesTerritory = newTerritory, StartDate = context.Now() };
            var prev = sp.TerritoryHistory.Where(n => n.EndDate == null).FirstOrDefault();
            var uPrev = prev with { EndDate = context.Now() };
            var uSp = sp with { SalesTerritory = newTerritory };
            return (uSp, context.WithNew(newHist).WithUpdated(sp, uSp).WithUpdated(prev, uPrev));
        }
    }
}