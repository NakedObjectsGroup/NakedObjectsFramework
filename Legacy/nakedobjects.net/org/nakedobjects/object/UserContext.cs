// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.UserContext
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using org.nakedobjects.@object;

namespace org.nakedobjects.@object
{
  [JavaInterfaces("1;org/nakedobjects/object/InternalNakedObject;")]
  [JavaInterface]
  public interface UserContext : InternalNakedObject
  {
    void addToClasses(NakedClass cls);

    void addToObjects(NakedObject cls);

    void removeFromClasses(NakedClass cls);

    void removeFromObjects(NakedObject cls);

    Vector getClasses();

    Vector getObjects();

    User getUser();

    string getName();
  }
}
