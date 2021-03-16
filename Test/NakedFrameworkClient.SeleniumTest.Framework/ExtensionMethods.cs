using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework
{
    public static class ExtensionMethods
    {
        public static IWebElement AssertIsDisabled(this IWebElement a, string reason = null)
        {
            Assert.IsNotNull(a.GetAttribute("disabled"), "Element " + a.Text + " is not disabled");
            if (reason != null)
            {
                Assert.AreEqual(reason, a.GetAttribute("title"));
            }
            return a;
        }

        public static IWebElement AssertIsEnabled(this IWebElement a)
        {
            Assert.IsNull(a.GetAttribute("disabled"), "Element " + a.Text + " is disabled");
            return a;
        }

        public static IWebElement AssertHasTooltip(this IWebElement a, string tooltip)
        {
            Assert.AreEqual(tooltip, a.GetAttribute("title"));
            return a;
        }

        public static IWebElement AssertIsInvisible(this IWebElement a)
        {
            Assert.IsNull(a.GetAttribute("displayed"));
            return a;
        }
    }
}
