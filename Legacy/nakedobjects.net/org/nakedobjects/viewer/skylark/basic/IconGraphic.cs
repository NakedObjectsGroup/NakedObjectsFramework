// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.IconGraphic
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.util;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class IconGraphic
  {
    private int baseline;
    private Content content;
    private Image icon;
    private int iconHeight;
    private string lastIconName;

    public IconGraphic(View view, int height, int baseline)
    {
      this.content = view.getContent();
      this.iconHeight = height;
      this.baseline = baseline;
    }

    public IconGraphic(View view, Text style)
    {
      this.content = view.getContent();
      this.iconHeight = style.getTextHeight();
      this.baseline = style.getAscent();
    }

    public virtual void draw(Canvas canvas, int x, int baseline)
    {
      int y = baseline - this.getBaseline();
      if (AbstractView.debug)
        canvas.drawDebugOutline(new Bounds(new Location(x, y), this.getSize()), this.getBaseline(), Color.DEBUG_DRAW_BOUNDS);
      Image icon = this.icon();
      if (icon == null)
        canvas.drawSolidOval(x, y, this.iconHeight, this.iconHeight, Style.PRIMARY3);
      else
        canvas.drawIcon(icon, x, y);
    }

    public virtual int getBaseline() => this.baseline;

    public virtual Size getSize()
    {
      Image mage = this.icon();
      return new Size(mage != null ? mage.getWidth() : this.iconHeight, this.iconHeight);
    }

    [JavaFlags(4)]
    public virtual Image icon()
    {
      string iconName = this.content.getIconName();
      if (this.icon != null && (iconName == null || StringImpl.equals(iconName, (object) this.lastIconName)))
        return this.icon;
      this.lastIconName = iconName;
      if (iconName != null)
        this.icon = ImageFactory.getInstance().loadIcon(iconName, this.iconHeight);
      if (this.icon == null)
        this.icon = this.content.getIconPicture(this.iconHeight);
      return this.icon;
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("baseline", this.baseline);
      toString.append("icon", (object) this.icon);
      return toString.ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      IconGraphic iconGraphic = this;
      ObjectImpl.clone((object) iconGraphic);
      return ((object) iconGraphic).MemberwiseClone();
    }
  }
}
