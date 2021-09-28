// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.CopyValueOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.awt;
using java.awt.datatransfer;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark.value
{
  public class CopyValueOption : AbstractValueOption
  {
    public CopyValueOption()
      : base("Copy")
    {
    }

    public override Consent disabled(View view)
    {
      NakedValue nakedValue = this.getValue(view);
      return this.isEmpty(view) ? (Consent) new Veto("Field is empty") : (Consent) new Allow(new StringBuffer().append("Copy value '").append(nakedValue.titleString()).append("' to clipboard").ToString());
    }

    public override void execute(Workspace frame, View view, Location at)
    {
      string str = this.getValue(view).titleString();
      Toolkit.getDefaultToolkit().getSystemClipboard().setContents((Transferable) new StringSelection(str), (ClipboardOwner) null);
    }

    public override string ToString() => nameof (CopyValueOption);
  }
}
