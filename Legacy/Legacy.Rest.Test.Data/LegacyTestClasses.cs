// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Legacy.NakedObjects.Application;
using Legacy.NakedObjects.Application.Action;
using Legacy.NakedObjects.Application.Collection;
using Legacy.NakedObjects.Application.Control;
using Legacy.NakedObjects.Application.ValueHolder;
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

        public TextString Name => _name ??= new TextString(name) { BackingField = s => name = s };

        public Title title() => Name.title();

        public ClassWithTextString ActionUpdateName(TextString newName) {
            Name.setValue(newName);
            return this;
        }
    }

    public class ClassWithInternalCollection {
        private InternalCollection _testCollection;

        public IDomainObjectContainer Container { private get; set; }

        [NakedObjectsIgnore]
        public virtual ICollection<ClassWithTextString> _TestCollection { get; } = new List<ClassWithTextString>();

        [Key]
        public int Id { get; init; }

        public InternalCollection TestCollection {
            get {
                if (_testCollection is null) {
                    _testCollection = new InternalCollection(typeof(ClassWithTextString).ToString());
                    _testCollection.init(_TestCollection.ToArray());

                    _testCollection.BackingField = (obj, add) => {
                        if (add) {
                            _TestCollection.Add((ClassWithTextString)obj);
                        }
                        else {
                            _TestCollection.Remove((ClassWithTextString)obj);
                        }
                    };
                }

                return _testCollection;
            }
        }

        public ClassWithInternalCollection ActionUpdateTestCollection(TextString newName) {
            var name = newName.stringValue();
            var bill = Container.Instances<ClassWithTextString>().Single(c => c.name == name);
            TestCollection.add(bill);

            return this;
        }
    }

    public class ClassWithNOFInternalCollection {
        private InternalCollection _testCollection;

        [NakedObjectsIgnore]
        public virtual ICollection<ClassWithString> _TestCollection { get; } = new List<ClassWithString>();

        [Key]
        public int Id { get; init; }

        public InternalCollection CollectionOfNOFClass {
            get {
                if (_testCollection is null) {
                    _testCollection = new InternalCollection(typeof(ClassWithTextString).ToString());
                    _testCollection.init(_TestCollection.ToArray());
                }

                return _testCollection;
            }
        }
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
            if (TestInvisibleFlag) {
                actionAbout.invisible();
            }
        }
    }

    public class ClassWithFieldAbout {
        public static bool TestInvisibleFlag = false;

        [Key]
        public int Id { get; init; }

        public TextString Name => new("");

        public Title title() => Name.title();

        public void aboutName(FieldAbout fieldAbout, TextString name) {
            if (TestInvisibleFlag) {
                fieldAbout.invisible();
            }
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

        public Title title() => new(Name);

        public ClassWithMenu ActionMethod1() => this;
        public ClassWithMenu actionMethod2() => this;

        public static MainMenu menuOrder() {
            var menu = new MainMenu();
            menu.addMenuItem("Method1");
            menu.addSubMenu("Submenu1")?.addMenuItem("Method2");
            return menu;
        }
    }
}

// class menu
//public static MainMenu sharedMenuOrder() {
//    MainMenu mainMenu = new MainMenu();
//    mainMenu.addMenuItem("ExtendChildClaim");
//    mainMenu.addMenuItem("FindSchool");
//    mainMenu.addMenuItem("AddChildBenefitToNewCase");
//    mainMenu.addMenuItem("AddAnotherChildBenefitToExistingCase");
//    mainMenu.addMenuItem("FindForPPSN");
//    mainMenu.addMenuItem("FindAllChildClaimsForPPSN");
//    mainMenu.addMenuItem("AddChildBenefitToExistingCase");
//    SubMenu subMenu = mainMenu.getSubMenu("Decisions");
//    if (subMenu != null) {
//        subMenu.addMenuItem("DisallowScheme");
//        subMenu.addMenuItem("WithdrawScheme");
//    }
//    return mainMenu;
//}

// instance menu
//public new static MainMenu menuOrder() {
//    MainMenu mainMenu = Scheme.menuOrder();
//    mainMenu.removeMenuItem("Individualised or Separate Payments");
//    mainMenu.addMenuItem("AddChildClaim");
//    mainMenu.addMenuItem("AddChildClaimForSelf");
//    mainMenu.addSubMenu(ChildBenefit.communicationsSubMenu());
//    SubMenu subMenu = mainMenu.getSubMenu("Update");
//    subMenu.addMenuItem("UpdateResidencyStatus");
//    subMenu.addSubMenu(ChildBenefit.NoEFTAllowedSubMenu());
//    subMenu.addMenuItem("UpdateCountryOfResidence");
//    mainMenu.getSubMenu("Decisions").addMenuItem("DisallowScheme");
//    mainMenu.getSubMenu("Decisions").addMenuItem("WithdrawScheme");
//    return mainMenu;
//}