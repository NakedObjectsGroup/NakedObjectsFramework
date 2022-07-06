// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Selenium.Helpers.Tests;
using OpenQA.Selenium;

namespace NakedObjects.Selenium.Test.ObjectTests; 

public abstract class AttachmentTestsRoot : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    public virtual void ImageAsProperty() {
        Debug.WriteLine(nameof(ImageAsProperty));
        GeminiUrl("object?o1=___1.Product--968");
        wait.Until(d => d.FindElements(By.CssSelector(".property")).Count == 23);
        wait.Until(dr => dr.FindElement(By.CssSelector(".property img")).GetAttribute("src").Length > 0);
    }

    public virtual void EmptyImageProperty() {
        Debug.WriteLine(nameof(EmptyImageProperty));
        GeminiUrl("object?i1=View&o1=___1.Person--13742");
        wait.Until(d => d.FindElements(By.CssSelector(".property"))[9].Text == "Photo:\r\nNo image");
    }

    public virtual void ClickOnImage() {
        Debug.WriteLine(nameof(ClickOnImage));
        GeminiUrl("object?o1=___1.Product--779");
        Click(WaitForCss(".property img"));
        WaitForView(Pane.Single, PaneType.Attachment);
        wait.Until(dr => dr.FindElement(By.CssSelector(".attachment .reference img")).GetAttribute("src").Length > 0);
    }

    public virtual void RightClickOnImage() {
        Debug.WriteLine(nameof(RightClickOnImage));
        GeminiUrl("object?o1=___1.Product--780");
        RightClick(WaitForCss(".property img"));
        WaitForView(Pane.Left, PaneType.Object);
        wait.Until(dr => dr.FindElement(By.CssSelector("#pane1 .property img")).GetAttribute("src").Length > 0);
        WaitForView(Pane.Right, PaneType.Attachment);
        wait.Until(dr => dr.FindElement(By.CssSelector("#pane2 .attachment .reference img")).GetAttribute("src").Length > 0);
    }
}

#region Mega tests

public abstract class MegaAttachmentTestsRoot : AttachmentTestsRoot {
    [TestMethod] //Mega
    [Priority(0)]
    public void AttachmentTests() {
        ImageAsProperty();
        EmptyImageProperty();
        ClickOnImage();
        RightClickOnImage();
    }

    [TestMethod]
    [Priority(-1)]
    public void ProblematicTests() {
        EmptyImageProperty();
    }
}

//[TestClass]
public class MegaAttachmentTestsFirefox : MegaAttachmentTestsRoot {
    [ClassInitialize]
    public new static void InitialiseClass(TestContext context) {
        GeminiTest.InitialiseClass(context);
    }

    [TestInitialize]
    public virtual void InitializeTest() {
        InitFirefoxDriver();
        Url(BaseUrl);
    }

    [TestCleanup]
    public virtual void CleanupTest() {
        CleanUpTest();
    }
}

//[TestClass]
public class MegaAttachmentTestsIe : MegaAttachmentTestsRoot {
    [ClassInitialize]
    public new static void InitialiseClass(TestContext context) {
        FilePath(@"drivers.IEDriverServer.exe");
        GeminiTest.InitialiseClass(context);
    }

    [TestInitialize]
    public virtual void InitializeTest() {
        InitIeDriver();
        Url(BaseUrl);
    }

    [TestCleanup]
    public virtual void CleanupTest() {
        CleanUpTest();
    }
}

[TestClass] //toggle
public class MegaAttachmentTestsChrome : MegaAttachmentTestsRoot {
    [ClassInitialize]
    public new static void InitialiseClass(TestContext context) {
        FilePath(@"drivers.chromedriver.exe");
        GeminiTest.InitialiseClass(context);
    }

    [TestInitialize]
    public virtual void InitializeTest() {
        InitChromeDriver();
        Url(BaseUrl);
    }

    [TestCleanup]
    public virtual void CleanupTest() {
        CleanUpTest();
    }
}

#endregion