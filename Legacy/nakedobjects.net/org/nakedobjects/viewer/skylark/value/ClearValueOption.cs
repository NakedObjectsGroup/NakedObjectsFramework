// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.ClearValueOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark.value
{
  public class ClearValueOption : AbstractValueOption
  {
    public ClearValueOption()
      : base("Reset")
    {
    }

    public override string getDescription(View view) => "Reset this field to its default value";

    public override Consent disabled(View view)
    {
      NakedValue nakedValue = this.getValue(view);
      if (!view.canChangeValue())
        return (Consent) new Veto("Field cannot be edited");
      if (!nakedValue.canClear())
        return (Consent) new Veto(new StringBuffer().append("Can't clear ").append(nakedValue.getSpecification().getShortName()).append(" values").ToString());
      return this.isEmpty(view) ? (Consent) new Veto("Field is already empty") : (Consent) new Allow(new StringBuffer().append("Clear value ").append(nakedValue.titleString()).ToString());
    }

    public override void execute(Workspace frame, View view, Location at)
    {
      ((ValueContent) view.getContent()).clear();
      this.updateParent(view);
      view.invalidateContent();
    }

    public override string ToString() => nameof (ClearValueOption);
  }
}
