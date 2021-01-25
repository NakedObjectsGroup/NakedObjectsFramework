// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
       
    public static class Customer_Functions
    {

        #region Action to test switchable view model
        public static StoreSalesInfo ReviewSalesResponsibility()
        {
            throw new NotImplementedException();
            //var ssi = Container.NewViewModel<StoreSalesInfo>();
            //ssi.PopulateUsingKeys(new string[] { AccountNumber, false.ToString() });
            //return ssi;
        }


        public static bool HideReviewSalesResponsibility(Customer c)
        {
            return IsStore(c);
        }

        #endregion

        [MemberOrder(15), DisplayAsProperty]
        public static string CustomerType(this Customer c) =>
            IsIndividual(c) ? "Individual" : "Store";


        internal static BusinessEntity BusinessEntity(this Customer c)
        {
            if (IsStore(c)) return c.Store;
            if (IsIndividual(c)) return c.Person;
            throw new Exception("Customer is neither Store nor Person!");
        }

        public static bool HideStore(this Customer c)
        {
            return !IsStore(c);
        }

        public static bool HidePerson(this Customer c)
        {
            return !IsIndividual(c);
        }

        internal static bool IsIndividual(this Customer c)
        {
            return !IsStore(c);
        }

        internal static bool IsStore(this Customer c)
        {
            return c.Store != null;
        }
    }
}