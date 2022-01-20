// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using NakedFramework.Core.Util;
using NakedLegacy.Rest.Test.Data.AppLib;
using NakedLegacy;
using NakedObjects;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace NakedLegacy.Rest.Test.Data;

public interface ILegacyRoleInterface { }

public class SimpleService : IContainerAware {
    public ClassWithTextString GetClassWithTextString() => Container.AllInstances<ClassWithTextString>().FirstOrDefault();
    public IContainer Container { get; set; }
}

public class ClassWithTextString {
    private TextString _name;
    public string name { get; set; }

    [Key]
    public int Id { get; init; }

    public TextString Name => _name ??= new TextString(name, s => name = s);

    public Title Title() => new Title(Name.Title());

    public ClassWithTextString ActionUpdateName(TextString newName) {
        Name.Value = newName.Value;
        return this;
    }
}

public class ClassWithBounded : IBounded
{
    private TextString _name;
    public string name;

    [Key]
    public int Id { get; init; }

    public TextString Name => _name ??= new TextString(name, s => name = s);

    public ClassWithBounded ChoicesProperty => this;

    public ITitle Title() => Name.Title();
}


public class ClassWithInternalCollection {
    private InternalCollection _testCollection;

    public IDomainObjectContainer Container { private get; set; }

    public virtual ICollection<ClassWithTextString> _TestCollection { get; } = new List<ClassWithTextString>();

    [Key]
    public int Id { get; init; }

    public InternalCollection TestCollection => _testCollection ??= new InternalCollection<ClassWithTextString>(_TestCollection);

    public ClassWithInternalCollection ActionUpdateTestCollection(TextString newName) {
        var name = newName.Value;
        var bill = Container.Instances<ClassWithTextString>().Single(c => c.name == name);
        _TestCollection.Add(bill);

        return this;
    }
}

public class ClassWithNOFInternalCollection {
    private InternalCollection _testCollection;

    public virtual ICollection<ClassWithString> _TestCollection { get; } = new List<ClassWithString>();

    [Key]
    public int Id { get; init; }

    public InternalCollection CollectionOfNOFClass => _testCollection ??= new InternalCollection<ClassWithString>(_TestCollection);
}

public class ClassWithActionAbout {
    public static bool TestInvisibleFlag = false;
    public static bool TestUsableFlag = false;
    public static bool TestValidFlag = false;
    public static string TestName = null;
    public static string TestDescription = null;

    public static int AboutCount;

    [Key]
    public int Id { get; init; }

    public void actionTestAction() {
        // do something
    }

    public void aboutActionTestAction(ActionAbout actionAbout) {
        AboutCount++;
        switch (actionAbout.TypeCode)
        {
            case AboutTypeCodes.Visible:
                actionAbout.Visible = !TestInvisibleFlag;
                break;
            case AboutTypeCodes.Usable:
                actionAbout.Usable = TestUsableFlag;
                if (!actionAbout.Usable)
                {
                    actionAbout.UnusableReason = "Unusable by about";
                }

                break;
            case AboutTypeCodes.Name:
                if (TestName is not null)
                {
                    actionAbout.Name = TestName;
                }

                if (TestDescription is not null)
                {
                    actionAbout.Description = TestDescription;
                }

                break;
            case AboutTypeCodes.Valid:
                //if (TestValidFlag && name.Value != "valid")
                //{
                //    actionAbout.IsValid = false;
                //    actionAbout.InvalidReason = "invalid by about";
                //}
                //else
                //{
                //    actionAbout.IsValid = true;
                //}

                break;
        }
    }

    public void actionTestActionWithParms(TextString ts, WholeNumber wn)
    {
        // values should not be null
        var t = ts.Value;
        var w = wn.Value;
    }

    public void aboutActionTestActionWithParms(ActionAbout actionAbout, TextString ts, WholeNumber wn) {
        AboutCount++;
        switch (actionAbout.TypeCode) {
            case AboutTypeCodes.Visible:
                actionAbout.Visible = !TestInvisibleFlag;
                break;
            case AboutTypeCodes.Usable:
                actionAbout.Usable = TestUsableFlag;
                if (!actionAbout.Usable) {
                    actionAbout.UnusableReason = "Unusable by about";
                }

                break;
            case AboutTypeCodes.Name:
                if (TestName is not null) {
                    actionAbout.Name = TestName;
                }

                if (TestDescription is not null) {
                    actionAbout.Description = TestDescription;
                }

                break;
            case AboutTypeCodes.Valid:
                switch (TestValidFlag) {
                    case true when ts.Value != "valid":
                        actionAbout.Usable = false;
                        actionAbout.UnusableReason = "ts is invalid";
                        break;
                    case true when wn.Value == 101:
                        actionAbout.Usable = false;
                        actionAbout.UnusableReason = "wn is invalid";
                        break;
                }
                break;
            case AboutTypeCodes.Parameters:
                if (TestName is not null) {
                    actionAbout.ParamLabels = new[] { "renamed param1", "renamed param2" };
                }
                break;
        }
    }
}

public class ClassWithFieldAbout {
    public static bool TestInvisibleFlag = false;
    public static bool TestUsableFlag = false;
    public static bool TestValidFlag = false;
    public static string TestName = null;
    public static string TestDescription = null;

    public static void ResetTest() {
        TestInvisibleFlag = TestUsableFlag = TestValidFlag = false;
        TestName = TestDescription = null;
    }

    private TextString _name;
    public string name;

    [Key]
    public int Id { get; init; }

    public virtual TextString Name => _name ??= new TextString(name, s => name = s);

    public ITitle Title() => Name.Title();

    public void aboutName(FieldAbout fieldAbout, TextString name) {
        switch (fieldAbout.TypeCode) {
            case AboutTypeCodes.Visible:
                fieldAbout.Visible = !TestInvisibleFlag;
                break;
            case AboutTypeCodes.Usable:
                fieldAbout.Usable = TestUsableFlag;
                if (!fieldAbout.Usable) {
                    fieldAbout.UnusableReason = "Unusable by about";
                }

                break;
            case AboutTypeCodes.Name:
                if (TestName is not null) {
                    fieldAbout.Name = TestName;
                }

                if (TestDescription is not null) {
                    fieldAbout.Description = TestDescription;
                }

                break;
            case AboutTypeCodes.Valid:
                if (TestValidFlag && name.Value != "valid") {
                    fieldAbout.IsValid = false;
                    fieldAbout.InvalidReason = "invalid by about";
                }
                else {
                    fieldAbout.IsValid = true;
                }

                break;
        }
    }
}

public class ClassWithLinkToNOFClass {
    [Key]
    public int Id { get; init; }

    public virtual ClassWithString LinkToNOFClass { get; set; }
}

public class ClassWithReferenceProperty : IContainerAware {
    [Key]
    public int Id { get; init; }

    public virtual ClassWithTextString ReferenceProperty { get; set; }

    public ClassWithReferenceProperty actionUpdateReferenceProperty(ClassWithTextString newReferenceProperty) {
        ReferenceProperty = newReferenceProperty;
        return this;
    }

    public ClassWithTextString actionGetObject(TextString name) {
        var ofName = name.Value;
        return Container.AllInstances<ClassWithTextString>().SingleOrDefault(c => c.name == ofName);
    }

    public ClassWithTextString actionGetObject1(TextString name)
    {
        var ofName = name.Value;
        var simpleService = (SimpleService)Container.Repository(typeof(SimpleService));
        return simpleService.GetClassWithTextString();
    }


    public void AboutActionUpdateReferenceProperty(ActionAbout actionAbout, ClassWithTextString newReferenceProperty) {
        if (actionAbout.TypeCode is AboutTypeCodes.Visible) {
            actionAbout.Visible = true;
        }

        if (actionAbout.TypeCode is AboutTypeCodes.Usable) {
            actionAbout.Usable = true;
        }
    }

    public IContainer Container { get; set; }
}

public class LegacyClassWithInterface : IRoleInterface {
    [Key]
    public int Id { get; init; }
}

public class ClassWithMenu {
    [Key]
    public int Id { get; init; }

    public TextString Name => new($"{nameof(ClassWithMenu)}/{Id}");

    private static IContainer Container => ThreadLocals.Container;

    public ITitle Title() => Name.Title();

    public ClassWithMenu ActionMethod1() => this;
    public ClassWithMenu actionMethod2() => this;

    public static ClassWithTextString ActionMenuAction() => (ClassWithTextString)Container.AllInstances(typeof(ClassWithTextString)).First();

    public static ArrayList ActionMenuAction1() => new(Container.AllInstances(typeof(ClassWithTextString)).ToList());

    public static IQueryable<ClassWithTextString> ActionMenuAction2() => Container.AllInstances<ClassWithTextString>();

    public static ClassWithTextString ActionCreateTransient() => Container.CreateTransientInstance(typeof(ClassWithTextString)) as ClassWithTextString;

    public static ClassWithTextString ActionPersistTransient() {
        var o = Container.CreateTransientInstance(typeof(ClassWithTextString));
        ((ClassWithTextString)o).Name.Value = "Jenny";
        Container.MakePersistent(ref o);
        return o as ClassWithTextString;
    }

    public static ClassWithTextString ActionMenuActionWithParm(TextString ts) {
        foreach (ClassWithTextString cts in Container.AllInstances(typeof(ClassWithTextString))) {
            if (cts.Name.Value == ts.Value) {
                return cts;
            }
        }

        return null;
    }

    public static IMenu menuOrder()
    {
        var menu = new Menu("ClassWithMenu Menu");
        menu.MenuItems().Add(new MenuAction(nameof(ClassWithMenu.ActionMethod1)));

        var newSubMenu = new Menu("Submenu1");
        menu.MenuItems().Add(newSubMenu);
        newSubMenu.MenuItems().Add(new MenuAction(nameof(ClassWithMenu.actionMethod2)));
        return menu;
    }

    public static IMenu sharedMenuOrder() {
        var menu = new Menu("ClassWithMenu Main Menu");
        menu.MenuItems().Add(new MenuAction(nameof(ActionMenuAction)) { DisplayName = "Renamed menu Action" });
        menu.MenuItems().Add(new MenuAction(nameof(ActionMenuAction1)));
        menu.MenuItems().Add(new MenuAction(nameof(ActionMenuAction2)));
        menu.MenuItems().Add(new MenuAction(nameof(ActionMenuActionWithParm)));
        menu.MenuItems().Add(new MenuAction(nameof(ActionCreateTransient)));
        menu.MenuItems().Add(new MenuAction(nameof(ActionPersistTransient)));
        return menu;
    }
}

public class ClassWithDate {
    private NODate _date;
    public DateTime date;

    private NODateNullable _date1;
    public DateTime? date1;

    [Key]
    public int Id { get; init; }

    public NODate Date => _date ??= new NODate(date, d => date = d);

    public NODateNullable DateNullable => _date1 ??= new NODateNullable(date1, d => date1 = d);

    public ITitle Title() => Date.Title();

    public ClassWithDate ActionUpdateDate(NODate newDate) {
        Date.Value = newDate.Value;
        return this;
    }
}

public class ClassWithTimeStamp {
    private TimeStamp _timestamp;
    public DateTime date;

    [Key]
    public int Id { get; init; }

    [ConcurrencyCheck]
    public TimeStamp TimeStamp => _timestamp ??= new TimeStamp(date, d => date = d);

    public ITitle Title() => TimeStamp.Title();

    public ClassWithTimeStamp ActionUpdateTimeStamp(TimeStamp newTimeStamp) {
        TimeStamp.Value = newTimeStamp.Value;
        return this;
    }
}

public class ClassWithWholeNumber {
    private WholeNumber _wholeNumber;
    public int number;

    [Key]
    public int Id { get; init; }

    public WholeNumber WholeNumber => _wholeNumber ??= new WholeNumber(number, i => number = i);

    public static bool TestVisible { get; set; } = true;

    public ITitle Title() => WholeNumber.Title();

    public ClassWithWholeNumber actionUpdateWholeNumber(WholeNumber newWholeNumber) {
        WholeNumber.Value = newWholeNumber.Value;
        return this;
    }

    public void AboutActionUpdateWholeNumber(ActionAbout actionAbout, WholeNumber newWholeNumber) {
        if (actionAbout.TypeCode is AboutTypeCodes.Visible) {
            actionAbout.Visible = TestVisible;
        }

        if (actionAbout.TypeCode is AboutTypeCodes.Usable) {
            actionAbout.Usable = true;
        }
    }
}

public class ClassWithLogical {
    private Logical _logical;
    public bool boolean;

    [Key]
    public int Id { get; init; }

    public Logical Logical => _logical ??= new Logical(boolean, i => boolean = i);

    public static bool TestVisible { get; set; } = true;

    public ITitle Title() => Logical.Title();

    public ClassWithLogical actionUpdateLogical(Logical newLogical) {
        Logical.Value = newLogical.Value;
        return this;
    }

    public void AboutActionUpdateLogical(ActionAbout actionAbout, Logical newLogical) {
        if (actionAbout.TypeCode is AboutTypeCodes.Visible) {
            actionAbout.Visible = TestVisible;
        }

        if (actionAbout.TypeCode is AboutTypeCodes.Usable) {
            actionAbout.Usable = true;
        }
    }
}

public class ClassWithMoney {
    private Money _money;
    public decimal amount;

    [Key]
    public int Id { get; init; }

    public Money Money => _money ??= new Money(amount, i => amount = i);

    public static bool TestVisible { get; set; } = true;

    public ITitle Title() => Money.Title();

    public ClassWithMoney actionUpdateMoney(Money newMoney) {
        Money.Value = newMoney.Value;
        return this;
    }

    public void AboutActionUpdateMoney(ActionAbout actionAbout, Money newMoney) {
        if (actionAbout.TypeCode is AboutTypeCodes.Visible) {
            actionAbout.Visible = TestVisible;
        }

        if (actionAbout.TypeCode is AboutTypeCodes.Usable) {
            actionAbout.Usable = true;
        }
    }
}

public class ClassWithDateTimeKey {
    private TextString _name;
    public string name;

    [Key]
    public DateTime Id { get; init; }

    public TextString Name => _name ??= new TextString(name, s => name = s);
}

public class ClassWithOrderedProperties {
    private TextString _name1;
    private TextString _name2;
    private TextString _name3;
    private TextString _name4;
    public string name1;
    public string name2;
    public string name3;
    public string name4;

    [Key]
    public int Id { get; init; }

    public TextString Name1 => _name1 ??= new TextString(name1, s => name1 = s);
    public TextString Name2 => _name2 ??= new TextString(name2, s => name2 = s);
    public TextString Name3 => _name3 ??= new TextString(name3, s => name3 = s);

    [Legacy(Order = 4, MaxLength = 10)]
    public TextString Name4 => _name4 ??= new TextString(name4, s => name4 = s);

    public static string FieldOrder() => $"{nameof(Name2)}, {nameof(Name3)}, {nameof(Name1)}";

}

public class ClassWithOrderedActions
{
    
    [Key]
    public int Id { get; init; }

    public void actionAction1() { }
    public void actionAction2() { }
    public void actionAction3() { }

    //[MemberOrder(4)]
    public void actionAction4() { }

    //public static string ActionOrder() => $"{nameof(actionAction2)}, {nameof(actionAction3)}, {nameof(actionAction1)}";

    public static IMenu menuOrder()
    {
        var menu = new Menu("Actions");
        menu.MenuItems().Add(new MenuAction(nameof(actionAction2)));
        menu.MenuItems().Add(new MenuAction(nameof(actionAction3)));
        menu.MenuItems().Add(new MenuAction(nameof(actionAction1)));
        menu.MenuItems().Add(new MenuAction(nameof(actionAction4)));
        return menu;
    }
}