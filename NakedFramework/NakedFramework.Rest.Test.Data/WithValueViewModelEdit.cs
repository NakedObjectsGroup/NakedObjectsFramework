﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFramework;
using NakedObjects;
using NakedObjects.Security;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data;

public class WithValueViewModelEdit : IViewModelEdit {
    private int deriveCheck;
    private int populateCheck;

    [Key]
    [Title]
    [ConcurrencyCheck]
    public virtual int Id { get; set; }

    [PresentationHint("class3 class4")]
    [UrlLink(true, "Name")]
    public virtual int AValue { get; set; }

    [Disabled]
    public virtual int ADisabledValue { get; set; }

    [Hidden(WhenTo.Always)]
    public virtual int AHiddenValue { get; set; }

    public virtual int AChoicesValue { get; set; }

    [MaxLength(101)]
    [RegEx(Validation = @"[A-Z]")]
    [Optionally]
    [DescribedAs("A string value for testing")]
    [MemberOrder(Sequence = "3")]
    [UrlLink(true, "Name")]
    public string AStringValue { get; set; }

    [Optionally]
    [DescribedAs("A datetime value for testing")]
    [Mask("d")]
    [MemberOrder(Sequence = "4")]
    public DateTime ADateTimeValue { get; set; } = new(2012, 2, 10);

    [Optionally]
    [DescribedAs("A timespan value for testing")]
    [Mask("d")]
    [MemberOrder(Sequence = "5")]
    public virtual TimeSpan ATimeSpanValue { get; set; } = new(1, 2, 3, 4, 5);

#pragma warning disable CS0618
    [AuthorizeProperty(ViewUsers = "viewUser")]
#pragma warning restore CS0618
    public virtual int AUserHiddenValue { get; set; }

#pragma warning disable CS0618
    [AuthorizeProperty(EditUsers = "editUser")]
#pragma warning restore CS0618
    public virtual int AUserDisabledValue { get; set; }

    public virtual int[] ChoicesAChoicesValue() {
        return new[] { 1, 2, 3 };
    }

    public virtual string Validate(int aValue, int aChoicesValue) {
        if (aValue == 101 && aChoicesValue == 3) {
            return "Cross validation failed";
        }

        return "";
    }

    #region IViewModelEdit Members

    [NakedObjectsIgnore]
    public string[] DeriveKeys() {
        deriveCheck++;

        if (deriveCheck > 1) {
            throw new Exception("Derive called multiple times");
        }

        return new[] {
            Id.ToString(),
            AValue.ToString(),
            ADisabledValue.ToString(),
            AHiddenValue.ToString(),
            AChoicesValue.ToString(),
            AStringValue,
            ADateTimeValue.Ticks.ToString(),
            ATimeSpanValue.Ticks.ToString(),
            AUserHiddenValue.ToString(),
            AUserDisabledValue.ToString()
        };
    }

    [NakedObjectsIgnore]
    public void PopulateUsingKeys(string[] instanceId) {
        populateCheck++;

        if (populateCheck > 1) {
            throw new Exception("PopulateUsingKeys called multiple times");
        }

        Id = int.Parse(instanceId[0]);
        AValue = int.Parse(instanceId[1]);
        ADisabledValue = int.Parse(instanceId[2]);
        AHiddenValue = int.Parse(instanceId[3]);
        AChoicesValue = int.Parse(instanceId[4]);
        AStringValue = instanceId[5];
        ADateTimeValue = new DateTime(long.Parse(instanceId[6]));
        ATimeSpanValue = new TimeSpan(long.Parse(instanceId[7]));
        AUserHiddenValue = int.Parse(instanceId[8]);
        AUserDisabledValue = int.Parse(instanceId[9]);
    }

    #endregion
}