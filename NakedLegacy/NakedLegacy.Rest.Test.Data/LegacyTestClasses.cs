// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFramework.Core.Util;
using NakedLegacy.Reflector.Component;
using NakedLegacy.Types;
using NakedLegacy.Types.Container;
using NakedObjects;

// ReSharper disable InconsistentNaming

namespace NakedLegacy.Rest.Test.Data;

public interface ILegacyRoleInterface { }

public class SimpleService {
    public ClassWithTextString GetClassWithTextString() => null;
}

public class ClassWithTextString {
    private AppLib.TextString _name;
    public string name;

    [Key]
    public int Id { get; init; }

    public AppLib.TextString Name => _name ??= new AppLib.TextString(name, s => name = s);

    public ITitle Title() => Name.Title();

    public ClassWithTextString ActionUpdateName(AppLib.TextString newName) {
        Name.Value = newName.Value;
        return this;
    }
}

public class ClassWithInternalCollection {
    private InternalCollection _testCollection;

    public IDomainObjectContainer Container { private get; set; }

    public virtual ICollection<ClassWithTextString> _TestCollection { get; } = new List<ClassWithTextString>();

    [Key]
    public int Id { get; init; }

    public InternalCollection TestCollection => _testCollection ??= new InternalCollection<ClassWithTextString>(_TestCollection);

    public ClassWithInternalCollection ActionUpdateTestCollection(AppLib.TextString newName) {
        var name = newName.Value;
        var bill = Container.Instances<ClassWithTextString>().Single(c => c.name == name);
        _TestCollection.Add(bill);

        return this;
    }
}

public class ClassWithNOFInternalCollection
{
    private InternalCollection _testCollection;

    public virtual ICollection<ClassWithString> _TestCollection { get; } = new List<ClassWithString>();

    [Key]
    public int Id { get; init; }

    public InternalCollection CollectionOfNOFClass => _testCollection ??= new InternalCollection<ClassWithString>(_TestCollection);
}

public class ClassWithActionAbout {
    public static bool TestInvisibleFlag = false;
    public static int AboutCount;

    [Key]
    public int Id { get; init; }

    public void actionTestAction() {
        // do something
    }

    public void aboutActionTestAction(ActionAbout actionAbout) {
        AboutCount++;
        actionAbout.Visible = !TestInvisibleFlag;
    }
}

public class ClassWithFieldAbout {
    public static bool TestInvisibleFlag = false;

    [Key]
    public int Id { get; init; }

    public AppLib.TextString Name => new("");

    public ITitle Title() => Name.Title();

    public void aboutName(FieldAbout fieldAbout, AppLib.TextString name) {
        fieldAbout.Visible = !TestInvisibleFlag;
    }
}

public class ClassWithLinkToNOFClass
{
    [Key]
    public int Id { get; init; }

    public virtual ClassWithString LinkToNOFClass { get; set; }
}

public class ClassWithReferenceProperty {
    [Key]
    public int Id { get; init; }

    public virtual ClassWithTextString ReferenceProperty { get; set; }

    public ClassWithReferenceProperty actionUpdateReferenceProperty(ClassWithTextString newReferenceProperty) {
        ReferenceProperty = newReferenceProperty;
        return this;
    }

    public void AboutActionUpdateReferenceProperty(ActionAbout actionAbout, ClassWithTextString newReferenceProperty) {
        if (actionAbout.TypeCode is AboutTypeCodes.Visible) {
            actionAbout.Visible = true;
        }

        if (actionAbout.TypeCode is AboutTypeCodes.Usable) {
            actionAbout.Usable = true;
        }
    }
}

public class LegacyClassWithInterface : IRoleInterface {
    [Key]
    public int Id { get; init; }
}

public class ClassWithMenu {
    [Key]
    public int Id { get; init; }

    public AppLib.TextString Name => new($"{nameof(GetType)}/{Id}");

    public ITitle Title() => Name.Title();

    public ClassWithMenu ActionMethod1() => this;
    public ClassWithMenu actionMethod2() => this;

    private static IContainer Container => ThreadLocals.Container;

    public static ClassWithTextString ActionMenuAction() => (ClassWithTextString) Container.AllInstances(typeof(ClassWithTextString)).First();

    public static MainMenu menuOrder() {
        var menu = new MainMenu();
        menu.Menus.Add(new Menu("Method1"));

        var newSubMenu = new SubMenu("Submenu1");
        menu.Menus.Add(newSubMenu);
        newSubMenu.Menus.Add(new Menu("Method2"));
        return menu;
    }

    public static MainMenu sharedMenuOrder() {
        var menu = new MainMenu();
        menu.Menus.Add(new Menu("MenuAction"));
        return menu;
    }
}

public class ClassWithDate {
    private AppLib.NODate _date;
    public DateTime date;

    [Key]
    public int Id { get; init; }

    public AppLib.NODate Date => _date ??= new AppLib.NODate(date, d => date = d);

    public ITitle Title() => Date.Title();

    public ClassWithDate ActionUpdateDate(AppLib.NODate newDate) {
        Date.Value = newDate.Value;
        return this;
    }
}

public class ClassWithTimeStamp {
    private AppLib.TimeStamp _timestamp;
    public DateTime date;

    [Key]
    public int Id { get; init; }

    [ConcurrencyCheck]
    public AppLib.TimeStamp TimeStamp => _timestamp ??= new AppLib.TimeStamp(date, d => date = d);

    public ITitle Title() => TimeStamp.Title();

    public ClassWithTimeStamp ActionUpdateTimeStamp(AppLib.TimeStamp newTimeStamp) {
        TimeStamp.Value = newTimeStamp.Value;
        return this;
    }
}

public class ClassWithWholeNumber {
    private AppLib.WholeNumber _wholeNumber;
    public int number;

    [Key]
    public int Id { get; init; }

    public AppLib.WholeNumber WholeNumber => _wholeNumber ??= new AppLib.WholeNumber(number, i => number = i);

    public static bool TestVisible { get; set; } = true;

    public ITitle Title() => WholeNumber.Title();

    public ClassWithWholeNumber actionUpdateWholeNumber(AppLib.WholeNumber newWholeNumber) {
        WholeNumber.Value = newWholeNumber.Value;
        return this;
    }

    public void AboutActionUpdateWholeNumber(ActionAbout actionAbout, AppLib.WholeNumber newWholeNumber) {
        if (actionAbout.TypeCode is AboutTypeCodes.Visible) {
            actionAbout.Visible = TestVisible;
        }

        if (actionAbout.TypeCode is AboutTypeCodes.Usable) {
            actionAbout.Usable = true;
        }
    }
}

public class ClassWithLogical {
    private AppLib.Logical _logical;
    public bool boolean;

    [Key]
    public int Id { get; init; }

    public AppLib.Logical Logical => _logical ??= new AppLib.Logical(boolean, i => boolean = i);

    public static bool TestVisible { get; set; } = true;

    public ITitle Title() => Logical.Title();

    public ClassWithLogical actionUpdateLogical(AppLib.Logical newLogical) {
        Logical.Value = newLogical.Value;
        return this;
    }

    public void AboutActionUpdateLogical(ActionAbout actionAbout, AppLib.Logical newLogical) {
        if (actionAbout.TypeCode is AboutTypeCodes.Visible) {
            actionAbout.Visible = TestVisible;
        }

        if (actionAbout.TypeCode is AboutTypeCodes.Usable) {
            actionAbout.Usable = true;
        }
    }
}

public class ClassWithMoney {
    private AppLib.Money _money;
    public decimal amount;

    [Key]
    public int Id { get; init; }

    public AppLib.Money Money => _money ??= new AppLib.Money(amount, i => amount = i);

    public static bool TestVisible { get; set; } = true;

    public ITitle Title() => Money.Title();

    public ClassWithMoney actionUpdateMoney(AppLib.Money newMoney) {
        Money.Value = newMoney.Value;
        return this;
    }

    public void AboutActionUpdateMoney(ActionAbout actionAbout, AppLib.Money newMoney) {
        if (actionAbout.TypeCode is AboutTypeCodes.Visible) {
            actionAbout.Visible = TestVisible;
        }

        if (actionAbout.TypeCode is AboutTypeCodes.Usable) {
            actionAbout.Usable = true;
        }
    }
}