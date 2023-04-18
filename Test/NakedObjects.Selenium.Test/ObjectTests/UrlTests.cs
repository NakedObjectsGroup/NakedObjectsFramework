﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;
using OpenQA.Selenium;

namespace NakedObjects.Selenium.Test.ObjectTests;

/// <summary>
///     Tests only that a given URLs return the correct views. No actions performed on them
/// </summary>
public abstract class UrlTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void UnrecognisedUrlGoesToHome() {
        GeminiUrl("unrecognised");
        WaitForView(Pane.Single, PaneType.Home, "Home");
        Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
    }

    #region Single pane Urls

    [TestMethod]
    public virtual void Home() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home, "Home");
        Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
    }

    [TestMethod]
    public virtual void HomeWithMenu() {
        GeminiUrl("home?m1=CustomerRepository");
        WaitForView(Pane.Single, PaneType.Home, "Home");
        wait.Until(d => d.FindElement(By.CssSelector("nof-action-list")));
        var actions = br.FindElements(By.CssSelector("nof-action-list nof-action"));
        Assert.AreEqual(CustomerServiceActions, actions.Count);
        //Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
        //Assert.AreEqual("Find Store By Name", actions[1].Text);
        //Assert.AreEqual("Create New Store Customer", actions[2].Text);
        //Assert.AreEqual("Random Store", actions[3].Text);
        //Assert.AreEqual("Find Individual Customer By Name", actions[4].Text);
        //Assert.AreEqual("Create New Individual Customer", actions[5].Text);
        //Assert.AreEqual("Random Individual", actions[6].Text);
        //Assert.AreEqual("Customer Dashboard", actions[7].Text);
        //Assert.AreEqual("Throw Domain Exception", actions[8].Text);
    }

    [TestMethod]
    public virtual void Object() {
        GeminiUrl("object?o1=___1.Store--350");
        wait.Until(d => d.FindElement(By.CssSelector(".object")));
        wait.Until(d => d.FindElement(By.CssSelector(".view")));
        AssertObjectElementsPresent();
    }

    [TestMethod]
    private void AssertObjectElementsPresent() {
        wait.Until(d => d.FindElement(By.CssSelector(".single")));
        wait.Until(d => d.FindElement(By.CssSelector(".object")));
        wait.Until(d => d.FindElement(By.CssSelector(".view")));
        wait.Until(d => d.FindElement(By.CssSelector(".header")));
        wait.Until(d => d.FindElement(By.CssSelector("input[value='Actions']")));
        wait.Until(d => d.FindElement(By.CssSelector(".main-column")));
        wait.Until(d => d.FindElement(By.CssSelector(".collections")));

        Assert.IsTrue(br.FindElements(By.CssSelector(".error")).Count == 0);
    }

    [TestMethod]
    public virtual void ObjectWithNoSuchObject() {
        GeminiUrl("object?o1=___1.Foo--555");
        wait.Until(d => d.FindElement(By.CssSelector(".error")));
    }

    [TestMethod]
    public virtual void ObjectWithActions() {
        GeminiUrl("object?o1=___1.Store--350&as1=open");
        GetObjectEnabledAction("Create New Address");
        AssertObjectElementsPresent();
    }

    [TestMethod]
    //TODO:  Need to add tests for object & home (later, list) with action (dialog) open
    public virtual void ObjectWithCollections() {
        GeminiUrl("object?o1=___1.Store--350&&c1_Addresses=List&c1_Contacts=Table");
        wait.Until(d => d.FindElement(By.CssSelector(".collections")));
        AssertObjectElementsPresent();
        wait.Until(d => d.FindElements(By.CssSelector(".collection")).Count == 2);
        var collections = br.FindElements(By.CssSelector(".collection"));
        wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.TagName("table")));
        //Assert.IsNotNull(collections[0].FindElement(By.TagName("table")));

        wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.CssSelector(".icon.table")));
        //Assert.IsNotNull(collections[0].FindElement(By.CssSelector(".icon.table")));
        wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.CssSelector(".icon.summary")));

        //Assert.IsNotNull(collections[0].FindElement(By.CssSelector(".icon.summary")));
        wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElements(By.CssSelector(".icon.list")).Count == 0);

        //Assert.IsTrue(collections[0].FindElements(By.CssSelector(".icon.list")).Count == 0);
    }

    [TestMethod]
    public virtual void ObjectInEditMode() {
        GeminiUrl("object?o1=___1.Store--350&i1=Edit");
        wait.Until(d => d.FindElement(By.CssSelector(".object")));
        wait.Until(d => d.FindElement(By.CssSelector(".edit")));
        SaveButton();
        GetCancelEditButton();
        // AssertObjectElementsPresent();
    }

    [TestMethod]
    public virtual void ListZeroParameterAction() {
        GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders");
        Reload();
        wait.Until(d => d.FindElement(By.CssSelector(".list")));
        WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
    }

    #endregion

    #region Split pane Urls

    [TestMethod]
    public virtual void SplitHomeHome() {
        GeminiUrl("home/home");
        WaitForView(Pane.Left, PaneType.Home, "Home");
        WaitForView(Pane.Right, PaneType.Home, "Home");
    }

    [TestMethod]
    public virtual void SplitHomeObject() {
        GeminiUrl("home/object?o2=___1.Store--350");
        WaitForView(Pane.Left, PaneType.Home, "Home");
        WaitForView(Pane.Right, PaneType.Object, "Twin Cycles");
    }

    [TestMethod]
    public virtual void SplitHomeList() {
        GeminiUrl("home/list?&m2=OrderRepository&a2=HighestValueOrders");
        WaitForView(Pane.Left, PaneType.Home, "Home");
        WaitForView(Pane.Right, PaneType.List, "Highest Value Orders");
    }

    [TestMethod]
    public virtual void SplitObjectHome() {
        GeminiUrl("object/home?o1=___1.Store--350");
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
        WaitForView(Pane.Right, PaneType.Home, "Home");
    }

    [TestMethod]
    public virtual void SplitObjectObject() {
        GeminiUrl("object/object?o1=___1.Store--350&o2=___1.Store--604");
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
        WaitForView(Pane.Right, PaneType.Object, "Mechanical Sports Center");
    }

    [TestMethod]
    public virtual void SplitObjectList() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        GeminiUrl("object/list?o1=___1.Store--350&m2=OrderRepository&a2=HighestValueOrders");
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
        WaitForView(Pane.Right, PaneType.List, "Highest Value Orders");
    }

    [TestMethod]
    public virtual void SplitListHome() {
        GeminiUrl("list/home?m1=OrderRepository&a1=HighestValueOrders");
        WaitForView(Pane.Left, PaneType.List, "Highest Value Orders");
        WaitForView(Pane.Right, PaneType.Home, "Home");
    }

    [TestMethod]
    public virtual void SplitListObject() {
        GeminiUrl("list/object?m1=OrderRepository&a1=HighestValueOrders&o2=___1.Store--604");
        WaitForView(Pane.Left, PaneType.List, "Highest Value Orders");
        WaitForView(Pane.Right, PaneType.Object, "Mechanical Sports Center");
    }

    [TestMethod]
    public virtual void SplitListList() {
        GeminiUrl("list/list?m2=PersonRepository&pm2_firstName=%22a%22&pm2_lastName=%22a%22&a2=FindContactByName&p2=1&ps2=20&s2_=0&m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0");
        WaitForView(Pane.Left, PaneType.List, "Current Special Offers");
        WaitForView(Pane.Right, PaneType.List, "Find Contact By Name");
    }

    #endregion
}

[TestClass]
public class UrlTestsChrome : UrlTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}