// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.AbstractOneToOnePeer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/OneToOnePeer;")]
  public abstract class AbstractOneToOnePeer : OneToOnePeer
  {
    private readonly OneToOnePeer decorated;

    public AbstractOneToOnePeer(OneToOnePeer local) => this.decorated = local;

    public virtual void clearAssociation(NakedObject inObject, NakedObject associate) => this.decorated.clearAssociation(inObject, associate);

    public virtual Naked getAssociation(NakedObject inObject) => this.decorated.getAssociation(inObject);

    public virtual void debugData(DebugString debugString) => this.decorated.debugData(debugString);

    public virtual string getDescription() => this.decorated.getDescription();

    public virtual object getExtension(Class cls) => this.decorated.getExtension(cls);

    public virtual Class[] getExtensions() => this.decorated.getExtensions();

    public virtual MemberIdentifier getIdentifier() => this.decorated.getIdentifier();

    public virtual string getHelp() => this.decorated.getHelp();

    public virtual string getName() => this.decorated.getName();

    public virtual NakedObjectSpecification getType() => this.decorated.getType();

    public virtual void initAssociation(NakedObject inObject, NakedObject associate) => this.decorated.initAssociation(inObject, associate);

    public virtual void initValue(NakedObject inObject, object associate) => this.decorated.initValue(inObject, associate);

    public virtual bool isAuthorised(Session session) => this.decorated.isAuthorised(session);

    public virtual bool isDerived() => this.decorated.isDerived();

    public virtual bool isEmpty(NakedObject inObject) => this.decorated.isEmpty(inObject);

    public virtual bool isHidden() => this.decorated.isHidden();

    public virtual bool isHiddenInTableViews() => this.decorated.isHiddenInTableViews();

    public virtual bool isMandatory() => this.decorated.isMandatory();

    public virtual bool isObject() => this.decorated.isObject();

    public virtual Consent isAvailable(NakedReference target) => this.decorated.isAvailable(target);

    public virtual Consent isVisible(NakedReference target) => this.decorated.isVisible(target);

    public virtual void setAssociation(NakedObject inObject, NakedObject associate) => this.decorated.setAssociation(inObject, associate);

    public virtual void setValue(NakedObject inObject, object associate) => this.decorated.setValue(inObject, associate);

    public virtual Consent isAssociationValid(NakedObject inObject, NakedObject value) => this.decorated.isAssociationValid(inObject, value);

    public virtual Consent isValueValid(NakedObject inObject, NakedValue value) => this.decorated.isValueValid(inObject, value);

    public virtual TypedNakedCollection proposedOptions(NakedReference target) => this.decorated.proposedOptions(target);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractOneToOnePeer abstractOneToOnePeer = this;
      ObjectImpl.clone((object) abstractOneToOnePeer);
      return ((object) abstractOneToOnePeer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
