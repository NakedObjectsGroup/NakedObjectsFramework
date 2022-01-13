

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace NakedFrameworkClient.TestFramework
{
    public class ObjectPresentation : ActionResult
    {
        public ObjectPresentation(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

    }
}
