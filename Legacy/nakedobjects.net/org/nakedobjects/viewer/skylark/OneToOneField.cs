// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.OneToOneField
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
  public class OneToOneField : ObjectContent, FieldContent
  {
    private static readonly UserAction CLEAR_ASSOCIATION;
    private readonly ObjectField field;
    private readonly NakedObject @object;

    public OneToOneField(NakedObject parent, NakedObject @object, OneToOneAssociation association)
    {
      this.field = new ObjectField(parent, (NakedObjectField) association);
      this.@object = @object;
    }

    public virtual TypedNakedCollection proposedOptions() => this.getOneToOneAssociation().proposedOptions((NakedReference) this.getParent());

    public override Consent canClear()
    {
      NakedObject parent = this.getParent();
      OneToOneAssociation toOneAssociation = this.getOneToOneAssociation();
      NakedObject nakedObject = this.getObject();
      Consent consent = parent.isValid(toOneAssociation, nakedObject);
      return consent.isAllowed() ? (Consent) new Allow(new StringBuffer().append("Clear the association to this object from '").append(parent.titleString()).append("'").ToString()) : (Consent) new Veto(consent.getReason());
    }

    public override Consent canSet(NakedObject @object)
    {
      if (@object.getObject() is NakedClass)
        return (Consent) new Allow();
      NakedObjectSpecification specification = this.getOneToOneAssociation().getSpecification();
      if (!@object.getSpecification().isOfType(specification))
        return (Consent) new Veto(new StringBuffer().append("Can only drop objects of type ").append(specification.getSingularName()).ToString());
      if (this.getParent().getResolveState().isPersistent() && @object.getResolveState().isTransient())
        return (Consent) new Veto("Can't drop a non-persistent into this persistent object");
      if (@object is Aggregated)
      {
        Aggregated aggregated = (Aggregated) @object;
        if (aggregated.isAggregated() && aggregated.parent() != this.getParent())
          return (Consent) new Veto(new StringBuffer().append("Object is already associated with another object: ").append((object) aggregated.parent()).ToString());
      }
      return this.getParent().isValid(this.getOneToOneAssociation(), @object);
    }

    public override void clear() => this.getParent().clearAssociation((NakedObjectField) this.getOneToOneAssociation(), this.@object);

    public override void debugDetails(DebugString debug)
    {
      this.field.debugDetails(debug);
      debug.appendln("object", (object) this.@object);
    }

    public virtual string getFieldName() => this.field.getName();

    public virtual NakedObjectField getField() => this.field.getFieldReflector();

    public virtual Consent isEditable() => this.getField().isAvailable((NakedReference) this.getParent());

    public override Naked getNaked() => (Naked) this.@object;

    public override NakedObject getObject() => this.@object;

    private OneToOneAssociation getOneToOneAssociation() => (OneToOneAssociation) this.getField();

    public virtual NakedObject getParent() => this.field.getParent();

    public override NakedObjectSpecification getSpecification() => this.getOneToOneAssociation().getSpecification();

    public override bool isDerived() => this.getOneToOneAssociation().isDerived();

    public virtual bool isLookup() => this.getOneToOneAssociation().getSpecification().isLookup();

    public virtual bool isMandatory() => this.getOneToOneAssociation().isMandatory();

    public override bool isPersistable() => this.getObject() != null && base.isPersistable();

    public override bool isObject() => true;

    public override bool isTransient() => this.@object != null && this.@object.getResolveState().isTransient();

    public override void contentMenuOptions(UserActionSet options)
    {
      base.contentMenuOptions(options);
      if (this.getObject() == null)
        return;
      options.add(OneToOneField.CLEAR_ASSOCIATION);
    }

    public override void setObject(NakedObject @object) => this.getParent().setAssociation((NakedObjectField) this.getOneToOneAssociation(), @object);

    public override string title() => this.@object == null ? "" : this.@object.titleString();

    public override string ToString() => new StringBuffer().append((object) this.getObject()).append("/").append((object) this.getField()).ToString();

    public override string windowTitle() => new StringBuffer().append(this.field.getName()).append(" for ").append(this.field.getParent().titleString()).ToString();

    public override string getId() => this.getOneToOneAssociation().getName();

    public override string getDescription() => this.getOneToOneAssociation().getDescription();

    public override string getHelp() => this.getOneToOneAssociation().getHelp();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static OneToOneField()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
