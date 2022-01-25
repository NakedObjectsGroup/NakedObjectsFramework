using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework
{
    public abstract class View
    {
        internal readonly IWebElement element;
        internal readonly Helper helper;
        internal readonly Pane pane;

        public View(IWebElement element, Helper helper, Pane pane = Pane.Single)
        {
            this.element = element;
            this.helper = helper;
            this.pane = pane;
        }

        //Works only when there is already an open dialog in view - otherwise open one through action menu
        public Dialog GetOpenedDialog()
        {
            var we = helper.WaitForChildElement(element, ".dialog");
            return new Dialog(we, helper, this);
        }

        public void WaitForMessage(string msg)
        {
            var el = helper.wait.Until(e => e.FindElement(By.CssSelector(".header .messages")).Text == msg);
        }
    }
}
