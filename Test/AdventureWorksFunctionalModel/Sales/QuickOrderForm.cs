// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using AW.Functions;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {

    [ViewModel(typeof(QuickOrderForm_Functions), VMEditability.EditOnly)]
    public record QuickOrderForm  {
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
        [Hidden]
        public Customer Customer { get; }

        public string AccountNumber { get;  }

        public ICollection<QuickOrderLine> Details { get; }
    }
}