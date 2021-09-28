// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ProxyPersistor
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.utility;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.distribution
{
  public class ProxyPersistor : AbstracObjectPersistor
  {
    [JavaFlags(24)]
    public static readonly org.apache.log4j.Logger LOG;
    private Distribution connection;
    private readonly Hashtable nakedClasses;
    private ObjectEncoder encoder;
    private Session session;
    private DirtyObjectSet updateNotifier;
    private ClientSideTransaction clientSideTransaction;
    private bool checkObjectsForDirtyFlag;
    private Hashtable cache;
    private OidGenerator oidGenerator;

    [JavaFlags(4)]
    public virtual ClientSideTransaction getClientSideTransaction() => this.clientSideTransaction;

    public override void abortTransaction()
    {
      this.checkTransactionInProgress();
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) nameof (abortTransaction));
      this.clientSideTransaction.rollback();
      this.clientSideTransaction = (ClientSideTransaction) null;
    }

    public override void addObjectChangedListener(DirtyObjectSet listener)
    {
    }

    public override TypedNakedCollection allInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) new StringBuffer().append("getInstances of ").append((object) specification).ToString());
      if (this.cache.containsKey((object) specification))
      {
        object obj = this.cache.get((object) specification);
        if (obj is TypedNakedCollection)
          return (TypedNakedCollection) obj;
      }
      ObjectData[] data = this.connection.allInstances(this.session, specification.getFullName(), false);
      TypedNakedCollection nakedObjects = this.convertToNakedObjects(specification, data);
      this.clearChanges();
      if (this.cache.containsKey((object) specification))
        this.cache.put((object) specification, (object) nakedObjects);
      return nakedObjects;
    }

    private TypedNakedCollection convertToNakedObjects(
      NakedObjectSpecification specification,
      ObjectData[] data)
    {
      int length = data.Length;
      NakedObject[] instances = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < data.Length; ++index)
        instances[index] = (NakedObject) ObjectDecoder.restore((Data) data[index]);
      return (TypedNakedCollection) new InstanceCollectionVector(specification, instances);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void destroyObject(NakedObject @object)
    {
      this.checkTransactionInProgress();
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) new StringBuffer().append("destroyObject ").append((object) @object).ToString());
      this.clientSideTransaction.addDestroyObject(@object);
    }

    private void checkTransactionInProgress()
    {
      if (this.clientSideTransaction == null)
        throw new TransactionException("No transaction in progress");
    }

    private void addOidToCorrelateNewlyPersistedObject(NakedObject persistedObject)
    {
      PojoAdapter pojoAdapter = (PojoAdapter) persistedObject;
      Oid oid = this.oidGenerator.next((Naked) pojoAdapter);
      oid.setNull(true);
      pojoAdapter.setOid(oid);
    }

    private void stripOidAfterExecutingClientSideAction(NakedObject persistedObject) => ((AbstractNakedReference) persistedObject).setOid((Oid) null);

    public virtual OidGenerator OidGenerator
    {
      set => this.oidGenerator = value;
    }

    public virtual void setOidGenerator(OidGenerator oidGenerator) => this.oidGenerator = oidGenerator;

    public override void endTransaction()
    {
      this.checkTransactionInProgress();
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) nameof (endTransaction));
      if (this.clientSideTransaction.isEmpty())
      {
        if (ProxyPersistor.LOG.isDebugEnabled())
          ProxyPersistor.LOG.debug((object) "  no transaction commands to process");
        this.clientSideTransaction = (ClientSideTransaction) null;
      }
      else
      {
        ObjectEncoder.KnownObjects knownObjects = ObjectEncoder.createKnownObjects();
        ClientSideTransaction.Entry[] entries = this.clientSideTransaction.getEntries();
        int length1 = entries.Length;
        int length2 = length1;
        int[] types = length2 >= 0 ? new int[length2] : throw new NegativeArraySizeException();
        bool serializePersistentObjects = true;
        int length3 = length1;
        ReferenceData[] deleted = length3 >= 0 ? new ReferenceData[length3] : throw new NegativeArraySizeException();
        for (int index = 0; index < length1; ++index)
        {
          types[index] = entries[index].getType();
          switch (types[index])
          {
            case 1:
              NakedObject nakedObject = entries[index].getObject();
              this.addOidToCorrelateNewlyPersistedObject(nakedObject);
              deleted[index] = (ReferenceData) this.encoder.createMakePersistentGraph(nakedObject, knownObjects, serializePersistentObjects);
              break;
            case 2:
              deleted[index] = (ReferenceData) this.encoder.createGraphForChangedObject(entries[index].getObject(), knownObjects, serializePersistentObjects);
              break;
            case 3:
              deleted[index] = (ReferenceData) this.encoder.createIdentityData(entries[index].getObject());
              break;
          }
        }
        ClientActionResultData actionResultData;
        try
        {
          actionResultData = this.connection.executeClientAction(this.session, deleted, types);
        }
        catch (ConcurrencyException ex)
        {
          ConcurrencyException concurrencyException1 = ex;
          Oid source = concurrencyException1.getSource();
          if (source == null)
          {
            ConcurrencyException concurrencyException2 = concurrencyException1;
            if (concurrencyException2 != ex)
              throw concurrencyException2;
            throw;
          }
          else
          {
            NakedObject adapterFor = NakedObjects.getObjectLoader().getAdapterFor(source);
            this.reload(adapterFor);
            throw new ConcurrencyException(new StringBuffer().append("Object automatically reloaded: ").append(adapterFor.titleString()).ToString(), (Throwable) concurrencyException1);
          }
        }
        if (actionResultData != null)
        {
          ObjectData[] persisted = actionResultData.getPersisted();
          for (int index = 0; index < length1; ++index)
          {
            if (types[index] == 1)
              this.stripOidAfterExecutingClientSideAction(entries[index].getObject());
          }
          Version[] changed = actionResultData.getChanged();
          for (int index = 0; index < length1; ++index)
          {
            switch (types[index])
            {
              case 1:
                ObjectDecoder.madePersistent(entries[index].getObject(), persisted[index]);
                break;
              case 2:
                entries[index].getObject().setOptimisticLock(changed[index]);
                break;
            }
          }
        }
        this.clientSideTransaction = (ClientSideTransaction) null;
      }
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/UnsupportedFindException;")]
    public override TypedNakedCollection findInstances(
      InstancesCriteria criteria)
    {
      NakedObjectSpecification specification = criteria.getSpecification();
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) new StringBuffer().append("getInstances of ").append((object) specification).append(" with ").append((object) criteria).ToString());
      if (this.cache.containsKey((object) specification))
      {
        object obj = this.cache.get((object) specification);
        if (obj is TypedNakedCollection)
        {
          TypedNakedCollection typedNakedCollection = (TypedNakedCollection) obj;
          Vector instances = new Vector();
          Enumeration enumeration = typedNakedCollection.elements();
          while (enumeration.hasMoreElements())
          {
            NakedObject @object = (NakedObject) enumeration.nextElement();
            if (criteria.matches(@object))
              instances.addElement((object) @object);
          }
          return (TypedNakedCollection) new InstanceCollectionVector(specification, instances);
        }
      }
      ObjectData[] instances1 = this.connection.findInstances(this.session, criteria);
      TypedNakedCollection nakedObjects = this.convertToNakedObjects(specification, instances1);
      this.clearChanges();
      return nakedObjects;
    }

    public override void debugData(DebugString debug) => debug.appendln("Connection", (object) this.connection);

    public override string getDebugTitle() => "Proxy Object Manager";

    [JavaFlags(4)]
    public override NakedObject[] getInstances(InstancesCriteria criteria) => throw new NotImplementedException();

    [JavaFlags(4)]
    public override NakedObject[] getInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      throw new NotImplementedException();
    }

    public override NakedClass getNakedClass(NakedObjectSpecification nakedClass)
    {
      if (this.nakedClasses.contains((object) nakedClass))
        return (NakedClass) this.nakedClasses.get((object) nakedClass);
      NakedClass nakedClass1 = (NakedClass) new NakedClassImpl(nakedClass.getFullName());
      this.nakedClasses.put((object) nakedClass, (object) nakedClass1);
      return nakedClass1;
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectNotFoundException;")]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public override NakedObject getObject(Oid oid, NakedObjectSpecification hint) => throw new NotImplementedException();

    public override bool hasInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) new StringBuffer().append("hasInstances of ").append((object) specification).ToString());
      if (this.cache.containsKey((object) specification))
      {
        object obj = this.cache.get((object) specification);
        if (obj is TypedNakedCollection)
          return ((NakedCollection) obj).size() > 0;
      }
      return this.connection.hasInstances(this.session, specification.getFullName());
    }

    public override void init() => this.session = NakedObjects.getCurrentSession();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void makePersistent(NakedObject @object)
    {
      this.checkTransactionInProgress();
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) new StringBuffer().append("makePersistent ").append((object) @object).ToString());
      this.clientSideTransaction.addMakePersistent(@object);
    }

    public override int numberOfInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) new StringBuffer().append("numberOfInstance of ").append((object) specification).ToString());
      if (this.cache.containsKey((object) specification))
      {
        object obj = this.cache.get((object) specification);
        if (obj is TypedNakedCollection)
          return ((NakedCollection) obj).size();
      }
      return this.connection.numberOfInstances(this.session, specification.getFullName());
    }

    public override void objectChanged(NakedObject @object)
    {
      if (@object.getResolveState().isTransient())
        this.updateNotifier.addDirty(@object);
      if (!@object.getResolveState().respondToChangesInPersistentObjects())
        return;
      this.checkTransactionInProgress();
      this.clientSideTransaction.addObjectChanged(@object);
    }

    public override void reset()
    {
    }

    public override void reload(NakedObject @object) => ObjectDecoder.restore((Data) this.connection.resolveImmediately(this.session, this.encoder.createIdentityData(@object)));

    public override void refresh(NakedReference root, Hashtable oids) => this.refreshRecursively(root, oids);

    private void refreshRecursively(NakedReference root, Hashtable noRefreshOids)
    {
      if (!this.shouldRefreshNode(root))
        return;
      this.startUpdatingNode(root);
      switch (root)
      {
        case NakedObject _:
          this.refreshObject((NakedObject) root, noRefreshOids);
          break;
        case NakedCollection _:
          this.refreshCollection((NakedCollection) root, noRefreshOids);
          break;
      }
      this.endUpdatingNode(root);
      if (this.oidInNoRefreshTable(root.getOid(), noRefreshOids))
        return;
      this.changeNodeToGHOST(root);
      this.storeOidInNoRefreshTable(root.getOid(), noRefreshOids);
    }

    private bool oidInNoRefreshTable(Oid oid, Hashtable noRefreshOids) => oid != null && noRefreshOids != null && noRefreshOids.containsKey((object) oid);

    private void storeOidInNoRefreshTable(Oid oid, Hashtable noRefreshOids)
    {
      if (oid == null || noRefreshOids == null || noRefreshOids.containsKey((object) oid))
        return;
      noRefreshOids.put((object) oid, (object) oid);
    }

    private void refreshObject(NakedObject root, Hashtable oids)
    {
      NakedObjectField[] fields = root.getFields();
      if (fields == null)
        return;
      for (int index = 0; index < fields.Length; ++index)
      {
        NakedObjectField field1 = fields[index];
        if (field1 != null)
        {
          Naked field2 = root.getField(field1);
          if (field2 != null && (field2.getSpecification().isCollection() || field2.getSpecification().isObject()))
            this.refreshRecursively((NakedReference) field2, oids);
        }
      }
    }

    private void refreshCollection(NakedCollection collection, Hashtable oids)
    {
      Enumeration enumeration = collection.elements();
      if (enumeration == null)
        return;
      while (enumeration.hasMoreElements())
        this.refreshRecursively((NakedReference) enumeration.nextElement(), oids);
    }

    private void startUpdatingNode(NakedReference node) => node.changeState(ResolveState.UPDATING);

    private void endUpdatingNode(NakedReference node)
    {
      if (node.getResolveState() != ResolveState.UPDATING)
        return;
      node.changeState(ResolveState.RESOLVED);
    }

    private void changeNodeToGHOST(NakedReference node)
    {
      if (node.getResolveState() != ResolveState.RESOLVED)
        return;
      node.changeState(ResolveState.GHOST);
    }

    private bool shouldRefreshNode(NakedReference node)
    {
      if (node == null || !node.getResolveState().isResolved())
        return false;
      NakedObjectSpecification specification = node.getSpecification();
      return !specification.isLookup() && (specification.isCollection() || specification.isObject());
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void resolveImmediately(NakedObject @object)
    {
      if (!@object.getResolveState().isResolvable(ResolveState.RESOLVING))
        return;
      Oid oid = @object.getOid();
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) new StringBuffer().append("resolve object (remotely from server)").append((object) oid).ToString());
      ObjectDecoder.restore((Data) this.connection.resolveImmediately(this.session, this.encoder.createIdentityData(@object)));
    }

    public override void resolveField(NakedObject @object, NakedObjectField field)
    {
      if (field.isValue())
        return;
      NakedReference field1 = (NakedReference) @object.getField(field);
      if (field1.getResolveState().isResolved() || !field1.getResolveState().isPersistent())
        return;
      if (ProxyPersistor.LOG.isInfoEnabled())
        ProxyPersistor.LOG.info((object) new StringBuffer().append("resolve-eagerly on server ").append((object) @object).append("/").append(field.getId()).ToString());
      ObjectDecoder.restore(this.connection.resolveField(this.session, this.encoder.createIdentityData(@object), field.getId()));
    }

    public override void saveChanges()
    {
      if (!this.checkObjectsForDirtyFlag)
        return;
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) "collating changed objects");
      Enumeration identifiedObjects = NakedObjects.getObjectLoader().getIdentifiedObjects();
      while (identifiedObjects.hasMoreElements())
      {
        object obj = identifiedObjects.nextElement();
        if (obj is NakedObject)
        {
          NakedObject @object = (NakedObject) obj;
          if (@object.getSpecification().isDirty(@object))
          {
            if (ProxyPersistor.LOG.isDebugEnabled())
              ProxyPersistor.LOG.debug((object) new StringBuffer().append("  found dirty object ").append((object) @object).ToString());
            this.objectChanged(@object);
            @object.getSpecification().clearDirty(@object);
          }
        }
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void clearChanges()
    {
      if (!this.checkObjectsForDirtyFlag)
        return;
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) "clearing changed objects");
      Enumeration identifiedObjects = NakedObjects.getObjectLoader().getIdentifiedObjects();
      while (identifiedObjects.hasMoreElements())
      {
        object obj = identifiedObjects.nextElement();
        if (obj is NakedObject)
        {
          NakedObject @object = (NakedObject) obj;
          if (@object.getSpecification().isDirty(@object))
          {
            if (ProxyPersistor.LOG.isDebugEnabled())
              ProxyPersistor.LOG.debug((object) new StringBuffer().append("  found dirty object ").append((object) @object).ToString());
            @object.getSpecification().clearDirty(@object);
          }
        }
      }
    }

    public virtual Distribution Connection
    {
      set => this.connection = value;
    }

    public virtual ObjectEncoder Encoder
    {
      set => this.encoder = value;
    }

    public virtual DirtyObjectSet UpdateNotifier
    {
      set
      {
        ObjectDecoder.setUpdateNotifer(value);
        this.updateNotifier = value;
      }
    }

    public virtual void setConnection(Distribution connection) => this.connection = connection;

    public virtual void setEncoder(ObjectEncoder factory) => this.encoder = factory;

    public virtual void setUpdateNotifier(DirtyObjectSet updateNotifier)
    {
      ObjectDecoder.setUpdateNotifer(updateNotifier);
      this.updateNotifier = updateNotifier;
    }

    public override void startTransaction()
    {
      if (ProxyPersistor.LOG.isDebugEnabled())
        ProxyPersistor.LOG.debug((object) nameof (startTransaction));
      this.clearChanges();
      if (this.clientSideTransaction == null)
      {
        this.clientSideTransaction = new ClientSideTransaction();
      }
      else
      {
        ClientSideTransaction clientSideTransaction = this.clientSideTransaction;
        this.clientSideTransaction = (ClientSideTransaction) null;
        throw new ObjectPerstsistenceException(new StringBuffer().append("Can't start transaction when one already started: ").append((object) clientSideTransaction).ToString());
      }
    }

    public override void shutdown()
    {
    }

    public virtual bool CheckObjectsForDirtyFlag
    {
      set => this.checkObjectsForDirtyFlag = value;
    }

    public virtual void setCheckObjectsForDirtyFlag(bool checkObjectsForDirtyFlag) => this.checkObjectsForDirtyFlag = checkObjectsForDirtyFlag;

    public virtual string[] CacheInstances
    {
      set => this.setCacheInstances(value);
    }

    public virtual void setCacheInstances(string[] names)
    {
      for (int index = 0; index < names.Length; ++index)
        this.cache.put((object) NakedObjects.getSpecificationLoader().loadSpecification(names[index]), (object) new Boolean(true));
    }

    public ProxyPersistor()
    {
      this.nakedClasses = new Hashtable();
      this.cache = new Hashtable();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ProxyPersistor()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
