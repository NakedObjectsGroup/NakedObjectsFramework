// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.CloseViewOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;

namespace org.nakedobjects.viewer.skylark.core
{
  public class CloseViewOption : AbstractUserAction
  {
    public CloseViewOption()
      : base("Close")
    {
    }

    public override void execute(Workspace workspace, View view, Location at) => view.dispose();

    public override string getDescription(View view) => new StringBuffer().append("Close this ").append(view.getSpecification().getName()).ToString();

    public override string ToString() => new org.nakedobjects.utility.ToString((object) this).ToString();
  }
}
