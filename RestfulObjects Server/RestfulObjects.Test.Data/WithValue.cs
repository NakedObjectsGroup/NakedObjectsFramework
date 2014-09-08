// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using NakedObjects.Security;

namespace RestfulObjects.Test.Data {

    [PresentationHint("class1 class2")]
    public class WithValue {
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
        public virtual string AStringValue { get; set; }

        [Optionally]
        [DescribedAs("A datetime value for testing")]
        [Mask("d")]
        [MemberOrder(Sequence = "4")]
        public virtual DateTime ADateTimeValue {
            get { return aDateTimeValue; }
            set { aDateTimeValue = value; }
        }

        [AuthorizeProperty(ViewUsers = "viewUser")]
        public virtual int AUserHiddenValue { get; set; }

        [AuthorizeProperty(EditUsers = "editUser")]
        public virtual int AUserDisabledValue { get; set; }

        public virtual int[] ChoicesAChoicesValue() {
            return new[] {1, 2, 3};
        }

        public virtual string Validate(int aValue, int aChoicesValue) {
            if (aValue == 101 && aChoicesValue == 3) {
                return "Cross validation failed";
            }
            return "";
        }

        public virtual int AConditionalChoicesValue { get; set; }

        public virtual int[] ChoicesAConditionalChoicesValue(int aValue, string aStringValue) {
            return new[] {  aValue, aStringValue == null ? 0 : int.Parse(aStringValue)  };
        }
    }
}