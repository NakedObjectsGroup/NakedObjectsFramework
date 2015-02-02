// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using System.Linq;

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

        #region Properties

        [DisplayName("Store Name"), MemberOrder(20)]
        public virtual string Name { get; set; }

        #region Demographics

        [Hidden]
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
        public IQueryable<SalesPerson> AutoCompleteSalesPerson([MinLength(2)] string name)
        {
            return SalesRepository.FindSalesPersonByName(null, name);
        }

        #endregion

        #region Contacts

        private ICollection<StoreContact> _contact = new List<StoreContact>();


        [Disabled]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true)]  //i.e. no colums shown, because all relevant info is in the title
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

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #endregion

        #endregion

        public Contact CreateNewContact() {
            var _Contact = Container.NewTransientInstance<Contact>();

            _Contact.Contactee = this;

            return _Contact;
        }

        #region test code ignore
        // this is just here to demonstrate that it's ignored by the reflector
        public void TestIgnoredAction(Dictionary<string, object> unrecognizedParm) { }
        public void TestIgnoredAction1(IDictionary<string, object> unrecognizedParm) { }
        #endregion test code ignore
    }
}