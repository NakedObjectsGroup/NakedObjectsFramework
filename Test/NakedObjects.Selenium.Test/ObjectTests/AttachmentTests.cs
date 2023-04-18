// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;
using OpenQA.Selenium;

namespace NakedObjects.Selenium.Test.ObjectTests;

public abstract class AttachmentTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void ImageAsProperty() {
        GeminiUrl("object?o1=___1.Product--968");
        wait.Until(d => d.FindElements(By.CssSelector(".property")).Count == 23);
        wait.Until(dr => dr.FindElement(By.CssSelector(".property img")).GetAttribute("src").Length > 0);
    }

    [TestMethod]
    public virtual void EmptyImageProperty() {
        GeminiUrl("object?i1=View&o1=___1.Person--13742");
        wait.Until(d => d.FindElements(By.CssSelector(".property"))[9].Text == "Photo:\r\nNo image");
    }

    [TestMethod]
    public virtual void ClickOnImage() {
        GeminiUrl("object?o1=___1.Product--779");
        Click(WaitForCss(".property img"));
        WaitForView(Pane.Single, PaneType.Attachment);
        wait.Until(dr => dr.FindElement(By.CssSelector(".attachment .reference img")).GetAttribute("src").Length > 0);
    }

    [TestMethod]
    public virtual void RightClickOnImage() {
        GeminiUrl("object?o1=___1.Product--780");
        RightClick(WaitForCss(".property img"));
        WaitForView(Pane.Left, PaneType.Object);
        wait.Until(dr => dr.FindElement(By.CssSelector("#pane1 .property img")).GetAttribute("src").Length > 0);
        WaitForView(Pane.Right, PaneType.Attachment);
        wait.Until(dr => dr.FindElement(By.CssSelector("#pane2 .attachment .reference img")).GetAttribute("src").Length > 0);
    }
}

[TestClass]
public class AttachmentTestsChrome : AttachmentTests {

    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}