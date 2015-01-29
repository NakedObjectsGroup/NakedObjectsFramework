// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [Immutable(WhenTo.OncePersisted), IconName("id_card.png")]
    public class CreditCard : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(ObfuscatedNumber);
            return t.ToString();
        }



        #endregion

        #region Life Cycle Methods  

        public void Persisted() {
            var link = Container.NewTransientInstance<ContactCreditCard>();
            link.CreditCard = this;
            link.Contact = ForContact;
            Container.Persist(ref link);

            Creator.CreatedCardHasBeenSaved(this);
        }

        #endregion

        private ICollection<ContactCreditCard> _ContactCreditCard = new List<ContactCreditCard>();
        private string _ObfuscatedNumber;

        [Hidden]
        public virtual int CreditCardID { get; set; }

        [MemberOrder(1)]
        public virtual string CardType { get; set; }

        [Hidden(WhenTo.OncePersisted)]
        [MemberOrder(2)]
        public virtual string CardNumber { get; set; }

        [Hidden(WhenTo.UntilPersisted)]
        [MemberOrder(2)]
        [DisplayName("Card No.")]
        public virtual string ObfuscatedNumber {
            get {
                if (_ObfuscatedNumber == null && CardNumber != null) {
                    _ObfuscatedNumber = CardNumber.Substring(CardNumber.Length - 4).PadLeft(CardNumber.Length, '*');
                }
                return _ObfuscatedNumber;
            }
        }

        [MemberOrder(3)]
        public virtual byte ExpMonth { get; set; }

        [MemberOrder(4)]
        public virtual short ExpYear { get; set; }


        public string Validate(byte expMonth, short expYear) {

            if (expMonth == 0 || expYear == 0) {
                return null;
            }

            DateTime today = DateTime.Now.Date;
            DateTime expiryDate = new DateTime(expYear, expMonth, 1).EndOfMonth();
          
            if (expiryDate <= today) {
                return "Expiry date must be in the future";
            }
            return null;
        }


        [DisplayName("Contacts")]
        [MemberOrder(5)]
        //[TableOrder(True, "Contact")]
        public virtual ICollection<ContactCreditCard> ContactCreditCard {
            get { return _ContactCreditCard; }
            set { _ContactCreditCard = value; }
        }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region Logic for creating new cards

        [Hidden]
        [NotPersisted]
        public ICreditCardCreator Creator { get; set; }

        [Hidden]
        [NotPersisted]
        public Contact ForContact { get; set; }

        #endregion

        public virtual string[] ChoicesCardType() {
            return new string[] {"Vista", "Distinguish", "SuperiorCard", "ColonialVoice"};
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
    }
}