// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.AbstractWindowBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  public abstract class AbstractWindowBorder : ResizeBorder
  {
    [JavaFlags(28)]
    public const int LINE_THICKNESS = 5;
    private static readonly Text TITLE_STYLE;
    private readonly int baseline;
    private readonly int titlebarHeight;
    [JavaFlags(4)]
    public WindowControl[] controls;
    private bool maximised;
    private Size nonMaximisedSize;
    private Location nonMaximisedLocation;
    private int nonMaximisedAllowDirections;

    public AbstractWindowBorder(View enclosedView)
      : base(enclosedView, 15, 5, 5)
    {
      this.titlebarHeight = Math.max(13 + View.VPADDING + AbstractWindowBorder.TITLE_STYLE.getDescent(), AbstractWindowBorder.TITLE_STYLE.getTextHeight());
      this.baseline = 18;
      this.left = 5;
      this.right = 5;
      this.top = 5 + this.titlebarHeight;
      this.bottom = 5;
    }

    public override void debugDetails(StringBuffer b)
    {
      b.append(new StringBuffer().append("WindowBorder ").append(this.left).append(" pixels\n").ToString());
      b.append(new StringBuffer().append("           titlebar ").append(this.top - this.titlebarHeight).append(" pixels").ToString());
      base.debugDetails(b);
    }

    public override Drag dragStart(DragStart drag)
    {
      if (!this.overTitlebar(drag.getLocation()))
        return base.dragStart(drag);
      Location location = drag.getLocation();
      DragViewOutline dragViewOutline = new DragViewOutline(this.getView());
      return (Drag) new ViewDrag((View) this, new Offset(location.getX(), location.getY()), (View) dragViewOutline);
    }

    [JavaFlags(4)]
    public virtual bool overTitlebar(Location location) => location.getY() > 5 && location.getY() < 5 + this.titlebarHeight;

    [JavaFlags(4)]
    public override int onBorder(Location at)
    {
      Size size = this.getSize();
      int width = size.getWidth();
      int height = size.getHeight();
      bool flag1 = this.canExtend(2) && at.getX() >= width - this.getLeft() - this.getRight();
      bool flag2 = this.canExtend(8) && at.getY() >= height - this.getBottom() && at.getY() <= height;
      return !flag1 || !flag2 ? (!flag1 ? (!flag2 ? 0 : 2) : 4) : 8;
    }

    [JavaFlags(4)]
    public virtual void setControls(WindowControl[] controls) => this.controls = controls;

    public override void setSize(Size size)
    {
      base.setSize(size);
      this.layoutControls(size.getWidth());
    }

    public override void setBounds(Bounds bounds)
    {
      base.setBounds(bounds);
      this.layoutControls(bounds.getWidth());
    }

    private void layoutControls(int width)
    {
      width -= this.getPadding().getRight();
      int x = width - (15 + View.HPADDING) * this.controls.Length;
      int y = 5 + View.VPADDING;
      for (int index = 0; index < this.controls.Length; ++index)
      {
        this.controls[index].setSize(this.controls[index].getRequiredSize(new Size()));
        this.controls[index].setLocation(new Location(x, y));
        x += this.controls[index].getSize().getWidth() + View.HPADDING;
      }
    }

    public override void draw(Canvas canvas)
    {
      Size size = this.getSize();
      int left1 = this.left;
      int width = size.getWidth();
      int height = size.getHeight();
      Bounds bounds = this.getBounds();
      canvas.drawSolidRectangle(3, 3, bounds.getWidth() - 6, bounds.getHeight() - 6, Style.background(this.getSpecification()));
      bool hasFocus = this.containsFocus();
      if (this.getState().isActive())
      {
        int left2 = this.left;
        canvas.drawRectangle(left2, this.top, width - 2 * left2, height - 2 * left2 - this.top, Style.ACTIVE);
      }
      Color color = !hasFocus ? Style.SECONDARY3 : Style.SECONDARY2;
      canvas.drawSolidRectangle(this.left, 5, width - this.left - this.right, this.titlebarHeight, color);
      int num = 5 + this.titlebarHeight - 1;
      canvas.drawLine(left1, num, width - this.right - 1, num, this.getLightColor(hasFocus));
      canvas.drawText(this.title(), left1 + View.HPADDING, this.baseline, this.getDarkColor(hasFocus), AbstractWindowBorder.TITLE_STYLE);
      for (int index = 0; this.controls != null && index < this.controls.Length; ++index)
      {
        Canvas subcanvas = canvas.createSubcanvas(this.controls[index].getBounds());
        this.controls[index].draw(subcanvas);
      }
      base.draw(canvas);
    }

    public virtual void setMaximised(bool maximised)
    {
      if (this.maximised == maximised)
        return;
      this.maximised = maximised;
      if (maximised)
      {
        this.nonMaximisedSize = this.getSize();
        this.nonMaximisedLocation = this.getLocation();
        this.nonMaximisedAllowDirections = this.getAllowDirections();
        this.maximise();
      }
      else
        this.setMaximisedSettings(this.nonMaximisedSize, this.nonMaximisedLocation, this.nonMaximisedAllowDirections);
      this.getWorkspace().markDamaged();
    }

    public virtual bool getMaximised() => this.maximised;

    private void setMaximisedSettings(Size newSize, Location newLocation, int allowDirections)
    {
      this.setSize(newSize);
      this.setLocation(newLocation);
      this.setAllowDirections(allowDirections);
    }

    private void maximise() => this.setMaximisedSettings(this.getWorkspace().getSize(), this.getWorkspace().getLocation(), 0);

    public override void layout(Size maximumSize)
    {
      if (this.maximised)
        this.maximise();
      else
        base.layout(maximumSize);
    }

    private Color getLightColor(bool hasFocus) => hasFocus ? Style.SECONDARY1 : Style.SECONDARY2;

    private Color getDarkColor(bool hasFocus) => hasFocus ? Style.BLACK : Style.SECONDARY1;

    public override void drawResizeBorder(Canvas canvas, Size size)
    {
      Size size1 = this.getSize();
      int width = size1.getWidth();
      int height = size1.getHeight();
      bool flag = this.containsFocus();
      Color color1 = !flag ? Style.SECONDARY2 : Style.SECONDARY1;
      Color color2 = !flag ? Style.SECONDARY1 : Style.BLACK;
      if (!this.maximised)
      {
        canvas.drawRectangle(1, 0, width - 2, height, color1);
        canvas.drawRectangle(0, 1, width, height - 2, color1);
      }
      for (int index = 2; index < this.left; ++index)
        canvas.drawRectangle(index, index, width - 2 * index, height - 2 * index, color1);
      canvas.drawLine(2, 15, 2, height - 15, color2);
      canvas.drawLine(3, 16, 3, height - 14, Style.PRIMARY1);
      canvas.drawLine(width - 3, 15, width - 3, height - 15, color2);
      canvas.drawLine(width - 2, 16, width - 2, height - 14, Style.PRIMARY1);
      canvas.drawLine(15, 2, width - 15, 2, color2);
      canvas.drawLine(16, 3, width - 14, 3, Style.PRIMARY1);
      canvas.drawLine(15, height - 3, width - 15, height - 3, color2);
      canvas.drawLine(16, height - 2, width - 14, height - 2, Style.PRIMARY1);
    }

    public override void markDamaged(Bounds bounds) => base.markDamaged(this.getBounds());

    [JavaFlags(1028)]
    public abstract string title();

    public override Size getRequiredSize(Size maximumSize)
    {
      Size requiredSize = base.getRequiredSize(maximumSize);
      int width = this.getLeft() + View.HPADDING + AbstractWindowBorder.TITLE_STYLE.stringWidth(this.title()) + View.HPADDING + this.controls.Length * (15 + View.HPADDING) + View.HPADDING + this.getRight();
      requiredSize.ensureWidth(width);
      return requiredSize;
    }

    public override void secondClick(Click click)
    {
      if (this.overControl(click.getLocation()) != null)
        return;
      base.secondClick(click);
    }

    public override void thirdClick(Click click)
    {
      if (this.overControl(click.getLocation()) != null)
        return;
      base.thirdClick(click);
    }

    public override void firstClick(Click click)
    {
      View view = this.overControl(click.getLocation());
      if (view == null)
      {
        Workspace workspace = this.getWorkspace();
        if (workspace != null)
        {
          if (click.button1())
            workspace.raise(this.getView());
          else if (click.button2() && this.overBorder(click.getLocation()))
            workspace.lower(this.getView());
        }
        base.firstClick(click);
      }
      else
        view.firstClick(click);
    }

    private View overControl(Location location)
    {
      for (int index = 0; index < this.controls.Length; ++index)
      {
        WindowControl control = this.controls[index];
        if (control.getBounds().contains(location))
          return (View) control;
      }
      return (View) null;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static AbstractWindowBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
