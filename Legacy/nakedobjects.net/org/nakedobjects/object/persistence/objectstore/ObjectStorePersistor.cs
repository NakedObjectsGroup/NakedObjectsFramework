// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.objectstore.ObjectStorePersistor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.@object.persistence.objectstore
{
  [JavaInterfaces("1;org/nakedobjects/object/persistence/PersistedObjectAdder;")]
  public class ObjectStorePersistor : AbstracObjectPersistor, PersistedObjectAdder
  {
    private static readonly org.apache.log4j.Logger LOG;
    private bool checkObjectsForDirtyFlag;
    private readonly Hashtable nakedClasses;
    private NakedObjectStore objectStore;
    private DirtyObjectSet objectsToRefreshViewsFor;
    private Transaction transaction;
    private int transactionLevel;
    private PersistAlgorithm persistAlgorithm;

    public ObjectStorePersistor()
    {
      this.nakedClasses = new Hashtable();
      this.objectsToRefreshViewsFor = (DirtyObjectSet) new NullDirtyObjectSet();
      if (!ObjectStorePersistor.LOG.isInfoEnabled() && !ObjectStorePersistor.LOG.isDebugEnabled())
        return;
      string str = new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" creating objectStorePersistor/ object manager").ToString();
      ObjectStorePersistor.LOG.info((object) str);
      ObjectStorePersistor.LOG.debug((object) str, (Throwable) new Exception("This is the stack trace of where ObjectStorePersistor was created:"));
    }

    public override void abortTransaction()
    {
      if (this.transaction == null)
        return;
      this.transaction.abort();
      this.transaction = (Transaction) null;
      this.transactionLevel = 0;
      this.objectStore.abortTransaction();
    }

    public override void addObjectChangedListener(DirtyObjectSet listener)
    {
      Assert.assertNotNull("must set a listener", (object) listener);
      if (ObjectStorePersistor.LOG.isDebugEnabled())
        ObjectStorePersistor.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" addObjectChangedListener ").append(" Listener hashcode:").append(Long.toHexString((long) listener.GetHashCode())).ToString());
      this.objectsToRefreshViewsFor = listener;
    }

    public virtual void createObject(NakedObject @object)
    {
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" create object  ").append((object) @object).ToString());
      this.addCreateCommand(@object);
    }

    [JavaFlags(4)]
    public virtual CreateObjectCommand addCreateCommand(NakedObject @object)
    {
      CreateObjectCommand createObjectCommand = this.objectStore.createCreateObjectCommand(@object);
      this.getTransaction().addCommand((PersistenceCommand) createObjectCommand);
      return createObjectCommand;
    }

    public override void destroyObject(NakedObject @object)
    {
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" destroy object ").append((object) @object).ToString());
      this.addDestroyCommand(@object);
    }

    [JavaFlags(4)]
    public virtual DestroyObjectCommand addDestroyCommand(NakedObject @object)
    {
      DestroyObjectCommand destroyObjectCommand = this.objectStore.createDestroyObjectCommand(@object);
      this.getTransaction().addCommand((PersistenceCommand) destroyObjectCommand);
      @object.destroyed();
      return destroyObjectCommand;
    }

    [JavaFlags(4)]
    public virtual NakedObjectLoader getLoader() => NakedObjects.getObjectLoader();

    public override void endTransaction()
    {
      this.transactionLevel += -1;
      if (this.transactionLevel != 0)
        return;
      this.saveChanges();
      this.getTransaction().commit();
      this.transaction = (Transaction) null;
    }

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    [JavaFlags(4)]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!ObjectStorePersistor.LOG.isInfoEnabled())
          return;
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" finalizing object manager").ToString());
      }
      catch (Exception ex)
      {
      }
    }

    public override void debugData(DebugString debug)
    {
      debug.appendTitle("Persistor");
      debug.appendln("Check dirty flag", this.checkObjectsForDirtyFlag);
      debug.appendln("Classes", (object) this.nakedClasses);
      debug.appendln("To Refresh", (object) this.objectsToRefreshViewsFor);
      debug.appendln("Transaction", (object) this.transaction);
      debug.appendln("Persist Algorithm", (object) this.persistAlgorithm);
      debug.appendln("Object Store", (object) this.objectStore);
      debug.appendln();
      debug.append((DebugInfo) this.objectStore);
    }

    public override string getDebugTitle() => "Object Store Persistor";

    [JavaFlags(4)]
    public override NakedObject[] getInstances(InstancesCriteria criteria)
    {
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" getInstances matching ").append((object) criteria).ToString());
      NakedObject[] instances = this.objectStore.getInstances(criteria);
      this.clearChanges();
      return instances;
    }

    [JavaFlags(4)]
    public override NakedObject[] getInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" getInstances of ").append(specification.getShortName()).ToString());
      NakedObject[] instances = this.objectStore.getInstances(specification, false);
      this.clearChanges();
      return instances;
    }

    public override NakedClass getNakedClass(NakedObjectSpecification specification)
    {
      if (this.nakedClasses.contains((object) specification))
        return (NakedClass) this.nakedClasses.get((object) specification);
      NakedClass nakedClass;
      try
      {
        nakedClass = this.objectStore.getNakedClass(specification.getFullName());
      }
      catch (ObjectNotFoundException ex)
      {
        nakedClass = (NakedClass) new NakedClassImpl(specification.getFullName());
      }
      this.nakedClasses.put((object) specification, (object) nakedClass);
      return nakedClass;
    }

    public override NakedObject getObject(
      Oid oid,
      NakedObjectSpecification specification)
    {
      Assert.assertNotNull("needs an OID", (object) oid);
      Assert.assertNotNull("needs a specification", (object) specification);
      return !NakedObjects.getObjectLoader().isIdentityKnown(oid) ? this.objectStore.getObject(oid, specification) : NakedObjects.getObjectLoader().getAdapterFor(oid);
    }

    [JavaFlags(4)]
    public virtual NakedObjectStore getObjectStore() => this.objectStore;

    [JavaFlags(4)]
    public virtual PersistAlgorithm getPersistAlgorithm() => this.persistAlgorithm;

    [JavaFlags(4)]
    public virtual Transaction getTransaction() => this.transaction != null ? this.transaction : throw new TransactionException("No transaction started");

    public override bool hasInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("hasInstances of ").append(specification.getShortName()).ToString());
      return this.objectStore.hasInstances(specification, false);
    }

    public override void init()
    {
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" initialising ").append((object) this).ToString());
      Assert.assertNotNull("persist algorithm required", (object) this.persistAlgorithm);
      Assert.assertNotNull("object store required", (object) this.objectStore);
      this.objectStore.init();
      this.persistAlgorithm.init();
    }

    [JavaFlags(4)]
    public virtual bool isPersistent(NakedReference @object) => @object.getOid() != null && !@object.getOid().isNull();

    public override void makePersistent(NakedObject @object)
    {
      if (this.isPersistent((NakedReference) @object))
        throw new NotPersistableException("Object already persistent");
      if (@object.persistable() == Persistable.TRANSIENT)
        throw new NotPersistableException("Object must be transient");
      this.persistAlgorithm.makePersistent(@object, (PersistedObjectAdder) this);
    }

    public override int numberOfInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("numberOfInstances like ").append(specification.getShortName()).ToString());
      return this.objectStore.numberOfInstances(specification, false);
    }

    public override void objectChanged(NakedObject @object) => this.addChangedCommand(@object);

    [JavaFlags(4)]
    public virtual SaveObjectCommand addChangedCommand(NakedObject @object)
    {
      SaveObjectCommand saveObjectCommand = (SaveObjectCommand) null;
      if (@object.getResolveState().respondToChangesInPersistentObjects())
      {
        saveObjectCommand = this.objectStore.createSaveObjectCommand(@object);
        this.getTransaction().addCommand((PersistenceCommand) saveObjectCommand);
      }
      this.addAsDirty(@object);
      return saveObjectCommand;
    }

    [JavaFlags(4)]
    public virtual void addAsDirty(NakedObject @object)
    {
      string str = (string) null;
      if (@object.getOid() != null)
        str = @object.getOid().ToString();
      if (ObjectStorePersistor.LOG.isDebugEnabled())
        ObjectStorePersistor.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" ObjectStorePersistor.addAsDirty() ").append(" OID:").append(str).append(" Pojoadapter hash:").append(Long.toHexString((long) @object.GetHashCode())).append(" Listener hashcode:").append(Long.toHexString((long) this.objectsToRefreshViewsFor.GetHashCode())).ToString());
      this.objectsToRefreshViewsFor.addDirty(@object);
    }

    public override void reset() => this.objectStore.reset();

    public override void resolveField(NakedObject @object, NakedObjectField field)
    {
      if (field.isValue())
        return;
      NakedReference field1 = (NakedReference) @object.getField(field);
      if (field1.getResolveState().isResolved() || !field1.getResolveState().isPersistent())
        return;
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" resolve-field").append((object) @object).append("/").append(field.getId()).ToString());
      this.objectStore.resolveField(@object, field);
    }

    public override void reload(NakedObject @object)
    {
    }

    public override void refresh(NakedReference @object, Hashtable nonRefreshOids)
    {
    }

    public override void resolveImmediately(NakedObject @object)
    {
      if (!@object.getResolveState().isResolvable(ResolveState.RESOLVING))
        return;
      Assert.assertFalse("only resolve object that is not yet resolved", (object) @object, @object.getResolveState().isResolved());
      Assert.assertTrue("only resolve object that is persistent", (object) @object, @object.getResolveState().isPersistent());
      if (ObjectStorePersistor.LOG.isInfoEnabled())
        ObjectStorePersistor.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" resolve-immediately: ").append((object) @object).ToString());
      this.objectStore.resolveImmediately(@object);
    }

    public override void saveChanges() => this.collateChanges();

    [JavaFlags(36)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void collateChanges()
    {
      if (!this.checkObjectsForDirtyFlag)
        return;
      if (ObjectStorePersistor.LOG.isDebugEnabled())
        ObjectStorePersistor.LOG.debug((object) "collating changed objects");
      Enumeration identifiedObjects = this.getLoader().getIdentifiedObjects();
      while (identifiedObjects.hasMoreElements())
      {
        object obj = identifiedObjects.nextElement();
        if (obj is NakedObject)
        {
          NakedObject @object = (NakedObject) obj;
          if (@object.getSpecification().isDirty(@object))
          {
            if (ObjectStorePersistor.LOG.isDebugEnabled())
              ObjectStorePersistor.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("  collateChanges() found dirty object ").append((object) @object).ToString());
            this.objectChanged(@object);
            @object.getSpecification().clearDirty(@object);
          }
        }
      }
    }

    [JavaFlags(36)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void clearChanges()
    {
      if (!this.checkObjectsForDirtyFlag)
        return;
      if (ObjectStorePersistor.LOG.isDebugEnabled())
        ObjectStorePersistor.LOG.debug((object) "clearing changed objects");
      Enumeration identifiedObjects = this.getLoader().getIdentifiedObjects();
      while (identifiedObjects.hasMoreElements())
      {
        object obj = identifiedObjects.nextElement();
        if (obj is NakedObject)
        {
          NakedObject @object = (NakedObject) obj;
          if (@object.getSpecification().isDirty(@object))
          {
            if (ObjectStorePersistor.LOG.isDebugEnabled())
              ObjectStorePersistor.LOG.debug((object) new StringBuffer().append("  found dirty object ").append((object) @object).ToString());
            @object.getSpecification().clearDirty(@object);
          }
        }
      }
    }

    public virtual bool CheckObjectsForDirtyFlag
    {
      set => this.checkObjectsForDirtyFlag = value;
    }

    public virtual NakedObjectStore ObjectStore
    {
      set => this.objectStore = value;
    }

    public virtual void setCheckObjectsForDirtyFlag(bool checkObjectsForDirtyFlag) => this.checkObjectsForDirtyFlag = checkObjectsForDirtyFlag;

    public virtual void setObjectStore(NakedObjectStore objectStore) => this.objectStore = objectStore;

    public override void shutdown()
    {
      // ISSUE: unable to decompile the method.
    }

    public override void startTransaction()
    {
      if (this.transaction == null)
      {
        this.transaction = this.objectStore.createTransaction();
        this.transactionLevel = 0;
        this.objectStore.startTransaction();
      }
      ++this.transactionLevel;
    }

    public virtual bool isTransactionInProgress() => this.transaction != null;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      if (this.objectStore != null)
        toString.append("objectStore", this.objectStore.name());
      if (this.persistAlgorithm != null)
        toString.append("persistAlgorithm", this.persistAlgorithm.name());
      return toString.ToString();
    }

    public virtual PersistAlgorithm PersistAlgorithm
    {
      set => this.persistAlgorithm = value;
    }

    public virtual void setPersistAlgorithm(PersistAlgorithm persistAlgorithm) => this.persistAlgorithm = persistAlgorithm;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ObjectStorePersistor()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
