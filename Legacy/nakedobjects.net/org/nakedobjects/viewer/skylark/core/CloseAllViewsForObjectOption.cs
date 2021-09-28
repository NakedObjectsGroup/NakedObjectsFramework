// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.CloseAllViewsForObjectOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.core
{
  public class CloseAllViewsForObjectOption : AbstractUserAction
  {
    public CloseAllViewsForObjectOption()
      : base("Close all views for this object")
    {
    }

    public override void execute(Workspace workspace, View view, Location at) => workspace.removeViewsFor((NakedObject) view.getContent().getNaked());

    public override string getDescription(View view) => new StringBuffer().append("Close all views for ").append(view.getContent().title()).ToString();

    public override string ToString() => new org.nakedobjects.utility.ToString((object) this).ToString();
  }
}
