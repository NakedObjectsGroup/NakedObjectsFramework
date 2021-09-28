// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.WindowResizeBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.apache.log4j;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.metal;
using org.nakedobjects.viewer.skylark.tree;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public class WindowResizeBorder : AbstractViewDecorator
  {
    private static readonly Logger LOG;
    private static readonly Logger UI_LOG;
    private bool resizable;
    private View resizeBorder;

    public WindowResizeBorder(View view)
      : this(view, false, false)
    {
    }

    public WindowResizeBorder(View view, bool scrollable, bool resizable)
      : base(WindowResizeBorder.addWindowBorder(view, scrollable, resizable))
    {
      this.resizable = false;
      this.resizable = resizable;
    }

    private static View addWindowBorder(View view, bool scrollable, bool resizable)
    {
      View view1 = (View) new WindowBorder(view, scrollable);
      return resizable ? (View) new TreeBrowserResizeBorder(view1) : view1;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static WindowResizeBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
