// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Menu;
using NakedFramework.RATL.Classic.Interface;

namespace NakedFramework.RATL.Classic.NonDocumenting;

public class TestMenuItem : ITestMenuItem {
    public TestMenuItem(ITestAction action, ITestMenu owningMenu) {
        Action = action;
        OwningMenu = owningMenu;
    }

    private ITestAction Action { get; }
    public ITestMenu OwningMenu { get; }

    #region ITestMenuItem Members

    public ITestMenuItem AssertNameEquals(string name) {
        Assert.AreEqual(name, Action.Name);
        return this;
    }

    public ITestMenuItem AssertSubMenuEquals(string name) {
        Assert.AreEqual(name, Action.SubMenu);
        return this;
    }

    public ITestMenuItem AssertIsAction() => this;

    public ITestAction AsAction() {
        AssertIsAction();
        return Action is null ? new TestAction(null) : Action;
    }

    public ITestMenuItem AssertIsSubMenu() {
        Assert.IsFalse(string.IsNullOrWhiteSpace(Action.SubMenu));
        return this;
    }

    internal bool IsInSubmenu(string subMenu) {
        return Action.SubMenu == subMenu;
    }

    public ITestMenu AsSubMenu() {
        AssertIsSubMenu();
        return new TestMenu(OwningMenu.AllItems().Cast<TestMenuItem>().Where(i => i.IsInSubmenu(Action.SubMenu)).Select(i => i.Action).ToArray(), "", "");
    }

    #endregion
}