// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.SpecificationCache
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterface]
  public interface SpecificationCache
  {
    NakedObjectSpecification get(Class cls);

    void cache(Class cls, NakedObjectSpecification spec);

    void clear();

    NakedObjectSpecification[] allSpecifications();

    void cache(string className, NakedObjectSpecification spec);
  }
}
