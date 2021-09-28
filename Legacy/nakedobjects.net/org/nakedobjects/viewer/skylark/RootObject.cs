// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.RootObject
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark
{
  public class RootObject : ObjectContent
  {
    private readonly NakedObject @object;

    public RootObject(NakedObject @object) => this.@object = @object;

    public override Consent canClear() => (Consent) Veto.DEFAULT;

    public override Consent canSet(NakedObject dragSource) => (Consent) Veto.DEFAULT;

    public override void clear() => throw new NakedObjectRuntimeException("Invalid call");

    public override void debugDetails(DebugString debug) => debug.appendln("object", (object) this.@object);

    public override Naked getNaked() => (Naked) this.@object;

    public override string getDescription() => "";

    public override string getHelp() => "";

    public override string getId() => "";

    public override NakedObject getObject() => this.@object;

    public override NakedObjectSpecification getSpecification() => this.@object.getSpecification();

    public override bool isObject() => true;

    public override bool isTransient() => this.@object != null && this.@object.getResolveState().isTransient();

    public override void setObject(NakedObject @object) => throw new NakedObjectRuntimeException("Invalid call");

    public override string title() => this.@object.titleString();

    public override string ToString() => new StringBuffer().append("Root Object: ").append((object) this.@object).ToString();

    public override string windowTitle() => new StringBuffer().append(!this.isTransient() ? "" : "UNSAVED ").append(this.getSpecification().getShortName()).ToString();
  }
}
