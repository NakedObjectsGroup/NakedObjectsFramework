// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.FieldAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.application.control
{
  [JavaInterface]
  public interface FieldAbout
  {
    void invisible();

    void invisibleToUser(User user);

    void invisibleToUsers(User[] users);

    bool isPersistent();

    void modifiableOnlyByRole(Role role);

    void modifiableOnlyByRoles(Role[] roles);

    void modifiableOnlyByUser(User user);

    void modifiableOnlyByUsers(User[] users);

    void modifiableOnlyInState(State state);

    void modifiableOnlyInStates(State[] states);

    void nonPersistent();

    void setDescription(string @string);

    void unmodifiable();

    void unmodifiable(string reason);

    void unmodifiableByUser(User user);

    void unmodifiableByUsers(User[] users);

    void unmodifiableInState(State state);

    void unmodifiableInStates(State[] states);

    void unmodifiableOnCondition(bool conditionMet, string reasonNotMet);

    void visibleOnlyToRole(Role role);

    void visibleOnlyToRoles(Role[] roles);

    void visibleOnlyToUser(User user);

    void visibleOnlyToUsers(User[] users);

    void invalid();

    void invalid(string reason);

    void invalidOnCondition(bool condition, string reason);
  }
}
