using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public class HomeView : View {
    public HomeView(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

    public Menu OpenMainMenu(string menuName) {
        var openMenu = helper.wait.Until(dr => element.FindElement(By.CssSelector("nof-menu-bar"))); //Open menu, if any
        var menu = element.FindElement(By.CssSelector($"nof-menu-bar nof-action input[title=\"{menuName}\"]"));
        helper.Click(menu);
        helper.wait.Until(dr => element.FindElement(By.CssSelector("nof-action-list")) != openMenu);
        var menuEl = element.FindElement(By.CssSelector("nof-action-list"));
        return new Menu(menuEl, helper, this);
    }

    public HomeView AssertMainMenusAre(params string[] menuNames) {
        var menus = element.FindElements(By.CssSelector("nof-menu-bar nof-action"));
        Assert.AreEqual(menus.Count(), menuNames.Count(), "Number of menus specified does not match the view");
        for (var i = 0; i < menus.Count; i++) {
            Assert.AreEqual(menuNames[i], menus[i].FindElement(By.CssSelector("input")).GetAttribute("value"));
        }

        return this;
    }
}