// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.ResizeBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public abstract class ResizeBorder : AbstractBorder
  {
    private static readonly Logger LOG;
    private static readonly Logger UI_LOG;
    public const int LEFT = 1;
    public const int RIGHT = 2;
    public const int UP = 4;
    public const int DOWN = 8;
    private int width;
    private int height;
    private int requiredDirection;
    private int allowDirections;
    [JavaFlags(4)]
    public bool resizing;
    private int onBorder;

    public ResizeBorder(View view)
      : this(view, 15, 1, 1)
    {
    }

    public ResizeBorder(View view, int allowDirections, int width, int minimumWidth)
      : base(view)
    {
      this.allowDirections = allowDirections;
      this.top = !this.canExtend(4) ? minimumWidth : width;
      this.bottom = !this.canExtend(8) ? minimumWidth : width;
      this.left = !this.canExtend(1) ? minimumWidth : width;
      this.right = !this.canExtend(2) ? minimumWidth : width;
    }

    [JavaFlags(4)]
    public override void debugDetails(StringBuffer b)
    {
      base.debugDetails(b);
      b.append(new StringBuffer().append("\n           width ").append(this.width != 0 ? Integer.toString(this.width) : "no change").ToString());
      b.append(new StringBuffer().append("\n           height ").append(this.height != 0 ? Integer.toString(this.height) : "no change").ToString());
      b.append(new StringBuffer().append("\n           resizable ").append(!this.canExtend(4) ? "" : "Up ").append(!this.canExtend(8) ? "" : "Down ").append(!this.canExtend(1) ? "" : "Left ").append(!this.canExtend(2) ? "" : "Right ").ToString());
    }

    public override void draw(Canvas canvas)
    {
      Size size = this.getSize();
      int width = size.getWidth();
      int height = size.getHeight();
      this.drawResizeBorder(canvas, size);
      this.wrappedView.draw(canvas.createSubcanvas(this.left, this.top, width - this.left - this.right, height - this.top - this.bottom));
    }

    [JavaFlags(1028)]
    public abstract void drawResizeBorder(Canvas canvas, Size size);

    public override ViewAreaType viewAreaType(Location mouseLocation) => this.isOnBorder() ? ViewAreaType.INTERNAL : base.viewAreaType(mouseLocation);

    public override void viewMenuOptions(UserActionSet menuOptions)
    {
      base.viewMenuOptions(menuOptions);
      menuOptions.add((UserAction) new ResizeBorder.\u0031(this, "Clear resizing"));
    }

    public override Drag dragStart(DragStart drag)
    {
      Location location = drag.getLocation();
      if (!this.overBorder(location))
        return base.dragStart(drag);
      this.requiredDirection = this.onBorder(location);
      return this.requiredDirection > 0 ? (Drag) new ResizeDrag((View) this, new Bounds(this.getAbsoluteLocation(), this.getView().getSize()), this.requiredDirection) : (Drag) null;
    }

    public override void drag(InternalDrag drag)
    {
      if ((ViewResizeOutline) drag.getOverlay() != null)
        return;
      base.drag(drag);
    }

    public override void dragTo(InternalDrag drag)
    {
      this.getViewManager().showDefaultCursor();
      ViewResizeOutline overlay = (ViewResizeOutline) drag.getOverlay();
      if (overlay != null)
      {
        this.resizing = false;
        this.onBorder = 0;
        if (this.requiredDirection == 4 || this.requiredDirection == 8)
          this.width = overlay.getSize().getWidth();
        if (this.requiredDirection == 2 || this.requiredDirection == 8)
          this.height = overlay.getSize().getHeight();
        if (ResizeBorder.LOG.isDebugEnabled())
          ResizeBorder.LOG.debug((object) new StringBuffer().append("resizing view ").append(this.width).append(",").append(this.height).ToString());
        this.invalidateLayout();
      }
      else
        base.dragTo(drag);
    }

    public override Size getRequiredSize(Size maximumSize)
    {
      maximumSize.contract(this.getLeft() + this.getRight(), this.getTop() + this.getBottom());
      if (this.width > 0 && maximumSize.getWidth() > this.width)
        maximumSize.setWidth(this.width);
      if (this.height > 0 && maximumSize.getHeight() > this.height)
        maximumSize.setHeight(this.height);
      Size requiredSize = this.wrappedView.getRequiredSize(maximumSize);
      requiredSize.extend(this.getLeft() + this.getRight(), this.getTop() + this.getBottom());
      if (this.width > 0)
        requiredSize.setWidth(this.width);
      if (this.height > 0)
        requiredSize.setHeight(this.height);
      return requiredSize;
    }

    public override void mouseMoved(Location at)
    {
      int num = this.onBorder(at);
      bool flag = num == 0;
      if (this.onBorder != num || flag)
      {
        switch (num)
        {
          case 2:
            this.getViewManager().showResizeDownCursor();
            this.resizing = true;
            this.markDamaged();
            break;
          case 4:
            this.getViewManager().showResizeRightCursor();
            this.resizing = true;
            this.markDamaged();
            break;
          case 8:
            this.getViewManager().showResizeDownRightCursor();
            this.resizing = true;
            this.markDamaged();
            break;
          default:
            this.getViewManager().showDefaultCursor();
            base.mouseMoved(at);
            this.resizing = false;
            this.markDamaged();
            break;
        }
        if (ResizeBorder.UI_LOG.isDebugEnabled())
          ResizeBorder.UI_LOG.debug((object) new StringBuffer().append("on resize border ").append(num).append(" ").append(this.resizing).ToString());
      }
      this.onBorder = num;
    }

    public override void exited()
    {
      this.getViewManager().showDefaultCursor();
      this.resizing = false;
      this.onBorder = 0;
      this.markDamaged();
      if (ResizeBorder.UI_LOG.isDebugEnabled())
        ResizeBorder.UI_LOG.debug((object) new StringBuffer().append("off resize border ").append(this.onBorder).append(" ").append(this.resizing).ToString());
      base.exited();
    }

    [JavaFlags(4)]
    public virtual int onBorder(Location at)
    {
      Bounds bounds = this.contentArea();
      bool flag1 = this.canExtend(2) && at.getX() >= bounds.getWidth() && at.getX() <= bounds.getWidth() + this.getRight();
      bool flag2 = this.canExtend(8) && at.getY() >= bounds.getHeight() && at.getY() <= bounds.getHeight() + this.getBottom();
      return !flag1 || !flag2 ? (!flag1 ? (!flag2 ? 0 : 2) : 4) : 8;
    }

    [JavaFlags(4)]
    public virtual bool canExtend(int extend) => (extend & this.allowDirections) == extend;

    [JavaFlags(4)]
    public virtual int getAllowDirections() => this.allowDirections;

    [JavaFlags(4)]
    public virtual void setAllowDirections(int allowDirections) => this.allowDirections = allowDirections;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ResizeBorder()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : AbstractUserAction
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ResizeBorder this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.width = 0;
        this.this\u00240.height = 0;
        this.this\u00240.invalidateLayout();
      }

      public \u0031(ResizeBorder _param1, string dummy0)
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
