// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ObjectParameter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ParameterContent;")]
  public class ObjectParameter : ObjectContent, ParameterContent
  {
    private readonly NakedObject @object;
    private readonly string name;
    private readonly NakedObjectSpecification specification;
    private readonly ActionHelper invocation;
    private readonly int i;
    private readonly bool isRequired;
    private readonly NakedObject[] options;

    public ObjectParameter(
      string name,
      Naked naked,
      NakedObjectSpecification specification,
      bool required,
      NakedObject[] options,
      int i,
      ActionHelper invocation)
    {
      this.name = name;
      this.specification = specification;
      this.isRequired = required;
      this.options = options;
      this.i = i;
      this.invocation = invocation;
      this.@object = (NakedObject) naked;
    }

    public ObjectParameter(ObjectParameter content, NakedObject @object)
    {
      this.name = content.name;
      this.specification = content.specification;
      this.isRequired = content.isRequired;
      this.options = content.options;
      this.i = content.i;
      this.invocation = content.invocation;
      this.@object = @object;
    }

    public override Consent canClear() => (Consent) Allow.DEFAULT;

    public override Consent canSet(NakedObject dragSource) => dragSource.getSpecification().isOfType(this.specification) ? (Consent) Allow.DEFAULT : (Consent) new Veto(new StringBuffer().append("Object must be ").append(this.specification.getShortName()).ToString());

    public override void clear() => this.setObject((NakedObject) null);

    public override void debugDetails(DebugString debug)
    {
      debug.appendln("name", (object) this.name);
      debug.appendln("required", this.isRequired);
      debug.appendln("object", (object) this.@object);
    }

    public override Naked getNaked() => (Naked) this.@object;

    public override NakedObject getObject() => this.@object;

    public virtual NakedObject[] getOptions() => this.options;

    public override bool isObject() => true;

    public virtual bool isRequired() => this.isRequired;

    public override bool isPersistable() => false;

    public override bool isTransient() => this.@object != null && this.@object.getResolveState().isTransient();

    public override void contentMenuOptions(UserActionSet options)
    {
      if (this.@object == null)
        return;
      options.add((UserAction) new ObjectParameter.\u0031(this, "Clear parameter"));
    }

    public override void setObject(NakedObject @object) => this.invocation.setParameter(this.i, (Naked) @object);

    public override string title() => this.@object == null ? "" : this.@object.titleString();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("label", this.name);
      toString.append("required", this.isRequired);
      toString.append("spec", this.getSpecification().getFullName());
      toString.append("object", this.@object != null ? this.@object.titleString() : "null");
      return toString.ToString();
    }

    public virtual string getParameterName() => this.name;

    public override NakedObjectSpecification getSpecification() => this.specification;

    public override string getDescription() => this.invocation.getDescription();

    public override string getHelp() => this.invocation.getHelp();

    public override string getId() => (string) null;

    [JavaFlags(32)]
    [Inner]
    public new class \u0031 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ObjectParameter this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.clear();
        view.getParent().invalidateContent();
      }

      public \u0031(ObjectParameter _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
