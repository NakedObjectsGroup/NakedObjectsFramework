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
using System.Linq;
using NakedFramework.Core.Util;
using NakedLegacy.Rest.Test.Data.AppLib;
using NakedObjects;

// ReSharper disable InconsistentNaming

namespace NakedLegacy.Rest.Test.Data;

public interface ILegacyRoleInterface { }

public class SimpleService : IContainerAware {
    public IContainer Container { get; set; }
    public ClassWithTextString GetClassWithTextString() => Container.AllInstances<ClassWithTextString>().FirstOrDefault();
}

public class ClassWithTextString {
    private TextString _name;
    public string name { get; set; }

    [Key]
    public int Id { get; init; }

    public TextString Name => _name ??= new TextString(name, s => name = s);

    public Title Title() => new(Name.Title());

    public ClassWithTextString ActionUpdateName(TextString newName) {
        Name.Value = newName.Value;
        return this;
    }
}

public class ClassToPersist : IContainerAware {
    public static bool TestSave;
    public static bool TestProperty;

    private TextString _name;
    public string name { get; set; }

    [Key]
    public int Id { get; init; }

    public TextString Name => _name ??= new TextString(name, s => name = s);

    public IContainer Container { get; set; }

    public static void ResetTest() {
        TestSave = TestProperty = false;
    }

    public void AboutName(FieldAbout fieldAbout, TextString name) {
        if (fieldAbout.TypeCode == AboutTypeCodes.Valid) {
            if (TestProperty) {
                if (name.Value == "invalid") {
                    fieldAbout.IsValid = false;
                    fieldAbout.InvalidReason = "Property Name is invalid";
                }
                else if (name.Value is null) {
                    fieldAbout.IsValid = false;
                    fieldAbout.InvalidReason = "Property Name is null";
                }
            }
        }
    }

    public Title Title() => new(Name.Title());

    public ClassToPersist ActionUpdateName(TextString newName) {
        Name.Value = newName.Value;
        return this;
    }

    public void ActionSave() {
        var toSave = this;
        Container.MakePersistent(ref toSave);
    }

    public void AboutActionSave(ActionAbout actionAbout) {
        if (actionAbout.TypeCode == AboutTypeCodes.Valid) {
            if (TestSave) {
                if (Name.Value == "invalid") {
                    actionAbout.Usable = false;
                    actionAbout.UnusableReason = "Object Name is invalid";
                }
                else if (Name.Value is null) {
                    actionAbout.Usable = false;
                    actionAbout.UnusableReason = "Object Name is null";
                }
            }
        }
    }
}

public class ClassWithBounded : IBounded {
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
    public static bool TestInvisibleFlag;
    public static bool TestUsableFlag;
    public static bool TestValidFlag;
    public static bool TestDefaults;
    public static bool TestOptions;
    public static string TestName;
    public static string TestDescription;

    public static int AboutCount;

    [Key]
    public int Id { get; init; }

    public static void ResetTest() {
        TestInvisibleFlag = TestUsableFlag = TestValidFlag = TestDefaults = TestOptions = false;
        TestName = TestDescription = null;
    }

    public void actionTestAction() {
        // do something
    }

    public void aboutActionTestAction(ActionAbout actionAbout) {
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
                break;
        }
    }

    public void actionTestActionWithParms(TextString ts, WholeNumber wn) {
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

                if (TestDefaults) {
                    actionAbout.ParamDefaultValues = new object[] { "def", 66 };
                }

                if (TestOptions) {
                    actionAbout.ParamOptions = new[] { new object[] { "opt1", "opt2" }, new object[] { 1, 2, 3, 4, 5 } };
                }

                break;
        }
    }
}

public class ClassWithFieldAbout {
    public static bool TestInvisibleFlag;
    public static bool TestUsableFlag;
    public static bool TestValidFlag;
    public static bool TestChoices;
    public static string TestName;
    public static string TestDescription;

    private TextString _name;
    public string name;

    [Key]
    public int Id { get; init; }

    public virtual TextString Name => _name ??= new TextString(name, s => name = s);

    public static void ResetTest() {
        TestInvisibleFlag = TestUsableFlag = TestValidFlag = TestChoices = false;
        TestName = TestDescription = null;
    }

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
            case AboutTypeCodes.Parameters:
                if (TestChoices) {
                    fieldAbout.Options = new object[] { "fieldopt1", "fieldopt2" };
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

    public IContainer Container { get; set; }

    public ClassWithReferenceProperty actionUpdateReferenceProperty(ClassWithTextString newReferenceProperty) {
        ReferenceProperty = newReferenceProperty;
        return this;
    }

    public ClassWithTextString actionGetObject(TextString name) {
        var ofName = name.Value;
        return Container.AllInstances<ClassWithTextString>().SingleOrDefault(c => c.name == ofName);
    }

    public ClassWithTextString actionGetObject1(TextString name) {
        var ofName = name.Value;
        var simpleService = (SimpleService)Container.DomainService(typeof(SimpleService));
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
}

public class LegacyClassWithInterface : IRoleInterface {
    [Key]
    public int Id { get; init; }
}

public class ClassWithMenu {
    public static bool TestInvisibleFlag;
    public static bool TestUsableFlag;
    public static bool TestValidFlag;
    public static bool TestNameFlag;
    public static bool TestDescriptionFlag;
    public static bool TestParametersFlag;

    [Key]
    public int Id { get; init; }

    public TextString Name => new($"{nameof(ClassWithMenu)}/{Id}");

    private static IContainer Container => ThreadLocals.Container;

    public static void ResetTest() {
        TestInvisibleFlag = TestUsableFlag = TestValidFlag = TestNameFlag = TestDescriptionFlag = TestParametersFlag = false;
    }

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

    public static void AboutActionMenuActionWithParm(ActionAbout actionAbout, TextString ts) {
        switch (actionAbout.TypeCode) {
            case AboutTypeCodes.Visible:
                if (TestInvisibleFlag) {
                    actionAbout.Visible = false;
                }

                break;
            case AboutTypeCodes.Usable:
                if (TestUsableFlag) {
                    actionAbout.Usable = false;
                    actionAbout.UnusableReason = "unusable by about";
                }

                break;
            case AboutTypeCodes.Name:
                if (TestNameFlag) {
                    actionAbout.Name = "Renamed Name";
                }

                if (TestDescriptionFlag) {
                    actionAbout.Description = "A Description";
                }

                break;
            case AboutTypeCodes.Valid:
                if (TestValidFlag) {
                    if (ts.Value == "invalid") {
                        actionAbout.Usable = false;
                        actionAbout.UnusableReason = "ts invalid";
                    }
                }

                break;
            case AboutTypeCodes.Parameters:
                if (TestParametersFlag) {
                    actionAbout.ParamLabels = new[] { "renamed ts" };
                    actionAbout.ParamDefaultValues = new object[] { "def" };
                    actionAbout.ParamOptions = new[] { new object[] { "opt1", "opt2" } };
                }

                break;
        }
    }

    public static ClassWithTextString ActionMethodInjected(IContainer container) => container.AllInstances<ClassWithTextString>().First();

    public static ClassWithTextString ActionMethodInjectedWithParm(TextString ts, IContainer container) => container.AllInstances<ClassWithTextString>().First();

    public static void AboutActionMethodInjectedWithParm(ActionAbout actionAbout, TextString ts, IContainer container) {
        if (actionAbout.TypeCode == AboutTypeCodes.Valid) {
            // make sure container is not null
            var a = container.AllInstances<ClassWithTextString>().First();
            if (ts.Value != a.Name.Value) {
                actionAbout.Usable = false;
                actionAbout.UnusableReason = "test fail";
            }
        }
    }

    public static IMenu menuOrder() {
        var menu = new Menu("ClassWithMenu Menu");
        menu.MenuItems().Add(new MenuAction(nameof(ActionMethod1)));

        var newSubMenu = new Menu("Submenu1");
        menu.MenuItems().Add(newSubMenu);
        newSubMenu.MenuItems().Add(new MenuAction(nameof(actionMethod2)));
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
        menu.MenuItems().Add(new MenuAction(nameof(ActionMethodInjected)));
        menu.MenuItems().Add(new MenuAction(nameof(ActionMethodInjectedWithParm)));
        return menu;
    }
}

public class ClassWithDate {
    private NODate _date;

    private NODateNullable _date1;
    public DateTime date;
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

public class ClassWithOrderedActions {
    [Key]
    public int Id { get; init; }

    public void actionAction1() { }
    public void actionAction2() { }
    public void actionAction3() { }

    //[MemberOrder(4)]
    public void actionAction4() { }

    //public static string ActionOrder() => $"{nameof(actionAction2)}, {nameof(actionAction3)}, {nameof(actionAction1)}";

    public static IMenu menuOrder() {
        var menu = new Menu("Actions");
        menu.MenuItems().Add(new MenuAction(nameof(actionAction2)));
        menu.MenuItems().Add(new MenuAction(nameof(actionAction3)));
        menu.MenuItems().Add(new MenuAction(nameof(actionAction1)));
        menu.MenuItems().Add(new MenuAction(nameof(actionAction4)));
        return menu;
    }
}