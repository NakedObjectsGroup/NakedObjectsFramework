// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.Naked
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.@object
{
  [JavaInterface]
  public interface Naked
  {
    string getIconName();

    object getObject();

    Oid getOid();

    NakedObjectSpecification getSpecification();

    string titleString();
  }
}
