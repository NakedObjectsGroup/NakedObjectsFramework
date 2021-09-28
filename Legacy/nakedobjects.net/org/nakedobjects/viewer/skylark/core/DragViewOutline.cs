// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DragViewOutline
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.viewer.skylark.core
{
  public class DragViewOutline : AbstractView
  {
    private readonly int thickness;
    private readonly Size size;

    public DragViewOutline(View view)
      : base(view.getContent(), (ViewSpecification) null, (ViewAxis) null)
    {
      this.thickness = 5;
      this.size = view.getSize();
      this.setLocation(view.getAbsoluteLocation());
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      Bounds bounds = this.getBounds();
      for (int index = 0; index < 5; ++index)
        canvas.drawRectangle(index, index, bounds.getWidth() - index * 2 - 1, bounds.getHeight() - index * 2 - 1, Style.SECONDARY1);
    }

    public override Size getMaximumSize() => new Size(this.size);
  }
}
