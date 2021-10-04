// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.ClassAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects.Application.Control {
    //[JavaInterface]
    public interface ClassAbout {
        void uninstantiable();

        void uninstantiable(string reason);

        void instantiableOnlyByRole(Role role);

        void instantiableOnlyByRoles(Role[] roles);

        void instantiableOnlyByUser(User user);

        void instantiableOnlyByUsers(User[] users);

        void instancesUnavailable();

        void instancesAvailableOnlyToRole(Role role);

        void instancesAvailableOnlyToRoles(Role[] roles);

        void instancesAvailableOnlyToUser(User user);

        void instancesAvailableOnlyToUsers(User[] users);
    }
}