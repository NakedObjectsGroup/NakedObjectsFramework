using System;

namespace NakedFunctions.Selenium.Test.Framework
{
    public abstract class ActionResult : Element
    {
        public virtual ActionResult AssertTitleIs(string title) => throw new NotImplementedException();

        public Menu OpenActions() => throw new NotImplementedException();

        public ActionResult WaitForTitle(string newDesc) => throw new NotImplementedException();
    }
}