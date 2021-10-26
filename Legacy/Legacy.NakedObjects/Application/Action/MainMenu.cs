// Decompiled with JetBrains decompiler
// Type: sdm.systems.application.action.MainMenu
// Assembly: sdm.systems.application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 99352736-2C6C-4D77-94E7-E744D03082F7
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls.part02\bin\sdm.systems.application.dll

using System.Collections;

namespace Legacy.NakedObjects.Application.Action {
    public class MainMenu : Menu, IMainMenu {
        private ArrayList myMenuItems;

        public MainMenu()
            : base("") =>
            myMenuItems = new ArrayList();

        public MainMenu(string menuName)
            : base(menuName) =>
            myMenuItems = new ArrayList();

        public void clearMenuItems() => Menus.Clear();

        public SubMenu addSubMenu(string menuName) {
            var subMenu = new SubMenu(menuName);
            Menus.Add(subMenu);
            return subMenu;
        }

        public void addMenuItem(string menuName) => Menus.Add(new Menu(menuName));

        public void removeMenuItem(string menuName) {
            Menu menu1 = null;
            try {
                foreach (Menu menu2 in Menus) {
                    menu1 = menu2;
                    if (menu1.Name.Equals(menuName)) {
                        break;
                    }
                }
            }
            finally {
                //IEnumerator enumerator;
                //if (enumerator is IDisposable) {
                //    (enumerator as IDisposable).Dispose();
                //}
            }

            if (menu1 == null) {
                return;
            }

            Menus.Remove(menu1);
        }

        public virtual ArrayList Menus {
            get => myMenuItems;
            set => myMenuItems = value;
        }

        public void addSubMenu(SubMenu submenu) => Menus.Add(submenu);

        public SubMenu getSubMenu(string subMenuName) {
            try {
                foreach (Menu menu in Menus) {
                    if (menu.Name.Equals(subMenuName)) {
                        return (SubMenu)menu;
                    }
                }
            }
            finally {
                //IEnumerator enumerator;
                //if (enumerator is IDisposable) {
                //    (enumerator as IDisposable).Dispose();
                //}
            }

            return null;
        }

        //public static MainMenu MenuOrderFromMetadata<T>() where T : SdmObject => BuildMenuFromMetadata<T>(BindingFlags.Instance);

        //private static MainMenu BuildMenuFromMetadata<T>(BindingFlags flag) where T : SdmObject {
        //    var mainMenu = new MainMenu();
        //    var type = typeof(T);
        //    var builderFromMetadata = new MenuBuilderFromMetadata();
        //    var methods = type.GetMethods(BindingFlags.Public | flag);
        //    var index = 0;
        //    while (index < methods.Length) {
        //        var method = methods[index];
        //        builderFromMetadata.Add(method);
        //        checked {
        //            ++index;
        //        }
        //    }

        //    return builderFromMetadata.BuildMainMenu;
        //}

        //public static MainMenu ClassMenuOrderFromMetadata<T>() where T : SdmObject => BuildMenuFromMetadata<T>(BindingFlags.Static);

        //public static string FieldOrderFromMetadata<T>() where T : SdmObject {
        //    var type = typeof(T);
        //    var builderFromMetadata = new FieldOrderBuilderFromMetadata();
        //    var methods = type.GetMethods();
        //    var index1 = 0;
        //    while (index1 < methods.Length) {
        //        var method = methods[index1];
        //        builderFromMetadata.Add(method);
        //        checked {
        //            ++index1;
        //        }
        //    }

        //    var properties = type.GetProperties();
        //    var index2 = 0;
        //    while (index2 < properties.Length) {
        //        var prop = properties[index2];
        //        builderFromMetadata.Add(prop);
        //        checked {
        //            ++index2;
        //        }
        //    }

        //    return builderFromMetadata.BuildResult();
        //}
    }
}