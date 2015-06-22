// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel.Sales {
    public class OrderLine : IViewModel {
        public IDomainObjectContainer Container { protected get; set; }

        [NakedObjectsIgnore]
        public Product Product { get; set; }

        [NakedObjectsIgnore]
        public short Number { get; set; }

        [Title]
        public string Description {
            get { return Number + " x " + Product.Name; }
        }

        #region IViewModel Members

        public string[] DeriveKeys() {
            return new[] {Product.ProductID.ToString(), Number.ToString()};
        }

        public void PopulateUsingKeys(string[] instanceId) {
            int p = int.Parse(instanceId.First());
            short n = short.Parse(instanceId.Skip(1).First());
            Product = Container.Instances<Product>().Single(c => c.ProductID == p);
            Number = n;
        }

        #endregion

        [NakedObjectsIgnore]
        public void AddTo(SalesOrderHeader salesOrder) {
            SalesOrderDetail sod = salesOrder.AddNewDetail(Product, Number);
            Container.Persist(ref sod);
        }
    }

    public class QuickOrderForm : IViewModel {
        private ICollection<OrderLine> details = new List<OrderLine>();
        public IDomainObjectContainer Container { protected get; set; }
        public OrderContributedActions OrderRepo { protected get; set; }

        [NakedObjectsIgnore]
        public Customer Customer { get; set; }

        public string AccountNumber {
            get { return Customer.AccountNumber; }
        }

        [Title]
        public string Description {
            get { return details.Any() ? details.First().Description + "..." : AccountNumber; }
        }

        [Disabled]
        public ICollection<OrderLine> Details {
            get { return details; }
            set { details = value; }
        }

        #region IViewModel Members

        public string[] DeriveKeys() {
            var keys = new List<string> {Customer.AccountNumber};
            foreach (OrderLine orderLine in details) {
                keys.AddRange(orderLine.DeriveKeys());
            }
            return keys.ToArray();
        }

        public void PopulateUsingKeys(string[] instanceId) {
            string an = instanceId.First();
            Customer = Container.Instances<Customer>().Single(c => c.AccountNumber == an);

            for (int i = 1; i < instanceId.Count(); i = i + 2) {
                var dKeys = new[] {instanceId[i], instanceId[i + 1]};
                var d = Container.NewViewModel<OrderLine>();
                d.PopulateUsingKeys(dKeys);
                details.Add(d);
            }
        }

        #endregion

        public IQueryable<OrderLine> GetOrders() {
            return details.AsQueryable();
        }

        public QuickOrderForm AddDetail([FindMenu] Product product, short number) {
            var ol = Container.NewViewModel<OrderLine>();
            ol.Product = product;
            ol.Number = number;
            details.Add(ol);

            return this;
        }

        public SalesOrderHeader CreateOrder() {
            SalesOrderHeader soh = OrderRepo.CreateNewOrder(Customer, true);
            soh.Status = (byte) OrderStatus.InProcess;
            Container.Persist(ref soh);

            foreach (OrderLine d in Details) {
                d.AddTo(soh);
            }

            return soh;
        }
    }
}