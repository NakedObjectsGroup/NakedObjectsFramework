// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ProxyOneToManyAssociation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.distribution
{
  public sealed class ProxyOneToManyAssociation : AbstractOneToManyPeer
  {
    private static readonly Category LOG;
    private Distribution connection;
    private bool fullProxy;
    private readonly ObjectEncoder objectDataFactory;

    public ProxyOneToManyAssociation(
      OneToManyPeer local,
      Distribution connection,
      ObjectEncoder objectDataFactory)
      : base(local)
    {
      this.fullProxy = false;
      this.connection = connection;
      this.objectDataFactory = objectDataFactory;
    }

    public override void addAssociation(NakedObject inObject, NakedObject associate)
    {
      if (this.isPersistent(inObject))
      {
        if (ProxyOneToManyAssociation.LOG.isDebugEnabled())
          ProxyOneToManyAssociation.LOG.debug((object) new StringBuffer().append("set association remotely ").append((object) this.getIdentifier()).append(" in ").append((object) inObject).append(" with ").append((object) associate).ToString());
        try
        {
          IdentityData identityData1 = this.objectDataFactory.createIdentityData(inObject);
          IdentityData identityData2 = this.objectDataFactory.createIdentityData(associate);
          this.updateChangedObjects(this.connection.setAssociation(NakedObjects.getCurrentSession(), this.getIdentifier().getName(), identityData1, identityData2));
        }
        catch (ConcurrencyException ex)
        {
          throw this.concurrencyException(ex);
        }
        catch (NakedObjectRuntimeException ex)
        {
          NakedObjectRuntimeException runtimeException1 = ex;
          ProxyOneToManyAssociation.LOG.error((object) new StringBuffer().append("remote exception: ").append(((Throwable) runtimeException1).getMessage()).ToString(), (Throwable) runtimeException1);
          NakedObjectRuntimeException runtimeException2 = runtimeException1;
          if (runtimeException2 != ex)
            throw runtimeException2;
          throw;
        }
      }
      else
      {
        if (ProxyOneToManyAssociation.LOG.isDebugEnabled())
          ProxyOneToManyAssociation.LOG.debug((object) new StringBuffer().append("set association locally ").append((object) this.getIdentifier()).append(" in ").append((object) inObject).append(" with ").append((object) associate).ToString());
        base.addAssociation(inObject, associate);
      }
    }

    private ConcurrencyException concurrencyException(ConcurrencyException e)
    {
      Oid source = e.getSource();
      if (source == null)
        return e;
      NakedObject adapterFor = NakedObjects.getObjectLoader().getAdapterFor(source);
      NakedObjects.getObjectPersistor().reload(adapterFor);
      return new ConcurrencyException(new StringBuffer().append("Object automatically reloaded: ").append(adapterFor.titleString()).ToString(), (Throwable) e);
    }

    private void updateChangedObjects(ObjectData[] updates)
    {
      for (int index = 0; index < updates.Length; ++index)
      {
        if (ProxyOneToManyAssociation.LOG.isDebugEnabled())
          ProxyOneToManyAssociation.LOG.debug((object) new StringBuffer().append("update ").append(DistributionLogger.dump((Data) updates[index])).ToString());
        ObjectDecoder.restore((Data) updates[index]);
      }
    }

    public override NakedCollection getAssociations(NakedObject inObject)
    {
      if (this.isPersistent(inObject) && this.fullProxy)
      {
        if (ProxyOneToManyAssociation.LOG.isDebugEnabled())
          ProxyOneToManyAssociation.LOG.debug((object) new StringBuffer().append("get association remotely ").append((object) inObject).ToString());
        throw new NotImplementedException();
      }
      if (ProxyOneToManyAssociation.LOG.isDebugEnabled())
        ProxyOneToManyAssociation.LOG.debug((object) new StringBuffer().append("get association locally ").append((object) inObject).ToString());
      return base.getAssociations(inObject);
    }

    private bool isPersistent(NakedObject inObject) => inObject.getOid() != null;

    public override void removeAssociation(NakedObject inObject, NakedObject associate)
    {
      if (this.isPersistent(inObject))
      {
        if (ProxyOneToManyAssociation.LOG.isDebugEnabled())
          ProxyOneToManyAssociation.LOG.debug((object) new StringBuffer().append("clear association remotely ").append((object) inObject).append("/").append((object) associate).ToString());
        try
        {
          IdentityData identityData1 = this.objectDataFactory.createIdentityData(inObject);
          IdentityData identityData2 = this.objectDataFactory.createIdentityData(associate);
          this.updateChangedObjects(this.connection.clearAssociation(NakedObjects.getCurrentSession(), this.getIdentifier().getName(), identityData1, identityData2));
        }
        catch (ConcurrencyException ex)
        {
          throw this.concurrencyException(ex);
        }
        catch (NakedObjectRuntimeException ex)
        {
          NakedObjectRuntimeException runtimeException1 = ex;
          ProxyOneToManyAssociation.LOG.error((object) new StringBuffer().append("remote exception: ").append(((Throwable) runtimeException1).getMessage()).ToString(), (Throwable) runtimeException1);
          NakedObjectRuntimeException runtimeException2 = runtimeException1;
          if (runtimeException2 != ex)
            throw runtimeException2;
          throw;
        }
      }
      else
      {
        if (ProxyOneToManyAssociation.LOG.isDebugEnabled())
          ProxyOneToManyAssociation.LOG.debug((object) new StringBuffer().append("clear association locally ").append((object) inObject).append("/").append((object) associate).ToString());
        base.removeAssociation(inObject, associate);
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ProxyOneToManyAssociation()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
