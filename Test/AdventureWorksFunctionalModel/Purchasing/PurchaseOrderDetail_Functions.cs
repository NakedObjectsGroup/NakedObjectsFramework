// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFunctions;
using AW.Types;


namespace AW.Functions {
        
    public static class PurchaseOrderDetail_Functions
    {
        internal static (PurchaseOrderDetail, IContext) UpdatePOD(
            PurchaseOrderDetail original, PurchaseOrderDetail updated, IContext context)
        {
            var updated2 = updated with { ModifiedDate = context.Now() };
            return (updated2, context.WithUpdated(original, updated));
        }

        [MemberOrder(1)]
        public static (PurchaseOrderDetail, IContext) ReceiveGoods(
            this PurchaseOrderDetail pod, int qtyReceived, int qtyRejected, int qtyIntoStock, IContext context) =>
            UpdatePOD(pod, pod with {ReceivedQty = qtyReceived, RejectedQty = qtyRejected, StockedQty = qtyIntoStock}, context);


        public static int Default1ReceiveGoods(this PurchaseOrderDetail pod)=>  pod.OrderQty;

        public static int Defaul32ReceiveGoods(this PurchaseOrderDetail pod) => pod.OrderQty;

        public static string ValidateReceiveGoods(this PurchaseOrderDetail pod, int qtyReceived, int qtyRejected, int qtyIntoStock) =>
        qtyRejected + qtyIntoStock != qtyReceived ? "Qty Into Stock + Qty Rejected must add up to Qty Received" : null;
    }
}