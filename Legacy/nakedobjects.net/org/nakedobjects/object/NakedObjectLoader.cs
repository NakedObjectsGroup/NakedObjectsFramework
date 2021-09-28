// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.NakedObjectLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object
{
  [JavaInterface]
  [JavaInterfaces("2;org/nakedobjects/object/NakedObjectsComponent;org/nakedobjects/utility/DebugInfo;")]
  public interface NakedObjectLoader : NakedObjectsComponent, DebugInfo
  {
    NakedCollection createAdapterForCollection(
      object collection,
      NakedObjectSpecification specification);

    NakedObject createAdapterForTransient(object @object);

    NakedValue createAdapterForValue(object value);

    NakedObject createTransientInstance(NakedObjectSpecification specification);

    NakedValue createValueInstance(NakedObjectSpecification specification);

    void end(NakedReference reference);

    NakedObject getAdapterFor(object @object);

    NakedObject getAdapterFor(Oid oid);

    NakedCollection getAdapterForElseCreateAdapterForCollection(
      NakedObject parent,
      string fieldName,
      NakedObjectSpecification elementSpecification,
      object collection);

    NakedObject getAdapterForElseCreateAdapterForTransient(object @object);

    Enumeration getIdentifiedObjects();

    bool isIdentityKnown(Oid oid);

    void madePersistent(NakedReference @object, Oid oid);

    NakedObject recreateAdapterForPersistent(Oid oid, NakedObjectSpecification spec);

    NakedCollection recreateCollection(NakedObjectSpecification specification);

    NakedObject recreateTransientInstance(NakedObjectSpecification specification);

    NakedObject recreateAdapterForPersistent(Oid oid, object @object);

    void reset();

    void start(NakedReference reference, ResolveState targetState);

    void stateCreateChangeLock(Oid oid, NakedObjectSpecification spec);

    void stateCreateChangeRelease(Oid oid, NakedObjectSpecification spec);

    void unloaded(NakedObject @object);
  }
}
