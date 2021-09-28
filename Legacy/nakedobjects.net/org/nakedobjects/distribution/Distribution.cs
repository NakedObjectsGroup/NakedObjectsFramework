// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.Distribution
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution
{
  [JavaInterface]
  public interface Distribution
  {
    ObjectData[] allInstances(Session session, string fullName, bool includeSubclasses);

    ObjectData[] clearAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associate);

    ServerActionResultData executeServerAction(
      Session session,
      string actionType,
      string actionIdentifier,
      ReferenceData target,
      Data[] parameters,
      int actionGraphDepth);

    ClientActionResultData executeClientAction(
      Session session,
      ReferenceData[] deleted,
      int[] types);

    ObjectData[] findInstances(Session session, InstancesCriteria criteria);

    bool hasInstances(Session session, string fullName);

    int numberOfInstances(Session sessionId, string fullName);

    Data resolveField(Session session, IdentityData data, string name);

    ObjectData resolveImmediately(Session session, IdentityData target);

    ObjectData[] setAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associate);

    ObjectData[] setValue(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      object value);
  }
}
