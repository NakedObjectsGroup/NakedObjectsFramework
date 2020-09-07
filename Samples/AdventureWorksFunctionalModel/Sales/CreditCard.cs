// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AdventureWorksModel {
        public record CreditCard {
        #region Injected Services
        
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        //TODO: How to handle this scenario! Maybe via Action(T)
        public void Persisted() {
            throw new NotImplementedException();
            //var link = Container.NewTransientInstance<PersonCreditCard>();
            //link.CreditCard = this;
            //link.Person = ForContact;
            //Container.Persist(ref link);

            //if (Creator != null) {
            //    Creator.CreatedCardHasBeenSaved(this);
            //}
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [Hidden]
        public virtual int CreditCardID { get; set; }

        [MemberOrder(1)]
        public virtual string CardType { get; set; }

        [MemberOrder(2)][DescribedAs("Without spaces")]
        public virtual string CardNumber { get; set; }

        private string _ObfuscatedNumber;

        
        [MemberOrder(2)]
        [Named("Card No.")]
        
        public virtual string ObfuscatedNumber {
            get {
                if (_ObfuscatedNumber == null && CardNumber != null && CardNumber.Length > 4) {
                    _ObfuscatedNumber = CardNumber.Substring(CardNumber.Length - 4).PadLeft(CardNumber.Length, '*');
                }
                return _ObfuscatedNumber;
            }
        }

        [MemberOrder(3)]
        public virtual byte ExpMonth { get; set; }

        [MemberOrder(4)]
        public virtual short ExpYear { get; set; }

        private ICollection<PersonCreditCard> _links = new List<PersonCreditCard>();
        private Person p;

        public CreditCard(Person p)
        {
            this.p = p;
        }

        [Named("Persons")]
        [MemberOrder(5)]
        //[TableOrder(True, "Contact")]
        public virtual ICollection<PersonCreditCard> PersonLinks {
            get { return _links; }
            set { _links = value; }
        }

        #region ModifiedDate

        [MemberOrder(99)]
        
        //[ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region Title

        public override string ToString() {
            return ObfuscatedNumber;
        }

        #endregion

        public string Validate(byte expMonth, short expYear) {
            if (expMonth == 0 || expYear == 0) {
                return null;
            }

            DateTime today = DateTime.Now.Date;
            DateTime expiryDate = new DateTime(expYear, expMonth, 1); //.EndOfMonth();

            if (expiryDate <= today) {
                return "Expiry date must be in the future";
            }
            return null;
        }

        public virtual string[] ChoicesCardType() {
            return new[] {"Vista", "Distinguish", "SuperiorCard", "ColonialVoice"};
        }

        public virtual string ValidateCardNumber(string cardNumber) {
            if (cardNumber != null && cardNumber.Length <= 4) {
                return "card number too short";
            }

            return null;
        }

        public byte[] ChoicesExpMonth() {
            return new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
        }

        public virtual short[] ChoicesExpYear() {
            return new short[] {2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020};
        }

        #region Logic for creating new cards

        [Hidden]
        
        public ICreditCardCreator Creator { get; set; }

        
        public Person ForContact { get; set; }

        #endregion
    }
}