// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.ScrollBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.awt.@event;
using java.lang;
using org.nakedobjects.viewer.skylark.@event;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public class ScrollBorder : AbstractViewDecorator
  {
    private const int CENTER = 3;
    private const int NORTH = 1;
    private const int SOUTH = 5;
    private const int CORNER = 0;
    private const int SCROLLBAR_WIDTH = 16;
    private const int WEST = 2;
    private const int EAST = 4;
    private ScrollBar horizontalScrollBar;
    private ScrollBar verticalScrollBar;
    [JavaFlags(4)]
    public int bottom;
    [JavaFlags(4)]
    public int left;
    private View leftHeader;
    [JavaFlags(4)]
    public int right;
    private Size size;
    [JavaFlags(4)]
    public int top;
    private View topHeader;
    private int dragArea;
    private int offsetToThumbEdge;
    private View activeSubviewView;
    private bool requiredSizeValid;
    private Size requiredSize;

    public ScrollBorder(View view)
      : this(view, (View) new NullView(), (View) new NullView())
    {
    }

    public ScrollBorder(View content, View leftHeader, View topHeader)
      : base(content)
    {
      this.horizontalScrollBar = new ScrollBar();
      this.verticalScrollBar = new ScrollBar();
      this.size = new Size();
      this.bottom = this.right = 16;
      this.horizontalScrollBar.setPostion(0);
      this.verticalScrollBar.setPostion(0);
      this.setLeftHeader(leftHeader);
      this.setTopHeader(topHeader);
    }

    public virtual void setTopHeader(View topHeader)
    {
      this.topHeader = topHeader;
      topHeader.setParent(this.getView());
      this.top = topHeader.getRequiredSize(new Size()).getHeight();
    }

    public virtual void setLeftHeader(View leftHeader)
    {
      this.leftHeader = leftHeader;
      leftHeader.setParent(this.getView());
      this.left = leftHeader.getRequiredSize(new Size()).getWidth();
    }

    private int adjust(Click click) => this.adjust(click.getLocation());

    private int adjust(ContentDrag drag) => this.adjust(drag.getTargetLocation());

    private int adjust(Location location)
    {
      Bounds bounds = this.viewportArea();
      if (bounds.contains(location))
      {
        location.subtract(this.left, this.top);
        location.add(this.offset());
        return 3;
      }
      int x = location.getX();
      int y = location.getY();
      if (x > bounds.getX2() && y >= bounds.getY() && y <= bounds.getY2())
      {
        location.subtract(0, bounds.getY());
        return 4;
      }
      if (y > bounds.getY2() && x >= bounds.getX() && x <= bounds.getX2())
      {
        location.subtract(bounds.getX(), 0);
        return 5;
      }
      if (y < bounds.getY() && x >= bounds.getX() && x <= bounds.getX2())
      {
        location.subtract(this.left, 0);
        location.add(this.offset().getDeltaX(), 0);
        return 1;
      }
      if (x < bounds.getX() && y >= bounds.getY() && y <= bounds.getY2())
      {
        location.subtract(0, this.top);
        location.add(0, this.offset().getDeltaY());
        return 2;
      }
      location.setX(-1);
      location.setY(-1);
      return 0;
    }

    [JavaFlags(4)]
    public virtual Bounds viewportArea() => new Bounds(this.left, this.top, this.getSize().getWidth() - this.left - this.right, this.getSize().getHeight() - this.top - this.bottom);

    [JavaFlags(4)]
    public override void debugDetails(StringBuffer b)
    {
      base.debugDetails(b);
      b.append(new StringBuffer().append("\n           Top header: ").append(this.topHeader != null ? this.topHeader.ToString() : "none").ToString());
      b.append(new StringBuffer().append("\n           Left header: ").append(this.leftHeader != null ? this.leftHeader.ToString() : "none").ToString());
      b.append("\n           Vertical scrollbar ");
      b.append(new StringBuffer().append("\n             offset ").append(this.top).ToString());
      b.append(new StringBuffer().append("\n             position ").append(this.verticalScrollBar.getPosition()).ToString());
      b.append(new StringBuffer().append("\n             minimum ").append(this.verticalScrollBar.getMinimum()).ToString());
      b.append(new StringBuffer().append("\n             maximum ").append(this.verticalScrollBar.getMaximum()).ToString());
      b.append(new StringBuffer().append("\n             visible amount ").append(this.verticalScrollBar.getVisibleAmount()).ToString());
      b.append("\n           Horizontal scrollbar ");
      b.append(new StringBuffer().append("\n             offset ").append(this.left).ToString());
      b.append(new StringBuffer().append("\n             position ").append(this.horizontalScrollBar.getPosition()).ToString());
      b.append(new StringBuffer().append("\n             minimum ").append(this.horizontalScrollBar.getMinimum()).ToString());
      b.append(new StringBuffer().append("\n             maximum ").append(this.horizontalScrollBar.getMaximum()).ToString());
      b.append(new StringBuffer().append("\n             visible amount ").append(this.horizontalScrollBar.getVisibleAmount()).ToString());
      b.append(new StringBuffer().append("\n           Viewport area ").append((object) this.viewportArea()).ToString());
      b.append(new StringBuffer().append("\n           Offset ").append((object) this.offset()).ToString());
    }

    public override void drag(InternalDrag drag)
    {
      switch (this.dragArea)
      {
        case 1:
          drag.getLocation().subtract(this.offset().getDeltaX(), this.top);
          this.topHeader.drag(drag);
          break;
        case 2:
          drag.getLocation().subtract(this.left, this.offset().getDeltaY());
          this.leftHeader.drag(drag);
          break;
        case 3:
          drag.getLocation().subtract(this.offset());
          this.wrappedView.drag(drag);
          break;
        case 4:
          this.verticalScrollBar.setPostion(drag.getLocation().getY() - this.top - this.offsetToThumbEdge);
          this.markDamaged();
          break;
        case 5:
          this.horizontalScrollBar.setPostion(drag.getLocation().getX() - this.left - this.offsetToThumbEdge);
          this.markDamaged();
          break;
      }
    }

    public override Drag dragStart(DragStart drag)
    {
      this.dragArea = this.adjust(drag);
      switch (this.dragArea)
      {
        case 1:
          return this.topHeader.dragStart(drag);
        case 2:
          return this.leftHeader.dragStart(drag);
        case 3:
          return this.wrappedView.dragStart(drag);
        case 4:
          return this.dragStartEast(drag);
        case 5:
          return this.dragStartSouth(drag);
        default:
          return (Drag) null;
      }
    }

    public override void dragCancel(InternalDrag drag)
    {
      switch (this.dragArea)
      {
        case 1:
          drag.getLocation().subtract(this.offset().getDeltaX(), this.top);
          this.topHeader.dragCancel(drag);
          break;
        case 2:
          drag.getLocation().subtract(this.left, this.offset().getDeltaY());
          this.leftHeader.dragCancel(drag);
          break;
        case 3:
          drag.getLocation().subtract(this.offset());
          this.wrappedView.dragCancel(drag);
          break;
      }
    }

    public override void dragTo(InternalDrag drag)
    {
      switch (this.dragArea)
      {
        case 1:
          drag.getLocation().subtract(this.offset().getDeltaX(), this.top);
          this.topHeader.dragTo(drag);
          break;
        case 2:
          drag.getLocation().subtract(this.left, this.offset().getDeltaY());
          this.leftHeader.dragTo(drag);
          break;
        case 3:
          drag.getLocation().subtract(this.offset());
          this.wrappedView.dragTo(drag);
          break;
      }
    }

    public override View dragFrom(Location location)
    {
      this.adjust(location);
      switch (this.dragArea)
      {
        case 1:
          return this.topHeader.dragFrom(location);
        case 2:
          return this.leftHeader.dragFrom(location);
        case 3:
          return this.wrappedView.dragFrom(location);
        default:
          return (View) null;
      }
    }

    public override void dragIn(ContentDrag drag)
    {
      this.adjust(drag);
      switch (this.dragArea)
      {
        case 1:
          this.topHeader.dragIn(drag);
          break;
        case 2:
          this.leftHeader.dragIn(drag);
          break;
        case 3:
          this.wrappedView.dragIn(drag);
          break;
      }
    }

    public override void dragOut(ContentDrag drag)
    {
      this.adjust(drag);
      switch (this.dragArea)
      {
        case 1:
          this.topHeader.dragOut(drag);
          break;
        case 2:
          this.leftHeader.dragOut(drag);
          break;
        case 3:
          this.wrappedView.dragOut(drag);
          break;
      }
    }

    private Drag dragStartEast(DragStart drag)
    {
      int y = drag.getLocation().getY();
      if (!this.verticalScrollBar.isOnThumb(y))
        return (Drag) null;
      this.offsetToThumbEdge = y - this.verticalScrollBar.getPosition();
      return (Drag) new SimpleInternalDrag((View) this, new Offset(base.getAbsoluteLocation()));
    }

    private Drag dragStartSouth(DragStart drag)
    {
      int x = drag.getLocation().getX();
      if (!this.horizontalScrollBar.isOnThumb(x))
        return (Drag) null;
      this.offsetToThumbEdge = x - this.horizontalScrollBar.getPosition();
      return (Drag) new SimpleInternalDrag((View) this, new Offset(base.getAbsoluteLocation()));
    }

    private int adjust(DragStart drag) => this.adjust(drag.getLocation());

    public override void draw(Canvas canvas)
    {
      Bounds bounds = this.viewportArea();
      Offset offset = this.offset();
      int deltaX = offset.getDeltaX();
      int deltaY = offset.getDeltaY();
      int width = bounds.getWidth();
      int height = bounds.getHeight();
      Canvas subcanvas1 = canvas.createSubcanvas(0, this.top, this.left, height);
      subcanvas1.offset(0, -deltaY);
      this.leftHeader.draw(subcanvas1);
      Canvas subcanvas2 = canvas.createSubcanvas(this.left, 0, width, this.top);
      subcanvas2.offset(-deltaX, 0);
      this.topHeader.draw(subcanvas2);
      Color primarY2 = Style.PRIMARY2;
      this.drawHorizontalScrollBar(canvas, width, height, primarY2);
      this.drawVerticalScrollBar(canvas, width, height, primarY2);
      Canvas subcanvas3 = canvas.createSubcanvas(this.left, this.top, width, height);
      subcanvas3.offset(-deltaX, -deltaY);
      if (AbstractView.debug)
        canvas.drawRectangle(bounds.getX(), bounds.getY(), bounds.getWidth(), bounds.getHeight(), Color.DEBUG_DRAW_BOUNDS);
      this.wrappedView.draw(subcanvas3);
      if (!AbstractView.debug)
        return;
      Size size = this.getSize();
      canvas.drawRectangle(0, 0, size.getWidth(), size.getHeight(), Color.DEBUG_VIEW_BOUNDS);
      canvas.drawLine(0, size.getHeight() / 2, size.getWidth() - 1, size.getHeight() / 2, Color.DEBUG_VIEW_BOUNDS);
      canvas.drawLine(0, this.getBaseline(), size.getWidth() - 1, this.getBaseline(), Color.DEBUG_BASELINE);
    }

    private void drawVerticalScrollBar(
      Canvas canvas,
      int contentWidth,
      int contentHeight,
      Color color)
    {
      int visibleAmount = this.verticalScrollBar.getVisibleAmount();
      int position = this.verticalScrollBar.getPosition();
      if (this.right <= 0 || position <= this.top && visibleAmount >= contentHeight)
        return;
      canvas.drawSolidRectangle(contentWidth + this.left + 1, this.top, 15, contentHeight, Style.SECONDARY3);
      canvas.drawSolidRectangle(contentWidth + this.left + 1, this.top + position, 14, visibleAmount, color);
      canvas.drawRectangle(contentWidth + this.left, this.top, 16, contentHeight, Style.SECONDARY2);
      canvas.drawRectangle(contentWidth + this.left + 1, this.top + position, 14, visibleAmount, Style.SECONDARY1);
    }

    private void drawHorizontalScrollBar(
      Canvas canvas,
      int contentWidth,
      int contentHeight,
      Color color)
    {
      int position = this.horizontalScrollBar.getPosition();
      int visibleAmount = this.horizontalScrollBar.getVisibleAmount();
      if (this.bottom <= 0 || position <= this.left && visibleAmount >= contentWidth)
        return;
      canvas.drawSolidRectangle(this.left, contentHeight + this.top + 1, contentWidth, 15, Style.SECONDARY3);
      canvas.drawSolidRectangle(this.left + position, contentHeight + this.top + 1, visibleAmount, 14, color);
      canvas.drawRectangle(this.left, contentHeight + this.top, contentWidth, 16, Style.SECONDARY2);
      canvas.drawRectangle(this.left + position, contentHeight + this.top + 1, visibleAmount, 14, Style.SECONDARY1);
    }

    public override void firstClick(Click click)
    {
      switch (this.adjust(click))
      {
        case 1:
          this.topHeader.firstClick(click);
          break;
        case 2:
          this.leftHeader.firstClick(click);
          break;
        case 3:
          this.wrappedView.firstClick(click);
          break;
        case 4:
          this.verticalScrollBar.firstClick(click.getLocation().getY(), click.button3());
          break;
        case 5:
          this.horizontalScrollBar.firstClick(click.getLocation().getX(), click.button3());
          break;
      }
    }

    public override Location getAbsoluteLocation()
    {
      Location absoluteLocation = base.getAbsoluteLocation();
      absoluteLocation.subtract(this.offset());
      return absoluteLocation;
    }

    public override Bounds getBounds() => new Bounds(this.getLocation(), this.getSize());

    public override Size getRequiredSize(Size maximumSize)
    {
      if (!this.requiredSizeValid)
      {
        Size requiredSize = this.wrappedView.getRequiredSize(new Size(maximumSize));
        if (requiredSize.getWidth() > maximumSize.getWidth())
          requiredSize.extendHeight(16);
        if (requiredSize.getHeight() > maximumSize.getHeight())
          requiredSize.extendWidth(16);
        requiredSize.extend(this.left, this.top);
        requiredSize.limitSize(maximumSize);
        this.requiredSize = requiredSize;
      }
      return this.requiredSize;
    }

    public override Size getSize() => new Size(this.size);

    public override View identify(Location location)
    {
      this.getViewManager().getSpy().addTrace((View) this, "mouse location within border", (object) location);
      this.getViewManager().getSpy().addTrace((View) this, "non border area", (object) this.viewportArea());
      switch (this.adjust(location))
      {
        case 1:
          return this.topHeader.identify(location);
        case 2:
          return this.leftHeader.identify(location);
        case 3:
          return this.wrappedView.identify(location);
        case 4:
          this.getViewManager().getSpy().addTrace((View) this, "over scroll bar area", (object) this.viewportArea());
          return this.getView();
        case 5:
          this.getViewManager().getSpy().addTrace((View) this, "over scroll bar area", (object) this.viewportArea());
          return this.getView();
        default:
          return (View) null;
      }
    }

    public override void limitBoundsWithin(Size size)
    {
      base.limitBoundsWithin(size);
      this.verticalScrollBar.limit();
      this.horizontalScrollBar.limit();
    }

    public override void markDamaged(Bounds bounds)
    {
      Offset offset = this.offset();
      bounds.translate(-offset.getDeltaX(), -offset.getDeltaY());
      bounds.translate(this.left, this.top);
      base.markDamaged(bounds);
    }

    public override void mouseMoved(Location location)
    {
      switch (this.adjust(location))
      {
        case 1:
          this.topHeader.mouseMoved(location);
          break;
        case 2:
          this.leftHeader.mouseMoved(location);
          break;
        case 3:
          this.wrappedView.mouseMoved(location);
          break;
      }
    }

    public override void mouseWheelMoved(MouseWheelEvent evt)
    {
      this.verticalScrollBar.scrollClicks(evt.getWheelRotation());
      this.markDamaged();
      ((InputEvent) evt).consume();
    }

    public override void keyPressed(KeyboardAction key)
    {
      base.keyPressed(key);
      this.checkActiveSubviewChanged();
    }

    public override void keyTyped(char keyCode)
    {
      base.keyTyped(keyCode);
      this.checkActiveSubviewChanged();
    }

    private void checkActiveSubviewChanged()
    {
      View activeSubview = this.wrappedView.getActiveSubview();
      if (activeSubview == this.activeSubviewView)
        return;
      if (activeSubview != null)
      {
        Offset offset = this.offset();
        Bounds bounds1 = this.viewportArea();
        int num1 = bounds1.getY() + offset.getDeltaY();
        int num2 = bounds1.getY2() + offset.getDeltaY();
        Bounds bounds2 = activeSubview.getBounds();
        if (bounds2.getY2() > num2)
        {
          this.verticalScrollBar.scroll(Utilities.doubleToInt((double) (bounds2.getY2() - num2) * 0.25));
          this.markDamaged();
        }
        else if (bounds2.getY() < num1)
        {
          this.verticalScrollBar.scroll(Utilities.doubleToInt((double) (bounds2.getY() - num1) * 0.25));
          this.markDamaged();
        }
      }
      this.activeSubviewView = activeSubview;
    }

    private Offset offset()
    {
      Bounds bounds = this.viewportArea();
      int width = bounds.getWidth();
      int dx = width != 0 ? this.horizontalScrollBar.getPosition() * this.wrappedView.getRequiredSize(new Size()).getWidth() / width : 0;
      int height = bounds.getHeight();
      int dy = height != 0 ? this.verticalScrollBar.getPosition() * this.wrappedView.getRequiredSize(new Size()).getHeight() / height : 0;
      return new Offset(dx, dy);
    }

    [JavaFlags(4)]
    public virtual bool overContent(Location location) => this.viewportArea().contains(location);

    public virtual void reset()
    {
      this.horizontalScrollBar.reset();
      this.verticalScrollBar.reset();
    }

    public override void secondClick(Click click)
    {
      switch (this.adjust(click))
      {
        case 1:
          this.topHeader.secondClick(click);
          break;
        case 2:
          this.leftHeader.secondClick(click);
          break;
        case 3:
          this.wrappedView.secondClick(click);
          break;
        case 4:
          this.verticalScrollBar.secondClick(click.getLocation().getY());
          break;
        case 5:
          this.horizontalScrollBar.secondClick(click.getLocation().getX());
          break;
      }
    }

    public override void thirdClick(Click click)
    {
      switch (this.adjust(click))
      {
        case 1:
          this.topHeader.thirdClick(click);
          break;
        case 2:
          this.leftHeader.thirdClick(click);
          break;
        case 3:
          this.wrappedView.thirdClick(click);
          break;
      }
    }

    public override void setBounds(Bounds bounds)
    {
      this.setLocation(bounds.getLocation());
      this.setSize(bounds.getSize());
    }

    public override void setMaximumSize(Size size)
    {
      Size size1 = new Size(size);
      size1.contract(this.left, this.top);
      this.wrappedView.setMaximumSize(size1);
    }

    public override void setSize(Size size)
    {
      this.size = new Size(size);
      Size requiredSize = this.wrappedView.getRequiredSize(new Size());
      this.wrappedView.setSize(requiredSize);
      this.right = size.getHeight() - this.top < requiredSize.getHeight() ? 16 : 0;
      this.bottom = size.getWidth() - this.left < requiredSize.getWidth() ? 16 : 0;
      Bounds bounds = this.viewportArea();
      int height = bounds.getHeight();
      int num1 = Math.max(height, requiredSize.getHeight());
      this.verticalScrollBar.setSize(height, num1);
      if (this.leftHeader != null)
        this.leftHeader.setSize(new Size(this.left, num1));
      int width = bounds.getWidth();
      int num2 = Math.max(width, requiredSize.getWidth());
      this.horizontalScrollBar.setSize(width, num2);
      if (this.topHeader == null)
        return;
      this.topHeader.setSize(new Size(num2, this.top));
    }

    public virtual int getVerticalPosition() => this.verticalScrollBar.getPosition();

    public virtual int getHorizontalPosition() => this.horizontalScrollBar.getPosition();

    public override ViewAreaType viewAreaType(Location location)
    {
      switch (this.adjust(location))
      {
        case 1:
          return this.topHeader.viewAreaType(location);
        case 2:
          return this.leftHeader.viewAreaType(location);
        case 3:
          return this.wrappedView.viewAreaType(location);
        default:
          return ViewAreaType.INTERNAL;
      }
    }

    public override void viewMenuOptions(UserActionSet menuOptions)
    {
      base.viewMenuOptions(menuOptions);
      menuOptions.add((UserAction) new ScrollBorder.\u0031(this, "Reset scroll border"));
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : AbstractUserAction
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ScrollBorder this\u00240;

      public override void execute(Workspace workspace, View view, Location at)
      {
        this.this\u00240.reset();
        this.this\u00240.invalidateLayout();
      }

      public \u0031(ScrollBorder _param1, string dummy0)
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
