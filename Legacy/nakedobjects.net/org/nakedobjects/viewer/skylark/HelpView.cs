// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.HelpView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.text;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("2;org/nakedobjects/viewer/skylark/View;org/nakedobjects/viewer/skylark/text/TextBlockTarget;")]
  public class HelpView : AbstractView, View, TextBlockTarget
  {
    private const int HEIGHT = 350;
    private const int WIDTH = 400;
    private TextContent content;

    public HelpView(string name, string description, string help)
    {
      string text = new StringBuffer().append(name == null || StringImpl.equals(StringImpl.trim(name), (object) "") ? "" : new StringBuffer().append(name).append("\n").ToString()).append(description == null || StringImpl.equals(StringImpl.trim(description), (object) "") ? "" : new StringBuffer().append(description).append("\n").ToString()).append(help != null ? help : "").ToString();
      this.content = new TextContent((TextBlockTarget) this, 10, 0);
      this.content.setText(text);
    }

    public override void draw(Canvas canvas)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = this.getSize().getWidth() - 1;
      int num4 = this.getSize().getHeight() - 1;
      int num5 = 9;
      canvas.drawSolidRectangle(num1 + 2, num2 + 2, num3 - 4, num4 - 4, Style.WHITE);
      Canvas canvas1 = canvas;
      int x1 = num1;
      int num6;
      int num7 = (num6 = num2) + 1;
      int y1 = num6;
      int width1 = num3;
      int height1 = num4;
      int arcWidth1 = num5;
      int arcHeight1 = num5;
      Color black1 = Style.BLACK;
      canvas1.drawRoundedRectangle(x1, y1, width1, height1, arcWidth1, arcHeight1, black1);
      Canvas canvas2 = canvas;
      int x2 = num1 + 1;
      int num8;
      int num9 = (num8 = num7) + 1;
      int y2 = num8;
      int width2 = num3 - 2;
      int height2 = num4 - 2;
      int arcWidth2 = num5;
      int arcHeight2 = num5;
      Color secondarY2 = Style.SECONDARY2;
      canvas2.drawRoundedRectangle(x2, y2, width2, height2, arcWidth2, arcHeight2, secondarY2);
      Canvas canvas3 = canvas;
      int x3 = num1 + 2;
      int num10;
      int num11 = (num10 = num9) + 1;
      int y3 = num10;
      int width3 = num3 - 4;
      int height3 = num4 - 4;
      int arcWidth3 = num5;
      int arcHeight3 = num5;
      Color black2 = Style.BLACK;
      canvas3.drawRoundedRectangle(x3, y3, width3, height3, arcWidth3, arcHeight3, black2);
      int x4 = num1 + 10;
      int y4 = num11 + View.VPADDING + Style.TITLE.getTextHeight();
      canvas.drawText("Help", x4, y4, Style.BLACK, Style.TITLE);
      foreach (string displayLine in this.content.getDisplayLines())
      {
        y4 += Style.NORMAL.getLineHeight();
        canvas.drawText(displayLine, x4, y4, Style.BLACK, Style.NORMAL);
      }
    }

    public override Size getMaximumSize() => new Size(400, 350);

    public override Size getRequiredSize(Size maximumSize)
    {
      int height = Math.min(350, maximumSize.getHeight());
      return new Size(Math.min(400, maximumSize.getWidth()), height);
    }

    public override void firstClick(Click click) => this.getViewManager().clearOverlayView((View) this);

    public virtual int getMaxWidth() => 380;

    public virtual Text getText() => Style.NORMAL;
  }
}
