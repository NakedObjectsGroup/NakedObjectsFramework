// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.AbstractOneToManyPeer
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
  [JavaInterfaces("1;org/nakedobjects/object/reflect/OneToManyPeer;")]
  public abstract class AbstractOneToManyPeer : OneToManyPeer
  {
    private readonly OneToManyPeer decorated;

    public AbstractOneToManyPeer(OneToManyPeer local) => this.decorated = local;

    public virtual void addAssociation(NakedObject inObject, NakedObject associate) => this.decorated.addAssociation(inObject, associate);

    public virtual NakedCollection getAssociations(NakedObject inObject) => this.decorated.getAssociations(inObject);

    public virtual void debugData(DebugString debugString) => this.decorated.debugData(debugString);

    public virtual string getDescription() => this.decorated.getDescription();

    public virtual object getExtension(Class cls) => this.decorated.getExtension(cls);

    public virtual Class[] getExtensions() => this.decorated.getExtensions();

    public virtual string getHelp() => this.decorated.getHelp();

    public virtual MemberIdentifier getIdentifier() => this.decorated.getIdentifier();

    public virtual string getName() => this.decorated.getName();

    public virtual NakedObjectSpecification getType() => this.decorated.getType();

    public virtual void initAssociation(NakedObject inObject, NakedObject associate) => this.decorated.initAssociation(inObject, associate);

    public virtual void initOneToManyAssociation(NakedObject inObject, NakedObject[] instances) => this.decorated.initOneToManyAssociation(inObject, instances);

    public virtual bool isAuthorised(Session session) => this.decorated.isAuthorised(session);

    public virtual bool isDerived() => this.decorated.isDerived();

    public virtual bool isEmpty(NakedObject inObject) => this.decorated.isEmpty(inObject);

    public virtual bool isHidden() => this.decorated.isHidden();

    public virtual bool isHiddenInTableViews() => this.decorated.isHiddenInTableViews();

    public virtual bool isMandatory() => this.decorated.isMandatory();

    public virtual Consent isAvailable(NakedReference target) => this.decorated.isAvailable(target);

    public virtual Consent isVisible(NakedReference target) => this.decorated.isVisible(target);

    public virtual void removeAllAssociations(NakedObject inObject) => this.decorated.removeAllAssociations(inObject);

    public virtual void removeAssociation(NakedObject inObject, NakedObject associate) => this.decorated.removeAssociation(inObject, associate);

    public virtual Consent isAddValid(NakedObject container, NakedObject element) => this.decorated.isAddValid(container, element);

    public virtual Consent isRemoveValid(NakedObject container, NakedObject element) => this.decorated.isRemoveValid(container, element);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractOneToManyPeer abstractOneToManyPeer = this;
      ObjectImpl.clone((object) abstractOneToManyPeer);
      return ((object) abstractOneToManyPeer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
