// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ProxyAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.distribution
{
  public sealed class ProxyAction : AbstractActionPeer
  {
    private static readonly org.apache.log4j.Logger LOG;
    private Distribution connection;
    private readonly ObjectEncoder encoder;
    private bool cacheResults;

    public ProxyAction(
      ActionPeer local,
      Distribution connection,
      ObjectEncoder encoder,
      bool cacheResults)
      : base(local)
    {
      this.connection = connection;
      this.encoder = encoder;
      this.cacheResults = cacheResults;
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/reflect/ReflectiveActionException;")]
    public override Naked execute(NakedReference target, Naked[] parameters)
    {
      if (this.isToBeExecutedRemotely(target))
      {
        bool serializePersistentObjects = true;
        return this.executeRemotely((NakedObject) target, parameters, serializePersistentObjects);
      }
      if (ProxyAction.LOG.isDebugEnabled())
        ProxyAction.LOG.debug((object) this.debug("execute locally", this.getIdentifier(), target, parameters));
      return base.execute(target, parameters);
    }

    private Naked executeRemotely(
      NakedObject target,
      Naked[] parameters,
      bool serializePersistentObjects)
    {
      ObjectEncoder.KnownObjects knownObjects = ObjectEncoder.createKnownObjects();
      Data[] parameters1 = this.parameterValues(parameters, knownObjects);
      if (ProxyAction.LOG.isDebugEnabled())
        ProxyAction.LOG.debug((object) this.debug("execute remotely", this.getIdentifier(), (NakedReference) target, parameters));
      ReferenceData target1 = target != null ? this.encoder.createActionTarget(target, knownObjects, serializePersistentObjects) : (ReferenceData) null;
      ServerActionResultData result;
      try
      {
        string actionIdentifier = new StringBuffer().append(this.getIdentifier().getClassName()).append("#").append(this.getIdentifier().getName()).ToString();
        result = this.connection.executeServerAction(NakedObjects.getCurrentSession(), this.getType().getName(), actionIdentifier, target1, parameters1, this.getActionGraphDepth());
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
          NakedObjects.getObjectPersistor().reload(adapterFor);
          throw new ConcurrencyException(new StringBuffer().append("Object automatically reloaded: ").append(adapterFor.titleString()).ToString(), (Throwable) concurrencyException1);
        }
      }
      catch (NakedObjectRuntimeException ex)
      {
        NakedObjectRuntimeException runtimeException1 = ex;
        ProxyAction.LOG.error((object) new StringBuffer().append("remote exception: ").append(((Throwable) runtimeException1).getMessage()).ToString(), (Throwable) runtimeException1);
        NakedObjectRuntimeException runtimeException2 = runtimeException1;
        if (runtimeException2 != ex)
          throw runtimeException2;
        throw;
      }
      ObjectDecoder.madePersistent(target, result.getPersistedTarget());
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (this.getParameterTypes()[index].isObject())
          ObjectDecoder.madePersistent((NakedObject) parameters[index], result.getPersistedParameters()[index]);
      }
      Data data = result.getReturn();
      Naked returnedObject = !(data is NullData) ? ObjectDecoder.restore(data) : (Naked) null;
      this.refreshIfNotCachingResults(returnedObject, result);
      ObjectData[] updates = result.getUpdates();
      for (int index = 0; index < updates.Length; ++index)
      {
        if (ProxyAction.LOG.isDebugEnabled())
          ProxyAction.LOG.debug((object) new StringBuffer().append("update ").append(DistributionLogger.dump((Data) updates[index])).ToString());
        ObjectDecoder.restore((Data) updates[index]);
      }
      foreach (string message in result.getMessages())
        NakedObjects.getMessageBroker().addMessage(message);
      foreach (string warning in result.getWarnings())
        NakedObjects.getMessageBroker().addWarning(warning);
      return returnedObject;
    }

    private void refreshIfNotCachingResults(Naked returnedObject, ServerActionResultData result)
    {
      if (this.cacheResults || !(returnedObject is NakedReference))
        return;
      NakedObjects.getObjectPersistor().refresh((NakedReference) returnedObject, result.getResolvedOids());
    }

    private bool isToBeExecutedRemotely(NakedReference target)
    {
      bool flag = this.getTarget() == Action.REMOTE;
      if (this.getTarget() == Action.LOCAL)
        return false;
      if (flag)
        return true;
      return target != null && target.getOid() != null;
    }

    private Data[] parameterValues(Naked[] parameters, ObjectEncoder.KnownObjects knownObjects)
    {
      NakedObjectSpecification[] parameterTypes = this.getParameterTypes();
      bool serializePersistentObjects = true;
      return this.encoder.createParameters(parameterTypes, parameters, knownObjects, serializePersistentObjects);
    }

    private string debug(
      string message,
      MemberIdentifier identifier,
      NakedReference target,
      Naked[] parameters)
    {
      if (!ProxyAction.LOG.isDebugEnabled())
        return "";
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(message);
      stringBuffer.append(" ");
      stringBuffer.append((object) identifier);
      stringBuffer.append(" on ");
      stringBuffer.append((object) target);
      for (int index = 0; index < parameters.Length; ++index)
      {
        if (index > 0)
          stringBuffer.append(',');
        stringBuffer.append((object) parameters[index]);
      }
      return stringBuffer.ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ProxyAction()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
