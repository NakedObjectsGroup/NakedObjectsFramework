// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.LabelBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class LabelBorder : AbstractBorder
  {
    private readonly string label;
    private readonly Text style;

    public LabelBorder(string label, View wrappedView)
      : this(label, Style.LABEL, wrappedView)
    {
    }

    public LabelBorder(string label, Text style, View wrappedView)
      : base(wrappedView)
    {
      this.label = new StringBuffer().append(label).append(":").ToString();
      this.style = style;
      int width = View.HPADDING + style.stringWidth(this.label) + View.HPADDING;
      if (this.getViewAxis() == null)
        this.left = width;
      else
        ((LabelAxis) this.getViewAxis()).accommodateWidth(width);
    }

    [JavaFlags(4)]
    public override int getLeft() => this.getViewAxis() == null ? this.left : ((LabelAxis) this.getViewAxis()).getWidth();

    public override void debugDetails(StringBuffer b) => b.append(new StringBuffer().append("Label '").append(this.label).ToString());

    public override void draw(Canvas canvas)
    {
      canvas.drawText(this.label, View.HPADDING, this.wrappedView.getBaseline(), Style.PRIMARY1, this.style);
      base.draw(canvas);
    }

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/").append(org.nakedobjects.utility.ToString.name((object) this)).ToString();
  }
}
