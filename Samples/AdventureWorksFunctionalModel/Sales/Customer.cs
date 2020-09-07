// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using AdventureWorksFunctionalModel;
using NakedFunctions;
using NakedFunctions;
using static AdventureWorksModel.CustomerFunctions;

namespace AdventureWorksModel {
        public record Customer 
    {
        public Customer()
        {

        }
        public Customer(Store store, Person person, Guid guid, DateTime dt)
        {
            Store = store;
            Person = person;
            CustomerRowguid = guid;
            CustomerModifiedDate = dt;
        }

        #region Life Cycle Methods
        public virtual void Persisting() {
            CustomerRowguid = Guid.NewGuid();
            CustomerModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            CustomerModifiedDate = DateTime.Now;
        }
        #endregion

        [Hidden]
        public virtual int CustomerID { get; set; }

        [DescribedAs("xxx"), MemberOrder(10)]
        public virtual string AccountNumber { get; set; }

        #region Sales Territory
        [Hidden]
        public virtual int? SalesTerritoryID { get; set; }

        [ MemberOrder(20)]
        public virtual SalesTerritory SalesTerritory { get; set; }
        #endregion

        #region Store & Personal customers

        internal BusinessEntity BusinessEntity() {
            if (IsStore(this)) return Store;
            if (IsIndividual(this)) return Person;
            throw new DomainException("Customer is neither Store nor Person!");
        }

        [Hidden]
        public virtual int? StoreID { get; set; }

        [ MemberOrder(20)]
        public virtual Store Store { get; set; }



        [Hidden]
        public virtual int? PersonID { get; set; }

        [ MemberOrder(20)]
        public virtual Person Person { get; set; }


        #endregion

        [MemberOrder(15)]
        public virtual string CustomerType {
            get {
                return CustomerFunctions.IsIndividual(this) ? "Individual" : "Store";
            }
        }


        #region ModifiedDate and rowguid

        #region ModifiedDate

        [Hidden]
        //[ConcurrencyCheck]
        public virtual DateTime CustomerModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid CustomerRowguid { get; set; }

        #endregion

        #endregion

    }

    public static class CustomerFunctions
    {

        //public  static string Title(Customer c)
        //{
        //    return c.CreateTitle($"{PartTitle(c)}, {c.AccountNumber}");
        //}
         private static string PartTitle(Customer c)
        {
            return IsStore(c) ? StoreFunctions.Title(c.Store) : PersonFunctions.Title(c.Person);
        }

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

        public static bool HideStore(Customer c)
        {
            return !IsStore(c);
        }

        public static bool HidePerson(Customer c)
        {
            return !IsIndividual(c);
        }


        [Hidden]
        public static bool IsIndividual(Customer c)
        {
            return !IsStore(c);
        }

        [Hidden]
        public static bool IsStore(Customer c)
        {
            return c.Store != null;
        }
    }
}