// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.ActionAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

// ReSharper disable InconsistentNaming

using NakedObjects;

namespace Legacy.NakedObjects.Application.Control {
    //[JavaInterface]
    public interface ActionAbout {
        [NakedObjectsIgnore]
        object[][] getOptions();

        [NakedObjectsIgnore]
        object[] getDefaultParameterValues();

        [NakedObjectsIgnore]
        string[] getParameterLabels();

        [NakedObjectsIgnore]
        bool[] getRequired();

        [NakedObjectsIgnore]
        void invisible();

        [NakedObjectsIgnore]
        void invisibleToUser(User user);

        [NakedObjectsIgnore]
        void invisibleToUsers(User[] users);

        [NakedObjectsIgnore] void setDescription(string @string);

        [NakedObjectsIgnore] void setName(string @string);

        [NakedObjectsIgnore] void setParameter(int index, object defaultValue);

        [NakedObjectsIgnore] void setParameter(int index, object[] options);

        [NakedObjectsIgnore] void setParameter(int index, string label);

        [NakedObjectsIgnore] void setParameter(int index, bool required);

        [NakedObjectsIgnore] void setParameter(int index, string label, object defaultValue, bool required);

        [NakedObjectsIgnore] void setParameters(object[] defaultValues);

        [NakedObjectsIgnore] void setParameters(string[] labels);

        [NakedObjectsIgnore] void setParameters(bool[] required);

        [NakedObjectsIgnore] void unusable();

        [NakedObjectsIgnore] void unusable(string reason);

        [NakedObjectsIgnore] void unusableInState(State state);

        [NakedObjectsIgnore] void unusableInStates(State[] states);

        [NakedObjectsIgnore] void unusableOnCondition(bool conditionMet, string reasonNotMet);

        [NakedObjectsIgnore] void usableOnlyInState(State state);

        [NakedObjectsIgnore] void usableOnlyInStates(State[] states);

        [NakedObjectsIgnore] void visibleOnlyToRole(Role role);

        [NakedObjectsIgnore] void visibleOnlyToRoles(Role[] roles);

        [NakedObjectsIgnore] void visibleOnlyToUser(User user);

        [NakedObjectsIgnore] void visibleOnlyToUsers(User[] users);
    }
}