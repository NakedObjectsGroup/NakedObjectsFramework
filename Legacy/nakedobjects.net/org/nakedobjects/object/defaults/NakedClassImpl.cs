// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.NakedClassImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.reflect.@internal.about;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedClass;")]
  public class NakedClassImpl : NakedClass
  {
    private static readonly org.apache.log4j.Logger LOG;
    private readonly string className;
    private bool createPersistentInstances;
    private NakedObjectSpecification specification;

    public NakedClassImpl(string name)
    {
      this.createPersistentInstances = NakedObjects.getConfiguration().getBoolean("nakedclass.create-persistent", true);
      this.specification = NakedObjects.getSpecificationLoader().loadSpecification(name);
      this.className = name;
    }

    public virtual void aboutExplorationActionFind(InternalAbout about)
    {
      about.setDescription(new StringBuffer().append("Get a simple finder object to start searches within the ").append(this.getSingularName()).append(" instances").ToString());
      about.setName(new StringBuffer().append("Find ").append(this.getPluralName()).ToString());
      about.unusableOnCondition(((this.getObjectManager().hasInstances(this.forObjectType(), false) ? 1 : 0) ^ 1) != 0, "No instances available to find");
      Hint classHint = this.specification.getClassHint();
      if (classHint == null || !classHint.canAccess().isVetoed())
        return;
      about.invisible();
    }

    public virtual Consent useAllInstance()
    {
      Hint classHint = this.specification.getClassHint();
      return classHint != null && classHint.canAccess().isVetoed() ? (Consent) Veto.DEFAULT : AbstractConsent.create(this.getObjectManager().hasInstances(this.forObjectType(), false), "", "No instances available");
    }

    public virtual Consent useCreate()
    {
      Hint classHint = this.specification.getClassHint();
      if (classHint != null && classHint.canUse().isVetoed())
        return (Consent) Veto.DEFAULT;
      return this.specification.isAbstract() ? (Consent) new Veto("Cannot create an instance of an abstract class") : (Consent) Allow.DEFAULT;
    }

    public virtual void aboutExplorationActionNewInstance(InternalAbout about)
    {
      about.setDescription(new StringBuffer().append("Create a new ").append(this.getSingularName()).append(" instance").ToString());
      about.setName(new StringBuffer().append("New ").append(this.getSingularName()).ToString());
      Hint classHint = this.specification.getClassHint();
      if (classHint != null && classHint.canUse().isVetoed())
        about.invisible();
      if (!this.specification.isAbstract())
        return;
      about.unusable("Cannot create an instance of an abstract class");
    }

    public virtual void aboutExplorationActionNewTransientInstance(InternalAbout about)
    {
      about.setDescription(new StringBuffer().append("Create a new ").append(this.getSingularName()).append(" instance").ToString());
      about.setName(new StringBuffer().append("New ").append(this.getSingularName()).ToString());
      Hint classHint = this.specification.getClassHint();
      if (classHint != null && classHint.canUse().isVetoed() && !this.createPersistentInstances)
        about.invisible();
      if (!this.specification.isAbstract())
        return;
      about.unusable("Cannot create an instance of an abstract class");
    }

    public virtual NakedCollection allInstances() => (NakedCollection) this.getObjectManager().allInstances(this.forObjectType(), this.specification.isAbstract());

    public virtual NakedCollection explorationActionInstances() => this.allInstances();

    public virtual NakedObject explorationActionNewInstance() => this.newInstance();

    public virtual NakedObject explorationActionNewTransientInstance() => this.getObjectManager().createTransientInstance(this.forObjectType());

    public virtual NakedObjectSpecification forObjectType()
    {
      if (this.specification == null)
      {
        if (StringImpl.length(this.getName()) == 0)
          throw new NakedObjectRuntimeException();
        this.specification = NakedObjects.getSpecificationLoader().loadSpecification(this.getName());
      }
      return this.specification;
    }

    public virtual string getFullName() => this.forObjectType().getFullName();

    public virtual string getName() => this.className;

    public virtual string getShortName() => this.forObjectType().getShortName();

    private NakedObjectPersistor getObjectManager() => NakedObjects.getObjectPersistor();

    public virtual string getPluralName() => this.forObjectType().getPluralName();

    public virtual string getSingularName() => this.forObjectType().getSingularName();

    private NakedObject newInstance()
    {
      NakedObject @object = this.getObjectManager().createTransientInstance(this.forObjectType());
      if (this.createPersistentInstances)
      {
        try
        {
          this.getObjectManager().makePersistent(@object);
        }
        catch (NotPersistableException ex)
        {
          @object = NakedObjects.getObjectLoader().createAdapterForTransient((object) new Error(new StringBuffer().append("Failed to create instance of ").append((object) this).ToString(), (Throwable) ex));
          NakedClassImpl.LOG.error((object) new StringBuffer().append("failed to create instance of ").append((object) this).ToString(), (Throwable) ex);
        }
      }
      return @object;
    }

    public virtual string title() => this.getSingularName();

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append("NakedClass");
      stringBuffer.append(" [");
      stringBuffer.append(this.className);
      stringBuffer.append("]");
      stringBuffer.append(new StringBuffer().append("  ").append(StringImpl.toUpperCase(Long.toHexString((long) this.GetHashCode()))).ToString());
      return stringBuffer.ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static NakedClassImpl()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      NakedClassImpl nakedClassImpl = this;
      ObjectImpl.clone((object) nakedClassImpl);
      return ((object) nakedClassImpl).MemberwiseClone();
    }
  }
}
