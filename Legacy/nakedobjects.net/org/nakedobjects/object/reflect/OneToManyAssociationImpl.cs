// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.OneToManyAssociationImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/OneToManyAssociation;")]
  public class OneToManyAssociationImpl : AbstractNakedObjectField, OneToManyAssociation
  {
    private readonly OneToManyPeer reflectiveAdapter;

    public OneToManyAssociationImpl(
      string className,
      string methodName,
      NakedObjectSpecification type,
      OneToManyPeer association)
      : base(methodName, type)
    {
      this.reflectiveAdapter = association;
    }

    public virtual void removeElement(NakedObject inObject, NakedObject associate)
    {
      if (associate == null)
        throw new IllegalArgumentException("element should not be null");
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.removeAssociation(inObject, associate);
    }

    public virtual void clearCollection(NakedObject inObject)
    {
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.removeAllAssociations(inObject);
    }

    public override Naked get(NakedObject fromObject) => (Naked) this.reflectiveAdapter.getAssociations(fromObject);

    public override object getExtension(Class cls) => this.reflectiveAdapter.getExtension(cls);

    public override Class[] getExtensions() => this.reflectiveAdapter.getExtensions();

    private bool readWrite() => ((this.reflectiveAdapter.isDerived() ? 1 : 0) ^ 1) != 0;

    public virtual void initElement(NakedObject inObject, NakedObject associate)
    {
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.initAssociation(inObject, associate);
    }

    public virtual void initCollection(NakedObject inObject, NakedObject[] instances)
    {
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.initOneToManyAssociation(inObject, instances);
    }

    public override bool isCollection() => true;

    public override bool isDerived() => this.reflectiveAdapter.isDerived();

    public override void debugData(DebugString debugString) => this.reflectiveAdapter.debugData(debugString);

    public override string getDescription() => "";

    public override string getHelp() => this.reflectiveAdapter.getHelp();

    public override string getName() => this.reflectiveAdapter.getName() ?? this.defaultLabel;

    public override bool isEmpty(NakedObject inObject) => this.reflectiveAdapter.isEmpty(inObject);

    public override bool isHidden() => this.reflectiveAdapter.isHidden();

    public override bool isHiddenInTableViews() => this.reflectiveAdapter.isHiddenInTableViews();

    public virtual bool isPart() => true;

    public virtual void addElement(NakedObject inObject, NakedObject associate)
    {
      if (associate == null)
        throw new IllegalArgumentException("Can't use null to add an item to a collection");
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.addAssociation(inObject, associate);
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append(base.ToString());
      toString.append(",");
      toString.append("derived", this.isDerived());
      toString.append("type", this.getSpecification() != null ? this.getSpecification().getShortName() : "unknown");
      return toString.ToString();
    }

    public virtual Consent validToRemove(NakedObject container, NakedObject element) => this.reflectiveAdapter.isRemoveValid(container, element);

    public virtual Consent validToAdd(NakedObject container, NakedObject element) => this.reflectiveAdapter.isAddValid(container, element);

    public override Consent isAvailable(NakedReference target) => this.reflectiveAdapter.isAvailable(target);

    public override Consent isVisible(NakedReference target) => this.reflectiveAdapter.isVisible(target);

    public override bool isAuthorised() => this.reflectiveAdapter.isAuthorised(NakedObjects.getCurrentSession());
  }
}
