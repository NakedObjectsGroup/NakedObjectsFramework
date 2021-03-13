using System;

namespace NakedFrameworkClient.SeleniumTestFramework
{
    public abstract class ActionResult : Element
    {
        public virtual ActionResult AssertTitleIs(string title) => throw new NotImplementedException();

        public Menu OpenActions() => throw new NotImplementedException();

        public virtual ActionResult ClickReload() => throw new NotImplementedException();
    }
}