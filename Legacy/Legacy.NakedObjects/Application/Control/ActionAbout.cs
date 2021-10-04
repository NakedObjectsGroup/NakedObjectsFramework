// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.ActionAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

// ReSharper disable InconsistentNaming
namespace Legacy.NakedObjects.Application.Control
{
  //[JavaInterface]
  public interface ActionAbout
  {
    object[][] getOptions();

    object[] getDefaultParameterValues();

    string[] getParameterLabels();

    bool[] getRequired();

    void invisible();

    void invisibleToUser(User user);

    void invisibleToUsers(User[] users);

    void setDescription(string @string);

    void setName(string @string);

    void setParameter(int index, object defaultValue);

    void setParameter(int index, object[] options);

    void setParameter(int index, string label);

    void setParameter(int index, bool required);

    void setParameter(int index, string label, object defaultValue, bool required);

    void setParameters(object[] defaultValues);

    void setParameters(string[] labels);

    void setParameters(bool[] required);

    void unusable();

    void unusable(string reason);

    void unusableInState(State state);

    void unusableInStates(State[] states);

    void unusableOnCondition(bool conditionMet, string reasonNotMet);

    void usableOnlyInState(State state);

    void usableOnlyInStates(State[] states);

    void visibleOnlyToRole(Role role);

    void visibleOnlyToRoles(Role[] roles);

    void visibleOnlyToUser(User user);

    void visibleOnlyToUsers(User[] users);
  }
}
