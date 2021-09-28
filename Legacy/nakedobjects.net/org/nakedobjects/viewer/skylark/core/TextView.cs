// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.TextView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.viewer.skylark.core
{
  public class TextView : AbstractView
  {
    private Text style;
    private Color color;
    private string text;

    public TextView(string text)
      : base((Content) null, (ViewSpecification) null, (ViewAxis) null)
    {
      this.style = Style.NORMAL;
      this.color = Style.BLACK;
      this.text = text;
    }

    public override bool canFocus() => false;

    public override void draw(Canvas canvas) => canvas.drawText(this.text, View.HPADDING, this.getBaseline(), this.color, this.style);

    public override int getBaseline() => this.style.getAscent() + View.VPADDING;

    public override Size getRequiredSize(Size maximumSize) => new Size(this.style.stringWidth(this.text) + View.HPADDING * 2, this.style.getTextHeight() + View.VPADDING * 2);
  }
}
