// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.AbstractBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;

namespace org.nakedobjects.viewer.skylark.core
{
  public class AbstractBorder : AbstractViewDecorator
  {
    [JavaFlags(4)]
    public int bottom;
    [JavaFlags(4)]
    public int left;
    private bool onBorder;
    [JavaFlags(4)]
    public int right;
    [JavaFlags(4)]
    public int top;

    [JavaFlags(4)]
    public AbstractBorder(View view)
      : base(view)
    {
    }

    [JavaFlags(4)]
    public virtual Bounds contentArea() => new Bounds(this.getLeft(), this.getTop(), this.getSize().getWidth() - this.getLeft() - this.getRight(), this.getSize().getHeight() - this.getTop() - this.getBottom());

    public override View dragFrom(Location location)
    {
      location.subtract(this.getLeft(), this.getTop());
      return base.dragFrom(location);
    }

    public override void dragIn(ContentDrag drag)
    {
      drag.subtract(this.getLeft(), this.getTop());
      base.dragIn(drag);
    }

    public override void dragOut(ContentDrag drag)
    {
      drag.subtract(this.getLeft(), this.getTop());
      base.dragOut(drag);
    }

    public override Drag dragStart(DragStart drag)
    {
      if (!this.overContent(drag.getLocation()))
        return (Drag) null;
      drag.subtract(this.getLeft(), this.getTop());
      return base.dragStart(drag);
    }

    public override void draw(Canvas canvas)
    {
      if (AbstractView.debug)
        canvas.drawDebugOutline(new Bounds(this.getSize()), this.getBaseline(), Color.DEBUG_BORDER_BOUNDS);
      this.wrappedView.draw(canvas.createSubcanvas(this.getLeft(), this.getTop(), this.getSize().getWidth() - this.getRight(), this.getSize().getHeight() - this.getBottom()));
    }

    public override void drop(ContentDrag drag)
    {
      drag.subtract(this.getLeft(), this.getTop());
      base.drop(drag);
    }

    public override void drop(ViewDrag drag)
    {
      drag.subtract(this.getLeft(), this.getTop());
      base.drop(drag);
    }

    public override void firstClick(Click click)
    {
      if (!this.overContent(click.getLocation()))
        return;
      click.subtract(this.getLeft(), this.getTop());
      this.wrappedView.firstClick(click);
    }

    public override int getBaseline() => this.wrappedView.getBaseline() + this.getTop();

    [JavaFlags(4)]
    public virtual int getBottom() => this.bottom;

    [JavaFlags(4)]
    public virtual int getLeft() => this.left;

    public override Padding getPadding()
    {
      Padding padding = this.wrappedView.getPadding();
      padding.extendTop(this.getTop());
      padding.extendLeft(this.getLeft());
      padding.extendBottom(this.getBottom());
      padding.extendRight(this.getRight());
      return padding;
    }

    public override Size getRequiredSize(Size maximumSize)
    {
      maximumSize.contract(this.getLeft() + this.getRight(), this.getTop() + this.getBottom());
      Size requiredSize = this.wrappedView.getRequiredSize(maximumSize);
      requiredSize.extend(this.getLeft() + this.getRight(), this.getTop() + this.getBottom());
      return requiredSize;
    }

    [JavaFlags(4)]
    public virtual int getRight() => this.right;

    public override Size getSize()
    {
      Size size = this.wrappedView.getSize();
      size.extend(this.getLeft() + this.getRight(), this.getTop() + this.getBottom());
      return size;
    }

    [JavaFlags(4)]
    public virtual int getTop() => this.top;

    [JavaFlags(4)]
    public override void debugDetails(StringBuffer b)
    {
      base.debugDetails(b);
      b.append(new StringBuffer().append("\n           border:  ").append(this.getTop()).append("/").append(this.getBottom()).append(" ").append(this.getLeft()).append("/").append(this.getRight()).append(" (top/bottom left/right)").ToString());
      b.append(new StringBuffer().append("\n           contents:  ").append((object) this.contentArea()).ToString());
    }

    [JavaFlags(4)]
    public virtual bool overBorder(Location location) => ((this.contentArea().contains(location) ? 1 : 0) ^ 1) != 0;

    [JavaFlags(4)]
    public virtual bool overContent(Location location) => this.contentArea().contains(location);

    [JavaFlags(4)]
    public virtual bool isOnBorder() => this.onBorder;

    public override View identify(Location location)
    {
      this.getViewManager().getSpy().addTrace((View) this, "mouse location within border", (object) location);
      this.getViewManager().getSpy().addTrace((View) this, "non border area", (object) this.contentArea());
      if (this.overBorder(location))
      {
        this.getViewManager().getSpy().addTrace((View) this, "over border area", (object) this.contentArea());
        return this.getView();
      }
      location.add(-this.getLeft(), -this.getTop());
      return base.identify(location);
    }

    public override void mouseDown(Click click)
    {
      if (!this.overContent(click.getLocation()))
        return;
      click.subtract(this.getLeft(), this.getTop());
      this.wrappedView.mouseDown(click);
    }

    public override void mouseMoved(Location at)
    {
      bool flag = this.overBorder(at);
      if (this.onBorder != flag)
      {
        this.markDamaged();
        this.onBorder = flag;
      }
      if (flag)
        return;
      at.move(-this.getLeft(), -this.getTop());
      this.wrappedView.mouseMoved(at);
    }

    public override void mouseUp(Click click)
    {
      if (!this.overContent(click.getLocation()))
        return;
      click.subtract(this.getLeft(), this.getTop());
      this.wrappedView.mouseUp(click);
    }

    public override void exited()
    {
      this.onBorder = false;
      base.exited();
    }

    public override View pickupContent(Location location)
    {
      location.subtract(this.getLeft(), this.getTop());
      return base.pickupContent(location);
    }

    public override View pickupView(Location location)
    {
      if (this.overBorder(location))
        return (View) new DragViewOutline(this.getView());
      location.subtract(this.getLeft(), this.getTop());
      return base.pickupView(location);
    }

    public override void secondClick(Click click)
    {
      if (!this.overContent(click.getLocation()))
        return;
      click.subtract(this.getLeft(), this.getTop());
      this.wrappedView.secondClick(click);
    }

    public override void setMaximumSize(Size size)
    {
      Size size1 = new Size(size);
      size1.contract(this.getLeft() + this.getRight(), this.getTop() + this.getBottom());
      this.wrappedView.setMaximumSize(size1);
    }

    public override void setSize(Size size)
    {
      Size size1 = new Size(size);
      size1.contract(this.getLeft() + this.getRight(), this.getTop() + this.getBottom());
      this.wrappedView.setSize(size1);
    }

    public override void setBounds(Bounds bounds)
    {
      Bounds bounds1 = new Bounds(bounds);
      bounds1.contract(this.getLeft() + this.getRight(), this.getTop() + this.getBottom());
      this.wrappedView.setBounds(bounds1);
    }

    public override void thirdClick(Click click)
    {
      if (!this.overContent(click.getLocation()))
        return;
      click.subtract(this.getLeft(), this.getTop());
      this.wrappedView.thirdClick(click);
    }

    public override ViewAreaType viewAreaType(Location mouseLocation)
    {
      Size size = this.wrappedView.getSize();
      if (!new Bounds(this.getLeft(), this.getTop(), size.getWidth(), size.getHeight()).contains(mouseLocation))
        return ViewAreaType.VIEW;
      mouseLocation.subtract(this.getLeft(), this.getTop());
      return this.wrappedView.viewAreaType(mouseLocation);
    }
  }
}
