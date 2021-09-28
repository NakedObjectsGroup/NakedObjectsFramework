// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.SetFieldAgent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.field
{
  public class SetFieldAgent : AbstractFieldAgent
  {
    public SetFieldAgent(string name, NakedObject @object, NakedObjectField field)
      : base(name, @object, field)
    {
    }

    public override string getName() => "Set Field";

    public override string getPrompt() => new StringBuffer().append(this.name).append(" (set)").ToString();

    [JavaFlags(0)]
    public virtual void setReferenceField(Context context, View view, NakedObject associate)
    {
      OneToOneAssociation field = (OneToOneAssociation) this.field;
      Consent consent = field.isAssociationValid(this.@object, associate);
      if (consent.isVetoed())
      {
        view.error(new StringBuffer().append("Can't set reference: ").append(consent.getReason()).ToString());
      }
      else
      {
        field.setAssociation(this.@object, associate);
        context.removeAgent();
      }
    }

    [JavaFlags(0)]
    public virtual void setValueField(Context context, View view, string entryText)
    {
      OneToOneAssociation field = (OneToOneAssociation) this.field;
      NakedValue valueInstance = NakedObjects.getObjectLoader().createValueInstance(field.getSpecification());
      valueInstance.parseTextEntry(entryText);
      Consent consent = field.isValueValid(this.@object, valueInstance);
      if (consent.isVetoed())
      {
        view.error(new StringBuffer().append("Can't set field: ").append(consent.getReason()).ToString());
      }
      else
      {
        sbyte[] data = valueInstance.asEncodedString();
        ((NakedValue) field.get(this.@object)).restoreFromEncodedString(data);
        field.setValue(this.@object, valueInstance.getObject());
        context.removeAgent();
      }
    }

    [JavaFlags(0)]
    public virtual void setField(Context context, View view, string entry)
    {
      if (this.field.isValue())
        this.setValueField(context, view, entry);
      else if (this.isLookup() && StringImpl.indexOf(entry, 46) == -1)
      {
        this.setOption(context, view, entry);
      }
      else
      {
        NakedObject associate = ((ObjectAgent) context.findAgent(entry)).getObject();
        this.setReferenceField(context, view, associate);
      }
    }

    public override bool isValueEntry() => this.field.isValue();

    public virtual bool isLookup() => this.field.getSpecification().isLookup();

    public virtual void listOptions(View view)
    {
      Enumeration enumeration = NakedObjects.getObjectPersistor().allInstances(this.field.getSpecification(), false).elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject = (NakedObject) enumeration.nextElement();
        view.display(nakedObject.titleString());
      }
    }

    public virtual void setOption(Context context, View view, string entry)
    {
      Matcher matcher = (Matcher) new SetFieldAgent.ElementMatcher(NakedObjects.getObjectPersistor().allInstances(this.field.getSpecification(), false).elements());
      this.setReferenceField(context, view, (NakedObject) ((Naked) MatchAlgorithm.match(entry, matcher) ?? throw new IllegalDispatchException(new StringBuffer().append("No object with title matching ").append(entry).ToString())));
    }

    [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
    [JavaFlags(58)]
    private sealed class ElementMatcher : Matcher
    {
      private readonly Enumeration enumeration;
      private NakedObject adapter;

      [JavaFlags(2)]
      public ElementMatcher(Enumeration enumeration) => this.enumeration = enumeration;

      public virtual bool hasMoreElements() => this.enumeration.hasMoreElements();

      public virtual object getElement() => (object) this.adapter;

      public virtual string nextElement()
      {
        this.adapter = (NakedObject) this.enumeration.nextElement();
        return this.adapter.titleString();
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        SetFieldAgent.ElementMatcher elementMatcher = this;
        ObjectImpl.clone((object) elementMatcher);
        return ((object) elementMatcher).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
