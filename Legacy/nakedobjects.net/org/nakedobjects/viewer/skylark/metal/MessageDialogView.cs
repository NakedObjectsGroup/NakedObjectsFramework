// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.MessageDialogView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.util;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaFlags(32)]
  public class MessageDialogView : AbstractView
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    private static Class MessageDialogView\u0024ClassObject;
    private const string NEWLINE = "\n\r";
    private const int PADDING = 10;
    private const int MARGIN_LEFT = 20;
    private const int MARGIN_TOP = 15;
    private Image errorIcon;
    private static int maxMessageLength;
    private static bool configurationRead;
    private FocusManager focusManager;

    [JavaFlags(4)]
    public MessageDialogView(
      MessageContent content,
      ViewSpecification specification,
      ViewAxis axis)
      : base((Content) content, specification, axis)
    {
      string iconName = this.getContent().getIconName();
      this.errorIcon = ImageFactory.getInstance().loadIcon(iconName, 32);
      if (this.errorIcon == null)
        this.errorIcon = ImageFactory.getInstance().loadFallbackIcon(32);
      MessageDialogView.ensureConfigurationRead();
    }

    private string truncate(string message) => MessageDialogView.maxMessageLength > 0 && message != null && StringImpl.length(message) > MessageDialogView.maxMessageLength ? message.Substring(0, MessageDialogView.maxMessageLength - 1) : message;

    [MethodImpl(MethodImplOptions.Synchronized)]
    private static void ensureConfigurationRead()
    {
      object dialogViewClassObject = (object) MessageDialogView.MessageDialogView\u0024ClassObject;
      \u003CCorArrayWrapper\u003E.Enter(dialogViewClassObject);
      try
      {
        if (MessageDialogView.configurationRead)
          return;
        MessageDialogView.configurationRead = true;
        MessageDialogView.maxMessageLength = NakedObjects.getConfiguration().getInteger(new StringBuffer().append("nakedobjects.viewer.skylark.").append("metal.MessageDialogSpecification.maxMessageLength").ToString(), -1);
      }
      finally
      {
        Monitor.Exit(dialogViewClassObject);
      }
    }

    public override Size getMaximumSize()
    {
      Size size = new Size();
      int height = this.errorIcon.getHeight();
      int width = this.errorIcon.getWidth();
      int textHeight1 = Style.TITLE.getTextHeight();
      int textHeight2 = Style.NORMAL.getTextHeight();
      string str1 = StringImpl.trim(this.getContent().title());
      string str2 = this.truncate(StringImpl.trim(((MessageContent) this.getContent()).getMessage()));
      if (StringImpl.startsWith(str1, "\n\r"))
        str1 = StringImpl.substring(str1, 2);
      if (StringImpl.startsWith(str2, "\n\r"))
        str2 = StringImpl.substring(str2, 2);
      size.setHeight(15);
      size.setWidth(20);
      size.extendHeight(height);
      if (StringImpl.length(str1) > 0)
      {
        size.ensureHeight(15 + textHeight1);
        size.contractHeight(textHeight1);
        StringTokenizer stringTokenizer = new StringTokenizer(str1, "\n\r");
        while (stringTokenizer.hasMoreTokens())
        {
          size.extendHeight(textHeight1);
          string text = stringTokenizer.nextToken();
          size.ensureWidth(20 + width + 10 + (!StringImpl.startsWith(text, "\t") ? 0 : 20) + Style.TITLE.stringWidth(text));
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
      size.extendHeight(textHeight2);
      size.extend(20, 15);
      return size;
    }

    public override void draw(Canvas canvas)
    {
      int num1 = 0;
      int height = this.errorIcon.getHeight();
      int width = this.errorIcon.getWidth();
      int textHeight1 = Style.TITLE.getTextHeight();
      int midPoint = Style.TITLE.getMidPoint();
      int textHeight2 = Style.NORMAL.getTextHeight();
      string str1 = StringImpl.trim(this.getContent().title());
      string str2 = this.truncate(StringImpl.trim(((MessageContent) this.getContent()).getMessage()));
      if (StringImpl.startsWith(str1, "\n\r"))
        str1 = StringImpl.substring(str1, 2);
      if (StringImpl.startsWith(str2, "\n\r"))
        str2 = StringImpl.substring(str2, 2);
      canvas.clearBackground((View) this, Style.WHITE);
      int y1 = 15;
      int x = 20;
      canvas.drawIcon(this.errorIcon, x, y1);
      int num2 = y1 + height;
      int num3 = 20 + width + 10;
      if (StringImpl.length(str1) > 0)
      {
        int num4 = Math.max(num2, 15 + textHeight1);
        int num5 = num4;
        int y2 = num4 - height / 2 + midPoint - textHeight1;
        StringTokenizer stringTokenizer = new StringTokenizer(str1, "\n\r");
        while (stringTokenizer.hasMoreTokens())
        {
          y2 += textHeight1;
          string text = stringTokenizer.nextToken();
          canvas.drawText(text, num3 + (!StringImpl.startsWith(text, "\t") ? 0 : 20), y2, Color.BLACK, Style.TITLE);
        }
        num2 = Math.max(y2, num5);
      }
      num1 = 20;
      if (StringImpl.length(str2) > 0)
      {
        int y3 = num2 + textHeight2;
        StringTokenizer stringTokenizer = new StringTokenizer(str2, "\n\r");
        while (stringTokenizer.hasMoreTokens())
        {
          y3 += textHeight2;
          string text = stringTokenizer.nextToken();
          canvas.drawText(text, 20 + (!StringImpl.startsWith(text, "\t") ? 0 : 20), y3, Color.BLACK, Style.NORMAL);
        }
      }
      base.draw(canvas);
    }

    public override ViewAreaType viewAreaType(Location mouseLocation) => ViewAreaType.VIEW;

    public override FocusManager getFocusManager() => this.focusManager;

    public override void setFocusManager(FocusManager focusManager) => this.focusManager = focusManager;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static MessageDialogView()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
