// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;
using System.Collections.Generic;

namespace AW.Types {

        public record Person : BusinessEntity, IHasRowGuid, IHasModifiedDate {

        [Hidden]
        public virtual string PersonType { get; init; }

        #region Name fields

        [MemberOrder(15),  Named("Reverse name order")] 
        public virtual bool NameStyle { get; init; }

        [MemberOrder(11)]
        public virtual string Title { get; init; }

        [MemberOrder(12)]
        public virtual string FirstName { get; init; }

        [MemberOrder(13)]
        public virtual string MiddleName { get; init; }

        [MemberOrder(14)]
        public virtual string LastName { get; init; }

        [MemberOrder(15)]
        public virtual string Suffix { get; init; }
        #endregion

        [MemberOrder(21), Hidden]
        public virtual EmailPromotion EmailPromotion { get; init; }
    
        //To test a null image
        //[NotMapped]
        //public virtual Image Photo { get { return null; } }

        [RenderEagerly]
        [TableView(false, nameof(EmailAddress.EmailAddress1))] 
        public virtual ICollection<EmailAddress> EmailAddresses { get; init; }

        [AWNotCounted]
        [TableView(false,
                   nameof(PersonPhone.PhoneNumberType),
                   nameof(PersonPhone.PhoneNumber))]
        public virtual ICollection<PersonPhone> PhoneNumbers { get; init; } = new List<PersonPhone>();

        
        public virtual Password Password { get; init; }

        [MemberOrder(30)]
        public virtual string AdditionalContactInfo { get; init; }


        [Hidden]
        public virtual Employee Employee { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99)]        
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => NameStyle ? $"{LastName} {FirstName}" : $"{FirstName} {LastName}";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(Person other) => ReferenceEquals(this, other);
    }
}