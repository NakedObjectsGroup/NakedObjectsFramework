// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.inmemory.MemoryObjectStoreInstances
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.@object.persistence.objectstore.inmemory;
using System;

namespace org.nakedobjects.@object.persistence.objectstore.inmemory
{
  public class MemoryObjectStoreInstances
  {
    [JavaFlags(20)]
    public readonly Hashtable objectInstances;
    [JavaFlags(20)]
    public readonly Hashtable titleIndex;
    private NakedObjectLoader objectLoader;

    public MemoryObjectStoreInstances(NakedObjectLoader objectLoader)
    {
      this.objectInstances = new Hashtable();
      this.titleIndex = new Hashtable();
      this.objectLoader = objectLoader;
    }

    public virtual Enumeration elements()
    {
      Vector vector = new Vector(this.objectInstances.size());
      Enumeration enumeration = this.objectInstances.keys();
      while (enumeration.hasMoreElements())
      {
        Oid oid = (Oid) enumeration.nextElement();
        vector.addElement((object) this.getObject(oid));
      }
      return vector.elements();
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        Logger.getLogger(Class.FromType(typeof (MemoryObjectStoreInstances))).info((object) "finalizing instances");
      }
      catch (Exception ex)
      {
      }
    }

    public virtual NakedObject getObject(Oid oid)
    {
      NakedObject adapterFor = this.objectLoader.getAdapterFor(oid);
      if (adapterFor != null)
        return adapterFor;
      object @object = this.objectInstances.get((object) oid);
      return @object == null ? (NakedObject) null : this.objectLoader.recreateAdapterForPersistent(oid, @object);
    }

    public virtual Oid getOidFor(object @object)
    {
      Enumeration enumeration = this.objectInstances.keys();
      while (enumeration.hasMoreElements())
      {
        Oid oid = (Oid) enumeration.nextElement();
        if (this.objectInstances.get((object) oid).Equals(@object))
          return oid;
      }
      return (Oid) null;
    }

    public virtual bool hasInstances() => this.numberOfInstances() > 0;

    public virtual void instances(InstancesCriteria criteria, Vector instances)
    {
      if (criteria is TitleCriteria)
      {
        object obj = this.titleIndex.get((object) ((TitleCriteria) criteria).getRequiredTitle());
        if (obj != null)
        {
          NakedObject nakedObject = this.getObject((Oid) obj);
          instances.addElement((object) nakedObject);
          return;
        }
      }
      Enumeration enumeration = this.elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject @object = (NakedObject) enumeration.nextElement();
        if (criteria.matches(@object))
          instances.addElement((object) @object);
      }
    }

    public virtual void instances(Vector instances)
    {
      Enumeration enumeration = this.elements();
      while (enumeration.hasMoreElements())
        instances.addElement(enumeration.nextElement());
    }

    public virtual int numberOfInstances() => this.objectInstances.size();

    public virtual void remove(Oid oid)
    {
      this.getObject(oid);
      this.objectInstances.remove((object) oid);
    }

    public virtual void save(NakedObject @object)
    {
      this.objectInstances.put((object) @object.getOid(), @object.getObject());
      this.titleIndex.put((object) StringImpl.toLowerCase(@object.titleString()), (object) @object.getOid());
    }

    public virtual void shutdown()
    {
      this.objectInstances.clear();
      this.titleIndex.clear();
    }

    public virtual void reset()
    {
      Enumeration enumeration = this.objectInstances.keys();
      while (enumeration.hasMoreElements())
      {
        object obj = (object) this.getObject((Oid) enumeration.nextElement());
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      MemoryObjectStoreInstances objectStoreInstances = this;
      ObjectImpl.clone((object) objectStoreInstances);
      return ((object) objectStoreInstances).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
