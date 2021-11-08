// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Legacy.Types;
using NakedObjects;

namespace AdventureWorksModel {

    [LegacyType]
    public class PurchaseOrderDetail : TitledObject     {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() => ModifiedDate.DateTime = DateTime.Now;

        public virtual void Updating() => Persisting();
        #endregion

        [NakedObjectsIgnore]
        public virtual int PurchaseOrderID { get; set; }

        [NakedObjectsIgnore]
        public virtual int PurchaseOrderDetailID { get; set; }


        #region DueDate
        internal DateTime mappedDueDate;
        private Date myDueDate;

        [MemberOrder(26)]
        public virtual Date DueDate => myDueDate ??= new Date(mappedDueDate, v => mappedDueDate = v);
        #endregion

        #region OrderQty
        internal short mappedOrderQty;
        private WholeNumber myOrderQty;

        [MemberOrder(20)]
        public virtual WholeNumber OrderQty => myOrderQty ??= new WholeNumber(mappedOrderQty, v => mappedOrderQty = (short) v );
        #endregion

        [MemberOrder(22)]
        [Mask("C")]
        public virtual decimal UnitPrice { get; set; } //TODO - all decimal fields

        [MemberOrder(24)]
        [Mask("C")]
        [Disabled]
        public virtual decimal LineTotal { get; set; }

        [Mask("#")]
        [MemberOrder(30)]
        public virtual decimal ReceivedQty { get; set; }

        [Mask("#")]
        [MemberOrder(32)]
        public virtual decimal RejectedQty { get; set; }

        [Mask("#")]
        [MemberOrder(34)]
        [Disabled]
        public virtual decimal StockedQty { get; set; }

        #region ModifiedDate
        internal DateTime mappedModifiedDate;
        private TimeStamp myModifiedDate;

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual TimeStamp ModifiedDate => myModifiedDate ??= new TimeStamp(mappedModifiedDate, s => mappedModifiedDate = s);
        #endregion

        #region Product
        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        [Disabled]
        [MemberOrder(10)]
        public virtual Product Product { get; set; }

        #endregion

        #region Header

        [NakedObjectsIgnore]
        public virtual PurchaseOrderHeader PurchaseOrderHeader { get; set; }

        #endregion

        #region Title

        public Title Title() => new Title(ToString());

        public override string ToString()
        {
            var t = new StringBuilder();
            t.Append(OrderQty.ToString()).Append(" x").Append(Product);
            return t.ToString();
        }
        #endregion

        #region Action ReceiveGoods

        private void ReceiveGoods(int qtyReceived, int qtyRejected, int qtyIntoStock)
        {
            ReceivedQty = qtyReceived;
            RejectedQty = qtyRejected;
            StockedQty = qtyIntoStock;
        }

        [MemberOrder(1)]
        public void ActionReceiveGoods(WholeNumber qtyReceived, WholeNumber qtyRejected, WholeNumber qtyIntoStock) => 
            ReceiveGoods(qtyReceived.Number, qtyRejected.Number, qtyIntoStock.Number);

        public void AboutActionReceiveGoods(ActionAbout a, WholeNumber qtyReceived, WholeNumber qtyRejected, WholeNumber qtyIntoStock)
        {
            switch (a.TypeCode)
            {
                case AboutTypeCodes.Name:
                    //a.Name = "";
                    break;
                case AboutTypeCodes.Parameters:
                    //a.ParamLabels[0] = "";
                    a.ParamDefaultValues[0] = OrderQty;
                    a.ParamDefaultValues[1] = 0;
                    a.ParamDefaultValues[2] = OrderQty;
                    //a.ParamOptions[0] = Choices0ReceiveGoods().ToArray();
                    break;
                case AboutTypeCodes.Usable: //TODO this or .Valid
                    //a.UnusableReason = DisableReceiveGoods();
                    //a.Usable = string.IsNullOrEmpty(a.UnusableReason))                
                    break;
                case AboutTypeCodes.Valid:
                    //var sb = new StringBuilder();
                    //sb.append(a.ValidateReceiveGoods(ParamList));
                    //sb.append(a.Validate0ReceiveGoods());
                    //a.UnusableReason = sb.ToString();
                    //a.Usable = string.IsNullOrEmpty(a.UnusableReason))   
                    break;
                case AboutTypeCodes.Visible:
                    //a.Visible = HideReceiveGoods();
                    break;
            }
        }

        public virtual string ValidateReceiveGoods(int qtyReceived, int qtyRejected, int qtyIntoStock) {
            var rb = new ReasonBuilder();
            if (qtyRejected + qtyIntoStock != qtyReceived) {
                rb.Append("Qty Into Stock + Qty Rejected must add up to Qty Received");
            }
            return rb.Reason;
        }
        #endregion
    }
}