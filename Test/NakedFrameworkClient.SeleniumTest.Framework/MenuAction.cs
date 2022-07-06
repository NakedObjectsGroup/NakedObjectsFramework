using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public abstract class MenuAction : SubView {
    public MenuAction(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    public virtual MenuAction AssertIsEnabled() {
        element.AssertIsEnabled();
        return this;
    }

    public virtual MenuAction AssertIsDisabled(string message) {
        element.AssertIsDisabled();
        AssertHasTooltip(message);
        return this;
    }

    public virtual MenuAction AssertHasTooltip(string tooltip) {
        Assert.AreEqual(tooltip, element.GetAttribute("title"));
        return this;
    }
}