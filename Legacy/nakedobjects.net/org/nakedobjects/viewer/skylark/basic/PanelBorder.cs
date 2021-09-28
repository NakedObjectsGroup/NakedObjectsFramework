// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.PanelBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class PanelBorder : LineBorder
  {
    private Color background;

    public PanelBorder(View wrappedView)
      : base(wrappedView)
    {
      this.background = Style.WHITE;
    }

    public PanelBorder(int size, Color border, Color background, View wrappedView)
      : base(size, border, wrappedView)
    {
      this.background = background;
    }

    public PanelBorder(int size, View wrappedView)
      : base(size, Style.SECONDARY2, wrappedView)
    {
      this.background = Style.WHITE;
    }

    public PanelBorder(Color border, Color background, View wrappedView)
      : base(border, wrappedView)
    {
      this.background = background;
    }

    public override void draw(Canvas canvas)
    {
      canvas.clearBackground((View) this, this.background);
      base.draw(canvas);
    }

    public virtual void setBackground(Color color) => this.background = color;

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/PanelBorder").ToString();
  }
}
