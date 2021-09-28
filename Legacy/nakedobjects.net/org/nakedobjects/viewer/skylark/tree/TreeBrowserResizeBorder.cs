// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.tree.TreeBrowserResizeBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.special;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.tree
{
  public class TreeBrowserResizeBorder : ResizeBorder
  {
    public static readonly int BORDER_WIDTH;

    public TreeBrowserResizeBorder(View view)
      : base(view, 2, TreeBrowserResizeBorder.BORDER_WIDTH, 2)
    {
    }

    [JavaFlags(4)]
    public override void drawResizeBorder(Canvas canvas, Size size)
    {
      int y = 0;
      int y2 = this.getSize().getHeight() - 1;
      for (int index = 0; index < this.getRight(); ++index)
      {
        int num = this.getSize().getWidth() - index - 1;
        canvas.drawLine(num, y, num, y2, Style.SECONDARY1);
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TreeBrowserResizeBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
