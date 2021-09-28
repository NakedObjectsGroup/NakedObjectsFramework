// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ValueParameter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaFlags(32)]
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ParameterContent;")]
  public class ValueParameter : ValueContent, ParameterContent
  {
    private readonly NakedValue @object;
    private readonly string name;
    private readonly NakedObjectSpecification specification;
    private readonly bool isRequired;

    public ValueParameter(
      string name,
      Naked naked,
      NakedObjectSpecification specification,
      bool required)
    {
      this.name = name;
      this.specification = specification;
      this.isRequired = required;
      this.@object = (NakedValue) naked;
    }

    public override void debugDetails(DebugString debug)
    {
      debug.appendln("name", (object) this.name);
      debug.appendln("required", this.isRequired);
      debug.appendln("object", (object) this.@object);
    }

    public override void entryComplete()
    {
    }

    public override string getIconName() => "";

    public override Image getIconPicture(int iconHeight) => (Image) null;

    public override Naked getNaked() => (Naked) this.@object;

    public override NakedValue getObject() => this.@object;

    public override bool isEmpty() => this.@object.isEmpty();

    public virtual bool isRequired() => this.isRequired;

    public override void clear() => this.@object.clear();

    public override bool isTransient() => true;

    public override bool isValue() => true;

    public override string title() => this.@object.titleString();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("object", (object) this.@object);
      return toString.ToString();
    }

    public virtual string getParameterName() => this.name;

    public override NakedObjectSpecification getSpecification() => this.specification;

    public override Naked drop(Content sourceContent) => (Naked) null;

    public override Consent canDrop(Content sourceContent) => (Consent) Veto.DEFAULT;

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entryText) => this.@object.parseTextEntry(entryText);

    public override string getDescription() => (string) null;

    public override string getHelp() => (string) null;

    public override string getId() => (string) null;

    public override Consent isEditable() => (Consent) Allow.DEFAULT;
  }
}
