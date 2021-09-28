// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ProxyOneToOneAssociation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using System.ComponentModel;

namespace org.nakedobjects.distribution
{
  public sealed class ProxyOneToOneAssociation : AbstractOneToOnePeer
  {
    private static readonly Logger LOG;
    private readonly Distribution connection;
    private readonly bool fullProxy;
    private readonly ObjectEncoder encoder;

    public ProxyOneToOneAssociation(
      OneToOnePeer local,
      Distribution connection,
      ObjectEncoder objectDataFactory)
      : base(local)
    {
      this.fullProxy = false;
      this.connection = connection;
      this.encoder = objectDataFactory;
    }

    public override void clearAssociation(NakedObject inObject, NakedObject associate)
    {
      if (this.isPersistent(inObject))
      {
        if (ProxyOneToOneAssociation.LOG.isDebugEnabled())
          ProxyOneToOneAssociation.LOG.debug((object) new StringBuffer().append("clear association remotely ").append((object) inObject).append("/").append((object) associate).ToString());
        IdentityData identityData1 = this.encoder.createIdentityData(inObject);
        IdentityData identityData2 = this.encoder.createIdentityData(associate);
        ObjectData[] updates;
        try
        {
          updates = this.connection.clearAssociation(NakedObjects.getCurrentSession(), this.getIdentifier().getName(), identityData1, identityData2);
        }
        catch (ConcurrencyException ex)
        {
          throw this.concurrencyException(ex);
        }
        this.updateChangedObjects(updates);
      }
      else
      {
        if (ProxyOneToOneAssociation.LOG.isDebugEnabled())
          ProxyOneToOneAssociation.LOG.debug((object) new StringBuffer().append("clear association locally ").append((object) inObject).append("/").append((object) associate).ToString());
        base.clearAssociation(inObject, associate);
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

    public override Naked getAssociation(NakedObject inObject) => base.getAssociation(inObject);

    private bool isPersistent(NakedObject inObject) => inObject.getOid() != null;

    public override void setAssociation(NakedObject inObject, NakedObject associate)
    {
      if (this.isPersistent(inObject))
      {
        if (ProxyOneToOneAssociation.LOG.isDebugEnabled())
          ProxyOneToOneAssociation.LOG.debug((object) new StringBuffer().append("set association remotely ").append((object) this.getIdentifier()).append(" in ").append((object) inObject).append(" with ").append((object) associate).ToString());
        IdentityData identityData1 = this.encoder.createIdentityData(inObject);
        IdentityData identityData2 = this.encoder.createIdentityData(associate);
        ObjectData[] updates;
        try
        {
          updates = this.connection.setAssociation(NakedObjects.getCurrentSession(), this.getIdentifier().getName(), identityData1, identityData2);
        }
        catch (ConcurrencyException ex)
        {
          throw this.concurrencyException(ex);
        }
        this.updateChangedObjects(updates);
      }
      else
      {
        if (ProxyOneToOneAssociation.LOG.isDebugEnabled())
          ProxyOneToOneAssociation.LOG.debug((object) new StringBuffer().append("set association locally ").append((object) this.getIdentifier()).append(" in ").append((object) inObject).append(" with ").append((object) associate).ToString());
        base.setAssociation(inObject, associate);
      }
    }

    public override void setValue(NakedObject inObject, object value)
    {
      if (this.isPersistent(inObject))
      {
        if (ProxyOneToOneAssociation.LOG.isDebugEnabled())
          ProxyOneToOneAssociation.LOG.debug((object) new StringBuffer().append("set value remotely ").append((object) this.getIdentifier()).append(" in ").append((object) inObject).append(" with ").append(value).ToString());
        IdentityData identityData = this.encoder.createIdentityData(inObject);
        ObjectData[] updates;
        try
        {
          updates = this.connection.setValue(NakedObjects.getCurrentSession(), this.getIdentifier().getName(), identityData, value);
        }
        catch (ConcurrencyException ex)
        {
          throw this.concurrencyException(ex);
        }
        this.updateChangedObjects(updates);
      }
      else
      {
        if (ProxyOneToOneAssociation.LOG.isDebugEnabled())
          ProxyOneToOneAssociation.LOG.debug((object) new StringBuffer().append("set value locally ").append((object) this.getIdentifier()).append(" in ").append((object) inObject).append(" with ").append(value).ToString());
        base.setValue(inObject, value);
      }
    }

    private void updateChangedObjects(ObjectData[] updates)
    {
      for (int index = 0; index < updates.Length; ++index)
      {
        if (ProxyOneToOneAssociation.LOG.isDebugEnabled())
          ProxyOneToOneAssociation.LOG.debug((object) new StringBuffer().append("update ").append(DistributionLogger.dump((Data) updates[index])).ToString());
        ObjectDecoder.restore((Data) updates[index]);
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ProxyOneToOneAssociation()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
