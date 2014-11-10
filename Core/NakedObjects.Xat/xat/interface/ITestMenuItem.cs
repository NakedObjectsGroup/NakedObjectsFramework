using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Xat
{
    public interface ITestMenuItem
    {
        ITestMenuItem AssertNameEquals(string name);

        ITestMenuItem AssertIsAction();

        ITestAction AsAction();

        ITestMenuItem AssertIsSubMenu();

        ITestMenu AsSubMenu();
    }
}
