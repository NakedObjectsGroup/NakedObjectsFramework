// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.FieldAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

// ReSharper disable InconsistentNaming

using NakedObjects;

namespace Legacy.NakedObjects.Application.Control
{
  //[JavaInterface]
  public interface FieldAbout
  {
      [NakedObjectsIgnore] void invisible();

      [NakedObjectsIgnore] void invisibleToUser(User user);

      [NakedObjectsIgnore] void invisibleToUsers(User[] users);

      [NakedObjectsIgnore] bool isPersistent();

      [NakedObjectsIgnore] void modifiableOnlyByRole(Role role);

      [NakedObjectsIgnore] void modifiableOnlyByRoles(Role[] roles);

      [NakedObjectsIgnore] void modifiableOnlyByUser(User user);

      [NakedObjectsIgnore] void modifiableOnlyByUsers(User[] users);

      [NakedObjectsIgnore] void modifiableOnlyInState(State state);

      [NakedObjectsIgnore] void modifiableOnlyInStates(State[] states);

      [NakedObjectsIgnore] void nonPersistent();

      [NakedObjectsIgnore] void setDescription(string @string);

      [NakedObjectsIgnore] void unmodifiable();

      [NakedObjectsIgnore] void unmodifiable(string reason);

      [NakedObjectsIgnore] void unmodifiableByUser(User user);

      [NakedObjectsIgnore] void unmodifiableByUsers(User[] users);

      [NakedObjectsIgnore] void unmodifiableInState(State state);

      [NakedObjectsIgnore] void unmodifiableInStates(State[] states);

      [NakedObjectsIgnore] void unmodifiableOnCondition(bool conditionMet, string reasonNotMet);

      [NakedObjectsIgnore] void visibleOnlyToRole(Role role);

      [NakedObjectsIgnore] void visibleOnlyToRoles(Role[] roles);

      [NakedObjectsIgnore] void visibleOnlyToUser(User user);

      [NakedObjectsIgnore] void visibleOnlyToUsers(User[] users);

      [NakedObjectsIgnore] void invalid();

      [NakedObjectsIgnore] void invalid(string reason);

      [NakedObjectsIgnore] void invalidOnCondition(bool condition, string reason);
  }
}
