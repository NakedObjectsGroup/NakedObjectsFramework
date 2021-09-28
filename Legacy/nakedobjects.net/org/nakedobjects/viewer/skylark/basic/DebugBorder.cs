// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.DebugBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class DebugBorder : AbstractBorder
  {
    public DebugBorder(View wrappedView)
      : base(wrappedView)
    {
      this.bottom = Style.DEBUG.getTextHeight();
    }

    [JavaFlags(4)]
    public override void debugDetails(StringBuffer b) => b.append(nameof (DebugBorder));

    public override void draw(Canvas canvas)
    {
      string text = new StringBuffer().append((object) this.getView()).append(" ").append((object) this.getState()).ToString();
      int y = this.wrappedView.getSize().getHeight() + Style.DEBUG.getAscent();
      Color debugBaseline = Color.DEBUG_BASELINE;
      canvas.drawText(text, 0, y, debugBaseline, Style.DEBUG);
      base.draw(canvas);
    }

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/DebugBorder").ToString();

    public override void firstClick(Click click) => new DebugOption().execute(this.getWorkspace(), this.getView(), click.getLocation());
  }
}
