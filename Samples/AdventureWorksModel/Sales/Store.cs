// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("skyscraper.png")]
    public class Store : Customer {
        #region Injected Servives

        public SalesRepository SalesRepository { set; protected get; }

        #endregion

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Name).Append(",", AccountNumber);
            return t.ToString();
        }

        #endregion

        public Contact CreateNewContact() {
            var _Contact = Container.NewTransientInstance<Contact>();

            _Contact.Contactee = this;

            return _Contact;
        }

        #region Properties

        [DisplayName("Store Name"), MemberOrder(20)]
        public virtual string Name { get; set; }

        #region Demographics

        [Hidden(WhenTo.Always)]
        public virtual string Demographics { get; set; }

        [DisplayName("Demographics"), MemberOrder(30), MultiLine(NumberOfLines = 10), TypicalLength(500)]
        public virtual string FormattedDemographics {
            get { return Utilities.FormatXML(Demographics); }
        }

        #endregion

        #region SalesPerson

        [Optionally]
        [MemberOrder(40), FindMenu]
        public virtual SalesPerson SalesPerson { get; set; }

        [PageSize(20)]
        public IQueryable<SalesPerson> AutoCompleteSalesPerson([MinLength(2)] string name) {
            return SalesRepository.FindSalesPersonByName(null, name);
        }

        #endregion

        #region Contacts

        private ICollection<StoreContact> _contact = new List<StoreContact>();

        [Disabled]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true)] //i.e. no colums shown, because all relevant info is in the title
        public virtual ICollection<StoreContact> Contacts {
            get { return _contact; }
            set { _contact = value; }
        }

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden(WhenTo.Always)]
        public override Guid rowguid { get; set; }

        #endregion

        #endregion

        #endregion

        #region test code ignore

        // this is just here to demonstrate that it's ignored by the reflector
        public void TestIgnoredAction(Dictionary<string, object> unrecognizedParm) {}
        public void TestIgnoredAction1(IDictionary<string, object> unrecognizedParm) {}

        #endregion test code ignore
    }
}