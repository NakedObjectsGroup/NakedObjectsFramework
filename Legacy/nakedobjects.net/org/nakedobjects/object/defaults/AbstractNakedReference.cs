// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.AbstractNakedReference
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.utility;
using System;
using System.ComponentModel;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedReference;")]
  public abstract class AbstractNakedReference : NakedReference
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly DateFormat DATE_TIME;
    private Oid oid;
    [JavaFlags(130)]
    [NonSerialized]
    private ResolveState resolveState;
    private NakedObjectSpecification specification;
    private org.nakedobjects.@object.Version version;
    private string defaultTitle;

    public AbstractNakedReference() => this.resolveState = ResolveState.NEW;

    public virtual void changeState(ResolveState newState)
    {
      if (!this.resolveState.isValidToChangeTo(newState))
        throw new NakedObjectAssertException(new StringBuffer().append("can't change from ").append(this.resolveState.name()).append(" to ").append(newState.name()).append(": ").append((object) this).ToString());
      if (AbstractNakedReference.LOG.isDebugEnabled())
        AbstractNakedReference.LOG.debug((object) new StringBuffer().append("recreate - change state ").append((object) this).append(" to ").append((object) newState).ToString());
      this.resolveState = newState;
    }

    public virtual void checkLock(org.nakedobjects.@object.Version version)
    {
      if (this.version != null && this.version.different(version))
        throw new ConcurrencyException(new StringBuffer().append(version.getUser()).append(" changed ").append(this.titleString()).append(" at ").append(AbstractNakedReference.DATE_TIME.format(version.getTime())).append(" (").append((object) this.version).append("~").append((object) version).append(")").ToString(), this.oid);
    }

    public virtual void debugClearResolved() => this.resolveState = ResolveState.GHOST;

    public virtual string getIconName() => (string) null;

    public virtual Oid getOid() => this.oid;

    public virtual ResolveState getResolveState() => this.resolveState;

    public virtual NakedObjectSpecification getSpecification()
    {
      if (this.specification == null)
      {
        this.specification = NakedObjects.getSpecificationLoader().loadSpecification(ObjectImpl.getClass(this.getObject()));
        this.defaultTitle = new StringBuffer().append("A ").append(StringImpl.toLowerCase(this.specification.getSingularName())).ToString();
      }
      return this.specification;
    }

    public virtual org.nakedobjects.@object.Version getVersion() => this.version;

    public virtual string getDefaultTitle() => this.defaultTitle;

    [JavaFlags(4)]
    public virtual bool isResolved() => this.resolveState == ResolveState.RESOLVED;

    public virtual Persistable persistable() => this.getSpecification().persistable();

    public virtual void persistedAs(Oid oid)
    {
      if (AbstractNakedReference.LOG.isDebugEnabled())
        AbstractNakedReference.LOG.debug((object) new StringBuffer().append("set OID ").append((object) oid).append(" ").append((object) this).ToString());
      Assert.assertTrue("Cannot make a non-transient object persistent", (object) this, this.getResolveState().isTransient());
      Assert.assertTrue("Oid can't be set again", (object) this, this.getOid() == null || this.getOid().isNull() || this.getOid() == oid);
      this.setOid(oid);
      this.setResolveState(ResolveState.RESOLVED);
    }

    public virtual void setOid(Oid oid) => this.oid = oid;

    public virtual void setOptimisticLock(org.nakedobjects.@object.Version version)
    {
      if (!this.shouldSetVersion(version))
        return;
      this.version = version;
    }

    private bool shouldSetVersion(org.nakedobjects.@object.Version version) => this.version == null || version == null || version.different(this.version);

    [JavaFlags(4)]
    public virtual void setResolveState(ResolveState resolveState) => this.resolveState = resolveState;

    [JavaFlags(4)]
    public virtual void toString(org.nakedobjects.utility.ToString str)
    {
      str.append(this.resolveState.code());
      Oid oid = this.getOid();
      if (oid != null)
      {
        str.append(":");
        str.append(StringImpl.toUpperCase(oid.ToString()));
      }
      else
        str.append(":-");
      str.setAddComma();
      str.append("specification", this.specification != null ? this.specification.getShortName() : "undetermined");
      str.append("version", (object) this.version);
    }

    public virtual Naked execute(Action action, Naked[] parameters)
    {
      if (AbstractNakedReference.LOG.isDebugEnabled())
        AbstractNakedReference.LOG.debug((object) new StringBuffer().append("execute ").append(action.getId()).append(" in ").append((object) this).ToString());
      return action.execute((NakedReference) this, parameters);
    }

    public virtual Consent isVisible(Action action) => action.isVisible((NakedReference) this);

    public virtual ActionParameterSet getParameters(Action action) => action.getParameterSet((NakedReference) this);

    public virtual Consent isValid(Action action, Naked[] parameters) => action.isParameterSetValid((NakedReference) this, parameters);

    public virtual Consent isAvailable(Action action) => action.isAvailable((NakedReference) this);

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static AbstractNakedReference()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractNakedReference abstractNakedReference = this;
      ObjectImpl.clone((object) abstractNakedReference);
      return ((object) abstractNakedReference).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void destroyed();

    public abstract object getObject();

    public abstract string titleString();
  }
}
