using NakedFramework;
using System;
using System.Linq;

namespace NakedFunctions
{
    public static class Helpers
    {
        public static Func<IMenuFactory, IMenu[]> GenerateMainMenus(Type[] mainMenuTypes) =>
                mf => mainMenuTypes.Select(t => mf.NewMenu(t, true)).ToArray();
    }
}
