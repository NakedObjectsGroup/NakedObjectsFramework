// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.WindowBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class WindowBorder : AbstractBorder
  {
    private static readonly Text TITLE_STYLE;
    private int baseline;
    private int padding;
    private int thickness;
    private IconGraphic icon;
    private TitleText text;

    public WindowBorder(int size, View wrappedView)
      : base(wrappedView)
    {
      this.padding = 2;
      this.thickness = size;
      this.icon = new IconGraphic((View) this, WindowBorder.TITLE_STYLE);
      this.text = (TitleText) new ObjectTitleText((View) this, WindowBorder.TITLE_STYLE);
      int height = this.icon.getSize().getHeight();
      this.baseline = this.icon.getBaseline();
      this.left = size;
      this.right = size;
      this.top = size + this.padding + height + this.padding + 1;
      this.bottom = size;
    }

    public WindowBorder(View wrappedView)
      : this(5, wrappedView)
    {
    }

    public override void debugDetails(StringBuffer b)
    {
      b.append(new StringBuffer().append("WindowBorder ").append(this.left).append(" pixels\n").ToString());
      b.append(new StringBuffer().append("           titlebar ").append(this.top - this.thickness).append(" pixels").ToString());
    }

    public override void draw(Canvas canvas)
    {
      Size size = this.getSize();
      int left = this.left;
      int thickness = this.thickness;
      int width = size.getWidth();
      bool flag = this.getState().isObjectIdentified();
      for (int index = 0; index < this.left; ++index)
        canvas.drawRectangle(index, index, width - 2 * index - 1, size.getHeight() - 2 * index - 1, !flag ? Style.SECONDARY2 : Style.PRIMARY1);
      canvas.drawSolidRectangle(left, thickness, width - this.left - this.right - 1, this.top - thickness - 1, !flag ? Style.SECONDARY3 : Style.PRIMARY3);
      canvas.drawLine(left, this.top - 1, width - this.right, this.top - 1, !flag ? Style.SECONDARY2 : Style.PRIMARY1);
      this.icon.draw(canvas, left, this.baseline + this.thickness);
      int x1 = left + this.icon.getSize().getWidth();
      this.text.draw(canvas, x1, this.baseline + this.thickness);
      int x2 = x1 + (this.text.getSize().getWidth() + this.padding);
      if (this.getState().isViewIdentified() || this.getState().isObjectIdentified())
      {
        int x2_1 = width - this.right - 2;
        for (int index = this.thickness + 2; index < this.top - 2; index += 3)
          canvas.drawLine(x2, index, x2_1, index, Style.SECONDARY2);
      }
      base.draw(canvas);
    }

    public override int getBaseline() => this.wrappedView.getBaseline() + this.baseline + this.thickness;

    public override Size getRequiredSize(Size maximumSize)
    {
      Size requiredSize = base.getRequiredSize(maximumSize);
      requiredSize.ensureWidth(this.left + this.icon.getSize().getWidth() + this.text.getSize().getWidth() + this.padding + this.right);
      return requiredSize;
    }

    public override void secondClick(Click click)
    {
      if (this.viewAreaType(click.getLocation()) == ViewAreaType.VIEW)
      {
        View view = new RootIconSpecification().createView(this.getContent(), (ViewAxis) null);
        view.setLocation(this.getView().getLocation());
        this.getWorkspace().replaceView(this.getView(), view);
      }
      else
        base.secondClick(click);
    }

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/WindowBorder [width=").append(this.left).append("]").ToString();

    public override ViewAreaType viewAreaType(Location mouseLocation) => new Bounds(this.thickness, this.thickness, this.icon.getSize().getWidth() + this.text.getSize().getWidth(), this.top - this.thickness).contains(mouseLocation) ? ViewAreaType.CONTENT : base.viewAreaType(mouseLocation);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static WindowBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
