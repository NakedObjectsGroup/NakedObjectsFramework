// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.inmemory.TransientObjectStoreInstances
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence.objectstore.inmemory;
using System;

namespace org.nakedobjects.@object.persistence.objectstore.inmemory
{
  [JavaFlags(32)]
  public class TransientObjectStoreInstances
  {
    [JavaFlags(20)]
    public readonly Vector objectInstances;

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        Logger.getLogger(Class.FromType(typeof (TransientObjectStoreInstances))).info((object) "finalizing instances");
      }
      catch (Exception ex)
      {
      }
    }

    public virtual bool hasInstances() => this.numberOfInstances() > 0;

    public virtual void instances(Vector instances)
    {
      Enumeration enumeration = this.objectInstances.elements();
      while (enumeration.hasMoreElements())
      {
        Oid oid = (Oid) enumeration.nextElement();
        instances.addElement((object) oid);
      }
    }

    public virtual int numberOfInstances() => this.objectInstances.size();

    public virtual void remove(Oid oid) => this.objectInstances.removeElement((object) oid);

    public virtual void add(NakedObject @object) => this.objectInstances.addElement((object) @object.getOid());

    public virtual void save(NakedObject @object)
    {
    }

    public virtual void shutdown() => this.objectInstances.removeAllElements();

    [JavaFlags(0)]
    public TransientObjectStoreInstances() => this.objectInstances = new Vector();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TransientObjectStoreInstances objectStoreInstances = this;
      ObjectImpl.clone((object) objectStoreInstances);
      return ((object) objectStoreInstances).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
