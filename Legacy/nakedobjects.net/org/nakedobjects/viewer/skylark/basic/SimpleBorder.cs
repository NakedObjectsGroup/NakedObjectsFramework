// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.SimpleBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class SimpleBorder : AbstractBorder
  {
    private int handleWidth;

    public SimpleBorder(View wrappedView)
      : this(1, wrappedView)
    {
    }

    public SimpleBorder(int size, View wrappedView)
      : base(wrappedView)
    {
      this.handleWidth = 14;
      this.top = size;
      this.left = size;
      this.bottom = size;
      this.right = size + this.handleWidth;
    }

    [JavaFlags(4)]
    public override void debugDetails(StringBuffer b)
    {
      b.append(new StringBuffer().append("SimpleBorder ").append(this.top).append(" pixels\n").ToString());
      b.append(new StringBuffer().append("           handle ").append(this.handleWidth).append(" pixels").ToString());
    }

    public override Drag dragStart(DragStart drag)
    {
      if (!this.overBorder(drag.getLocation()))
        return base.dragStart(drag);
      Location location = drag.getLocation();
      DragViewOutline dragViewOutline = new DragViewOutline(this.getView());
      return (Drag) new ViewDrag((View) this, new Offset(location.getX(), location.getY()), (View) dragViewOutline);
    }

    public override void entered()
    {
      this.getState().setObjectIdentified();
      this.getState().setViewIdentified();
      this.wrappedView.entered();
      this.markDamaged();
    }

    public override void exited()
    {
      this.getState().clearObjectIdentified();
      this.getState().clearViewIdentified();
      this.wrappedView.exited();
      this.markDamaged();
    }

    public override void draw(Canvas canvas)
    {
      if (this.getState().isViewIdentified())
      {
        Color secondarY2 = Style.SECONDARY2;
        Size size = this.getSize();
        int width = size.getWidth();
        for (int index = 0; index < this.left; ++index)
          canvas.drawRectangle(index, index, width - 2 * index - 1, size.getHeight() - 2 * index - 1, secondarY2);
        int num1 = width - this.left - 2;
        int num2 = num1 - this.handleWidth;
        for (int index = num1; index > num2; index -= 2)
          canvas.drawLine(index, this.top, index, size.getHeight() - this.top, secondarY2);
      }
      base.draw(canvas);
    }

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/SimpleBorder").ToString();
  }
}
