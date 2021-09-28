// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ObjectEncoder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.distribution
{
  public sealed class ObjectEncoder
  {
    private static readonly org.apache.log4j.Logger LOG;
    private int actionGraphDepth;
    private DataStructure dataStructure;
    private DataFactory factory;
    private int persistentGraphDepth;
    private int updateGraphDepth;
    private int resolveFieldGraphDepth;
    private int resolveImmediatelyGraphDepth;

    public static ObjectEncoder.KnownObjects createKnownObjects() => new ObjectEncoder.KnownObjects();

    [JavaFlags(17)]
    public ReferenceData createActionTarget(
      NakedObject @object,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      return this.serializeObject(@object, this.actionGraphDepth, knownObjects, serializePersistentObjects);
    }

    public virtual ClientActionResultData createClientActionResult(
      ObjectData[] madePersistent,
      Version[] changedVersion)
    {
      return this.factory.createActionResultData(madePersistent, changedVersion);
    }

    private CollectionData createCollection(
      NakedCollection collection,
      int graphDepth,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      // ISSUE: unable to decompile the method.
    }

    private CollectionData originalCreateCollectionMethod(
      NakedCollection collection,
      int graphDepth,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      Oid oid = collection.getOid();
      string fullName = collection.getSpecification().getFullName();
      if (ObjectEncoder.LOG.isDebugEnabled())
        ObjectEncoder.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" createCollection ").ToString());
      bool hasAllElements = collection.getResolveState().isTransient() || collection.getResolveState().isResolved();
      ReferenceData[] elements;
      if (hasAllElements)
      {
        Enumeration enumeration = collection.elements();
        int length = collection.size();
        elements = length >= 0 ? new ReferenceData[length] : throw new NegativeArraySizeException();
        int num1 = 0;
        while (enumeration.hasMoreElements())
        {
          NakedObject @object = (NakedObject) enumeration.nextElement();
          ReferenceData[] referenceDataArray = elements;
          int num2;
          num1 = (num2 = num1) + 1;
          int index = num2;
          ReferenceData referenceData = this.serializeObject(@object, graphDepth, knownObjects, serializePersistentObjects);
          referenceDataArray[index] = referenceData;
        }
      }
      else
      {
        int length = 0;
        elements = length >= 0 ? (ReferenceData[]) new ObjectData[length] : throw new NegativeArraySizeException();
      }
      return this.factory.createCollectionData(oid, fullName, elements, hasAllElements, collection.getVersion());
    }

    [JavaFlags(17)]
    public ObjectData createCompletePersistentGraph(
      NakedObject @object,
      bool serializePersistentObjects)
    {
      return this.createCompletePersistentGraph(@object, this.persistentGraphDepth, serializePersistentObjects);
    }

    [JavaFlags(17)]
    public ObjectData createCompletePersistentGraph(
      NakedObject @object,
      int graphDepth,
      bool serializePersistentObjects)
    {
      return (ObjectData) this.serializeObject(@object, graphDepth, serializePersistentObjects);
    }

    [JavaFlags(17)]
    public ObjectData createResolveImmediatelyGraph(
      NakedObject @object,
      bool serializePersistentObjects)
    {
      return (ObjectData) this.serializeObject(@object, this.resolveImmediatelyGraphDepth, serializePersistentObjects);
    }

    [JavaFlags(17)]
    public ObjectData createCompletePersistentGraph(
      NakedObject @object,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      return (ObjectData) this.serializeObject(@object, this.persistentGraphDepth, knownObjects, serializePersistentObjects);
    }

    [JavaFlags(17)]
    public ObjectData createCompletePersistentGraph(
      NakedObject @object,
      int persistentGraphDepth,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      return (ObjectData) this.serializeObject(@object, persistentGraphDepth, knownObjects, serializePersistentObjects);
    }

    public virtual Data createForResolveField(
      NakedObject @object,
      string fieldName,
      bool serializePersistentObjects)
    {
      Oid oid = @object.getOid();
      NakedObjectSpecification specification = @object.getSpecification();
      string fullName = specification.getFullName();
      ResolveState resolveState = @object.getResolveState();
      NakedObjectField[] fields = this.getFields(specification);
      int length = fields.Length;
      Data[] fieldContent = length >= 0 ? new Data[length] : throw new NegativeArraySizeException();
      NakedObjects.getObjectLoader().stateCreateChangeLock(@object.getOid(), @object.getSpecification());
      try
      {
        NakedObjects.getObjectLoader().start((NakedReference) @object, @object.getResolveState().serializeFrom());
        ObjectEncoder.KnownObjects knownObjects = new ObjectEncoder.KnownObjects();
        for (int index = 0; index < fields.Length; ++index)
        {
          if (StringImpl.equals(fields[index].getId(), (object) fieldName))
          {
            Naked field = @object.getField(fields[index]);
            fieldContent[index] = field != null ? (!fields[index].isValue() ? (!fields[index].isCollection() ? (Data) this.serializeObject((NakedObject) field, this.resolveFieldGraphDepth, knownObjects, serializePersistentObjects) : (Data) this.createCollection((NakedCollection) field, this.resolveFieldGraphDepth, knownObjects, serializePersistentObjects)) : (Data) this.createValueData(field)) : (Data) this.factory.createNullData(fields[index].getSpecification().getFullName());
            break;
          }
        }
        NakedObjects.getObjectLoader().end((NakedReference) @object);
      }
      finally
      {
        NakedObjects.getObjectLoader().stateCreateChangeRelease(@object.getOid(), @object.getSpecification());
      }
      ObjectData objectData = this.factory.createObjectData(oid, fullName, resolveState.isResolved(), @object.getVersion());
      objectData.setFieldContent(fieldContent);
      return (Data) objectData;
    }

    [JavaFlags(17)]
    public ObjectData createForUpdate(
      NakedObject @object,
      bool serializePersistentObjects)
    {
      return this.createForUpdate(@object, this.updateGraphDepth, serializePersistentObjects);
    }

    [JavaFlags(17)]
    public ObjectData createForUpdate(
      NakedObject @object,
      int updateGraphDepth,
      bool serializePersistentObjects)
    {
      ResolveState resolveState = @object.getResolveState();
      string str = (string) null;
      if (@object.getOid() != null)
        str = @object.getOid().ToString();
      if (ObjectEncoder.LOG.isDebugEnabled())
        ObjectEncoder.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" createForUpdate, about to serialise object ").append((object) @object).append(" OID:").append(str).append(" current state: ").append(@object.getResolveState().ToString()).ToString());
      if (resolveState.isSerializing())
      {
        string msg = new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" Invalid Resolve State Serializing when encoding OID:").append((object) @object.getOid()).append(" object:").append((object) @object).ToString();
        ObjectEncoder.LOG.error((object) msg);
        throw new NakedObjectsRemoteException(msg);
      }
      if (resolveState.isGhost())
      {
        string msg = new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" Invalid Resolve State Ghost when encoding OID:").append((object) @object.getOid()).append(" object:").append((object) @object).ToString();
        ObjectEncoder.LOG.error((object) msg);
        throw new NakedObjectsRemoteException(msg);
      }
      return (ObjectData) this.serializeObject(@object, updateGraphDepth, serializePersistentObjects);
    }

    public virtual ObjectData createGraphForChangedObject(
      NakedObject @object,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      return (ObjectData) this.serializeObject(@object, 1, knownObjects, serializePersistentObjects);
    }

    public virtual ObjectData createMadePersistentGraph(
      ObjectData data,
      NakedObject @object,
      SingleResponseUpdateNotifier updateNotifier)
    {
      return this.createMadePersistentGraph(data, @object, updateNotifier, false);
    }

    public virtual ObjectData createMadePersistentGraph(
      ObjectData data,
      NakedObject @object,
      SingleResponseUpdateNotifier updateNotifier,
      bool forcePersist)
    {
      Oid oid = @object.getOid();
      if (ObjectEncoder.LOG.isDebugEnabled())
        ObjectEncoder.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" start createMadePersistentGraph - OID:").append((object) oid).append(" Object:").append((object) @object).ToString());
      if (@object.getResolveState().isSerializing())
      {
        if (ObjectEncoder.LOG.isDebugEnabled())
          ObjectEncoder.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" createMadePersistentGraph: returning null, because object is already serialising, OID:").append((object) oid).append(" Object:").append((object) @object).ToString());
        return (ObjectData) null;
      }
      if (data == null || oid == null)
        return (ObjectData) null;
      if (data.getOid() != null && !forcePersist)
      {
        if (ObjectEncoder.LOG.isDebugEnabled())
          ObjectEncoder.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("createMadePersistentGraph - returning null because the inbound Data already has an Oid attached to it. OID:").append((object) data.getOid()).append(" object:").append((object) @object).ToString());
        return (ObjectData) null;
      }
      string type = data.getType();
      ReferenceData[] referenceDataArray1;
      if (data.getFieldContent() == null)
      {
        referenceDataArray1 = (ReferenceData[]) null;
      }
      else
      {
        int length = data.getFieldContent().Length;
        referenceDataArray1 = length >= 0 ? new ReferenceData[length] : throw new NegativeArraySizeException();
      }
      ReferenceData[] referenceDataArray2 = referenceDataArray1;
      Version version = @object.getVersion();
      NakedObjectField[] fields = this.dataStructure.getFields(@object.getSpecification());
      NakedObjects.getObjectLoader().stateCreateChangeLock(@object.getOid(), @object.getSpecification());
      try
      {
        NakedObjects.getObjectLoader().start((NakedReference) @object, @object.getResolveState().serializeFrom());
        for (int index1 = 0; index1 < fields.Length; ++index1)
        {
          if (!fields[index1].isValue() && data.getFieldContent()[index1] != null)
          {
            if (fields[index1].isCollection())
            {
              CollectionData collectionData = (CollectionData) data.getFieldContent()[index1];
              int length = collectionData.getElements().Length;
              ObjectData[] objectDataArray = length >= 0 ? new ObjectData[length] : throw new NegativeArraySizeException();
              NakedCollection field = (NakedCollection) @object.getField(fields[index1]);
              for (int index2 = 0; index2 < collectionData.getElements().Length; ++index2)
              {
                ReferenceData element = collectionData.getElements()[index2];
                if (element is ObjectData)
                {
                  NakedObject object1 = field.elementAt(index2);
                  objectDataArray[index2] = this.createMadePersistentGraph((ObjectData) element, object1, updateNotifier);
                }
              }
              referenceDataArray2[index1] = (ReferenceData) this.factory.createCollectionData(field.getOid(), collectionData.getType(), (ReferenceData[]) objectDataArray, collectionData.hasAllElements(), field.getVersion());
            }
            else
            {
              Data data1 = fields[index1].isObject() ? data.getFieldContent()[index1] : throw new UnknownTypeException();
              switch (data1)
              {
                case null:
                case NullData _:
                  continue;
                default:
                  ReferenceData referenceData = (ReferenceData) data1;
                  if (referenceData.getOid() == null || referenceData.getOid().isNull())
                  {
                    NakedObject field = (NakedObject) @object.getField(fields[index1]);
                    referenceDataArray2[index1] = (ReferenceData) this.createMadePersistentGraph((ObjectData) data1, field, updateNotifier);
                    continue;
                  }
                  continue;
              }
            }
          }
        }
        NakedObjects.getObjectLoader().end((NakedReference) @object);
      }
      finally
      {
        NakedObjects.getObjectLoader().stateCreateChangeRelease(@object.getOid(), @object.getSpecification());
      }
      ObjectData objectData = this.factory.createObjectData(oid, type, true, version);
      objectData.setFieldContent((Data[]) referenceDataArray2);
      if (ObjectEncoder.LOG.isDebugEnabled())
        ObjectEncoder.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" end createMadePersistentGraph - OID:").append((object) oid).append(" Object:").append((object) @object).ToString());
      return objectData;
    }

    [JavaFlags(17)]
    public ObjectData createMakePersistentGraph(
      NakedObject @object,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      Assert.assertTrue("transient", @object.getResolveState().isTransient());
      return (ObjectData) this.serializeObject(@object, 1, knownObjects, serializePersistentObjects);
    }

    [JavaFlags(18)]
    private Data createParameter(
      string type,
      Naked @object,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      if (@object == null)
        return (Data) this.factory.createNullData(type);
      if (@object.getSpecification().isObject())
        return (Data) this.serializeObject((NakedObject) @object, 0, knownObjects, serializePersistentObjects);
      return @object.getSpecification().isValue() ? (Data) this.createValueData(@object) : throw new UnknownTypeException((object) @object.getSpecification());
    }

    [JavaFlags(17)]
    public Data[] createParameters(
      NakedObjectSpecification[] parameterTypes,
      Naked[] parameters,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      int length = parameters.Length;
      Data[] dataArray = length >= 0 ? new Data[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < parameters.Length; ++index)
      {
        Naked parameter = parameters[index];
        string fullName = parameterTypes[index].getFullName();
        dataArray[index] = this.createParameter(fullName, parameter, knownObjects, serializePersistentObjects);
      }
      return dataArray;
    }

    [JavaFlags(17)]
    public IdentityData createIdentityData(NakedObject @object)
    {
      Assert.assertNotNull("OID needed for reference", (object) @object, (object) @object.getOid());
      return this.factory.createIdentityData(@object.getSpecification().getFullName(), @object.getOid(), @object.getVersion());
    }

    public virtual ServerActionResultData createServerActionResult(
      Naked returns,
      ObjectData[] updatesData,
      ObjectData persistedTarget,
      ObjectData[] persistedParameters,
      string[] messages,
      string[] warnings,
      bool serializePersistentObjects)
    {
      return this.createServerActionResult(returns, updatesData, persistedTarget, persistedParameters, messages, warnings, this.persistentGraphDepth, serializePersistentObjects);
    }

    public virtual ServerActionResultData createServerActionResult(
      Naked returns,
      ObjectData[] updatesData,
      ObjectData persistedTarget,
      ObjectData[] persistedParameters,
      string[] messages,
      string[] warnings,
      int actionGraphDepth,
      bool serializePersistentObjects)
    {
      if (actionGraphDepth == -1)
        actionGraphDepth = this.actionGraphDepth;
      Data result;
      switch (returns)
      {
        case null:
          result = (Data) this.factory.createNullData("");
          break;
        case NakedCollection _:
          result = (Data) this.createCollection((NakedCollection) returns, actionGraphDepth, new ObjectEncoder.KnownObjects(), serializePersistentObjects);
          break;
        case NakedObject _:
          result = (Data) this.createCompletePersistentGraph((NakedObject) returns, actionGraphDepth, serializePersistentObjects);
          break;
        default:
          throw new UnknownTypeException((object) returns);
      }
      return this.factory.createActionResultData(result, updatesData, persistedTarget, persistedParameters, messages, warnings);
    }

    [JavaFlags(18)]
    private ValueData createValueData(Naked @object) => this.factory.createValueData(@object.getSpecification().getFullName(), @object.getObject());

    private NakedObjectField[] getFields(NakedObjectSpecification specification) => this.dataStructure.getFields(specification);

    [JavaFlags(18)]
    private ReferenceData serializeObject(
      NakedObject @object,
      int graphDepth,
      bool serializePersistentObjects)
    {
      Assert.assertNotNull((object) @object);
      return (ReferenceData) this.serializeObject2(@object, graphDepth, new ObjectEncoder.KnownObjects(), serializePersistentObjects);
    }

    [JavaFlags(18)]
    private ReferenceData serializeObject(
      NakedObject @object,
      int depth,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      Assert.assertNotNull((object) @object);
      return (ReferenceData) this.serializeObject2(@object, depth, knownObjects, serializePersistentObjects);
    }

    private bool isDomainObjectStillTransientInBOMi2(NakedObject naked) => naked != null && naked.getResolveState().isTransient() || naked != null && naked.getOid() != null && !naked.getOid().ToString().Contains("|");

    [JavaFlags(18)]
    private Data serializeObject2(
      NakedObject @object,
      int graphDepth,
      ObjectEncoder.KnownObjects knownObjects,
      bool serializePersistentObjects)
    {
      Assert.assertNotNull((object) @object);
      ResolveState resolveState = @object.getResolveState();
      bool flag1 = resolveState.isTransient();
      if (ObjectEncoder.LOG.isDebugEnabled())
        ObjectEncoder.LOG.debug((object) new StringBuffer().append("#").append(this.GetHashCode()).append(" SerialiseObject2() - serialising object OID:").append((object) @object.getOid()).append(" state: ").append(resolveState.ToString()).append(" Object:").append((object) @object).ToString());
      bool flag2 = ((serializePersistentObjects ? 1 : 0) ^ 1) != 0;
      bool flag3 = this.isDomainObjectStillTransientInBOMi2(@object);
      if (flag2 && !flag3)
        return (Data) this.createIdentityData(@object);
      if (!flag1 && (resolveState.isSerializing() || resolveState.isGhost() || graphDepth <= 0))
        return (Data) this.createIdentityData(@object);
      if (flag1 && knownObjects.containsKey(@object))
        return (Data) knownObjects.get(@object);
      bool hasCompleteData = resolveState == ResolveState.TRANSIENT || resolveState == ResolveState.RESOLVED;
      string fullName = @object.getSpecification().getFullName();
      ObjectData objectData = this.factory.createObjectData(@object.getOid(), fullName, hasCompleteData, @object.getVersion());
      if (flag1)
        knownObjects.put(@object, objectData);
      NakedObjectField[] fields = this.getFields(@object.getSpecification());
      int length = fields.Length;
      Data[] fieldContent = length >= 0 ? new Data[length] : throw new NegativeArraySizeException();
      NakedObjects.getObjectLoader().stateCreateChangeLock(@object.getOid(), @object.getSpecification());
      try
      {
        NakedObjects.getObjectLoader().start((NakedReference) @object, @object.getResolveState().serializeFrom());
        for (int index = 0; index < fields.Length; ++index)
        {
          if (!fields[index].isDerived())
          {
            Naked field = @object.getField(fields[index]);
            if (fields[index].isValue())
              fieldContent[index] = (Data) this.createValueData(field);
            else if (fields[index].isCollection())
            {
              fieldContent[index] = (Data) this.createCollection((NakedCollection) field, graphDepth - 1, knownObjects, serializePersistentObjects);
            }
            else
            {
              if (!fields[index].isObject())
                throw new UnknownTypeException((object) fields[index]);
              fieldContent[index] = field != null ? this.serializeObject2((NakedObject) field, graphDepth - 1, knownObjects, serializePersistentObjects) : (hasCompleteData ? (Data) this.factory.createNullData(fields[index].getSpecification().getFullName()) : (Data) null);
            }
          }
        }
        NakedObjects.getObjectLoader().end((NakedReference) @object);
      }
      finally
      {
        NakedObjects.getObjectLoader().stateCreateChangeRelease(@object.getOid(), @object.getSpecification());
      }
      objectData.setFieldContent(fieldContent);
      return (Data) objectData;
    }

    public virtual int ActionGraphDepth
    {
      set => this.setActionGraphDepth(value);
    }

    public virtual DataFactory DataFactory
    {
      set => this.setDataFactory(value);
    }

    public virtual int PersistentGraphDepth
    {
      set => this.setPersistentGraphDepth(value);
    }

    public virtual int UpdateGraphDepth
    {
      set => this.setUpdateGraphDepth(value);
    }

    public virtual int ResolveFieldGraphDepth
    {
      set => this.setResolveFieldGraphDepth(value);
    }

    public virtual int ResolveImmediatelyGraphDepth
    {
      set => this.setResolveImmediatelyGraphDepth(value);
    }

    public virtual void setActionGraphDepth(int actionGraphDepth) => this.actionGraphDepth = actionGraphDepth;

    public virtual void setDataFactory(DataFactory factory) => this.factory = factory;

    public virtual void setPersistentGraphDepth(int persistentGraphDepth) => this.persistentGraphDepth = persistentGraphDepth;

    public virtual void setUpdateGraphDepth(int updateGraphDepth) => this.updateGraphDepth = updateGraphDepth;

    public virtual void setResolveFieldGraphDepth(int resolveFieldGraphDepth) => this.resolveFieldGraphDepth = resolveFieldGraphDepth;

    public virtual void setResolveImmediatelyGraphDepth(int resolveImmediatelyGraphDepth) => this.resolveImmediatelyGraphDepth = resolveImmediatelyGraphDepth;

    public ObjectEncoder()
    {
      this.actionGraphDepth = 0;
      this.dataStructure = new DataStructure();
      this.persistentGraphDepth = 100;
      this.updateGraphDepth = 1;
      this.resolveFieldGraphDepth = 100;
      this.resolveImmediatelyGraphDepth = 100;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ObjectEncoder()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ObjectEncoder objectEncoder = this;
      ObjectImpl.clone((object) objectEncoder);
      return ((object) objectEncoder).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(41)]
    public class KnownObjects
    {
      private Hashtable knownObjects;

      [JavaFlags(2)]
      public KnownObjects() => this.knownObjects = new Hashtable();

      [JavaFlags(2)]
      public virtual bool containsKey(NakedObject @object) => this.knownObjects.containsKey((object) @object);

      [JavaFlags(2)]
      public virtual ObjectData get(NakedObject @object) => (ObjectData) this.knownObjects.get((object) @object);

      [JavaFlags(2)]
      public virtual void put(NakedObject @object, ObjectData data) => this.knownObjects.put((object) @object, (object) data);

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ObjectEncoder.KnownObjects knownObjects = this;
        ObjectImpl.clone((object) knownObjects);
        return ((object) knownObjects).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
