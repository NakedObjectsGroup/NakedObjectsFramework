// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.CommandClient
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.distribution.command
{
  [JavaInterfaces("1;org/nakedobjects/distribution/Distribution;")]
  public abstract class CommandClient : Distribution
  {
    private static readonly org.apache.log4j.Logger LOG;

    public virtual ObjectData[] allInstances(
      Session session,
      string fullName,
      bool includeSubclasses)
    {
      AllInstances allInstances = new AllInstances(session, fullName, includeSubclasses);
      this.execute((Request) allInstances);
      return allInstances.getInstances();
    }

    public virtual ObjectData[] clearAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associate)
    {
      ClearAssociation clearAssociation = new ClearAssociation(session, fieldIdentifier, target, associate);
      this.execute((Request) clearAssociation);
      return clearAssociation.getChanges();
    }

    public virtual ServerActionResultData executeServerAction(
      Session session,
      string actionType,
      string actionIdentifier,
      ReferenceData target,
      Data[] parameters,
      int actionGraphDepth)
    {
      ExecuteAction executeAction = new ExecuteAction(session, actionType, actionIdentifier, target, parameters);
      this.execute((Request) executeAction);
      return executeAction.getActionResult();
    }

    public virtual ClientActionResultData executeClientAction(
      Session session,
      ReferenceData[] data,
      int[] types)
    {
      ExecuteClientAction executeClientAction = new ExecuteClientAction(session, data, types);
      this.execute((Request) executeClientAction);
      return executeClientAction.getActionResult();
    }

    public virtual ObjectData[] findInstances(Session session, InstancesCriteria criteria)
    {
      FindInstancesByTitle instancesByTitle = criteria is TitleCriteria ? new FindInstancesByTitle(session, (TitleCriteria) criteria) : throw new NakedObjectRuntimeException();
      this.execute((Request) instancesByTitle);
      return instancesByTitle.getInstances();
    }

    public virtual Hint getActionHint(
      Session session,
      string actionType,
      string actionIdentifier,
      ObjectData target,
      Data[] parameters)
    {
      throw new NotImplementedException();
    }

    public virtual ObjectData resolveImmediately(Session session, IdentityData target)
    {
      Resolve resolve = new Resolve(session, target);
      this.execute((Request) resolve);
      return resolve.getUpdateData();
    }

    public virtual Data resolveField(Session session, IdentityData target, string name)
    {
      ResolveField resolveField = new ResolveField(session, target, name);
      this.execute((Request) resolveField);
      return resolveField.getUpdateData();
    }

    public virtual bool hasInstances(Session session, string fullName)
    {
      HasInstances hasInstances = new HasInstances(session, fullName);
      this.execute((Request) hasInstances);
      return hasInstances.getFlag();
    }

    public virtual int numberOfInstances(Session session, string fullName) => throw new NotImplementedException();

    private void execute(Request request)
    {
      Response response = this.executeRemotely(request);
      if (request.getId() != response.getId())
        throw new NakedObjectRuntimeException(new StringBuffer().append("Response out of sequence with respect to the request: ").append(request.getId()).append(" & ").append(response.getId()).append(" respectively").ToString());
      if (CommandClient.LOG.isDebugEnabled())
        CommandClient.LOG.debug((object) new StringBuffer().append("response ").append((object) response).ToString());
      request.setResponse(response.getObject());
    }

    [JavaFlags(1028)]
    public abstract Response executeRemotely(Request request);

    public virtual ObjectData[] setAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associate)
    {
      SetAssociation setAssociation = new SetAssociation(session, fieldIdentifier, target, associate);
      this.execute((Request) setAssociation);
      return setAssociation.getChanges();
    }

    public virtual ObjectData[] setValue(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      object associate)
    {
      SetValue setValue = new SetValue(session, fieldIdentifier, target, associate);
      this.execute((Request) setValue);
      return setValue.getChanges();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static CommandClient()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      CommandClient commandClient = this;
      ObjectImpl.clone((object) commandClient);
      return ((object) commandClient).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
