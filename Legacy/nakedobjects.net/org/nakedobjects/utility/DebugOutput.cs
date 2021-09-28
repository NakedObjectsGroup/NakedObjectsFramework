// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.DebugOutput
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.awt.datatransfer;
using java.io;
using java.lang;
using java.text;
using java.util;
using System.ComponentModel;

namespace org.nakedobjects.utility
{
  public class DebugOutput
  {
    private static readonly DateFormat FORMAT;
    private static readonly Font TEXT_FONT;
    private static readonly Font TITLE_FONT;

    public static void print(string title, string text)
    {
      Frame frame = new Frame();
      PrintJob printJob = Toolkit.getDefaultToolkit().getPrintJob(frame, new StringBuffer().append("Print ").append(title).ToString(), (Properties) null);
      if (printJob != null)
      {
        Graphics graphics = printJob.getGraphics();
        printJob.getPageDimension();
        if (graphics != null)
        {
          graphics.translate(10, 10);
          int num1 = 50;
          int num2 = 50;
          graphics.setFont(DebugOutput.TITLE_FONT);
          int ascent = graphics.getFontMetrics().getAscent();
          int num3 = graphics.getFontMetrics().stringWidth(title);
          graphics.drawRect(num1 - 10, num2 - 10 - ascent, num3 + 20, ascent + 20);
          graphics.drawString(title, num1, num2);
          int num4 = num2 + graphics.getFontMetrics().getHeight() + 20;
          graphics.setFont(DebugOutput.TEXT_FONT);
          StringTokenizer stringTokenizer = new StringTokenizer(text, "\n\r");
          while (stringTokenizer.hasMoreTokens())
          {
            string str = stringTokenizer.nextToken();
            graphics.drawString(str, num1, num4);
            num4 += graphics.getFontMetrics().getHeight();
          }
          graphics.dispose();
        }
        printJob.end();
      }
      frame.dispose();
    }

    public static void saveToClipboard(string text)
    {
      Clipboard systemClipboard = Toolkit.getDefaultToolkit().getSystemClipboard();
      StringSelection stringSelection = new StringSelection(text);
      systemClipboard.setContents((Transferable) stringSelection, (ClipboardOwner) stringSelection);
    }

    public static void saveToFile(DebugInfo @object)
    {
      string str1 = DebugOutput.FORMAT.format(new Date());
      string str2 = new StringBuffer().append(ObjectImpl.getClass((object) @object).getName()).append("-").append(str1).append(".txt").ToString();
      DebugString debug = new DebugString();
      @object.debugData(debug);
      string debugTitle = @object.getDebugTitle();
      DebugOutput.saveToFile(new File(str2), debugTitle, debug.ToString());
    }

    public static void saveToFile(File file, string title, string text)
    {
      try
      {
        PrintWriter printWriter = new PrintWriter((Writer) new FileWriter(file));
        printWriter.println(title);
        printWriter.println();
        printWriter.println(StringImpl.toString(text));
        printWriter.close();
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException((Throwable) ex);
      }
    }

    public static void saveToFile(string saveDialogTitle, string title, string text)
    {
      Frame frame = new Frame();
      FileDialog fileDialog = new FileDialog(frame, saveDialogTitle, 1);
      ((Dialog) fileDialog).show();
      string file = fileDialog.getFile();
      string directory = fileDialog.getDirectory();
      frame.dispose();
      DebugOutput.saveToFile(new File(directory, file), title, text);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static DebugOutput()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DebugOutput debugOutput = this;
      ObjectImpl.clone((object) debugOutput);
      return ((object) debugOutput).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
