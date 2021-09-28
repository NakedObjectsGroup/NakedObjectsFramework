// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.XmlObjectStore
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.@object.persistence.objectstore;
using org.nakedobjects.@object.transaction;
using org.nakedobjects.utility;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.persistence.file
{
  [JavaInterfaces("1;org/nakedobjects/object/persistence/objectstore/NakedObjectStore;")]
  public class XmlObjectStore : NakedObjectStore
  {
    private static readonly org.apache.log4j.Logger LOG;
    private DataManager dataManager;
    private NakedObjectLoader objectLoader;

    public virtual void abortTransaction()
    {
      if (!XmlObjectStore.LOG.isDebugEnabled())
        return;
      XmlObjectStore.LOG.debug((object) "transaction aborted");
    }

    private NakedObjectSpecification specFor(Data data) => NakedObjects.getSpecificationLoader().loadSpecification(data.getClassName());

    public virtual CreateObjectCommand createCreateObjectCommand(
      NakedObject @object)
    {
      return (CreateObjectCommand) new XmlObjectStore.\u0031(this, @object);
    }

    public virtual DestroyObjectCommand createDestroyObjectCommand(
      NakedObject @object)
    {
      return (DestroyObjectCommand) new XmlObjectStore.\u0032(this, @object);
    }

    private ObjectData createObjectData(NakedObject @object, bool ensurePersistent)
    {
      if (XmlObjectStore.LOG.isDebugEnabled())
        XmlObjectStore.LOG.debug((object) new StringBuffer().append("compiling object data for ").append((object) @object).ToString());
      ObjectData objectData = new ObjectData(@object.getSpecification(), (SerialOid) @object.getOid());
      NakedObjectField[] fields = @object.getSpecification().getFields();
      for (int index = 0; index < fields.Length; ++index)
      {
        if (!fields[index].isDerived())
        {
          Naked field = @object.getField(fields[index]);
          Naked naked = field;
          string id = fields[index].getId();
          if (naked is InternalCollection)
            objectData.addInternalCollection((InternalCollection) naked, id, ensurePersistent);
          else if (fields[index].isValue())
            objectData.saveValue(id, @object.isEmpty(fields[index]), field?.getObject().ToString());
          else
            objectData.addAssociation((NakedObject) naked, id, ensurePersistent);
        }
      }
      return objectData;
    }

    public virtual SaveObjectCommand createSaveObjectCommand(NakedObject @object) => (SaveObjectCommand) new XmlObjectStore.\u0033(this, @object);

    public virtual Transaction createTransaction() => (Transaction) new ObjectStoreTransaction((NakedObjectStore) this);

    public virtual void endTransaction()
    {
      if (!XmlObjectStore.LOG.isDebugEnabled())
        return;
      XmlObjectStore.LOG.debug((object) "end transaction");
    }

    public virtual void debugData(DebugString debug)
    {
      debug.appendTitle("Business Objects");
      debug.appendln(this.dataManager.getDebugData());
    }

    public virtual string getDebugTitle() => "XML Object Store";

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectPerstsistenceException;org/nakedobjects/object/UnsupportedFindException;")]
    public virtual NakedObject[] getInstances(InstancesCriteria criteria)
    {
      if (XmlObjectStore.LOG.isDebugEnabled())
        XmlObjectStore.LOG.debug((object) new StringBuffer().append("getInstances of ").append((object) criteria.getSpecification()).append(" where ").append((object) criteria).ToString());
      return this.getInstances(new ObjectData(criteria.getSpecification(), (SerialOid) null), criteria);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    private NakedObject[] getInstances(NakedObjectSpecification specification)
    {
      if (XmlObjectStore.LOG.isDebugEnabled())
        XmlObjectStore.LOG.debug((object) new StringBuffer().append("getInstances of ").append((object) specification).ToString());
      return this.getInstances(new ObjectData(specification, (SerialOid) null), (InstancesCriteria) null);
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual NakedObject[] getInstances(
      NakedObjectSpecification specification,
      bool includeSubclasses)
    {
      if (XmlObjectStore.LOG.isDebugEnabled())
        XmlObjectStore.LOG.debug((object) new StringBuffer().append("getInstances of ").append((object) specification).ToString());
      if (!includeSubclasses)
        return this.getInstances(specification);
      NakedObject[] nakedObjectArray1 = this.getInstances(specification);
      foreach (NakedObjectSpecification subclass in specification.subclasses())
      {
        NakedObject[] instances = this.getInstances(subclass, true);
        if (instances != null)
        {
          int length = nakedObjectArray1.Length + instances.Length;
          NakedObject[] nakedObjectArray2 = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
          java.lang.System.arraycopy((object) nakedObjectArray1, 0, (object) nakedObjectArray2, 0, nakedObjectArray1.Length);
          java.lang.System.arraycopy((object) instances, 0, (object) nakedObjectArray2, 0, instances.Length);
          nakedObjectArray1 = nakedObjectArray2;
        }
      }
      return nakedObjectArray1;
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    private NakedObject[] getInstances(
      ObjectData patternData,
      InstancesCriteria criteria)
    {
      ObjectDataVector instances = this.dataManager.getInstances(patternData);
      int length1 = instances.size();
      NakedObject[] nakedObjectArray1 = length1 >= 0 ? new NakedObject[length1] : throw new NegativeArraySizeException();
      int num1 = 0;
      for (int i = 0; i < instances.size(); ++i)
      {
        ObjectData data = instances.element(i);
        if (XmlObjectStore.LOG.isDebugEnabled())
          XmlObjectStore.LOG.debug((object) new StringBuffer().append("instance data ").append((object) data).ToString());
        NakedObject @object = this.objectLoader.recreateAdapterForPersistent((Oid) data.getOid(), this.specFor((Data) data));
        this.initObject(@object, data);
        if (criteria == null || criteria.matches(@object))
        {
          NakedObject[] nakedObjectArray2 = nakedObjectArray1;
          int num2;
          num1 = (num2 = num1) + 1;
          int index = num2;
          NakedObject nakedObject = @object;
          nakedObjectArray2[index] = nakedObject;
        }
      }
      int length2 = num1;
      NakedObject[] nakedObjectArray3 = length2 >= 0 ? new NakedObject[length2] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) nakedObjectArray1, 0, (object) nakedObjectArray3, 0, num1);
      return nakedObjectArray3;
    }

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual NakedClass getNakedClass(string name) => throw new ObjectNotFoundException();

    [JavaThrownExceptions("2;org/nakedobjects/object/ObjectNotFoundException;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual NakedObject getObject(Oid oid, NakedObjectSpecification hint)
    {
      if (XmlObjectStore.LOG.isDebugEnabled())
        XmlObjectStore.LOG.debug((object) new StringBuffer().append("getObject ").append((object) oid).ToString());
      Data data = this.dataManager.loadData((SerialOid) oid);
      if (XmlObjectStore.LOG.isDebugEnabled())
        XmlObjectStore.LOG.debug((object) new StringBuffer().append("  data read ").append((object) data).ToString());
      switch (data)
      {
        case ObjectData _:
          return this.recreateObject((ObjectData) data);
        case CollectionData _:
          throw new NakedObjectRuntimeException();
        default:
          throw new ObjectNotFoundException();
      }
    }

    public virtual bool hasInstances(NakedObjectSpecification cls, bool includeSubclasses)
    {
      if (XmlObjectStore.LOG.isDebugEnabled())
        XmlObjectStore.LOG.debug((object) new StringBuffer().append("checking instance of ").append((object) cls).ToString());
      return this.numberOfInstances(cls, includeSubclasses) > 0;
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void init() => this.objectLoader = NakedObjects.getObjectLoader();

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    private void initObject(NakedObject @object, ObjectData data)
    {
      if (!@object.getResolveState().isResolvable(ResolveState.RESOLVING))
        return;
      this.objectLoader.start((NakedReference) @object, ResolveState.RESOLVING);
      foreach (NakedObjectField field in @object.getFields())
      {
        if (!field.isDerived())
        {
          if (field.isValue())
            @object.initValue((OneToOneAssociation) field, data.get(field.getId()));
          else if (field is OneToManyAssociation)
            this.initObjectSetupCollection(@object, data, field);
          else
            this.initObjectSetupReference(@object, data, field);
        }
      }
      this.objectLoader.end((NakedReference) @object);
    }

    private void initObjectSetupReference(
      NakedObject @object,
      ObjectData data,
      NakedObjectField field)
    {
      SerialOid oid = (SerialOid) data.get(field.getId());
      if (XmlObjectStore.LOG.isDebugEnabled())
        XmlObjectStore.LOG.debug((object) new StringBuffer().append("setting up field ").append((object) field).append(" with ").append((object) oid).ToString());
      if (oid == null || oid.isNull())
        return;
      Data data1 = this.dataManager.loadData(oid);
      NakedObject associatedObject = this.objectLoader.recreateAdapterForPersistent((Oid) oid, this.specFor(data1));
      @object.initAssociation(field, associatedObject);
    }

    private void initObjectSetupCollection(
      NakedObject @object,
      ObjectData data,
      NakedObjectField field)
    {
      ReferenceVector referenceVector = (ReferenceVector) data.get(field.getId());
      NakedCollection field1 = (NakedCollection) @object.getField(field);
      NakedObjects.getObjectLoader().start((NakedReference) field1, ResolveState.RESOLVING);
      for (int index = 0; referenceVector != null && index < referenceVector.size(); ++index)
      {
        SerialOid serialOid = referenceVector.elementAt(index);
        NakedObject associatedObject = !this.objectLoader.isIdentityKnown((Oid) serialOid) ? this.getObject((Oid) serialOid, (NakedObjectSpecification) null) : this.objectLoader.getAdapterFor((Oid) serialOid);
        @object.initAssociation(field, associatedObject);
      }
      NakedObjects.getObjectLoader().end((NakedReference) field1);
    }

    public virtual string name() => "XML";

    public virtual int numberOfInstances(NakedObjectSpecification cls, bool includedSubclasses) => this.dataManager.numberOfInstances(new ObjectData(cls, (SerialOid) null));

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    private NakedObject recreateObject(ObjectData data)
    {
      NakedObject @object = this.objectLoader.recreateAdapterForPersistent((Oid) data.getOid(), this.specFor((Data) data));
      this.initObject(@object, data);
      return @object;
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void resolveField(NakedObject @object, NakedObjectField field)
    {
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void resolveImmediately(NakedObject @object)
    {
      if (XmlObjectStore.LOG.isInfoEnabled())
        XmlObjectStore.LOG.info((object) new StringBuffer().append("resolve-immediately: ").append((object) @object).ToString());
      ObjectData data = (ObjectData) this.dataManager.loadData((SerialOid) @object.getOid());
      Assert.assertNotNull("Not able to read in data during resolve", (object) @object, (object) data);
      this.initObject(@object, data);
    }

    public virtual void reset()
    {
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void execute(PersistenceCommand[] commands)
    {
      bool flag = XmlObjectStore.LOG.isInfoEnabled();
      if (flag)
        XmlObjectStore.LOG.info((object) "start execution of transaction");
      for (int index = 0; index < commands.Length; ++index)
        commands[index].execute((ExecutionContext) null);
      if (!flag)
        return;
      XmlObjectStore.LOG.info((object) "end execution");
    }

    public virtual DataManager DataManager
    {
      set => this.dataManager = value;
    }

    public virtual void setDataManager(DataManager dataManager) => this.dataManager = dataManager;

    [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
    public virtual void shutdown()
    {
      if (!XmlObjectStore.LOG.isInfoEnabled())
        return;
      XmlObjectStore.LOG.info((object) new StringBuffer().append("shutdown ").append((object) this).ToString());
    }

    public virtual void startTransaction()
    {
      if (!XmlObjectStore.LOG.isDebugEnabled())
        return;
      XmlObjectStore.LOG.debug((object) "start transaction");
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static XmlObjectStore()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      XmlObjectStore xmlObjectStore = this;
      ObjectImpl.clone((object) xmlObjectStore);
      return ((object) xmlObjectStore).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;org/nakedobjects/object/transaction/CreateObjectCommand;")]
    public class \u0031 : CreateObjectCommand
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private XmlObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
      public virtual void execute(ExecutionContext context)
      {
        if (XmlObjectStore.LOG.isDebugEnabled())
          XmlObjectStore.LOG.debug((object) new StringBuffer().append("  create object ").append((object) this.object_\u003E).ToString());
        this.this\u00240.dataManager.insert((Data) this.this\u00240.createObjectData(this.object_\u003E, true));
      }

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("CreateObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0031(XmlObjectStore _param1, [In] NakedObject obj1)
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
        XmlObjectStore.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;org/nakedobjects/object/transaction/DestroyObjectCommand;")]
    public class \u0032 : DestroyObjectCommand
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private XmlObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
      public virtual void execute(ExecutionContext context)
      {
        if (XmlObjectStore.LOG.isDebugEnabled())
          XmlObjectStore.LOG.debug((object) new StringBuffer().append("  destroy object ").append((object) this.object_\u003E).ToString());
        this.this\u00240.dataManager.remove((SerialOid) this.object_\u003E.getOid());
      }

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("DestroyObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0032(XmlObjectStore _param1, [In] NakedObject obj1)
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
        XmlObjectStore.\u0032 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;org/nakedobjects/object/transaction/SaveObjectCommand;")]
    public class \u0033 : SaveObjectCommand
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private XmlObjectStore this\u00240;
      [JavaFlags(16)]
      public readonly NakedObject object_\u003E;

      [JavaThrownExceptions("1;org/nakedobjects/object/ObjectPerstsistenceException;")]
      public virtual void execute(ExecutionContext context)
      {
        if (XmlObjectStore.LOG.isDebugEnabled())
          XmlObjectStore.LOG.debug((object) new StringBuffer().append("  save object ").append((object) this.object_\u003E).ToString());
        if (this.object_\u003E is InternalCollection)
          this.this\u00240.dataManager.save((Data) this.this\u00240.createObjectData(((Aggregated) this.object_\u003E).parent(), true));
        else
          this.this\u00240.dataManager.save((Data) this.this\u00240.createObjectData(this.object_\u003E, true));
      }

      public virtual NakedObject onObject() => this.object_\u003E;

      public override string ToString() => new StringBuffer().append("SaveObjectCommand [object=").append((object) this.object_\u003E).append("]").ToString();

      public \u0033(XmlObjectStore _param1, [In] NakedObject obj1)
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
        XmlObjectStore.\u0033 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }
  }
}
