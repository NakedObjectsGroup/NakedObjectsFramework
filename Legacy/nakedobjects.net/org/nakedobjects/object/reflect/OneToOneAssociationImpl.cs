// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.OneToOneAssociationImpl
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
  [JavaInterfaces("1;org/nakedobjects/object/OneToOneAssociation;")]
  public class OneToOneAssociationImpl : AbstractNakedObjectField, OneToOneAssociation
  {
    private readonly OneToOnePeer reflectiveAdapter;

    public OneToOneAssociationImpl(
      string className,
      string fieldName,
      NakedObjectSpecification specification,
      OneToOnePeer association)
      : base(fieldName, specification)
    {
      this.reflectiveAdapter = association;
    }

    public virtual TypedNakedCollection proposedOptions(NakedReference target) => this.reflectiveAdapter.proposedOptions(target);

    public virtual void clearAssociation(NakedObject inObject, NakedObject associate)
    {
      if (associate == null)
        throw new NullPointerException("Must specify the item to remove/dissociate");
      this.reflectiveAdapter.clearAssociation(inObject, associate);
    }

    public virtual void clearValue(NakedObject inObject) => ((NakedValue) this.get(inObject))?.clear();

    public override Naked get(NakedObject fromObject) => this.reflectiveAdapter.getAssociation(fromObject);

    public override void debugData(DebugString debugString) => this.reflectiveAdapter.debugData(debugString);

    public override object getExtension(Class cls) => this.reflectiveAdapter.getExtension(cls);

    public override Class[] getExtensions() => this.reflectiveAdapter.getExtensions();

    public override string getHelp() => this.reflectiveAdapter.getHelp();

    public override string getName() => this.reflectiveAdapter.getName() ?? this.defaultLabel;

    public virtual void initAssociation(NakedObject inObject, NakedObject associate)
    {
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.initAssociation(inObject, associate);
    }

    private bool readWrite() => ((this.reflectiveAdapter.isDerived() ? 1 : 0) ^ 1) != 0;

    public virtual void initValue(NakedObject inObject, object associate)
    {
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.initValue(inObject, associate);
    }

    public override bool isDerived() => this.reflectiveAdapter.isDerived();

    public override bool isHidden() => this.reflectiveAdapter.isHidden();

    public override bool isHiddenInTableViews() => this.reflectiveAdapter.isHiddenInTableViews();

    public override bool isMandatory() => this.reflectiveAdapter.isMandatory();

    public override bool isEmpty(NakedObject inObject) => this.reflectiveAdapter.isEmpty(inObject);

    public virtual void setAssociation(NakedObject inObject, NakedObject associate)
    {
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.setAssociation(inObject, associate);
    }

    public virtual void setValue(NakedObject inObject, object value)
    {
      if (!this.readWrite())
        return;
      this.reflectiveAdapter.setValue(inObject, value);
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("type", !this.isValue() ? "OBJECT" : "VALUE");
      toString.append(",");
      toString.append(base.ToString());
      toString.append("derived", this.isDerived());
      toString.append("type", this.getSpecification().getShortName());
      return toString.ToString();
    }

    public override bool isObject() => this.reflectiveAdapter.isObject();

    public override bool isValue() => ((this.reflectiveAdapter.isObject() ? 1 : 0) ^ 1) != 0;

    public virtual Consent isValueValid(NakedObject inObject, NakedValue value) => this.reflectiveAdapter.isValueValid(inObject, value);

    public virtual Consent isAssociationValid(NakedObject inObject, NakedObject value) => this.reflectiveAdapter.isAssociationValid(inObject, value);

    public override Consent isAvailable(NakedReference inObject) => this.reflectiveAdapter.isAvailable(inObject);

    public override string getDescription() => this.reflectiveAdapter.getDescription();

    public override Consent isVisible(NakedReference target) => this.reflectiveAdapter.isVisible(target);

    public override bool isAuthorised() => this.reflectiveAdapter.isAuthorised(NakedObjects.getCurrentSession());
  }
}
