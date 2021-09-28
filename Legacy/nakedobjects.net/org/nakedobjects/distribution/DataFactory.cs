// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.DataFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution
{
  [JavaInterface]
  public interface DataFactory
  {
    CollectionData createCollectionData(
      Oid oid,
      string type,
      ReferenceData[] elements,
      bool hasAllElements,
      Version version);

    NullData createNullData(string type);

    ObjectData createObjectData(
      Oid oid,
      string type,
      bool hasCompleteData,
      Version version);

    IdentityData createIdentityData(string type, Oid oid, Version version);

    ServerActionResultData createActionResultData(
      Data result,
      ObjectData[] updatesData,
      ObjectData persistedTarget,
      ObjectData[] persistedParameters,
      string[] messages,
      string[] warnings);

    ClientActionResultData createActionResultData(
      ObjectData[] madePersistent,
      Version[] changedVersion);

    ValueData createValueData(string fullName, object @object);
  }
}
