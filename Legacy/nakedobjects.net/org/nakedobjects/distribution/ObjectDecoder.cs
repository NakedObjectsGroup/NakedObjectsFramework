// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ObjectDecoder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.distribution
{
  public class ObjectDecoder
  {
    private static DataStructure dataStructure;
    private static readonly org.apache.log4j.Logger LOG;
    private static DirtyObjectSet updateNotifier;

    public static ObjectDecoder.KnownTransients createKnownTransients() => new ObjectDecoder.KnownTransients();

    private static ResolveState nextState(ResolveState initialState, bool complete)
    {
      ResolveState resolveState = (ResolveState) null;
      if (initialState == ResolveState.RESOLVED)
        resolveState = ResolveState.UPDATING;
      else if (initialState == ResolveState.GHOST || initialState == ResolveState.PART_RESOLVED)
        resolveState = !complete ? ResolveState.RESOLVING_PART : ResolveState.RESOLVING;
      else if (initialState == ResolveState.TRANSIENT)
        resolveState = ResolveState.SERIALIZING_TRANSIENT;
      return resolveState;
    }

    public static Naked restore(Data data)
    {
      switch (data)
      {
        case ValueData _:
          return (Naked) ObjectDecoder.restoreValue((ValueData) data);
        case CollectionData _:
          return ObjectDecoder.restoreCollection((CollectionData) data, new ObjectDecoder.KnownTransients());
        default:
          return (Naked) ObjectDecoder.restoreObject(data, new ObjectDecoder.KnownTransients());
      }
    }

    public static Naked restore(Data data, ObjectDecoder.KnownTransients knownObjects) => data is CollectionData ? ObjectDecoder.restoreCollection((CollectionData) data, knownObjects) : (Naked) ObjectDecoder.restoreObject(data, knownObjects);

    private static Naked restoreCollection(
      CollectionData data,
      ObjectDecoder.KnownTransients knownTransients)
    {
      string type = data.getType();
      NakedObjectSpecification specification = NakedObjects.getSpecificationLoader().loadSpecification(type);
      NakedCollection nakedCollection = NakedObjects.getObjectLoader().recreateCollection(specification);
      if (data.getElements() == null)
      {
        if (ObjectDecoder.LOG.isDebugEnabled())
          ObjectDecoder.LOG.debug((object) "restoring empty collection");
        return (Naked) nakedCollection;
      }
      ReferenceData[] elements1 = data.getElements();
      if (ObjectDecoder.LOG.isDebugEnabled())
        ObjectDecoder.LOG.debug((object) new StringBuffer().append("restoring collection ").append(elements1.Length).append(" elements").ToString());
      int length = elements1.Length;
      object[] elements2 = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < elements1.Length; ++index)
      {
        NakedObject nakedObject = ObjectDecoder.restoreObject((Data) elements1[index], knownTransients);
        if (ObjectDecoder.LOG.isDebugEnabled())
          ObjectDecoder.LOG.debug((object) new StringBuffer().append("restoring collection element :").append((object) nakedObject.getOid()).ToString());
        elements2[index] = nakedObject.getObject();
      }
      nakedCollection.init(elements2);
      return (Naked) nakedCollection;
    }

    private static NakedObject restoreObject(
      Data data,
      ObjectDecoder.KnownTransients knownTransients)
    {
      switch (data)
      {
        case NullData _:
          return (NakedObject) null;
        case ObjectData _:
          return ObjectDecoder.restoreObjectFromObject((ObjectData) data, knownTransients);
        case IdentityData _:
          return ObjectDecoder.restoreObjectFromIdentity((IdentityData) data, knownTransients);
        default:
          throw new UnknownTypeException((object) data);
      }
    }

    private static NakedObject restoreObjectFromObject(
      ObjectData data,
      ObjectDecoder.KnownTransients knownTransients)
    {
      if (knownTransients.containsKey(data))
        return knownTransients.get(data);
      Oid oid = data.getOid();
      NakedObjectLoader objectLoader = NakedObjects.getObjectLoader();
      return oid == null || oid.isNull() ? ObjectDecoder.restoreTransient(data, objectLoader, knownTransients) : (!objectLoader.isIdentityKnown(oid) ? ObjectDecoder.restorePersistentObject(data, oid, objectLoader, knownTransients) : ObjectDecoder.updateLoadedObject(data, oid, objectLoader, knownTransients));
    }

    private static NakedObject restoreObjectFromIdentity(
      IdentityData data,
      ObjectDecoder.KnownTransients knownTransients)
    {
      Oid oid = data.getOid();
      NakedObjectLoader objectLoader = NakedObjects.getObjectLoader();
      NakedObject nakedObject = objectLoader.getAdapterFor(oid);
      if (nakedObject == null)
      {
        NakedObjectSpecification spec = NakedObjects.getSpecificationLoader().loadSpecification(data.getType());
        nakedObject = objectLoader.recreateAdapterForPersistent(oid, spec);
      }
      return nakedObject;
    }

    private static NakedObject restorePersistentObject(
      ObjectData data,
      Oid oid,
      NakedObjectLoader objectLoader,
      ObjectDecoder.KnownTransients knownTransients)
    {
      NakedObjectSpecification spec = NakedObjects.getSpecificationLoader().loadSpecification(data.getType());
      NakedObject @object = objectLoader.recreateAdapterForPersistent(oid, spec);
      if (data.getFieldContent() != null)
      {
        @object.setOptimisticLock(data.getVersion());
        ResolveState state = !data.hasCompleteData() ? ResolveState.RESOLVING_PART : ResolveState.RESOLVING;
        if (ObjectDecoder.LOG.isDebugEnabled())
          ObjectDecoder.LOG.debug((object) new StringBuffer().append("restoring existing object (").append(state.name()).append(") ").append((object) @object.getOid()).ToString());
        ObjectDecoder.setupFields(data, objectLoader, @object, state, knownTransients);
      }
      return @object;
    }

    private static NakedObject restoreTransient(
      ObjectData data,
      NakedObjectLoader objectLoader,
      ObjectDecoder.KnownTransients knownTransients)
    {
      NakedObjectSpecification specification = NakedObjects.getSpecificationLoader().loadSpecification(data.getType());
      NakedObject @object = objectLoader.recreateTransientInstance(specification);
      if (ObjectDecoder.LOG.isDebugEnabled())
        ObjectDecoder.LOG.debug((object) new StringBuffer().append("restore transient object ").append((object) @object.getOid()).ToString());
      knownTransients.put(data, @object);
      Oid oid = data.getOid();
      if (oid != null)
      {
        junit.framework.Assert.assertTrue("require oid.isNull() == true", oid.isNull());
        ((AbstractNakedReference) @object).setOid(oid);
      }
      ObjectDecoder.setUpFields(data, @object, knownTransients);
      return @object;
    }

    private static NakedValue restoreValue(ValueData valueData) => NakedObjects.getObjectLoader().createAdapterForValue(valueData.getValue());

    private static void setUpCollectionField(
      ObjectData parentData,
      NakedObject @object,
      NakedObjectField field,
      CollectionData content,
      ObjectDecoder.KnownTransients knownTransients)
    {
      if (!content.hasAllElements())
      {
        NakedCollection field1 = (NakedCollection) @object.getField(field);
        if (field1.getResolveState() == ResolveState.GHOST)
          return;
        if (ObjectDecoder.LOG.isDebugEnabled())
          ObjectDecoder.LOG.debug((object) new StringBuffer().append("No data for collection: ").append(field.getId()).ToString());
        if (!@object.getVersion().different(parentData.getVersion()))
          return;
        if (ObjectDecoder.LOG.isDebugEnabled())
          ObjectDecoder.LOG.debug((object) new StringBuffer().append("clearing collection as versions differ: ").append((object) @object.getVersion()).append(" ").append((object) parentData.getVersion()).ToString());
        NakedCollection nakedCollection = field1;
        int length = 0;
        object[] elements = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        nakedCollection.init(elements);
        field1.changeState(ResolveState.GHOST);
      }
      else
      {
        int length = content.getElements().Length;
        NakedObject[] instances = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
        for (int index = 0; index < instances.Length; ++index)
        {
          instances[index] = ObjectDecoder.restoreObject((Data) content.getElements()[index], knownTransients);
          if (ObjectDecoder.LOG.isDebugEnabled())
            ObjectDecoder.LOG.debug((object) new StringBuffer().append("adding element to ").append(field.getId()).append(": ").append((object) instances[index].getOid()).ToString());
        }
        NakedCollection field2 = (NakedCollection) @object.getField(field);
        ResolveState resolveState = field2.getResolveState();
        ResolveState targetState = ObjectDecoder.nextState(resolveState, content.hasAllElements());
        if (targetState != null)
        {
          NakedObjects.getObjectLoader().start((NakedReference) field2, targetState);
          @object.initAssociation((OneToManyAssociation) field, instances);
          NakedObjects.getObjectLoader().end((NakedReference) field2);
        }
        else
        {
          if (!ObjectDecoder.LOG.isWarnEnabled())
            return;
          ObjectDecoder.LOG.warn((object) new StringBuffer().append("not initialising collection ").append((object) field2).append(" due to current state ").append((object) resolveState).ToString());
        }
      }
    }

    public static void setUpdateNotifer(DirtyObjectSet updateNotifier) => ObjectDecoder.updateNotifier = updateNotifier;

    private static void setupFields(
      ObjectData data,
      NakedObjectLoader objectLoader,
      NakedObject @object,
      ResolveState state,
      ObjectDecoder.KnownTransients knownTransients)
    {
      if (!@object.getResolveState().isResolvable(state))
        return;
      objectLoader.start((NakedReference) @object, state);
      ObjectDecoder.setUpFields(data, @object, knownTransients);
      objectLoader.end((NakedReference) @object);
    }

    private static void setUpFields(
      ObjectData data,
      NakedObject @object,
      ObjectDecoder.KnownTransients knownTransients)
    {
      Data[] fieldContent = data.getFieldContent();
      if (fieldContent == null || fieldContent.Length <= 0)
        return;
      NakedObjectField[] fields = ObjectDecoder.dataStructure.getFields(@object.getSpecification());
      if (fields.Length != fieldContent.Length)
        throw new NakedObjectsRemoteException(new StringBuffer().append("Data received for different number of fields; expected ").append(fields.Length).append(", but was ").append(fieldContent.Length).ToString());
      for (int index = 0; index < fields.Length; ++index)
      {
        NakedObjectField field = fields[index];
        Data data1 = fieldContent[index];
        if (data1 == null || field.isDerived())
        {
          if (ObjectDecoder.LOG.isDebugEnabled())
            ObjectDecoder.LOG.debug((object) new StringBuffer().append("no data for field ").append(field.getId()).ToString());
        }
        else if (field.isCollection())
          ObjectDecoder.setUpCollectionField(data, @object, field, (CollectionData) data1, knownTransients);
        else if (field.isValue())
          ObjectDecoder.setUpValueField(@object, field, data1);
        else
          ObjectDecoder.setUpReferenceField(@object, field, data1, knownTransients);
      }
    }

    private static void setUpReferenceField(
      NakedObject @object,
      NakedObjectField field,
      Data data,
      ObjectDecoder.KnownTransients knownTransients)
    {
      NakedObject associatedObject = ObjectDecoder.restoreObject(data, knownTransients);
      string str = (string) null;
      if (associatedObject != null)
        str = new StringBuffer().append("").append((object) associatedObject.getOid()).ToString();
      if (ObjectDecoder.LOG.isDebugEnabled())
        ObjectDecoder.LOG.debug((object) new StringBuffer().append("setting association for field ").append(field.getId()).append(": ").append(str).ToString());
      @object.initAssociation(field, associatedObject);
    }

    private static void setUpValueField(NakedObject @object, NakedObjectField field, Data data)
    {
      object object1 = !(data is NullData) ? ((ValueData) data).getValue() : (object) null;
      if (ObjectDecoder.LOG.isDebugEnabled())
        ObjectDecoder.LOG.debug((object) new StringBuffer().append("setting value for field ").append(field.getId()).append(": ").append(object1).ToString());
      @object.initValue((OneToOneAssociation) field, object1);
    }

    private static NakedObject updateLoadedObject(
      ObjectData data,
      Oid oid,
      NakedObjectLoader objectLoader,
      ObjectDecoder.KnownTransients knownTransients)
    {
      NakedObject adapterFor = objectLoader.getAdapterFor(oid);
      if (data.getFieldContent() != null)
      {
        adapterFor.setOptimisticLock(data.getVersion());
        ResolveState state = ObjectDecoder.nextState(adapterFor.getResolveState(), data.hasCompleteData());
        if (state != null)
        {
          if (ObjectDecoder.LOG.isDebugEnabled())
            ObjectDecoder.LOG.debug((object) new StringBuffer().append("updating existing object (").append(state.name()).append(") ").append((object) adapterFor.getOid()).ToString());
          ObjectDecoder.setupFields(data, objectLoader, adapterFor, state, knownTransients);
          ObjectDecoder.updateNotifier.addDirty(adapterFor);
        }
      }
      else if (data.getVersion() == null || !data.getVersion().different(adapterFor.getVersion()))
        ;
      return adapterFor;
    }

    public static void madePersistent(NakedObject @object, ObjectData updates)
    {
      if (updates == null)
        return;
      if ((@object.getOid() == null || @object.getOid().isNull()) && @object.persistable() != Persistable.TRANSIENT)
      {
        NakedObjects.getObjectLoader().madePersistent((NakedReference) @object, updates.getOid());
        @object.setOptimisticLock(updates.getVersion());
      }
      Data[] fieldContent = updates.getFieldContent();
      if (fieldContent == null)
        return;
      NakedObjectField[] fields = ObjectDecoder.dataStructure.getFields(@object.getSpecification());
      for (int index1 = 0; index1 < fieldContent.Length; ++index1)
      {
        if (fieldContent[index1] != null)
        {
          if (fields[index1].isObject())
          {
            NakedObject association = @object.getAssociation((OneToOneAssociation) fields[index1]);
            ObjectData updates1 = (ObjectData) updates.getFieldContent()[index1];
            if (association != null)
              ObjectDecoder.madePersistent(association, updates1);
          }
          else if (fields[index1].isCollection())
          {
            CollectionData collectionData = (CollectionData) updates.getFieldContent()[index1];
            NakedCollection field = (NakedCollection) @object.getField(fields[index1]);
            if (!field.getResolveState().isPersistent())
              field.changeState(ResolveState.RESOLVED);
            for (int index2 = 0; index2 < collectionData.getElements().Length; ++index2)
            {
              if (collectionData.getElements()[index2] is ObjectData)
                ObjectDecoder.madePersistent(((NakedCollection) @object.getField(fields[index1])).elementAt(index2), (ObjectData) collectionData.getElements()[index2]);
            }
          }
        }
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ObjectDecoder()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ObjectDecoder objectDecoder = this;
      ObjectImpl.clone((object) objectDecoder);
      return ((object) objectDecoder).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(41)]
    public class KnownTransients
    {
      private Hashtable knownObjects;

      [JavaFlags(2)]
      public virtual bool containsKey(ObjectData data) => this.knownObjects.containsKey((object) data);

      [JavaFlags(2)]
      public virtual NakedObject get(ObjectData data) => (NakedObject) this.knownObjects.get((object) data);

      [JavaFlags(2)]
      public virtual void put(ObjectData data, NakedObject @object) => this.knownObjects.put((object) data, (object) @object);

      public KnownTransients() => this.knownObjects = new Hashtable();

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ObjectDecoder.KnownTransients knownTransients = this;
        ObjectImpl.clone((object) knownTransients);
        return ((object) knownTransients).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
