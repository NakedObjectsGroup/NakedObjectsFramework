// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.OneToManyField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.util;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/FieldContent;")]
  public class OneToManyField : CollectionContent, FieldContent
  {
    private readonly NakedCollection collection;
    private readonly ObjectField field;

    public override Consent canDrop(Content sourceContent)
    {
      if (!(sourceContent.getNaked() is NakedObject))
        return (Consent) Veto.DEFAULT;
      NakedObject naked1 = (NakedObject) sourceContent.getNaked();
      NakedObject parent = this.field.getParent();
      InternalCollection naked2 = (InternalCollection) this.getNaked();
      if (!naked1.getSpecification().isOfType(naked2.getElementSpecification()))
        return (Consent) new Veto(new StringBuffer().append("Only objects of type ").append(naked2.getElementSpecification().getSingularName()).append(" are allowed in this collection").ToString());
      if (parent.getResolveState().isPersistent() && naked1.getResolveState().isTransient())
        return (Consent) new Veto("Can't set field in persistent object with reference to non-persistent object");
      if (naked1 is Aggregated)
      {
        Aggregated aggregated = (Aggregated) naked1;
        if (aggregated.isAggregated() && aggregated.parent() != parent)
          return (Consent) new Veto(new StringBuffer().append("Object is already associated with another object: ").append((object) aggregated.parent()).ToString());
      }
      return this.getOneToManyAssociation().validToAdd(parent, naked1);
    }

    public override Naked drop(Content sourceContent)
    {
      NakedObject naked = (NakedObject) sourceContent.getNaked();
      NakedObject parent = this.field.getParent();
      if (this.canDrop(sourceContent).isAllowed())
        parent.setAssociation((NakedObjectField) this.getOneToManyAssociation(), naked);
      return (Naked) null;
    }

    public OneToManyField(
      NakedObject parent,
      InternalCollection @object,
      OneToManyAssociation association)
    {
      this.field = new ObjectField(parent, (NakedObjectField) association);
      this.collection = (NakedCollection) @object;
    }

    public override NakedObject[] elements()
    {
      int num = this.getCollection().size();
      int length = num;
      NakedObject[] nakedObjectArray = length >= 0 ? new NakedObject[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < num; ++index)
        nakedObjectArray[index] = this.getCollection().elementAt(index);
      return nakedObjectArray;
    }

    public virtual Consent canClear() => (Consent) Veto.DEFAULT;

    public virtual Consent canSet(NakedObject dragSource) => (Consent) Veto.DEFAULT;

    public virtual void clear() => throw new NakedObjectRuntimeException("Invalid call");

    public override void debugDetails(DebugString debug)
    {
      this.field.debugDetails(debug);
      debug.appendln("collection", (object) this.collection);
      base.debugDetails(debug);
    }

    public override NakedCollection getCollection() => this.collection;

    public virtual string getFieldName() => this.field.getName();

    public virtual NakedObjectField getField() => this.field.getFieldReflector();

    public override string getIconName() => (string) null;

    public override Naked getNaked() => (Naked) this.collection;

    public virtual OneToManyAssociation getOneToManyAssociation() => (OneToManyAssociation) this.field.getFieldReflector();

    public virtual NakedObject getParent() => this.field.getParent();

    public override NakedObjectSpecification getSpecification() => this.field.getSpecification();

    public override bool isCollection() => true;

    public override bool isDerived() => this.getOneToManyAssociation().isDerived();

    public override bool isTransient() => false;

    public virtual bool isMandatory() => this.getOneToManyAssociation().isMandatory();

    public virtual void setObject(NakedObject @object) => throw new NakedObjectRuntimeException("Invalid call");

    [JavaFlags(17)]
    public override sealed string title() => this.field.getName();

    public override string ToString() => new StringBuffer().append((object) this.collection).append("/").append((object) this.field.getFieldReflector()).ToString();

    public override string windowTitle() => new StringBuffer().append(this.title()).append(" for ").append(this.field.getParent().titleString()).ToString();

    public override void contentMenuOptions(UserActionSet options)
    {
      base.contentMenuOptions(options);
      OptionFactory.addClassMenuOptions(this.getOneToManyAssociation().getSpecification(), options);
    }

    public override Image getIconPicture(int iconHeight)
    {
      NakedObjectSpecification specification = this.getOneToManyAssociation().getSpecification();
      return ImageFactory.getInstance().loadObjectIcon(specification, "", iconHeight);
    }

    public override string getId() => this.getOneToManyAssociation().getId();

    public override string getDescription() => this.getOneToManyAssociation().getId();

    public override string getHelp() => this.getOneToManyAssociation().getHelp();
  }
}
