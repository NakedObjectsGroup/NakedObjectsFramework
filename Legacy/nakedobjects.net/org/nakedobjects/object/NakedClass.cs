// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedClass
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.@object
{
  [JavaInterface]
  [JavaInterfaces("1;org/nakedobjects/object/InternalNakedObject;")]
  public interface NakedClass : InternalNakedObject
  {
    NakedCollection allInstances();

    NakedObjectSpecification forObjectType();

    string getFullName();

    string getName();

    string getShortName();

    string getPluralName();

    string getSingularName();

    string title();

    Consent useAllInstance();

    Consent useCreate();
  }
}
