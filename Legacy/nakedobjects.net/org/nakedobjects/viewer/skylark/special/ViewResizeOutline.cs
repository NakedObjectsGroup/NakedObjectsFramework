// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.ViewResizeOutline
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.special
{
  public class ViewResizeOutline : AbstractView
  {
    private readonly int thickness;
    private string label;
    private readonly Size size;

    [JavaFlags(4)]
    public ViewResizeOutline(Bounds resizeArea)
      : base((Content) null, (ViewSpecification) null, (ViewAxis) null)
    {
      this.thickness = 1;
      this.label = "";
      this.size = resizeArea.getSize();
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      Size size = this.getSize();
      for (int index = 0; index < 1; ++index)
        canvas.drawRectangle(index, index, size.getWidth() - index * 2 - 1, size.getHeight() - index * 2 - 1, Style.PRIMARY2);
      canvas.drawText(this.label, 2, 16, Style.PRIMARY2, Style.NORMAL);
    }

    public virtual void setDisplay(string label) => this.label = label != null ? label : "";

    public override void dispose()
    {
      this.getViewManager().showArrowCursor();
      base.dispose();
    }

    public override Size getMaximumSize() => new Size(this.size);
  }
}
