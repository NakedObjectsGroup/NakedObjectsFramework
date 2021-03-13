using System;

namespace NakedFrameworkClient.TestFramework
{
    public class Menu : Element
    {
        /// <summary>
        /// Members means action names and submenu names, and should be specified in presented order
        /// </summary>
        public Menu AssertHasMembers(params string[] members) => throw new NotImplementedException();

        public ActionWithDialog GetActionWithDialog(string actionName) => throw new NotImplementedException();

        public ActionWithoutDialog GetActionWithoutDialog(string actionName) => throw new NotImplementedException();

        public Menu GetSubMenu(string subMenuName) => throw new NotImplementedException();
  
        public void Close() => throw new NotImplementedException();
    }
}