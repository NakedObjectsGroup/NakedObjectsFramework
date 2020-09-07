// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;


using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NakedFunctions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations.Schema;


namespace AdventureWorksModel {

        public record Person : BusinessEntity, IHasRowGuid, IHasModifiedDate {
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

        
        [MemberOrder(30)]
        public virtual string AdditionalContactInfo { get; set; }

        
        [Hidden]
        public IBusinessEntity ForEntity { get; set; }

        [MemberOrder(2)]

        public ContactType ContactType { get; set; }

        #region Name fields

        #region NameStyle

        [MemberOrder(15), DefaultValue(false), Named("Reverse name order")]
        public virtual bool NameStyle { get; set; }

        #endregion

        #region Title

        
        
        [MemberOrder(11)]
        public virtual string Title { get; set; }

        #endregion

        #region FirstName

        
        [MemberOrder(12)]
        public virtual string FirstName { get; set; }

        #endregion

        #region MiddleName

        
        
        [MemberOrder(13)]
        public virtual string MiddleName { get; set; }

        #endregion

        #region LastName

        
        [MemberOrder(14)]
        public virtual string LastName { get; set; }

        #endregion

        #region Suffix

        
        
        [MemberOrder(15)]
        public virtual string Suffix { get; set; }

        #endregion

        #endregion

        [MemberOrder(21), DefaultValue(1)]
        public virtual EmailPromotion EmailPromotion { get; set; }

        [Hidden]
        public virtual Password Password { get; set; }
      

        //To test a null image
        [NotMapped]
        public virtual Image Photo { get { return null; } }

        [RenderEagerly]
        [TableView(false, nameof(EmailAddress.EmailAddress1))] 
        public virtual ICollection<EmailAddress> EmailAddresses { get; set; }

        [AWNotCounted]
        [TableView(false, 
            nameof(PersonPhone.PhoneNumberType),
            nameof(PersonPhone.PhoneNumber))] 
        public virtual ICollection<PersonPhone> PhoneNumbers { get; set; }

        [Hidden]
        public virtual Employee Employee { get; set; }

        #region Row Guid and Modified Date

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

    }

    public static class PersonFunctions
    {
        public static string Title(this Person p)
        {
            return p.ToString(); // CreateTitle(p.NameStyle ? $"{p.LastName} {p.FirstName}" : $"{p.FirstName} {p.LastName}");
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
            throw new NotImplementedException();
            ////return Updating(p, now) with {rowguid =  guid).CreateSaltAndHash(p.InitialPassword}
            ////     with {BusinessEntityRowguid =  guid}
            ////     with {BusinessEntityModifiedDate =  now};
        }

        public static Person Updating(Person p, [Injected] DateTime now)
        {
            return p with {BusinessEntityModifiedDate =  now, ModifiedDate = now};
        }
        #endregion

        #region ChangePassword (Action)

        
        [MemberOrder(1)]
        public static (Person,Password) ChangePassword(this Person p, [Password] string oldPassword, [Password] string newPassword, [Named("New Password (Confirm)"), Password] string confirm)
        {
            throw new NotImplementedException();
            //var p1 = CreateSaltAndHash(p, newPassword));
        }

        internal static Person CreateSaltAndHash(this Person p, string newPassword, [Injected] Guid guid, [Injected] DateTime now)
        {
            var pw = new Password(p.BusinessEntityID, Hashed(newPassword, p.Password.PasswordSalt),
                CreateRandomSalt(), guid, now);
            return p with { Password = pw };
        }

        public static string ValidateChangePassword(this Person p, string oldPassword, string newPassword, string confirm)
        {
            var reason = "";
            if (Hashed(oldPassword, p.Password.PasswordSalt) != p.Password.PasswordHash)
            {
                reason += "Old Password is incorrect";
            }
            if (newPassword != confirm)
            {
                reason += "New Password and Confirmation don't match";
            }
            if (newPassword.Length < 6)
            {
                reason += "New Password must be at least 6 characters";
            }
            if (newPassword == oldPassword)
            {
                reason += "New Password should be different from Old Password";
            }
            return reason;
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
            var p1 = p with {MiddleName =  newName};
            return (p1, p1);
        }

        public static (Person, Person) UpdateSuffix(this Person p, string newSuffix)
        {
            var p1 = p with {Suffix =  newSuffix};
            return (p1, p1);
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
            return (c,c);
        }

        public static IList<CreditCard> ListCreditCards(this Person p, IQueryable<PersonCreditCard> pccs)
        {
            int id = p.BusinessEntityID;
            return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).ToList();
        }

        public static IQueryable<CreditCard> RecentCreditCards(this Person p, IQueryable<PersonCreditCard> pccs)
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
    [RegEx(@"[0-9][0-9\s-]+")]string phoneNumber, [Injected] DateTime now)
        {
            return (p, new PersonPhone(p.BusinessEntityID, p, type, type.PhoneNumberTypeID, phoneNumber, now));
        }
    }
}