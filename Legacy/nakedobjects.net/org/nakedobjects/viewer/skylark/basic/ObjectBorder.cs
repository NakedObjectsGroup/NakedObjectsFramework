// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ObjectBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.util;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ObjectBorder : AbstractBorder
  {
    private const int BORDER = 13;

    public ObjectBorder(int size, View wrappedView)
      : base(wrappedView)
    {
      this.top = size;
      this.left = size;
      this.bottom = size;
      this.right = size + 13;
    }

    public ObjectBorder(View wrappedView)
      : this(1, wrappedView)
    {
    }

    [JavaFlags(4)]
    public override void debugDetails(StringBuffer b) => b.append(new StringBuffer().append("ObjectBorder ").append(this.top).append(" pixels").ToString());

    public override Drag dragStart(DragStart drag)
    {
      if (drag.getLocation().getX() <= this.getSize().getWidth() - this.right)
        return base.dragStart(drag);
      View dragView = (View) new DragViewOutline(this.getView());
      return (Drag) new ViewDrag((View) this, new Offset(drag.getLocation()), dragView);
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      Color color = (Color) null;
      ViewState state = this.getState();
      bool flag = this.getViewManager().hasFocus(this.getView());
      if (state.canDrop())
        color = Style.VALID;
      else if (state.cantDrop())
        color = Style.INVALID;
      else if (flag)
        color = Style.IDENTIFIED;
      else if (state.isObjectIdentified())
        color = Style.SECONDARY2;
      Size size = this.getSize();
      if (this.getContent().isPersistable() && this.getContent().isTransient())
      {
        int x = size.getWidth() - 13;
        int y = 0;
        Image icon = ImageFactory.getInstance().loadIcon("transient", 8);
        if (icon == null)
          canvas.drawText("*", x, y + Style.NORMAL.getAscent(), Style.BLACK, Style.NORMAL);
        else
          canvas.drawIcon(icon, x, y, 12, 12);
      }
      if (color == null)
        return;
      if (flag)
      {
        int num = size.getWidth() - this.left;
        for (int index = 0; index < this.left; ++index)
          canvas.drawRectangle(index, index, num - 2 * index, size.getHeight() - 2 * index, color);
      }
      else
      {
        int width = size.getWidth();
        for (int index = 0; index < this.left; ++index)
          canvas.drawRectangle(index, index, width - 2 * index, size.getHeight() - 2 * index, color);
        canvas.drawLine(width - 13, this.left, width - 13, this.left + size.getHeight(), color);
        canvas.drawSolidRectangle(width - 13 + 1, this.left, 11, size.getHeight() - 2 * this.left, Style.SECONDARY3);
      }
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

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/ObjectBorder [").append((object) this.getSpecification()).append("]").ToString();
  }
}
