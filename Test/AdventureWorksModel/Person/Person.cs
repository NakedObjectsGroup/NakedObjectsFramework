// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NakedObjects;
using System.Collections.Generic;
using NakedObjects.Value;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorksModel {

    [IconName("cellphone.png")]
    public class Person : BusinessEntity {

        #region Life Cycle Methods
        public override void Persisting() {
            //base.Persisting();
            CreateSaltAndHash(InitialPassword);
            rowguid = Guid.NewGuid();
            BusinessEntityRowguid = rowguid;
            ModifiedDate = DateTime.Now;
            BusinessEntityModifiedDate = ModifiedDate;
        }

        public override void Updating() {
            //base.Updating();
            ModifiedDate = DateTime.Now;
            BusinessEntityModifiedDate = ModifiedDate;
        }
        #endregion

        #region AdditionalContactInfo

        [Optionally]
        [MemberOrder(30)]
        public virtual string AdditionalContactInfo { get; set; }

        #endregion

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            if (NameStyle) {
                t.Append(LastName).Append(FirstName);
            }
            else {
                t.Append(FirstName).Append(LastName);
            }
            return t.ToString();
        }

        #endregion

        [NotPersisted]
        [NakedObjectsIgnore]
        public IBusinessEntity ForEntity { get; set; }

        [MemberOrder(2)]
        [NotPersisted][Hidden(WhenTo.OncePersisted)]
        public ContactType ContactType { get; set; }

        public void Persisted() {
                var relationship = Container.NewTransientInstance<BusinessEntityContact>();
                relationship.BusinessEntityID =  ForEntity.BusinessEntityID;
                relationship.PersonID = this.BusinessEntityID;
                relationship.ContactTypeID = ContactType.ContactTypeID;
                Container.Persist(ref relationship);
        }

        #region Name fields

        #region NameStyle

        [MemberOrder(15), DefaultValue(false), DisplayName("Reverse name order")]
        public virtual bool NameStyle { get; set; }

        #endregion

        #region Title

        [Optionally]
        [StringLength(8)]
        [MemberOrder(11)]
        public virtual string Title { get; set; }

        #endregion

        #region FirstName

        [StringLength(50)]
        [MemberOrder(12)]
        public virtual string FirstName { get; set; }

        #endregion

        #region MiddleName

        [StringLength(50)]
        [Optionally]
        [MemberOrder(13)]
        public virtual string MiddleName { get; set; }

        #endregion

        #region LastName

        [StringLength(50)]
        [MemberOrder(14)]
        public virtual string LastName { get; set; }

        #endregion

        #region Suffix

        [Optionally]
        [StringLength(10)]
        [MemberOrder(15)]
        public virtual string Suffix { get; set; }

        #endregion

        #endregion

        #region Communication


        #region EmailPromotion

        [MemberOrder(21), DefaultValue(1)]
        public virtual EmailPromotion EmailPromotion { get; set; }

        #endregion

        #endregion

        #region Password

        [NakedObjectsIgnore]
        public virtual Password Password { get; set; }
      
        #region ChangePassword (Action)

        [Hidden(WhenTo.UntilPersisted)]
        [MemberOrder(1)]
        public void ChangePassword([DataType(DataType.Password)] string oldPassword, [DataType(DataType.Password)] string newPassword, [Named("New Password (Confirm)"), DataType(DataType.Password)] string confirm) {
            CreateSaltAndHash(newPassword);
        }

        internal void CreateSaltAndHash(string newPassword) {
            Password.PasswordSalt = CreateRandomSalt();
            Password.PasswordHash = Hashed(newPassword, Password.PasswordSalt);
        }

        public virtual string ValidateChangePassword(string oldPassword, string newPassword, string confirm) {
            var rb = new ReasonBuilder();
            //if (Hashed(oldPassword, Password.PasswordSalt) != Password.PasswordHash) {
            //    rb.Append("Old Password is incorrect");
            //}
            if (newPassword != confirm) {
                rb.Append("New Password and Confirmation don't match");
            }
            if (newPassword.Length < 6) {
                rb.Append("New Password must be at least 6 characters");
            }
            if (newPassword == oldPassword) {
                rb.Append("New Password should be different from Old Password");
            }
            return rb.Reason;
        }

        #endregion

        #region InitialPassword Property

        //See Persisting method
        [NotPersisted]
        [Hidden(WhenTo.OncePersisted)]
        [MemberOrder(27)]
        [DataType(DataType.Password)]
        public string InitialPassword { get; set; }

        public void ModifyInitialPassword([DataType(DataType.Password)] string value) {
            InitialPassword = value;
            ChangePassword(null, value, null);
        }

        #endregion

        private static string Hashed(string password, string salt) {
            string saltedPassword = password + salt;
            byte[] data = Encoding.UTF8.GetBytes(saltedPassword);
            byte[] hash = SHA256.Create().ComputeHash(data);
            char[] chars = Encoding.UTF8.GetChars(hash);
            return new string(chars);
        }

        private static string CreateRandomSalt() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var output = new StringBuilder();
            for (int i = 1; i <= 7; i++) {
                output.Append(chars.Substring(random.Next(chars.Length - 1), 1));
            }
            output.Append("=");
            return output.ToString();
        }

        #endregion

        //To test a null image
        [NotMapped]
        public virtual Image Photo { get { return null; } }

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        public override bool HideContacts() {
            return true;
        }

        #region EmailAddresses (collection)
        private ICollection<EmailAddress> _EmailAddresses = new List<EmailAddress>();


        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false, nameof(EmailAddress.EmailAddress1))] 
        public virtual ICollection<EmailAddress> EmailAddresses {
            get {
                return _EmailAddresses;
            }
            set {
                _EmailAddresses = value;
            }
        }
        #endregion

        #region PhoneNumbers (collection)
        private ICollection<PersonPhone> _PhoneNumbers = new List<PersonPhone>();

        [AWNotCounted]
        [TableView(false, 
            nameof(PersonPhone.PhoneNumberType),
            nameof(PersonPhone.PhoneNumber))] 
        public virtual ICollection<PersonPhone> PhoneNumbers {
            get {
                return _PhoneNumbers;
            }
            set {
                _PhoneNumbers = value;
            }
        }

        public void CreateNewPhoneNumber(PhoneNumberType type, 
            [RegularExpression(@"[0-9][0-9\s-]+")]string phoneNumber)
        {
            var pp = Container.NewTransientInstance<PersonPhone>();
            pp.BusinessEntityID = this.BusinessEntityID;
            pp.Person = this;
            pp.PhoneNumberType = type;
            pp.PhoneNumberTypeID = type.PhoneNumberTypeID;
            pp.PhoneNumber = phoneNumber;
            Container.Persist(ref pp);
            this.PhoneNumbers.Add(pp);
        }


        #endregion


        [NakedObjectsIgnore]
        public virtual Employee Employee { get; set; }

        #region CreditCards
        public CreditCard CreateNewCreditCard() {
            var newCard = Container.NewTransientInstance<CreditCard>();
            newCard.ForContact = this;
            return newCard;
        }

        public IList<CreditCard> ListCreditCards() {
            int id = this.BusinessEntityID;
            return Container.Instances<PersonCreditCard>().Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).ToList();
        }

        public IQueryable<CreditCard> RecentCreditCards()
        {
            int id = this.BusinessEntityID;
            return Container.Instances<PersonCreditCard>().Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).OrderByDescending(cc => cc.ModifiedDate);
        }

        #endregion

        #region Actions for test purposes only

        public void UpdateMiddleName(string newName)
        {
            MiddleName = newName;
        }

        [QueryOnly] //This action is deliberately marked QueryOnly even
        //though it is not. Don't change it!
        public void UpdateSuffix(string newSuffix)
        {
            Suffix = newSuffix;
        }


        //To test a ViewModelEdit
        public EmailTemplate CreateEmail()
        {
            var email = Container.NewViewModel<EmailTemplate>();
            email.Status = EmailStatus.New;
            return email;
        }

        public void CreateLetter(Address toAddress)
        {

        }


        public Address Default0CreateLetter()
        {
            return this.Addresses.First().Address;
        }
        #endregion
    }
    public class EmailTemplate : IViewModelEdit
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region 
        public string[] DeriveKeys()
        {
            return new[] {
                To,
                From,
                Subject,
                Message,
                Status.ToString() }; 
        }

        public void PopulateUsingKeys(string[] keys)
        {
            this.To = keys[0];
            this.From = keys[1];
            this.Subject = keys[2];
            this.Message = keys[3];
            this.Status =  (EmailStatus)Enum.Parse(typeof(EmailStatus), keys[4]);
        }
        #endregion


        public override string ToString()
        {
            var t = Container.NewTitleBuilder();
            t.Append(Status).Append("email");
            return t.ToString();
        }

        [MemberOrder(10), Optionally]
        public virtual string To { get; set; }

        [MemberOrder(20), Optionally]
        public virtual string From { get; set; }

        [MemberOrder(30), Optionally]
        public virtual string Subject { get; set; }

        public virtual IQueryable<string> AutoCompleteSubject([MinLength(2)] string value) {
            var matchingNames = new List<string> { "Subject1", "Subject2", "Subject3" };
            return from p in matchingNames.AsQueryable() select p.Trim();
        }

        [MemberOrder(40), Optionally]
        public virtual string Message { get; set; }

        [Disabled]
        public virtual EmailStatus Status { get; set; }

        public EmailTemplate Send()
        {
            this.Status = EmailStatus.Sent;
            return this;
        }

    }
    public enum EmailStatus
    {
        New, Sent, Failed
    }

    public enum EmailPromotion
    {
        NoPromotions = 0,
        AdventureworksOnly = 1,
        AdventureworksAndPartners = 2   
    }
}