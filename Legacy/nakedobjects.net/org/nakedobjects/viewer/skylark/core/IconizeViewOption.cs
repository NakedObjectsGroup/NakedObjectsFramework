// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.IconizeViewOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark.core
{
  public class IconizeViewOption : AbstractUserAction
  {
    public IconizeViewOption()
      : base("Iconize")
    {
    }

    public override Consent disabled(View view) => (Consent) Allow.DEFAULT;

    public override void execute(Workspace workspace, View view, Location at)
    {
      MinimizedView minimizedView = new MinimizedView(view);
      minimizedView.setLocation(view.getLocation());
      foreach (View subview in workspace.getSubviews())
      {
        if (subview == view)
        {
          workspace.removeView(view);
          workspace.addView((View) minimizedView);
          workspace.invalidateLayout();
          break;
        }
      }
    }

    public override string getDescription(View view) => "Show this object as an icon on the workspace";
  }
}
