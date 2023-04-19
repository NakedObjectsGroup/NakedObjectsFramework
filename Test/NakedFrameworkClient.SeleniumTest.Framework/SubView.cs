using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework;

public abstract class SubView {
    internal readonly IWebElement element;
    internal readonly View enclosingView;
    internal readonly Helper helper;

    protected SubView(IWebElement element, Helper helper, View enclosingView) {
        this.element = element;
        this.helper = helper;
        this.enclosingView = enclosingView;
    }
}