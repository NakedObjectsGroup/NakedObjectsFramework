// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.IconOpenAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaFlags(32)]
  public class IconOpenAction : AbstractViewDecorator
  {
    [JavaFlags(4)]
    public IconOpenAction(View wrappedView)
      : base(wrappedView)
    {
    }

    private void closeIcon() => this.getView().dispose();

    public override void viewMenuOptions(UserActionSet menuOptions)
    {
      base.viewMenuOptions(menuOptions);
      menuOptions.add((UserAction) new IconOpenAction.\u0031(this, "Open"));
      menuOptions.add((UserAction) new IconOpenAction.\u0032(this, "Close"));
    }

    private void openIcon() => this.getWorkspace().addOpenViewFor(this.getContent().getNaked(), this.getLocation());

    public override void secondClick(Click click) => this.openIcon();

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private IconOpenAction this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.openIcon();

      public \u0031(IconOpenAction _param1, string dummy0)
        : base(dummy0)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0032 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private IconOpenAction this\u00240;

      public override void execute(Workspace workspace, View view, Location at) => this.this\u00240.closeIcon();

      public \u0032(IconOpenAction _param1, string dummy0)
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
