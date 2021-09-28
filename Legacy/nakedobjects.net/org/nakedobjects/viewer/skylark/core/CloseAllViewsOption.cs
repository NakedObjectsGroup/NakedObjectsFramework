// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.CloseAllViewsOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark.core
{
  public class CloseAllViewsOption : AbstractUserAction
  {
    public CloseAllViewsOption()
      : base("Close all others")
    {
    }

    public override void execute(Workspace workspace, View view, Location at)
    {
      foreach (View subview in view.getWorkspace().getSubviews())
      {
        if (subview.getSpecification().isOpen() && subview != view)
          subview.dispose();
      }
    }

    public override string getDescription(View view) => new StringBuffer().append("Close all view except ").append(StringImpl.toLowerCase(view.getSpecification().getName())).ToString();

    public override string ToString() => new org.nakedobjects.utility.ToString((object) this).ToString();
  }
}
