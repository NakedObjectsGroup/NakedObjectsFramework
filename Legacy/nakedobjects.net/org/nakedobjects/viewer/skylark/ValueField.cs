// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ValueField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/FieldContent;")]
  public class ValueField : ValueContent, FieldContent
  {
    private readonly ObjectField field;
    private NakedValue @object;

    public ValueField(NakedObject parent, NakedValue @object, OneToOneAssociation association)
    {
      this.field = new ObjectField(parent, (NakedObjectField) association);
      this.@object = @object;
    }

    public override Consent canDrop(Content sourceContent) => (Consent) Veto.DEFAULT;

    private void checkValidEntry(string entryText)
    {
      NakedValue valueInstance = NakedObjects.getObjectLoader().createValueInstance(this.getSpecification());
      valueInstance.parseTextEntry(entryText);
      Consent consent = this.getParent().isValid((OneToOneAssociation) this.getField(), valueInstance);
      if (consent.isVetoed())
        throw new InvalidEntryException(consent.getReason());
    }

    public override void clear() => this.@object.clear();

    public override void debugDetails(DebugString debug)
    {
      this.field.debugDetails(debug);
      debug.appendln("object", (object) this.@object);
    }

    public override Naked drop(Content sourceContent) => (Naked) null;

    public override void entryComplete() => this.getParent().setValue(this.getOneToOneAssociation(), this.@object.getObject());

    public override string getDescription() => this.field.getDescription();

    public override string getHelp() => this.field.getHelp();

    public virtual string getFieldName() => this.field.getName();

    public virtual NakedObjectField getField() => this.field.getFieldReflector();

    public override string getIconName() => this.@object.getIconName();

    public override Naked getNaked() => (Naked) this.@object;

    public override string getId() => this.field.getName();

    public override NakedValue getObject() => this.@object;

    private OneToOneAssociation getOneToOneAssociation() => (OneToOneAssociation) this.getField();

    public virtual NakedObject getParent() => this.field.getParent();

    public override NakedObjectSpecification getSpecification() => this.getOneToOneAssociation().getSpecification();

    public override bool isDerived() => this.getOneToOneAssociation().isDerived();

    public override Consent isEditable() => this.getOneToOneAssociation().isAvailable((NakedReference) this.getParent());

    public override bool isEmpty() => this.getParent().isEmpty(this.getField());

    public virtual bool isMandatory() => this.getOneToOneAssociation().isMandatory();

    public override void parseTextEntry(string entryText)
    {
      this.checkValidEntry(entryText);
      this.saveEntry(entryText);
    }

    private void saveEntry(string entryText)
    {
      if (this.@object == null)
        this.@object = NakedObjects.getObjectLoader().createValueInstance(this.getSpecification());
      this.@object.parseTextEntry(entryText);
    }

    public override string title() => this.field.getName();

    public override string ToString() => new StringBuffer().append(this.@object != null ? this.@object.titleString() : "null").append("/").append((object) this.getField()).ToString();

    public override string windowTitle() => this.title();
  }
}
