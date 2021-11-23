






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
        public bool NameStyle { get; init; }

        [MemberOrder(11)]
        public string? Title { get; init; }

        [MemberOrder(12)]
        public string FirstName { get; init; } = "";

        [MemberOrder(13)]
        public string? MiddleName { get; init; }

        [MemberOrder(14)]
        public string LastName { get; init; } = "";

        [MemberOrder(15)]
        public string? Suffix { get; init; }

        #endregion

        [Hidden]
        public string? PersonType { get; init; }

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
        public string? AdditionalContactInfo { get; init; }

        [Hidden]
        public virtual Employee? Employee { get; init; }


        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => NameStyle ? $"{LastName} {FirstName}" : $"{FirstName} {LastName}";
    }
}