using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Xat
{
    public interface ITestMenu   //Because menu can be a sub-menu (item) of another menu
    {
        ITestMenu AssertNameEquals(string name);

        ITestMenu AssertItemCountIs(int count);

        ITestAction GetAction(string name);

        ITestMenu GetSubMenu(string name);

        ITestMenuItem GetItem(string name);

        ITestMenuItem[] AllItems();
    }
}
