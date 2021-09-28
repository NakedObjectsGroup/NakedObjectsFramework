// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.IconView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class IconView : ObjectView
  {
    private IconGraphic icon;
    private TitleText text;

    public IconView(Content content, ViewSpecification specification, ViewAxis axis, Text style)
      : base(content, specification, axis)
    {
      this.icon = new IconGraphic((View) this, style);
      this.text = (TitleText) new ObjectTitleText((View) this, style);
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      int x1 = 0;
      int baseline = this.icon.getBaseline();
      this.icon.draw(canvas, x1, baseline);
      int x2 = x1 + this.icon.getSize().getWidth() + View.HPADDING;
      this.text.draw(canvas, x2, baseline);
    }

    public override int getBaseline() => this.icon.getBaseline();

    public override Size getMaximumSize()
    {
      Size size = this.icon.getSize();
      size.extendWidth(View.HPADDING);
      size.extendWidth(this.text.getSize().getWidth());
      return size;
    }

    public override string ToString() => new StringBuffer().append(nameof (IconView)).append(this.getId()).ToString();

    public override void update(Naked @object) => this.getParent()?.invalidateLayout();
  }
}
