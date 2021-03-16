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
    }
}
