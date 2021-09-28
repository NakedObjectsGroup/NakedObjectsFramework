// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.loader.IdentityAdapterMap
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.loader
{
  [JavaInterface]
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public interface IdentityAdapterMap : DebugInfo
  {
    void add(Oid oid, NakedReference adapter);

    NakedObject getAdapter(Oid oid);

    bool isIdentityKnown(Oid oid);

    Enumeration oids();

    void remove(Oid oid);

    void reset();

    void shutdown();
  }
}
