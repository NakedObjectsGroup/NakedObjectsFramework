// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.IconBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.metal
{
  public class IconBorder : AbstractBorder
  {
    private static readonly Text TITLE_STYLE;
    private int baseline;
    private int titlebarHeight;
    private int padding;
    private IconGraphic icon;
    private TitleText text;

    public IconBorder(View wrappedView)
      : base(wrappedView)
    {
      this.padding = 0;
      this.icon = new IconGraphic((View) this, IconBorder.TITLE_STYLE);
      this.text = (TitleText) new ObjectTitleText((View) this, IconBorder.TITLE_STYLE);
      this.titlebarHeight = this.icon.getSize().getHeight() + 1;
      this.top = this.titlebarHeight;
      this.baseline = this.icon.getBaseline() + 1;
    }

    public override void debugDetails(StringBuffer b)
    {
      b.append(new StringBuffer().append("IconBorder ").append(this.left).append(" pixels\n").ToString());
      b.append(new StringBuffer().append("           titlebar ").append(this.top - this.titlebarHeight).append(" pixels").ToString());
      base.debugDetails(b);
    }

    public override Drag dragStart(DragStart drag)
    {
      if (!this.overBorder(drag.getLocation()))
        return base.dragStart(drag);
      View dragView = (View) new DragContentIcon(this.getContent());
      return (Drag) new ContentDrag((View) this, drag.getLocation(), dragView);
    }

    public override void draw(Canvas canvas)
    {
      int x1 = this.left + View.HPADDING;
      if (AbstractView.debug)
        canvas.drawDebugOutline(new Bounds(this.getSize()), this.baseline, Color.DEBUG_DRAW_BOUNDS);
      this.icon.draw(canvas, x1, this.baseline);
      int x2 = x1 + this.icon.getSize().getWidth() + View.HPADDING;
      this.text.draw(canvas, x2, this.baseline);
      base.draw(canvas);
    }

    public override int getBaseline() => this.wrappedView.getBaseline() + this.baseline + this.titlebarHeight;

    public override Size getRequiredSize(Size maximumSize)
    {
      Size requiredSize = base.getRequiredSize(maximumSize);
      requiredSize.ensureWidth(this.left + this.icon.getSize().getWidth() + View.HPADDING + this.text.getSize().getWidth() + this.padding + this.right);
      return requiredSize;
    }

    public override ViewAreaType viewAreaType(Location mouseLocation)
    {
      Bounds bounds = new Bounds(new Location(), this.icon.getSize());
      bounds.extendWidth(this.left);
      bounds.extendWidth(this.text.getSize().getWidth());
      return bounds.contains(mouseLocation) ? ViewAreaType.CONTENT : base.viewAreaType(mouseLocation);
    }

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/WindowBorder [").append((object) this.getSpecification()).append("]").ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static IconBorder()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
