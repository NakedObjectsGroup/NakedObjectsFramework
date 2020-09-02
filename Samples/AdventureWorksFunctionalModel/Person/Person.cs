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
using NakedFunctions;
using static NakedFunctions.Result;

namespace AdventureWorksModel {

    [IconName("cellphone.png")]
    public class Person : BusinessEntity, IHasRowGuid, IHasModifiedDate {
        public Person() {
           
        }

        public Person(string additionalContactInfo,
                        bool nameStyle,
                      string title,
                      string firstName,
                      string middleName,
                      string lastName,
                      string suffix,
                      int emailPromotion,
                      Password password,
                      Guid rowGuid,
                      DateTime modifiedDate,
                      ICollection<PersonPhone> phoneNumbers,
                      ICollection<EmailAddress> emailAddresses,
                      int businessEntityID,
                      ICollection<BusinessEntityAddress> addresses,
                      ICollection<BusinessEntityContact> contacts,
                      Guid businessEntityRowguid,
                      DateTime businessEntityModifiedDate      
                      ) : base(businessEntityID, addresses, contacts, businessEntityRowguid, businessEntityModifiedDate)
        {
            AdditionalContactInfo = additionalContactInfo;
            NameStyle = nameStyle;
            Title = title;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            Suffix = suffix;
            EmailPromotion = (EmailPromotion)emailPromotion;
            Password = password;
            ModifiedDate = modifiedDate;
            rowguid = rowGuid;
            EmailAddresses = emailAddresses;
            PhoneNumbers = phoneNumbers;
        }

        [Optionally]
        [MemberOrder(30)]
        public virtual string AdditionalContactInfo { get; set; }

        [NotPersisted]
        [NakedObjectsIgnore]
        public IBusinessEntity ForEntity { get; set; }

        [MemberOrder(2)]
        [NotPersisted][Hidden(WhenTo.OncePersisted)]
        public ContactType ContactType { get; set; }

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

        [MemberOrder(21), DefaultValue(1)]
        public virtual EmailPromotion EmailPromotion { get; set; }

        [NakedObjectsIgnore]
        public virtual Password Password { get; set; }
      
        //See Persisting method
        [NotPersisted]
        [Hidden(WhenTo.OncePersisted)]
        [MemberOrder(27)]
        [DataType(DataType.Password)]
        public string InitialPassword { get; set; }


        //To test a null image
        [NotMapped]
        public virtual Image Photo { get { return null; } }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false, nameof(EmailAddress.EmailAddress1))] 
        public virtual ICollection<EmailAddress> EmailAddresses { get; set; }

        [AWNotCounted]
        [TableView(false, 
            nameof(PersonPhone.PhoneNumberType),
            nameof(PersonPhone.PhoneNumber))] 
        public virtual ICollection<PersonPhone> PhoneNumbers { get; set; }

        [NakedObjectsIgnore]
        public virtual Employee Employee { get; set; }

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

    }

    public static class PersonFunctions
    {
        public static string Title(this Person p)
        {
            return p.CreateTitle(p.NameStyle ? $"{p.LastName} {p.FirstName}" : $"{p.FirstName} {p.LastName}");
        }

        #region Life Cycle Methods
        public static BusinessEntityContact Persisted(Person p, Guid guid, DateTime now)
        {
            return new BusinessEntityContact(
                p.ForEntity.BusinessEntityID,
                p,
                p.BusinessEntityID,
                p,
                p.ContactType.ContactTypeID,
                p.ContactType,
                guid,
                now
                );
        }

        public static Person Persisting(Person p, [Injected] Guid guid, [Injected] DateTime now)
        {
            return Updating(p, now).With(x => x.rowguid, guid).CreateSaltAndHash(p.InitialPassword)
                .With(x => x.BusinessEntityRowguid, guid)
                .With(x => x.BusinessEntityModifiedDate, now);
        }

        public static Person Updating(Person p, [Injected] DateTime now)
        {
            return p.With(x => x.BusinessEntityModifiedDate, now).With(x => x.ModifiedDate, now);
        }
        #endregion

        #region ChangePassword (Action)

        [Hidden(WhenTo.UntilPersisted)]
        [MemberOrder(1)]
        public static (Person,Person) ChangePassword(this Person p, [DataType(DataType.Password)] string oldPassword, [DataType(DataType.Password)] string newPassword, [Named("New Password (Confirm)"), DataType(DataType.Password)] string confirm)
        {
            return  DisplayAndPersist(CreateSaltAndHash(p, newPassword));
        }

        internal static Person CreateSaltAndHash(this Person p, string newPassword)
        {
            return p.With(x => x.Password.PasswordSalt,CreateRandomSalt())
                .With(x => x.Password.PasswordHash, Hashed(newPassword, p.Password.PasswordSalt));
        }

        public static string ValidateChangePassword(this Person p, string oldPassword, string newPassword, string confirm)
        {
            var rb = new ReasonBuilder();
            //if (Hashed(oldPassword, Password.PasswordSalt) != Password.PasswordHash) {
            //    rb.Append("Old Password is incorrect");
            //}
            if (newPassword != confirm)
            {
                rb.Append("New Password and Confirmation don't match");
            }
            if (newPassword.Length < 6)
            {
                rb.Append("New Password must be at least 6 characters");
            }
            if (newPassword == oldPassword)
            {
                rb.Append("New Password should be different from Old Password");
            }
            return rb.Reason;
        }
        #endregion


        private static string Hashed(this string password, string salt)
        {
            string saltedPassword = password + salt;
            byte[] data = Encoding.UTF8.GetBytes(saltedPassword);
            byte[] hash = SHA256.Create().ComputeHash(data);
            char[] chars = Encoding.UTF8.GetChars(hash);
            return new string(chars);
        }

        private static string CreateRandomSalt()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var output = new StringBuilder();
            for (int i = 1; i <= 7; i++)
            {
                output.Append(chars.Substring(random.Next(chars.Length - 1), 1));
            }
            output.Append("=");
            return output.ToString();
        }

        #region Actions for test purposes only

        public static (Person, Person) UpdateMiddleName(this Person p, string newName)
        {
            return DisplayAndPersist(p.With(x => x.MiddleName, newName));
        }

        public static (Person, Person) UpdateSuffix(this Person p, string newSuffix)
        {
            return DisplayAndPersist(p.With(x => x.Suffix, newSuffix));
        }


        //To test a ViewModelEdit
        public static EmailTemplate CreateEmail(this Person p)
        {
            return new EmailTemplate("", "", "", "",EmailStatus.New);
        }

        public static object CreateLetter(this Person p,Address toAddress)
        {
            //No real implementation; function is for framework testing purposes only
            return null;
        }

        public static Address Default0CreateLetter(this Person p)
        {
            return p.Addresses.First().Address;
        }
        #endregion

        #region CreditCards

        //TODO: This must be changed to request all fields & return object to be persisted.
        public static (CreditCard, CreditCard) CreateNewCreditCard(this Person p)
        {
            var c = new CreditCard(p);
            return Result.DisplayAndPersist(c);
        }

        public static IList<CreditCard> ListCreditCards(this Person p, [Injected] IQueryable<PersonCreditCard> pccs)
        {
            int id = p.BusinessEntityID;
            return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).ToList();
        }

        public static IQueryable<CreditCard> RecentCreditCards(this Person p, [Injected] IQueryable<PersonCreditCard> pccs)
        {
            int id = p.BusinessEntityID;
            return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).OrderByDescending(cc => cc.ModifiedDate);
        }

        #endregion

        //Yes, this could be done with a Hidden attribute, but is done as a function here for testing.
        public static bool HideContacts(this Person p)
        {
            return true;
        }

        public static (Person, PersonPhone) CreateNewPhoneNumber(this Person p, PhoneNumberType type,
    [RegularExpression(@"[0-9][0-9\s-]+")]string phoneNumber, [Injected] DateTime now)
        {
            return DisplayAndPersistDifferentItems(p, new PersonPhone(p.BusinessEntityID, p, type, type.PhoneNumberTypeID, phoneNumber, now));
        }
    }
}