// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.NullSpecifcationLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.reflect.@internal;

namespace org.nakedobjects.@object.reflect.@internal
{
  public class NullSpecifcationLoader : AbstractSpecificationLoader
  {
    [JavaFlags(4)]
    public override NakedObjectSpecification install(
      Class cls,
      ReflectionPeerBuilder builder)
    {
      return (NakedObjectSpecification) new NullSpecification(cls.getName());
    }
  }
}
