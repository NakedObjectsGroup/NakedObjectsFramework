// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace NakedFrameworkClient.TestFramework
{
    public class Footer
    {
        private IWebElement element;
        private Helper helper;

        public Footer(IWebElement element, Helper helper)
        {
            this.element = element;
            this.helper = helper;
        }

        public Footer AssertHasMessage(string message)
        {
            var actual = helper.WaitForChildElement(element, ".messages").Text;
            Assert.AreEqual(message, actual);
            return this;
        }

        public HomeView RightClickHome()
        {
            var homeB = helper.WaitForCss(".footer .home").AssertIsEnabled();
            helper.Click(homeB, MouseClick.SecondaryButton);
            Thread.Sleep(100);
            return helper.GetHomeView(Pane.Right);
        }

        public T ClickBack<T>(MouseClick button = MouseClick.MainButton) where T : View, new() => throw new NotImplementedException();

        public HomeView ClickForward(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

        public HomeView ClickExpandLeft(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

        public HomeView ClickBack(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

    }
}