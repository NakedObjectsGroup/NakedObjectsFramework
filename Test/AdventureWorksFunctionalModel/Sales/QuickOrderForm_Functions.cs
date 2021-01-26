// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {


    public static class QuickOrderForm_Functions {

        public static string[] DeriveKeys(this QuickOrderForm vm)
        {
            var keys = new List<string> { vm.Customer.AccountNumber };
            keys.AddRange(vm.Details.SelectMany(x => QuickOrderLine_Functions.DeriveKeys(x)));
            return keys.ToArray();
        }

        public static QuickOrderForm CreateFromKeys(string[] keys, IContext context)
        {
            var cust = context.Instances<Customer>().Single(c => c.AccountNumber == keys[0]);
            var lines = new List<QuickOrderLine>();
            for (int i = 1; i < keys.Count(); i = i + 2)
            {
                var dKeys = new[] { keys[i], keys[i + 1] };
                var line = QuickOrderLine_Functions.CreateFromKeys(dKeys, context);
                lines.Add(line);
            }
            return new QuickOrderForm {
                Customer = cust, 
                AccountNumber = cust.AccountNumber, 
                Details = lines
            };
        }

        public static QuickOrderForm AddDetail( 
            QuickOrderForm vm, Product product,short number)
        {
            var ol = new QuickOrderLine { Product = product, Number = number };
            var details2 = new List<QuickOrderLine>(vm.Details);
            details2.Add(ol); //TODO: redo as Immutable collection?
            return vm with {Details =  details2};
        }

        public static (SalesOrderHeader, SalesOrderHeader) CreateOrder(
            this QuickOrderForm vm, IContext context)
        {
            throw new NotImplementedException();
            //SalesOrderHeader soh = Order_AdditionalFunctions.CreateAnotherOrder(vm.Customer, context);
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