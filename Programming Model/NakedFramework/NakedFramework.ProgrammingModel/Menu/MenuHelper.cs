using System;
using System.Linq;

namespace NakedFramework.Menu
{
    public static class MenuHelper
    {
        /// <summary>
        /// Given a list of types, returns a function, compatible with the requirements of the 
        /// NakedCoreOptions.MainMenus property used in systems configuration that will generate
        /// a main menu from each type, including all the available actions in each case. 
        /// </summary>
        /// <param name="typesDefiningMainMenuActions"></param>
        /// <returns></returns>
        public static Func<IMenuFactory, IMenu[]> GenerateMenus(Type[] typesDefiningMainMenuActions) =>
                mf => typesDefiningMainMenuActions.Select(t => mf.NewMenu(t, true)).ToArray();
    }
}
