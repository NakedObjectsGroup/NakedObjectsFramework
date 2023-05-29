// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Diagnostics.Eventing.Reader;
using NakedFramework.Architecture.Menu;
using NakedFramework.RATL.Classic.Interface;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestMenu : ITestMenu {
    private ITestAction[] Actions { get; }

    public TestMenu(ITestAction[] actions, string title, string menuId) {
        Actions = actions;
        Title = title;
        MenuId = menuId;
    }

    #region ITestMenu Members

    public string Title { get; }
    public string MenuId { get; }

    public ITestMenu AssertNameEquals(string name) {
        if (string.IsNullOrWhiteSpace(Title)) {
            var all = Actions.All(a => a.SubMenu == name);
            Assert.IsTrue(all);
        }
        else {
            Assert.AreEqual(Title, name);
        }

        return this;
    }

    public ITestMenu AssertItemCountIs(int count) {
        Assert.AreEqual(count, Actions.Length);
        return this;
    }

    public ITestAction GetAction(string name) => GetItem(name).AsAction();

    public ITestMenu GetSubMenu(string name) => GetItem(name).AsSubMenu();

    public ITestMenuItem GetItem(string name) {
        var item = Actions.FirstOrDefault(i => i.Name == name);
        //Assert.IsNotNull(item, "No menu item with name: " + name);
        return item is null ? new TestMenuItem(null, this) :  new TestMenuItem(item, this);
    }

    public ITestMenuItem[] AllItems() {
        return Actions.Select(a => new TestMenuItem(a, this)).Cast<ITestMenuItem>().ToArray();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.