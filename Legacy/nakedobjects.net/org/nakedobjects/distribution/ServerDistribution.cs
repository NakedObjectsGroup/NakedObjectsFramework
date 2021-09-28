// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ServerDistribution
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.distribution
{
  [JavaInterfaces("1;org/nakedobjects/distribution/Distribution;")]
  public class ServerDistribution : Distribution
  {
    private static readonly org.apache.log4j.Logger LOG;
    private ObjectEncoder encoder;
    private SingleResponseUpdateNotifier updateNotifier;
    private int myNotifierHashCode;
    private bool myNotifierAlreadySet;

    public ServerDistribution()
    {
      this.myNotifierAlreadySet = false;
      ObjectDecoder.setUpdateNotifer((DirtyObjectSet) new NullDirtyObjectSet());
    }

    public virtual ObjectData[] allInstances(
      Session session,
      string fullName,
      bool includeSubclasses)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request allInstances of ").append(fullName).append(!includeSubclasses ? "" : "(including subclasses)").append(" from ").append((object) session).ToString());
      return this.convertToNakedCollection(this.persistor().allInstances(this.getSpecification(fullName), includeSubclasses));
    }

    public virtual ObjectData[] clearAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associated)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request clearAssociation ").append(fieldIdentifier).append(" on ").append((object) target).append(" of ").append((object) associated).append(" for ").append((object) session).ToString());
      NakedObject persistentNakedObject1 = this.getPersistentNakedObject(session, target);
      NakedObject persistentNakedObject2 = this.getPersistentNakedObject(session, associated);
      NakedObjectField field = persistentNakedObject1.getSpecification().getField(fieldIdentifier);
      if (!field.isAuthorised() || field.isAvailable((NakedReference) persistentNakedObject1).isVetoed())
        throw new IllegalRequestException("can't modify field as not visible or editable");
      persistentNakedObject1.clearAssociation(field, persistentNakedObject2);
      return this.getUpdates();
    }

    private ObjectData[] convertToNakedCollection(TypedNakedCollection instances)
    {
      int length = instances.size();
      ObjectData[] objectDataArray = length >= 0 ? new ObjectData[length] : throw new NegativeArraySizeException();
      bool serializePersistentObjects = true;
      for (int index = 0; index < instances.size(); ++index)
        objectDataArray[index] = this.encoder.createCompletePersistentGraph(instances.elementAt(index), serializePersistentObjects);
      return objectDataArray;
    }

    public virtual ServerActionResultData executeServerAction(
      Session session,
      string actionType,
      string actionIdentifier,
      ReferenceData target,
      Data[] parameterData)
    {
      return this.executeServerAction(session, actionType, actionIdentifier, target, parameterData, -1);
    }

    public virtual ServerActionResultData executeServerAction(
      Session session,
      string actionType,
      string actionIdentifier,
      ReferenceData target,
      Data[] parameterData,
      int actionGraphDepth)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request executeAction ").append(actionIdentifier).append(" on ").append((object) target).append(" for ").append((object) session).ToString());
      ObjectDecoder.KnownTransients knownTransients = ObjectDecoder.createKnownTransients();
      NakedObject @object;
      switch (target)
      {
        case IdentityData _:
          @object = this.getPersistentNakedObject(session, (IdentityData) target);
          break;
        case ObjectData _:
          @object = (NakedObject) ObjectDecoder.restore((Data) target, knownTransients);
          break;
        case null:
          @object = (NakedObject) null;
          break;
        default:
          throw new NakedObjectRuntimeException();
      }
      Action actionMethod = this.getActionMethod(actionType, actionIdentifier, parameterData, @object);
      Naked[] parameters = this.getParameters(session, parameterData, knownTransients);
      if (actionMethod == null)
        throw new NakedObjectsRemoteException(new StringBuffer().append("Could not find method ").append(actionIdentifier).ToString());
      Naked returns = actionMethod.execute((NakedReference) @object, parameters);
      this.dumpUpdateNotifierObjectStates("UpdatesStateDump: - the following is a list of objects from updateNotifier immediately after execute(), before makePersistentGraph.");
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) "About to Persist Target...");
      ObjectData persistedTarget = target != null ? (!(target is ObjectData) ? (ObjectData) null : this.encoder.createMadePersistentGraph((ObjectData) target, @object, this.updateNotifier)) : (ObjectData) null;
      this.dumpUpdateNotifierObjectStates("UpdatesStateDump: the following is a list of objects from updateNotifier immediately after persisting Target()");
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) "About to Persist parameters...");
      int length = parameterData.Length;
      ObjectData[] persistedParameters = length >= 0 ? new ObjectData[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < persistedParameters.Length; ++index)
      {
        if (actionMethod.getParameterTypes()[index].isObject() && parameterData[index] is ObjectData)
          persistedParameters[index] = this.encoder.createMadePersistentGraph((ObjectData) parameterData[index], (NakedObject) parameters[index], this.updateNotifier);
      }
      this.dumpUpdateNotifierObjectStates("UpdatesStateDump: the following is a list of objects from updateNotifier immediately after persisting Parameters()");
      string[] messages = NakedObjects.getMessageBroker().getMessages();
      string[] warnings = NakedObjects.getMessageBroker().getWarnings();
      bool serializePersistentObjects = true;
      return this.encoder.createServerActionResult(returns, this.getUpdates(), persistedTarget, persistedParameters, messages, warnings, actionGraphDepth, serializePersistentObjects);
    }

    private void dumpUpdateNotifierObjectStates(string headermsg)
    {
      if (!ServerDistribution.LOG.isDebugEnabled())
        return;
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(headermsg).append("\n").append(this.updateNotifier.updateList());
      ServerDistribution.LOG.debug((object) stringBuffer);
    }

    public virtual ClientActionResultData executeClientAction(
      Session session,
      ReferenceData[] data,
      int[] types)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual ObjectData[] findInstances(Session session, InstancesCriteria criteria)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request findInstances ").append((object) criteria).append(" for ").append((object) session).ToString());
      return this.convertToNakedCollection(this.persistor().findInstances(criteria));
    }

    public virtual Hint getActionHint(
      Session session,
      string actionType,
      string actionIdentifier,
      ObjectData target,
      Data[] parameters)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request getActionHint ").append(actionIdentifier).append(" for ").append((object) session).ToString());
      return (Hint) new DefaultHint();
    }

    private Action getActionMethod(
      string actionType,
      string actionIdentifier,
      Data[] parameterData,
      NakedObject @object)
    {
      int length = parameterData.Length;
      NakedObjectSpecification[] parameters = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < parameters.Length; ++index)
        parameters[index] = this.getSpecification(parameterData[index].getType());
      Action.Type type = ActionImpl.getType(actionType);
      int num = StringImpl.indexOf(actionIdentifier, 35);
      string name = StringImpl.substring(actionIdentifier, 0, num);
      string lowerCase = StringImpl.toLowerCase(StringImpl.substring(actionIdentifier, num + 1));
      return @object != null ? @object.getSpecification().getObjectAction(type, lowerCase, parameters) : NakedObjects.getSpecificationLoader().loadSpecification(name).getClassAction(type, lowerCase, parameters);
    }

    private Naked[] getParameters(
      Session session,
      Data[] parameterData,
      ObjectDecoder.KnownTransients knownObjects)
    {
      int length = parameterData.Length;
      Naked[] nakedArray = length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < nakedArray.Length; ++index)
      {
        Data data = parameterData[index];
        switch (data)
        {
          case NullData _:
            continue;
          case IdentityData _:
            nakedArray[index] = (Naked) this.getPersistentNakedObject(session, (IdentityData) data);
            continue;
          case ObjectData _:
            nakedArray[index] = ObjectDecoder.restore(data, knownObjects);
            continue;
          case ValueData _:
            ValueData valueData = (ValueData) data;
            nakedArray[index] = (Naked) NakedObjects.getObjectLoader().createAdapterForValue(valueData.getValue());
            continue;
          default:
            throw new UnknownTypeException((object) data);
        }
      }
      return nakedArray;
    }

    private NakedObject getPersistentNakedObject(Session session, IdentityData @object)
    {
      NakedObjectSpecification specification = this.getSpecification(@object.getType());
      NakedObject nakedObject = NakedObjects.getObjectPersistor().getObject(@object.getOid(), specification);
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("get object ").append((object) @object).append(" for ").append((object) session).append(" --> ").append((object) nakedObject).ToString());
      if (@object.getVersion() != null)
        nakedObject.checkLock(@object.getVersion());
      return nakedObject;
    }

    private NakedObjectSpecification getSpecification(string fullName) => NakedObjects.getSpecificationLoader().loadSpecification(fullName);

    private ObjectData[] getUpdates()
    {
      NakedObject[] updates = this.updateNotifier.getUpdates();
      int length1 = updates.Length;
      int length2 = length1;
      ObjectData[] objectDataArray = length2 >= 0 ? new ObjectData[length2] : throw new NegativeArraySizeException();
      bool serializePersistentObjects = true;
      for (int index = 0; index < length1; ++index)
      {
        ObjectData forUpdate = this.encoder.createForUpdate(updates[index], serializePersistentObjects);
        objectDataArray[index] = forUpdate;
      }
      return objectDataArray;
    }

    public virtual bool hasInstances(Session session, string objectType)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request hasInstances of ").append(objectType).append(" for ").append((object) session).ToString());
      return this.persistor().hasInstances(this.getSpecification(objectType), false);
    }

    public virtual int numberOfInstances(Session session, string objectType)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request numberOfInstances of ").append(objectType).append(" for ").append((object) session).ToString());
      return this.persistor().numberOfInstances(this.getSpecification(objectType), false);
    }

    private NakedObjectPersistor persistor() => NakedObjects.getObjectPersistor();

    public virtual Data resolveField(Session session, IdentityData target, string fieldName)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request resolveEagerly ").append((object) target).append("/").append(fieldName).append(" for ").append((object) session).ToString());
      NakedObjectSpecification specification = this.getSpecification(target.getType());
      NakedObjectField field = specification.getField(fieldName);
      NakedObject @object = NakedObjects.getObjectLoader().recreateAdapterForPersistent(target.getOid(), specification);
      bool serializePersistentObjects = true;
      NakedObjects.getObjectPersistor().resolveField(@object, field);
      return this.encoder.createForResolveField(@object, fieldName, serializePersistentObjects);
    }

    public virtual ObjectData resolveImmediately(Session session, IdentityData target)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request resolveImmediately ").append((object) target).append(" for ").append((object) session).ToString());
      NakedObjectSpecification specification = this.getSpecification(target.getType());
      NakedObject @object = NakedObjects.getObjectPersistor().getObject(target.getOid(), specification);
      if (@object.getResolveState().isResolvable(ResolveState.RESOLVING))
        NakedObjects.getObjectPersistor().resolveImmediately(@object);
      bool serializePersistentObjects = true;
      return this.encoder.createResolveImmediatelyGraph(@object, serializePersistentObjects);
    }

    public virtual ObjectEncoder Encoder
    {
      set => this.encoder = value;
    }

    public virtual SingleResponseUpdateNotifier UpdateNotifier
    {
      set => this.setUpdateNotifier(value);
    }

    public virtual ObjectData[] setAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associated)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request setAssociation ").append(fieldIdentifier).append(" on ").append((object) target).append(" with ").append((object) associated).append(" for ").append((object) session).ToString());
      NakedObject persistentNakedObject1 = this.getPersistentNakedObject(session, target);
      NakedObject persistentNakedObject2 = this.getPersistentNakedObject(session, associated);
      NakedObjectField field = persistentNakedObject1.getSpecification().getField(StringImpl.toLowerCase(fieldIdentifier));
      if (!field.isAuthorised() || field.isAvailable((NakedReference) persistentNakedObject1).isVetoed())
        throw new IllegalRequestException("can't modify field as not visible or editable");
      persistentNakedObject1.setAssociation(field, persistentNakedObject2);
      return this.getUpdates();
    }

    public virtual void setEncoder(ObjectEncoder objectDataFactory) => this.encoder = objectDataFactory;

    public virtual void setUpdateNotifier(SingleResponseUpdateNotifier updateNotifier)
    {
      int hashCode = updateNotifier.GetHashCode();
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("ServerDistribution.setUpdateNotifier() ").append(Long.toHexString((long) hashCode)).ToString());
      if (this.myNotifierAlreadySet && this.myNotifierHashCode != hashCode)
        throw new RuntimeException(new StringBuffer().append("Setting UpdateNotifier after it has been already set - OldHashCode:").append(Long.toHexString((long) this.myNotifierHashCode)).append(" new HashCode:").append(Long.toHexString((long) hashCode)).ToString());
      this.myNotifierAlreadySet = true;
      this.myNotifierHashCode = hashCode;
      this.updateNotifier = updateNotifier;
    }

    public virtual ObjectData[] setValue(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      object value)
    {
      if (ServerDistribution.LOG.isDebugEnabled())
        ServerDistribution.LOG.debug((object) new StringBuffer().append("request setValue ").append(fieldIdentifier).append(" on ").append((object) target).append(" with ").append(value).append(" for ").append((object) session).ToString());
      NakedObject persistentNakedObject = this.getPersistentNakedObject(session, target);
      OneToOneAssociation field = (OneToOneAssociation) persistentNakedObject.getSpecification().getField(StringImpl.toLowerCase(fieldIdentifier));
      if (!field.isAuthorised() || field.isAvailable((NakedReference) persistentNakedObject).isVetoed())
        throw new IllegalRequestException("can't modify field as not visible or editable");
      persistentNakedObject.getValue(field)?.restoreFromEncodedString(NakedObjects.getObjectLoader().createAdapterForValue(value).asEncodedString());
      persistentNakedObject.setValue(field, value);
      return this.getUpdates();
    }

    public virtual string updateList() => this.updateNotifier.updateList();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ServerDistribution()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ServerDistribution serverDistribution = this;
      ObjectImpl.clone((object) serverDistribution);
      return ((object) serverDistribution).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
