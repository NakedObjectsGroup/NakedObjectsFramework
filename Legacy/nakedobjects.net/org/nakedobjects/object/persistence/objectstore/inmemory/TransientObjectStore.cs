// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.inmemory.TransientObjectStore
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
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.persistence.objectstore.inmemory;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.utility;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.@object.persistence.objectstore.inmemory
{
  [JavaInterfaces("1;org/nakedobjects/object/persistence/objectstore/NakedObjectStore;")]
  public class TransientObjectStore : NakedObjectStore
  {
    private static readonly Category LOG;
    [JavaFlags(20)]
    public readonly Hashtable objects;
    [JavaFlags(20)]
    public readonly Hashtable instances;

    public TransientObjectStore()
    {
      if (TransientObjectStore.LOG.isInfoEnabled())
        TransientObjectStore.LOG.info((object) "creating object store");
      this.instances = new Hashtable();
      this.objects = new Hashtable();
    }

    public virtual void abortTransaction()
    {
      if (!TransientObjectStore.LOG.isDebugEnabled())
        return;
      TransientObjectStore.LOG.debug((object) "transaction aborted");
    }

    public virtual CreateObjectCommand createCreateObjectCommand(
      NakedObject @object)
    {
      return (CreateObjectCommand) new TransientObjectStore.\u0031(this, @object);
    }

    public virtual DestroyObjectCommand createDestroyObjectCommand(
      NakedObject @object)
    {
      return (DestroyObjectCommand) new TransientObjectStore.\u0032(this, @object);
    }

    public virtual SaveObjectCommand createSaveObjectCommand(NakedObject @object) => (SaveObjectCommand) new TransientObjectStore.\u0033(this, @object);

    public virtual Transaction createTransaction() => (Transaction) new ObjectStoreTransaction((NakedObjectStore) this);

    public virtual void endTransaction()
    {
      if (!TransientObjectStore.LOG.isDebugEnabled())
        return;
      TransientObjectStore.LOG.debug((object) "end transaction");
    }

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    [JavaFlags(4)]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!TransientObjectStore.LOG.isInfoEnabled())
          return;
        TransientObjectStore.LOG.info((object) "finalizing object store");
      }
      catch (Exception ex)
      {
      }
    }

    public virtual void debugData(DebugString debug)
    {
      debug.appendTitle("Business Objects");
      Enumeration enumeration1 = this.instances.keys();
      while (enumeration1.hasMoreElements())
      {
        NakedObjectSpecification spec = (NakedObjectSpecification) enumeration1.nextElement();
        debug.appendln(spec.getFullName());
        TransientObjectStoreInstances objectStoreInstances = this.instancesFor(spec);
        Vector instances = new Vector();
        objectStoreInstances.instances(instances);
        Enumeration enumeration2 = instances.elements();
        debug.indent();
        if (!enumeration2.hasMoreElements())
          debug.appendln("no instances");
        while (enumeration2.hasMoreElements())
          debug.appendln(this.objects.get(enumeration2.nextElement()).ToString());
        debug.unindent();
      }
      debug.appendln();
      debug.appendTitle("Object graphs");
      Vector excludedObjects = new Vector();
      Enumeration enumeration3 = this.instances.keys();
      while (enumeration3.hasMoreElements())
      {
        NakedObjectSpecification spec = (NakedObjectSpecification) enumeration3.nextElement();
        TransientObjectStoreInstances objectStoreInstances = this.instancesFor(spec);
        Vector instances = new Vector();
        objectStoreInstances.instances(instances);
        Enumeration enumeration4 = instances.elements();
        while (enumeration4.hasMoreElements())
        {
          debug.append((object) spec.getFullName());
          debug.append((object) ": ");
          NakedObject nakedObject = (NakedObject) this.objects.get((object) (Oid) enumeration4.nextElement());
          debug.appendln(Dump.graph((Naked) nakedObject, excludedObjects));
        }
      }
    }

    public virtual string getDebugTitle() => this.name();

    public virtual NakedObject[] getInstances(InstancesCriteria criteria)
    {
      Vector instances = new Vector();
      this.getInstances(criteria, instances);
      int length1 = instances.size();
      NakedObject[] nakedObjectArray1 = length1 >= 0 ? new NakedObject[length1] : throw new NegativeArraySizeException();
      int num1 = 0;
      for (int index1 = 0; index1 < instances.size(); ++index1)
      {
        NakedObject @object = (NakedObject) this.objects.get((object) (Oid) instances.elementAt(index1));
        if (criteria.matches(@object))
        {
          NakedObject[] nakedObjectArray2 = nakedObjectArray1;
          int num2;
          num1 = (num2 = num1) + 1;
          int index2 = num2;
          NakedObject nakedObject = @object;
          nakedObjectArray2[index2] = nakedObject;
        }
      }
      int length2 = num1;
      NakedObject[] nakedObjectArray3 = length2 >= 0 ? new NakedObject[length2] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) nakedObjectArray1, 0, (object) nakedObjectArray3, 0, num1);
      return nakedObjectArray3;
    }

    private void getInstances(InstancesCriteria criteria, Vector instances)
    {
      NakedObjectSpecification specification = criteria.getSpecification();
      this.instancesFor(specification).instances(instances);
      if (!criteria.includeSubclasses())
        return;
      foreach (NakedObjectSpecification subclass in specification.subclasses())
        this.getInstances(subclass, instances, true);
    }

    public virtual NakedObject[] getInstances(
      NakedObjectSpecification spec,
      bool includeSubclasses)
    {
      if (TransientObjectStore.LOG.isDebugEnabled())
        TransientObjectStore.LOG.debug((object) new StringBuffer().append("get instances").append(!includeSubclasses ? "" : " (included subclasses)").ToString());
      Vector instances = new Vector();
      this.getInstances(spec, instances, includeSubclasses);
      int length = instances.size();
      NakedObject[] nakedObjectArray = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < nakedObjectArray.Length; ++index)
      {
        Oid oid = (Oid) instances.elementAt(index);
        nakedObjectArray[index] = (NakedObject) this.objects.get((object) oid);
      }
      return nakedObjectArray;
    }

    private void getInstances(
      NakedObjectSpecification spec,
      Vector instances,
      bool includeSubclasses)
    {
      this.instancesFor(spec).instances(instances);
      if (!includeSubclasses)
        return;
      foreach (NakedObjectSpecification subclass in spec.subclasses())
        this.getInstances(subclass, instances, true);
    }

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual NakedClass getNakedClass(string name) => throw new ObjectNotFoundException();

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual NakedObject getObject(Oid oid, NakedObjectSpecification hint)
    {
      if (TransientObjectStore.LOG.isDebugEnabled())
        TransientObjectStore.LOG.debug((object) new StringBuffer().append("getObject ").append((object) oid).ToString());
      return (NakedObject) this.objects.get((object) oid) ?? throw new ObjectNotFoundException((object) oid);
    }

    public virtual bool hasInstances(NakedObjectSpecification spec, bool includeSubclasses)
    {
      if (this.instancesFor(spec).hasInstances())
        return true;
      if (includeSubclasses)
      {
        foreach (NakedObjectSpecification subclass in spec.subclasses())
        {
          if (this.hasInstances(subclass, includeSubclasses))
            return true;
        }
      }
      return false;
    }

    public virtual void init()
    {
      if (!TransientObjectStore.LOG.isInfoEnabled())
        return;
      TransientObjectStore.LOG.info((object) new StringBuffer().append("initialising ").append((object) this).ToString());
    }

    private TransientObjectStoreInstances instancesFor(
      NakedObjectSpecification spec)
    {
      TransientObjectStoreInstances objectStoreInstances = (TransientObjectStoreInstances) this.instances.get((object) spec);
      if (objectStoreInstances == null)
      {
        objectStoreInstances = new TransientObjectStoreInstances();
        this.instances.put((object) spec, (object) objectStoreInstances);
      }
      return objectStoreInstances;
    }

    public virtual string name() => "Transient Object Store";

    public virtual int numberOfInstances(NakedObjectSpecification spec, bool includeSubclasses)
    {
      int num = this.instancesFor(spec).numberOfInstances();
      if (includeSubclasses)
      {
        foreach (NakedObjectSpecification subclass in spec.subclasses())
          num += this.numberOfInstances(subclass, true);
      }
      return num;
    }

    public virtual void resolveField(NakedObject @object, NakedObjectField field)
    {
      NakedCollection nakedCollection = field.isCollection() ? (NakedCollection) @object.getField(field) : throw new UnexpectedCallException();
      NakedObjects.getObjectLoader().start((NakedReference) nakedCollection, ResolveState.RESOLVING);
      NakedObjects.getObjectLoader().end((NakedReference) nakedCollection);
    }

    public virtual void reset()
    {
    }

    public virtual void execute(PersistenceCommand[] commands)
    {
      bool flag = TransientObjectStore.LOG.isInfoEnabled();
      if (flag)
        TransientObjectStore.LOG.info((object) "start execution of transaction ");
      for (int index = 0; index < commands.Length; ++index)
        commands[index].execute((ExecutionContext) null);
      if (!flag)
        return;
      TransientObjectStore.LOG.info((object) "end execution");
    }

    public virtual void resolveImmediately(NakedObject @object)
    {
      if (!TransientObjectStore.LOG.isDebugEnabled())
        return;
      TransientObjectStore.LOG.debug((object) new StringBuffer().append("resolve ").append((object) @object).ToString());
    }

    public virtual void shutdown()
    {
      if (TransientObjectStore.LOG.isInfoEnabled())
        TransientObjectStore.LOG.info((object) new StringBuffer().append("shutting down ").append((object) this).ToString());
      this.objects.clear();
      Enumeration enumeration = this.instances.elements();
      while (enumeration.hasMoreElements())
        ((TransientObjectStoreInstances) enumeration.nextElement()).shutdown();
      this.instances.clear();
    }

    public virtual void startTransaction()
    {
      if (!TransientObjectStore.LOG.isDebugEnabled())
        return;
      TransientObjectStore.LOG.debug((object) "start transaction");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TransientObjectStore()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TransientObjectStore transientObjectStore = this;
      ObjectImpl.clone((object) transientObjectStore);
      return ((object) transientObjectStore).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(32)]
    [JavaInterfaces("1;org/nakedobjects/object/transaction/CreateObjectCommand;")]
    [Inner]
    public class \u0031 : CreateObjectCommand
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TransientObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      public virtual void execute(ExecutionContext context)
      {
        if (TransientObjectStore.LOG.isDebugEnabled())
          TransientObjectStore.LOG.debug((object) new StringBuffer().append("  create object ").append((object) this.object_\u003E).ToString());
        NakedObjectSpecification specification = this.object_\u003E.getSpecification();
        if (TransientObjectStore.LOG.isDebugEnabled())
          TransientObjectStore.LOG.debug((object) new StringBuffer().append("   saving object ").append((object) this.object_\u003E).append(" as instance of ").append(specification.getFullName()).ToString());
        this.this\u00240.instancesFor(specification).add(this.object_\u003E);
        this.this\u00240.objects.put((object) this.object_\u003E.getOid(), (object) this.object_\u003E);
        this.object_\u003E.setOptimisticLock((org.nakedobjects.@object.Version) new LongNumberVersion(1L, "user", new Date()));
      }

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("CreateObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0031(TransientObjectStore _param1, [In] NakedObject obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.object_\u003E = obj1;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        TransientObjectStore.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }

    [Inner]
    [JavaInterfaces("1;org/nakedobjects/object/transaction/DestroyObjectCommand;")]
    [JavaFlags(32)]
    public class \u0032 : DestroyObjectCommand
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TransientObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      public virtual void execute(ExecutionContext context)
      {
        if (TransientObjectStore.LOG.isInfoEnabled())
          TransientObjectStore.LOG.info((object) new StringBuffer().append("  delete object '").append((object) this.object_\u003E).append("'").ToString());
        this.this\u00240.objects.remove((object) this.object_\u003E.getOid());
        NakedObjectSpecification specification = this.object_\u003E.getSpecification();
        if (TransientObjectStore.LOG.isDebugEnabled())
          TransientObjectStore.LOG.debug((object) new StringBuffer().append("   destroy object ").append((object) this.object_\u003E).append(" as instance of ").append(specification.getFullName()).ToString());
        this.this\u00240.instancesFor(specification).remove(this.object_\u003E.getOid());
      }

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("DestroyObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0032(TransientObjectStore _param1, [In] NakedObject obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.object_\u003E = obj1;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        TransientObjectStore.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;org/nakedobjects/object/transaction/SaveObjectCommand;")]
    public class \u0033 : SaveObjectCommand
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private TransientObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      public virtual void execute(ExecutionContext context)
      {
        NakedObjectSpecification specification = this.object_\u003E.getSpecification();
        if (TransientObjectStore.LOG.isDebugEnabled())
          TransientObjectStore.LOG.debug((object) new StringBuffer().append("   saving object ").append((object) this.object_\u003E).append(" as instance of ").append(specification.getFullName()).ToString());
        this.this\u00240.instancesFor(specification).save(this.object_\u003E);
        org.nakedobjects.@object.Version version = this.object_\u003E.getVersion();
        if (!(version is LongNumberVersion))
          return;
        this.object_\u003E.setOptimisticLock(((AbstractVersion) version).next("user", new Date()));
      }

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("SaveObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0033(TransientObjectStore _param1, [In] NakedObject obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.object_\u003E = obj1;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        TransientObjectStore.\u0033 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }
  }
}
