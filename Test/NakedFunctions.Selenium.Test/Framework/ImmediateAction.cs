using System;

namespace NakedFunctions.Selenium.Test.Framework
{
    public class ImmediateAction : MenuAction
    {
        public override ImmediateAction AssertIsEnabled() => throw new NotImplementedException();

        public T Click<T>() where T : ActionResult, new() => throw new NotImplementedException();

        public T RightClick<T>() where T : ActionResult, new() => throw new NotImplementedException();
    }
}
