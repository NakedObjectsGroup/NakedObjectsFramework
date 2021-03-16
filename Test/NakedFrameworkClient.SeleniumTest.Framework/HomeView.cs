
using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class HomeView : View
    {
        public HomeView(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

        public Menu OpenMainMenu(string menuName)
        {
            var openMenu = helper.wait.Until(dr => element.FindElement(By.CssSelector("nof-menu-bar"))); //Open menu, if any
            IWebElement menu = element.FindElement(By.CssSelector($"nof-menu-bar nof-action input[title=\"{menuName}\"]"));
            helper.Click(menu);
            helper.wait.Until(dr => element.FindElement(By.CssSelector("nof-action-list")) != openMenu);
            var menuEl = element.FindElement(By.CssSelector("nof-action-list"));
            return new Menu(menuEl, helper, this);
        }

        public HomeView AssertMainMenusAre(params string[] menuNames) => throw new NotImplementedException();
    }
}
