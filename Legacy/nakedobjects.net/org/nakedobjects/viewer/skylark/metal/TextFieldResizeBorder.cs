// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.TextFieldResizeBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.special;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class TextFieldResizeBorder : ResizeBorder
  {
    public static readonly int BORDER_WIDTH;

    public TextFieldResizeBorder(View view)
      : base(view, 10, 1, 1)
    {
    }

    [JavaFlags(4)]
    public override void drawResizeBorder(Canvas canvas, Size size)
    {
      if (!this.resizing)
        return;
      Shape shape = new Shape(0, 0);
      int num = 10;
      shape.extendsLine(num, 0);
      shape.extendsLine(0, num);
      shape.extendsLine(-num, -num);
      canvas.drawSolidShape(shape, size.getWidth() - num, size.getHeight(), Style.RESIZE);
      canvas.drawRectangle(0, 0, size.getWidth(), size.getHeight(), Style.RESIZE);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TextFieldResizeBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
