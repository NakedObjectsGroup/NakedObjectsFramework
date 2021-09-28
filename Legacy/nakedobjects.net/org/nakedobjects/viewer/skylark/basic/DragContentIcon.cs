// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.DragContentIcon
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.viewer.skylark.basic
{
  public class DragContentIcon : IconView
  {
    private readonly int thickness;

    public DragContentIcon(Content content)
      : base(content, (ViewSpecification) null, (ViewAxis) null, Style.LABEL)
    {
      this.thickness = 0;
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      Bounds bounds = this.getBounds();
      for (int index = 0; index < 0; ++index)
        canvas.drawRectangle(index, index, bounds.getWidth() - index * 2 - 1, bounds.getHeight() - index * 2 - 1, Style.SECONDARY1);
    }
  }
}
