// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.BusinessObjectContainer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;

namespace org.nakedobjects.application
{
  [JavaInterface]
  public interface BusinessObjectContainer
  {
    Vector allInstances(Class cls);

    Vector allInstances(Class cls, bool includeSubclasses);

    object createInstance(Class cls);

    object createTransientInstance(Class cls);

    void destroyObject(object @object);

    bool hasInstances(Class cls);

    void informUser(string message);

    void init();

    bool isPersitent(object @object);

    void makePersistent(object transientObject);

    int numberOfInstances(Class cls);

    void objectChanged(object @object);

    void raiseError(string message);

    void resolve(object parent, object @object);

    long serialNumber(string sequence);

    void warnUser(string message);
  }
}
