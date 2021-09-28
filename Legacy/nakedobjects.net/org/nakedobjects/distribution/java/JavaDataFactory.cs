// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.java.JavaDataFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.java
{
  [JavaInterfaces("1;org/nakedobjects/distribution/DataFactory;")]
  public class JavaDataFactory : DataFactory
  {
    public virtual CollectionData createCollectionData(
      Oid oid,
      string type,
      ReferenceData[] elements,
      bool hasAllElements,
      Version version)
    {
      return (CollectionData) new JavaCollectionData(oid, type, elements, hasAllElements, version);
    }

    public virtual NullData createNullData(string type) => (NullData) new JavaNullData(type);

    public virtual ObjectData createObjectData(
      Oid oid,
      string type,
      bool hasCompleteData,
      Version version)
    {
      return (ObjectData) new JavaObjectData(oid, type, hasCompleteData, version);
    }

    public virtual IdentityData createIdentityData(
      string type,
      Oid oid,
      Version version)
    {
      return (IdentityData) new JavaReferenceData(type, oid, version);
    }

    public virtual ServerActionResultData createActionResultData(
      Data result,
      ObjectData[] updatesData,
      ObjectData persistedTarget,
      ObjectData[] persistedParameters,
      string[] messages,
      string[] warnings)
    {
      return (ServerActionResultData) new JavaResult(result, updatesData, persistedTarget, persistedParameters, messages, warnings);
    }

    public virtual ValueData createValueData(string type, object value) => (ValueData) new JavaValueData(type, value);

    public virtual ClientActionResultData createActionResultData(
      ObjectData[] madePersistent,
      Version[] changedVersion)
    {
      return (ClientActionResultData) new JavaClientResult(madePersistent, changedVersion);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaDataFactory javaDataFactory = this;
      ObjectImpl.clone((object) javaDataFactory);
      return ((object) javaDataFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
