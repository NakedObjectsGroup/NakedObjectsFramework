// Decompiled with JetBrains decompiler
// Type: sdm.systems.application.action.SubMenu
// Assembly: sdm.systems.application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 99352736-2C6C-4D77-94E7-E744D03082F7
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls.part02\bin\sdm.systems.application.dll

using System.Collections.Generic;

namespace Legacy.Types {
    public class SubMenu : MainMenu, ISubMenu {
        private IList<IMenu> myMenus;

        public SubMenu(string menuName)
            : base(menuName) =>
            myMenus = new List<IMenu>();

        public override IList<IMenu> Menus {
            get => myMenus;
            set => myMenus = value;
        }
    }
}