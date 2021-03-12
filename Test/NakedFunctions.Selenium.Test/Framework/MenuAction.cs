using System;

namespace NakedFunctions.Selenium.Test.Framework
{
    public abstract class MenuAction : Element
    {
        public abstract MenuAction AssertIsEnabled();
    }
}