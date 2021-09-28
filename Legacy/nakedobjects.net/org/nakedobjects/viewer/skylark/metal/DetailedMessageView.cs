// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.DetailedMessageView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaFlags(32)]
  public class DetailedMessageView : AbstractView
  {
    private const string NEWLINE = "\n\r";
    private const int PADDING = 10;
    private const int MARGIN_LEFT = 20;
    private const int MARGIN_TOP = 15;
    private FocusManager focusManager;

    [JavaFlags(4)]
    public DetailedMessageView(Content content, ViewSpecification specification, ViewAxis axis)
      : base(content, specification, axis)
    {
    }

    public override Size getMaximumSize()
    {
      Size size = new Size();
      int textHeight1 = Style.TITLE.getTextHeight();
      int textHeight2 = Style.NORMAL.getTextHeight();
      string str1 = StringImpl.trim(this.getContent().title());
      string str2 = StringImpl.trim(((MessageContent) this.getContent()).getMessage());
      string str3 = StringImpl.trim(((MessageContent) this.getContent()).getDetail());
      if (StringImpl.startsWith(str1, "\n\r"))
        str1 = StringImpl.substring(str1, 2);
      if (StringImpl.startsWith(str2, "\n\r"))
        str2 = StringImpl.substring(str2, 2);
      if (StringImpl.startsWith(str3, "\n\r"))
        str3 = StringImpl.substring(str3, 2);
      size.setHeight(15);
      size.setWidth(20);
      if (StringImpl.length(str1) > 0)
      {
        StringTokenizer stringTokenizer = new StringTokenizer(str1, "\n\r");
        while (stringTokenizer.hasMoreTokens())
        {
          size.extendHeight(textHeight1);
          string text = stringTokenizer.nextToken();
          size.ensureWidth(20 + (!StringImpl.startsWith(text, "\t") ? 0 : 20) + Style.TITLE.stringWidth(text));
        }
      }
      if (StringImpl.length(str2) > 0)
      {
        size.extendHeight(textHeight2);
        StringTokenizer stringTokenizer = new StringTokenizer(str2, "\n\r");
        while (stringTokenizer.hasMoreTokens())
        {
          size.extendHeight(textHeight2);
          string text = stringTokenizer.nextToken();
          size.ensureWidth(20 + (!StringImpl.startsWith(text, "\t") ? 0 : 20) + Style.NORMAL.stringWidth(text));
        }
      }
      if (StringImpl.length(str3) > 0)
      {
        size.extendHeight(textHeight2);
        StringTokenizer stringTokenizer = new StringTokenizer(str3, "\n\r");
        while (stringTokenizer.hasMoreTokens())
        {
          size.extendHeight(textHeight2);
          string text = stringTokenizer.nextToken();
          size.ensureWidth(20 + (!StringImpl.startsWith(text, "\t") ? 0 : 20) + Style.NORMAL.stringWidth(text));
        }
      }
      size.extendHeight(textHeight2);
      size.extend(20, 15);
      return size;
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      int x = 0;
      int y1 = 0;
      int width = this.getSize().getWidth();
      int height = this.getSize().getHeight();
      int textHeight1 = Style.TITLE.getTextHeight();
      int textHeight2 = Style.NORMAL.getTextHeight();
      string str1 = this.getContent().title();
      string str2 = ((MessageContent) this.getContent()).getMessage();
      string str3 = ((MessageContent) this.getContent()).getDetail();
      if (StringImpl.startsWith(str1, "\n\r"))
        str1 = StringImpl.substring(str1, 2);
      if (StringImpl.startsWith(str2, "\n\r"))
        str2 = StringImpl.substring(str2, 2);
      if (StringImpl.startsWith(str3, "\n\r"))
        str3 = StringImpl.substring(str3, 2);
      canvas.drawSolidRectangle(x, y1, width - 1, height - 1, Style.WHITE);
      canvas.drawRectangle(x, y1, width - 1, height - 1, Style.BLACK);
      int y2 = 15;
      int num1 = 20;
      if (StringImpl.length(str1) > 0)
      {
        StringTokenizer stringTokenizer = new StringTokenizer(str1, "\n\r");
        while (stringTokenizer.hasMoreTokens())
        {
          y2 += textHeight1;
          string text = stringTokenizer.nextToken();
          canvas.drawText(text, num1 + (!StringImpl.startsWith(text, "\t") ? 0 : 20), y2, Color.RED, Style.TITLE);
        }
      }
      int num2 = 20;
      if (StringImpl.length(str2) > 0)
      {
        y2 += textHeight2;
        StringTokenizer stringTokenizer = new StringTokenizer(str2, "\n\r");
        while (stringTokenizer.hasMoreTokens())
        {
          y2 += textHeight2;
          string text = stringTokenizer.nextToken();
          canvas.drawText(text, num2 + (!StringImpl.startsWith(text, "\t") ? 0 : 20), y2, Color.RED, Style.NORMAL);
        }
      }
      int num3 = 20;
      if (StringImpl.length(str3) <= 0)
        return;
      int y3 = y2 + textHeight2;
      StringTokenizer stringTokenizer1 = new StringTokenizer(str3, "\n\r");
      while (stringTokenizer1.hasMoreTokens())
      {
        y3 += textHeight2;
        string text = stringTokenizer1.nextToken();
        canvas.drawText(text, num3 + (!StringImpl.startsWith(text, "\t") ? 0 : 20), y3, Color.BLACK, Style.NORMAL);
      }
    }

    public override ViewAreaType viewAreaType(Location mouseLocation) => ViewAreaType.VIEW;

    public override FocusManager getFocusManager() => this.focusManager;

    public override void setFocusManager(FocusManager focusManager) => this.focusManager = focusManager;
  }
}
