using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Selenium
{
    public class NewTestFramework: GeminiTest
    {
        #region Helpers
        private string CurrentOutput()
        {
            return br.FindElement(By.CssSelector(".output")).Text;
        }
        private string WaitUntilOutputDiffersFrom(string text)
        {
            wait.Until(dr => dr.FindElement(By.CssSelector(".output")).Text != text);
            return br.FindElement(By.CssSelector(".output")).Text;
        }

        protected void PartialUrl(string url)
        {
            CiceroUrl(url);
        }
        #endregion

        #region Commands
        protected void Home()
        {
            CiceroUrl("home");
            WaitForOutputStarting("Welcome");
        }
        protected void OK()
        {
            var prior = CurrentOutput();
            EnterCommand("ok");
            WaitUntilOutputDiffersFrom(prior);
        }

        protected void Edit()
        {
            var prior = CurrentOutput();
            EnterCommand("ok");
            var output = WaitUntilOutputDiffersFrom(prior);
            Assert.IsTrue(output.StartsWith("Editing: "), "Could not go into Edit mode");
        }

        protected void Menu(string menuName)
        {
            var prior = CurrentOutput();
            EnterCommand("menu "+ menuName);
            var output = WaitUntilOutputDiffersFrom(prior);
            Assert.IsTrue(output.Contains("menu"), "No menu was opened");
            Assert.IsFalse(output.Contains("does not match any menu"), output);
            Assert.IsFalse(output.StartsWith("Matching menus:"), menuName+" matches multiple menus");
            Assert.IsTrue(output.Contains(menuName), "Menu opened did not match "+menuName);
        }

        protected void OpenActionDialog(string actionName, string subMenu = null)
        {
            var prior = CurrentOutput();
            EnterCommand("action "+actionName + " " +subMenu);
            var output = WaitUntilOutputDiffersFrom(prior);
            Assert.IsFalse(output.Contains("Matching actions:"), actionName + " "+subMenu +" matches multiple actions");
            Assert.IsFalse(output.Contains("does not match any actions"), output);
            Assert.IsTrue(output.Contains("Action dialog:"), "No action opened");
        }

        #endregion
    }
}
