// Decompiled with JetBrains decompiler
// Type: sdm.systems.application.action.MenuBuilderFromMetadata
// Assembly: sdm.systems.application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 99352736-2C6C-4D77-94E7-E744D03082F7
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls.part02\bin\sdm.systems.application.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Legacy.NakedObjects.Application.Action {
    public class MenuBuilderFromMetadata {
        private readonly MainMenu _myBuildMain;
        private readonly SortedDictionary<MenuOrderAttribute, string> myMenuOrders;

        public MenuBuilderFromMetadata() {
            _myBuildMain = new MainMenu();
            myMenuOrders = new SortedDictionary<MenuOrderAttribute, string>();
        }

        public MainMenu BuildMainMenu {
            get {
                try {
                    foreach (var menuOrder in myMenuOrders) {
                        GetMenu(menuOrder.Key.GetCategories()).addMenuItem(menuOrder.Value);
                    }
                }
                finally {
                    //SortedDictionary<MenuOrderAttribute, string>.Enumerator enumerator;
                    //enumerator.Dispose();
                }

                return _myBuildMain;
            }
        }

        private MainMenu GetMenu(string[] category, int index, MainMenu menu) {
            if (index >= category.Length || string.IsNullOrEmpty(category[index])) {
                return menu;
            }

            if (menu.getSubMenu(category[index]) == null) {
                menu.addSubMenu(category[index]);
            }

            var subMenu = menu.getSubMenu(category[index]);
            return GetMenu(category, checked(index + 1), subMenu);
        }

        private MainMenu GetMenu(string[] category) => GetMenu(category, 0, _myBuildMain);

        public void Add(MethodInfo method) {
            if (!method.Name.StartsWith("action")) {
                return;
            }

            var str = RemoveActionPrefix(method);
            var customAttributes = method.GetCustomAttributes(typeof(MenuOrderAttribute), true);
            if (customAttributes.Length == 0) {
                return;
            }

            var key = (MenuOrderAttribute)customAttributes[0];
            key.Name = str;
            if (myMenuOrders.ContainsKey(key)) {
                throw new ApplicationException(string.Format("Type '{0}' has two action methods with order {1}", method.DeclaringType.FullName, key.Order));
            }

            myMenuOrders.Add(key, str);
        }

        private string RemoveActionPrefix(MethodInfo withAction) => withAction.Name.Remove(0, 6);
    }
}