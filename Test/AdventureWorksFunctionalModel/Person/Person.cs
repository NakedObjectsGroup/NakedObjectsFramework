// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
    public class Person : BusinessEntity, IHasRowGuid, IHasModifiedDate {
        public Person() { }

        public Person(Person cloneFrom) : base(cloneFrom)
        {
            NameStyle = cloneFrom.NameStyle;
            Title = cloneFrom.Title;
            FirstName = cloneFrom.FirstName;
            MiddleName = cloneFrom.MiddleName;
            LastName = cloneFrom.LastName;
            Suffix = cloneFrom.Suffix;
            PersonType = cloneFrom.PersonType;
            EmailPromotion = cloneFrom.EmailPromotion;
            EmailAddresses = cloneFrom.EmailAddresses;
            PhoneNumbers = cloneFrom.PhoneNumbers;
            Password = cloneFrom.Password;
            AdditionalContactInfo = cloneFrom.AdditionalContactInfo;
            Employee = cloneFrom.Employee;
            ModifiedDate = cloneFrom.ModifiedDate;
            rowguid = cloneFrom.rowguid;
        }

        #region Name fields

        [MemberOrder(15)]
        [Named("Reverse name order")]
        public virtual bool NameStyle { get; init; }

        [MemberOrder(11)]
        public virtual string? Title { get; init; }

        [MemberOrder(12)]
        public virtual string FirstName { get; init; } = "";

        [MemberOrder(13)]
        public virtual string? MiddleName { get; init; }

        [MemberOrder(14)]
        public virtual string LastName { get; init; } = "";

        [MemberOrder(15)]
        public virtual string? Suffix { get; init; }

        #endregion

        [Hidden]
        public virtual string? PersonType { get; init; }

        [MemberOrder(21)] [Hidden]
        public virtual EmailPromotion EmailPromotion { get; init; }

        //To test a null image
        //[NotMapped]
        //public virtual Image Photo { get { return null; } }

        [RenderEagerly]
        [TableView(false, nameof(EmailAddress.EmailAddress1))]
        public virtual ICollection<EmailAddress> EmailAddresses { get; init; } = new List<EmailAddress>();

        [AWNotCounted]
        [TableView(false,
                   nameof(PersonPhone.PhoneNumberType),
                   nameof(PersonPhone.PhoneNumber))]
        public virtual ICollection<PersonPhone> PhoneNumbers { get; init; } = new List<PersonPhone>();

        public virtual Password? Password { get; init; }

        [MemberOrder(30)]
        public virtual string? AdditionalContactInfo { get; init; }

        [Hidden]
        public virtual Employee? Employee { get; init; }


        [MemberOrder(99)]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        public override string ToString() => NameStyle ? $"{LastName} {FirstName}" : $"{FirstName} {LastName}";
    }
}