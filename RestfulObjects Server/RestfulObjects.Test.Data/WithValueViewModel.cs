// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using NakedObjects.Security;

namespace RestfulObjects.Test.Data {
    public class WithValueViewModel : IViewModel {
        private DateTime aDateTimeValue = new DateTime(2012, 2, 10);

        [Key, Title, ConcurrencyCheck]
        public virtual int Id { get; set; }

        [PresentationHint("class3 class4")]
        public virtual int AValue { get; set; }

        [Disabled]
        public virtual int ADisabledValue { get; set; }

        [Hidden]
        public virtual int AHiddenValue { get; set; }

        public virtual int AChoicesValue { get; set; }

        [System.ComponentModel.DataAnnotations.MaxLength(101)]
        [RegEx(Validation = @"[A-Z]")]
        [Optionally]
        [DescribedAs("A string value for testing")]
        [MemberOrder(Sequence = "3")]
        public string AStringValue { get; set; }

        [Optionally]
        [DescribedAs("A datetime value for testing")]
        [Mask("d")]
        [MemberOrder(Sequence = "4")]
        public DateTime ADateTimeValue {
            get { return aDateTimeValue; }
            set { aDateTimeValue = value; }
        }

        [AuthorizeProperty(ViewUsers = "viewUser")]
        public virtual int AUserHiddenValue { get; set; }

        [AuthorizeProperty(EditUsers = "editUser")]
        public virtual int AUserDisabledValue { get; set; }

        [NakedObjectsIgnore]
        public string[] DeriveKeys() {
            return new[] {
                Id.ToString(),
                AValue.ToString(),
                ADisabledValue.ToString(),
                AHiddenValue.ToString(),
                AChoicesValue.ToString(),
                AStringValue,
                ADateTimeValue.Ticks.ToString(),
                AUserHiddenValue.ToString(),
                AUserDisabledValue.ToString()
            };
        }

        [NakedObjectsIgnore]
        public void PopulateUsingKeys(string[] instanceId) {
            Id = int.Parse(instanceId[0]);
            AValue = int.Parse(instanceId[1]);
            ADisabledValue = int.Parse(instanceId[2]);
            AHiddenValue = int.Parse(instanceId[3]);
            AChoicesValue = int.Parse(instanceId[4]);
            AStringValue = instanceId[5];
            ADateTimeValue = new DateTime(long.Parse(instanceId[6]));
            AUserHiddenValue = int.Parse(instanceId[7]);
            AUserDisabledValue = int.Parse(instanceId[8]);
        }

        public virtual int[] ChoicesAChoicesValue() {
            return new[] {1, 2, 3};
        }

        public virtual string Validate(int aValue, int aChoicesValue) {
            if (aValue == 101 && aChoicesValue == 3) {
                return "Cross validation failed";
            }
            return "";
        }
    }
}