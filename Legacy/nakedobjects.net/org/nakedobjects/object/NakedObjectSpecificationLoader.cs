// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjectSpecificationLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object
{
  [JavaInterfaces("2;org/nakedobjects/object/NakedObjectsComponent;org/nakedobjects/utility/DebugInfo;")]
  [JavaInterface]
  public interface NakedObjectSpecificationLoader : NakedObjectsComponent, DebugInfo
  {
    NakedObjectSpecification loadSpecification(string name);

    NakedObjectSpecification loadSpecification(Class cls);

    NakedObjectSpecification[] allSpecifications();
  }
}
