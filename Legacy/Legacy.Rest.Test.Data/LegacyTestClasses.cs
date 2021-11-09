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
using Legacy.Types;
using NakedObjects;

// ReSharper disable InconsistentNaming

namespace Legacy.Rest.Test.Data {
    public interface ILegacyRoleInterface { }

    public class SimpleService {
        public ClassWithTextString GetClassWithTextString() => null;
    }

    public class ClassWithTextString {
        private TextString _name;
        public string name;

        [Key]
        public int Id { get; init; }

        public TextString Name => _name ??= new TextString(name, s => name = s);

        public Title Title() => Name.Title();

        public ClassWithTextString ActionUpdateName(TextString newName) {
            Name.Text = newName.Text;
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

        public ClassWithInternalCollection ActionUpdateTestCollection(TextString newName) {
            var name = newName.Text;
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

        public TextString Name => new("");

        public Title Title() => Name.Title();

        public void aboutName(FieldAbout fieldAbout, TextString name) {
            fieldAbout.Visible = !TestInvisibleFlag;
        }
    }

    public class ClassWithLinkToNOFClass {
        [Key]
        public int Id { get; init; }

        public virtual ClassWithString LinkToNOFClass { get; set; }
    }

    public class LegacyClassWithInterface : IRoleInterface {
        [Key]
        public int Id { get; init; }
    }

    public class ClassWithMenu {
        [Key]
        public int Id { get; init; }

        public TextString Name => new($"{nameof(GetType)}/{Id}");

        public Title Title() => new(Name);

        public ClassWithMenu ActionMethod1() => this;
        public ClassWithMenu actionMethod2() => this;

        public static ClassWithMenu ActionMenuAction() => null;

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
        private Date _date;
        public DateTime date;

        [Key]
        public int Id { get; init; }

        public Date Date => _date ??= new Date(date, d => date = d);

        public Title Title() => Date.Title();

        public ClassWithDate ActionUpdateDate(Date newDate) {
            Date.DateTime = newDate.DateTime;
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

        public Title Title() => TimeStamp.Title();

        public ClassWithTimeStamp ActionUpdateTimeStamp(TimeStamp newTimeStamp) {
            TimeStamp.DateTime = newTimeStamp.DateTime;
            return this;
        }
    }

    public class ClassWithWholeNumber {
        private WholeNumber _wholeNumber;
        public int number;

        [Key]
        public int Id { get; init; }

        public WholeNumber WholeNumber => _wholeNumber ??= new WholeNumber(number, i => number = i);

        public Title Title() => WholeNumber.Title();

        public ClassWithWholeNumber actionUpdateWholeNumber(WholeNumber newWholeNumber) {
            WholeNumber.Number = newWholeNumber.Number;
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

        public static bool TestVisible { get; set; } = true;
    }
}