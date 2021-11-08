// Decompiled with JetBrains decompiler
// Type: sdm.systems.application.action.Menu
// Assembly: sdm.systems.application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 99352736-2C6C-4D77-94E7-E744D03082F7
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls.part02\bin\sdm.systems.application.dll

namespace Legacy.Types {
    public class Menu : IMenu {
        protected string myName;

        public Menu(string name) => myName = name;

        public string Name {
            get => myName;
            set => myName = value;
        }
    }
}