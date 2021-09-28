// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.inmemory.MemoryObjectStore
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
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
  public class MemoryObjectStore : NakedObjectStore
  {
    private static readonly Category LOG;
    [JavaFlags(4)]
    public Hashtable instances;

    public MemoryObjectStore()
    {
      if (MemoryObjectStore.LOG.isInfoEnabled())
        MemoryObjectStore.LOG.info((object) "creating object store");
      this.instances = new Hashtable();
    }

    public virtual void abortTransaction()
    {
      if (!MemoryObjectStore.LOG.isDebugEnabled())
        return;
      MemoryObjectStore.LOG.debug((object) "transaction aborted");
    }

    public virtual CreateObjectCommand createCreateObjectCommand(
      NakedObject @object)
    {
      return (CreateObjectCommand) new MemoryObjectStore.\u0031(this, @object);
    }

    public virtual DestroyObjectCommand createDestroyObjectCommand(
      NakedObject @object)
    {
      return (DestroyObjectCommand) new MemoryObjectStore.\u0032(this, @object);
    }

    public virtual SaveObjectCommand createSaveObjectCommand(NakedObject @object) => (SaveObjectCommand) new MemoryObjectStore.\u0033(this, @object);

    public virtual Transaction createTransaction() => (Transaction) new ObjectStoreTransaction((NakedObjectStore) this);

    private string debugCollectionGraph(
      NakedCollection collection,
      string name,
      int level,
      Vector recursiveElements)
    {
      // ISSUE: unable to decompile the method.
    }

    private string debugGraph(
      NakedObject @object,
      string name,
      int level,
      Vector recursiveElements)
    {
      if (level > 3)
        return "...\n";
      if (recursiveElements == null)
        recursiveElements = new Vector(25, 10);
      return @object is NakedCollection ? new StringBuffer().append("\n").append(this.debugCollectionGraph((NakedCollection) @object, name, level, recursiveElements)).ToString() : new StringBuffer().append("\n").append(this.debugObjectGraph(@object, name, level, recursiveElements)).ToString();
    }

    private string debugObjectGraph(
      NakedObject @object,
      string name,
      int level,
      Vector recursiveElements)
    {
      StringBuffer s = new StringBuffer();
      recursiveElements.addElement((object) @object);
      foreach (NakedObjectField field1 in @object.getSpecification().getFields())
      {
        object field2 = (object) @object.getField(field1);
        name = field1.getId();
        this.indent(s, level);
        if (field1.isCollection())
          s.append(new StringBuffer().append(name).append(": \n").append(this.debugCollectionGraph((NakedCollection) field2, "nnn", level + 1, recursiveElements)).ToString());
        else if (field2 is NakedObject)
        {
          if (recursiveElements.contains(field2))
          {
            s.append(new StringBuffer().append(name).append(": ").append(field2).append("*\n").ToString());
          }
          else
          {
            s.append(new StringBuffer().append(name).append(": ").append(field2).ToString());
            s.append(this.debugGraph((NakedObject) field2, name, level + 1, recursiveElements));
          }
        }
        else
        {
          s.append(new StringBuffer().append(name).append(": ").append(field2).ToString());
          s.append("\n");
        }
      }
      return s.ToString();
    }

    private void destroy(NakedObject @object)
    {
      NakedObjectSpecification specification = @object.getSpecification();
      if (MemoryObjectStore.LOG.isDebugEnabled())
        MemoryObjectStore.LOG.debug((object) new StringBuffer().append("   destroy object ").append((object) @object).append(" as instance of ").append(specification.getShortName()).ToString());
      this.instancesFor(specification).remove(@object.getOid());
    }

    public virtual void endTransaction()
    {
      if (!MemoryObjectStore.LOG.isDebugEnabled())
        return;
      MemoryObjectStore.LOG.debug((object) "end transaction");
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!MemoryObjectStore.LOG.isInfoEnabled())
          return;
        MemoryObjectStore.LOG.info((object) "finalizing object store");
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
        Enumeration enumeration2 = this.instancesFor(spec).elements();
        debug.indent();
        if (!enumeration2.hasMoreElements())
          debug.appendln("no instances");
        while (enumeration2.hasMoreElements())
          debug.appendln(enumeration2.nextElement().ToString());
      }
      debug.unindent();
      debug.appendln();
      debug.appendTitle("Object graphs");
      Vector recursiveElements = new Vector();
      Enumeration enumeration3 = this.instances.keys();
      while (enumeration3.hasMoreElements())
      {
        NakedObjectSpecification spec = (NakedObjectSpecification) enumeration3.nextElement();
        Enumeration enumeration4 = this.instancesFor(spec).elements();
        while (enumeration4.hasMoreElements())
        {
          NakedObject @object = (NakedObject) enumeration4.nextElement();
          debug.append((object) spec.getFullName());
          debug.append((object) ": ");
          debug.append((object) @object);
          debug.appendln(this.debugGraph(@object, "name???", 0, recursiveElements));
        }
      }
    }

    public virtual string getDebugTitle() => this.name();

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectPerstsistenceException;org/nakedobjects/object/UnsupportedFindException;")]
    public virtual NakedObject[] getInstances(InstancesCriteria criteria)
    {
      Vector instances = new Vector();
      this.getInstances(criteria, instances);
      return this.toInstancesArray(instances);
    }

    private void getInstances(InstancesCriteria criteria, Vector instances)
    {
      NakedObjectSpecification specification = criteria.getSpecification();
      this.instancesFor(specification).instances(criteria, instances);
      if (!criteria.includeSubclasses())
        return;
      foreach (NakedObjectSpecification subclass in specification.subclasses())
        this.getInstances(subclass, instances, true);
    }

    public virtual NakedObject[] getInstances(
      NakedObjectSpecification spec,
      bool includeSubclasses)
    {
      if (MemoryObjectStore.LOG.isDebugEnabled())
        MemoryObjectStore.LOG.debug((object) new StringBuffer().append("get instances").append(!includeSubclasses ? "" : " (included subclasses)").ToString());
      Vector instances = new Vector();
      this.getInstances(spec, instances, includeSubclasses);
      return this.toInstancesArray(instances);
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
      if (MemoryObjectStore.LOG.isDebugEnabled())
        MemoryObjectStore.LOG.debug((object) new StringBuffer().append("getObject ").append((object) oid).ToString());
      NakedObject @object = this.instancesFor(hint).getObject(oid);
      if (@object == null)
        throw new ObjectNotFoundException((object) oid);
      this.setupReferencedObjects(@object);
      return @object;
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

    private void indent(StringBuffer s, int level)
    {
      for (int index = 0; index < level; ++index)
        s.append(new StringBuffer().append(Debug.indentString(4)).append("|").ToString());
      s.append(new StringBuffer().append(Debug.indentString(4)).append("+--").ToString());
    }

    private MemoryObjectStoreInstances instancesFor(
      NakedObjectSpecification spec)
    {
      MemoryObjectStoreInstances objectStoreInstances = (MemoryObjectStoreInstances) this.instances.get((object) spec);
      if (objectStoreInstances == null)
      {
        objectStoreInstances = new MemoryObjectStoreInstances(NakedObjects.getObjectLoader());
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

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void resolveField(NakedObject @object, NakedObjectField field)
    {
      NakedReference field1 = (NakedReference) @object.getField(field);
      NakedObjects.getObjectLoader().start(field1, ResolveState.RESOLVING);
      NakedObjects.getObjectLoader().end(field1);
    }

    public virtual void reset()
    {
      NakedObjects.getObjectLoader().reset();
      Enumeration enumeration = this.instances.elements();
      while (enumeration.hasMoreElements())
        ((MemoryObjectStoreInstances) enumeration.nextElement()).reset();
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void execute(PersistenceCommand[] commands)
    {
      if (MemoryObjectStore.LOG.isInfoEnabled())
        MemoryObjectStore.LOG.info((object) "start execution of transaction ");
      for (int index = 0; index < commands.Length; ++index)
        commands[index].execute((ExecutionContext) null);
      if (!MemoryObjectStore.LOG.isInfoEnabled())
        return;
      MemoryObjectStore.LOG.info((object) "end execution");
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    private void save(NakedObject @object)
    {
      NakedObjectSpecification spec = !(@object.getObject() is NakedClass) ? @object.getSpecification() : throw new ObjectPerstsistenceException("Can't make changes to a NakedClass object");
      if (MemoryObjectStore.LOG.isDebugEnabled())
        MemoryObjectStore.LOG.debug((object) new StringBuffer().append("   saving object ").append((object) @object).append(" as instance of ").append(spec.getShortName()).ToString());
      this.instancesFor(spec).save(@object);
    }

    private void setupReferencedObjects(NakedObject @object) => this.setupReferencedObjects(@object, new Vector());

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void resolveImmediately(NakedObject @object)
    {
      if (MemoryObjectStore.LOG.isDebugEnabled())
        MemoryObjectStore.LOG.debug((object) new StringBuffer().append("resolve ").append((object) @object).ToString());
      this.setupReferencedObjects(@object);
      NakedObjects.getObjectLoader().start((NakedReference) @object, ResolveState.RESOLVING);
      NakedObjects.getObjectLoader().end((NakedReference) @object);
    }

    private void setupReferencedObjects(NakedObject @object, Vector all)
    {
    }

    public virtual void shutdown()
    {
      if (MemoryObjectStore.LOG.isInfoEnabled())
        MemoryObjectStore.LOG.info((object) new StringBuffer().append("shutdown ").append((object) this).ToString());
      Enumeration enumeration = this.instances.elements();
      while (enumeration.hasMoreElements())
        ((MemoryObjectStoreInstances) enumeration.nextElement()).shutdown();
      this.instances.clear();
    }

    public virtual void startTransaction()
    {
      if (!MemoryObjectStore.LOG.isDebugEnabled())
        return;
      MemoryObjectStore.LOG.debug((object) "start transaction");
    }

    private NakedObject[] toInstancesArray(Vector instances)
    {
      int length = instances.size();
      NakedObject[] nakedObjectArray = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < nakedObjectArray.Length; ++index)
      {
        NakedObject @object = (NakedObject) instances.elementAt(index);
        this.setupReferencedObjects(@object);
        if (@object.getResolveState().isResolvable(ResolveState.RESOLVING))
        {
          NakedObjects.getObjectLoader().start((NakedReference) @object, ResolveState.RESOLVING);
          NakedObjects.getObjectLoader().end((NakedReference) @object);
        }
        nakedObjectArray[index] = @object;
      }
      return nakedObjectArray;
    }

    public virtual void init()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static MemoryObjectStore()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      MemoryObjectStore memoryObjectStore = this;
      ObjectImpl.clone((object) memoryObjectStore);
      return ((object) memoryObjectStore).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaInterfaces("1;org/nakedobjects/object/transaction/CreateObjectCommand;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0031 : CreateObjectCommand
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private MemoryObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
      public virtual void execute(ExecutionContext context)
      {
        if (MemoryObjectStore.LOG.isDebugEnabled())
          MemoryObjectStore.LOG.debug((object) new StringBuffer().append("  create object ").append((object) this.object_\u003E).ToString());
        this.this\u00240.save(this.object_\u003E);
      }

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("CreateObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0031(MemoryObjectStore _param1, [In] NakedObject obj1)
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
        MemoryObjectStore.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }

    [JavaFlags(32)]
    [JavaInterfaces("1;org/nakedobjects/object/transaction/DestroyObjectCommand;")]
    [Inner]
    public class \u0032 : DestroyObjectCommand
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private MemoryObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
      public virtual void execute(ExecutionContext context)
      {
        if (MemoryObjectStore.LOG.isInfoEnabled())
          MemoryObjectStore.LOG.info((object) new StringBuffer().append("  delete object '").append((object) this.object_\u003E).append("'").ToString());
        this.this\u00240.destroy(this.object_\u003E);
      }

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("DestroyObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0032(MemoryObjectStore _param1, [In] NakedObject obj1)
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
        MemoryObjectStore.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }

    [JavaInterfaces("1;org/nakedobjects/object/transaction/SaveObjectCommand;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0033 : SaveObjectCommand
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private MemoryObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
      public virtual void execute(ExecutionContext context) => this.this\u00240.save(this.object_\u003E);

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("SaveObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0033(MemoryObjectStore _param1, [In] NakedObject obj1)
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
        MemoryObjectStore.\u0033 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }
  }
}
