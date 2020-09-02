// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksModel.Sales {

    [ViewModelEdit]
    public class QuickOrderForm  {
        public QuickOrderForm(Customer customer, 
            string accountNumber, 
            ICollection<QuickOrderLine> details)
        {
            Customer = customer;
            AccountNumber = accountNumber;
            Details = details;
        }

        public QuickOrderForm()
        {

        }

        //TODO: Properties defined as read-only, even though the user will appear to modify them.
        [NakedObjectsIgnore]
        public Customer Customer { get; }

        public string AccountNumber { get; }

        public ICollection<QuickOrderLine> Details { get; }
    }

    public static class QuickOrderFormFunctions {

        public static string[] DeriveKeys(QuickOrderForm vm )
        {
            //TODO: redo using immutable collection
            var keys = new List<string> { vm.Customer.AccountNumber };
            keys.AddRange(vm.Details.SelectMany(x => QuickOrderLineFunctions.DeriveKeys(x)));
            return keys.ToArray();
        }

        public static QuickOrderForm PopulateUsingKeys(
            QuickOrderForm vm, 
            string[] keys,
            [Injected] IQueryable<Customer> customers)
        {
            throw new NotImplementedException(); //TODO
            //var cust = customers.Single(c => c.AccountNumber == keys[0]);

            //for (int i = 1; i < keys.Count(); i = i + 2)
            //{
            //    var dKeys = new[] { keys[i], keys[i + 1] };
            //    var d = Container.NewViewModel<OrderLine>();
            //    d.PopulateUsingKeys(dKeys);
            //    details.Add(d);
            //}
        }

        public static IQueryable<QuickOrderLine> GetOrders(QuickOrderForm vm)
        {
            return vm.Details.AsQueryable();
        }

        public static QuickOrderForm AddDetail( 
            QuickOrderForm vm, 
            [FindMenu] Product product, 
            short number)
        {
            var ol = new QuickOrderLine(product, number);
            var details = vm.Details;
            details.Add(ol); //TODO: redo using immutable collection
            return vm.With(x => x.Details, details);
        }

        public static (SalesOrderHeader, SalesOrderHeader) CreateOrder(
            QuickOrderForm vm,
            [Injected] IQueryable<BusinessEntityAddress> addresses,
            [Injected] IQueryable<SpecialOfferProduct> sops)
        {
            throw new NotImplementedException();
            //SalesOrderHeader soh = OrderRepository.CreateNewOrder(Customer, true, addresses);
            //soh.Status = (byte)OrderStatus.InProcess;
            //Container.Persist(ref soh);

            //foreach (OrderLine d in Details)
            //{
            //    d.AddTo(soh, sops);
            //}

            //return soh;
        }
    }
}