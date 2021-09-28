// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.AboutView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.lang;
using java.awt;
using java.awt.datatransfer;
using java.lang;
using java.util;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.util;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class AboutView : AbstractView
  {
    private readonly int padding;
    private readonly Image image;
    private readonly int left;
    private bool copiedToClipboard;

    public AboutView()
      : base((Content) null, (ViewSpecification) null, (ViewAxis) null)
    {
      this.padding = 6;
      this.image = ImageFactory.getInstance().createImage(AboutNakedObjects.getImageName());
      if (this.showingImage())
        this.left = 6 + this.image.getWidth() + 6;
      else
        this.left = 6;
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      canvas.clearBackground((View) this, Style.WHITE);
      canvas.drawRectangleAround((View) this, Style.SECONDARY1);
      if (this.showingImage())
        canvas.drawIcon(this.image, 6, 6);
      int y1 = 6 + Style.LABEL.getAscent();
      StringBuffer stringBuffer = new StringBuffer();
      string applicationName = AboutNakedObjects.getApplicationName();
      stringBuffer.append(applicationName);
      if (!this.IsEmpty(applicationName))
      {
        canvas.drawText(applicationName, this.left, y1, Style.BLACK, Style.TITLE);
        y1 += Style.TITLE.getLineHeight();
      }
      string applicationCopyrightNotice = AboutNakedObjects.getApplicationCopyrightNotice();
      if (!this.IsEmpty(applicationCopyrightNotice))
      {
        canvas.drawText(applicationCopyrightNotice, this.left, y1, Style.BLACK, Style.LABEL);
        y1 += Style.LABEL.getLineHeight();
      }
      string applicationVersion = AboutNakedObjects.getApplicationVersion();
      stringBuffer.append(" ").append(applicationVersion);
      stringBuffer.append("\nClient Time: ").append(new Date().toLocaleString());
      if (!this.IsEmpty(applicationVersion))
      {
        canvas.drawText(applicationVersion, this.left, y1, Style.BLACK, Style.LABEL);
        y1 += 2 * Style.LABEL.getLineHeight();
      }
      canvas.drawText(AboutNakedObjects.getFrameworkName(), this.left, y1, Style.BLACK, Style.TITLE);
      int y2 = y1 + Style.TITLE.getLineHeight();
      canvas.drawText(AboutNakedObjects.getFrameworkCopyrightNotice(), this.left, y2, Style.BLACK, Style.LABEL);
      int y3 = y2 + Style.LABEL.getLineHeight();
      string text1 = this.frameworkVersion();
      canvas.drawText(text1, this.left, y3, Style.BLACK, Style.LABEL);
      stringBuffer.append("\nNaked Objects Framework: ").append(text1).append("\n");
      int y4 = y3 + 2 * Style.LABEL.getLineHeight();
      foreach (string text2 in AboutNakedObjects.buildInformationExtension())
      {
        stringBuffer.append(text2).append("\n");
        if (!this.IsEmpty(text2))
        {
          canvas.drawText(text2, this.left, y4, Style.BLACK, Style.LABEL);
          y4 += Style.LABEL.getLineHeight();
        }
      }
      int y5 = y4 + 2 * Style.LABEL.getLineHeight();
      canvas.drawText("Information was copied to system clipboard", this.left, y5, Style.BLACK, Style.LABEL);
      if (this.copiedToClipboard)
        return;
      Toolkit.getDefaultToolkit().getSystemClipboard().setContents((Transferable) new StringSelection(stringBuffer.ToString()), (ClipboardOwner) null);
      this.copiedToClipboard = true;
    }

    private bool IsEmpty(string text) => text == null || StringImpl.length(text) == 0;

    private string frameworkVersion() => new StringBuffer().append(AboutNakedObjects.getFrameworkVersion()).append(" (").append(AboutNakedObjects.getFrameworkBuild()).append(")").ToString();

    private bool showingImage() => this.image != null;

    public override Size getMaximumSize()
    {
      int num1 = Style.TITLE.getAscent() + Style.LABEL.getLineHeight() + 2 * Style.LABEL.getLineHeight();
      int num2 = Math.max(Math.max(Style.TITLE.stringWidth(AboutNakedObjects.getFrameworkName()), Style.LABEL.stringWidth(AboutNakedObjects.getFrameworkCopyrightNotice())), Style.LABEL.stringWidth(this.frameworkVersion()));
      string applicationName = AboutNakedObjects.getApplicationName();
      if (applicationName != null)
      {
        num1 += Style.TITLE.getAscent();
        num2 = Math.max(num2, Style.TITLE.stringWidth(applicationName));
      }
      string applicationCopyrightNotice = AboutNakedObjects.getApplicationCopyrightNotice();
      if (applicationCopyrightNotice != null)
      {
        num1 += Style.LABEL.getLineHeight();
        num2 = Math.max(num2, Style.LABEL.stringWidth(applicationCopyrightNotice));
      }
      string applicationVersion = AboutNakedObjects.getApplicationVersion();
      if (applicationVersion != null)
      {
        num1 += Style.LABEL.getLineHeight();
        num2 = Math.max(num2, Style.LABEL.stringWidth(applicationVersion));
      }
      foreach (string text in AboutNakedObjects.buildInformationExtension())
      {
        if (text != null)
        {
          num1 += Style.LABEL.getLineHeight();
          num2 = Math.max(num2, Style.LABEL.stringWidth(text));
        }
      }
      if (this.showingImage())
      {
        num1 = Math.max(num1, this.image.getHeight());
        num2 = this.image.getWidth() + 6 + num2;
      }
      return new Size(6 + num2 + 6, 6 + num1 + 6);
    }

    public override void firstClick(Click click) => this.dispose();

    public override void minimize()
    {
    }

    public override void restore()
    {
    }
  }
}
