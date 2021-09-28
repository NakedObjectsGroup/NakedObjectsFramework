// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.OneToManyFieldElement
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/FieldContent;")]
  public class OneToManyFieldElement : ObjectContent, FieldContent
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly UserAction REMOVE_ASSOCIATION;
    private readonly NakedObject element;
    private readonly ObjectField field;

    public OneToManyFieldElement(
      NakedObject parent,
      NakedObject element,
      OneToManyAssociation association)
    {
      this.field = new ObjectField(parent, (NakedObjectField) association);
      this.element = element;
    }

    public override Consent canClear()
    {
      NakedObject parent = this.getParent();
      OneToManyAssociation toManyAssociation = this.getOneToManyAssociation();
      NakedObject element = this.getObject();
      Consent remove = toManyAssociation.validToRemove(parent, element);
      return remove.isAllowed() ? (Consent) new Allow(new StringBuffer().append("Clear the association to this object from '").append(parent.titleString()).append("'").ToString()) : (Consent) new Veto(remove.getReason());
    }

    public override Consent canSet(NakedObject dragSource) => (Consent) Veto.DEFAULT;

    public override void clear()
    {
      NakedObject parent = this.getParent();
      OneToManyAssociation toManyAssociation = this.getOneToManyAssociation();
      if (OneToManyFieldElement.LOG.isDebugEnabled())
        OneToManyFieldElement.LOG.debug((object) new StringBuffer().append("remove ").append((object) this.element).append(" from ").append((object) parent).ToString());
      toManyAssociation.removeElement(parent, this.element);
    }

    public override void debugDetails(DebugString debug)
    {
      this.field.debugDetails(debug);
      debug.appendln("element", (object) this.element);
    }

    public virtual string getFieldName() => this.field.getName();

    public virtual NakedObjectField getField() => this.field.getFieldReflector();

    public override Naked getNaked() => (Naked) this.element;

    public override NakedObject getObject() => this.element;

    private OneToManyAssociation getOneToManyAssociation() => (OneToManyAssociation) this.field.getFieldReflector();

    public virtual NakedObject getParent() => this.field.getParent();

    public override NakedObjectSpecification getSpecification() => this.field.getSpecification();

    public virtual bool isMandatory() => false;

    public override bool isObject() => true;

    public override bool isTransient() => false;

    public override void contentMenuOptions(UserActionSet options)
    {
      base.contentMenuOptions(options);
      options.add(OneToManyFieldElement.REMOVE_ASSOCIATION);
    }

    public override void setObject(NakedObject @object)
    {
    }

    public override string title() => this.element.titleString();

    public override string ToString() => new StringBuffer().append((object) this.getObject()).append("/").append((object) this.field.getFieldReflector()).ToString();

    public override string windowTitle() => new StringBuffer().append(this.field.getName()).append(" element").append(" for ").append(this.field.getParent().titleString()).ToString();

    public override string getId() => this.getOneToManyAssociation().getName();

    public override string getDescription() => this.getOneToManyAssociation().getDescription();

    public override string getHelp() => this.getOneToManyAssociation().getHelp();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static OneToManyFieldElement()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
