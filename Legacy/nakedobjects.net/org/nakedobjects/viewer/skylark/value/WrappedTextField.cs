// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.WrappedTextField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.text;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.value
{
  public class WrappedTextField : TextField
  {
    private static readonly org.apache.log4j.Logger LOG;

    public WrappedTextField(
      Content content,
      ViewSpecification specification,
      ViewAxis axis,
      bool showLines,
      int width)
      : base(content, specification, axis, showLines, width, 0)
    {
    }

    public override string debugDetails() => new StringBuffer().append(base.debugDetails()).append("\n").append((object) this.textContent).ToString();

    public virtual void setWrapping(bool wrapping)
    {
    }

    [JavaFlags(4)]
    public override void drawLines(Canvas canvas, Color color, int width)
    {
      int baseline = this.getBaseline();
      int noDisplayLines = this.textContent.getNoDisplayLines();
      for (int index = 0; index < noDisplayLines; ++index)
      {
        canvas.drawLine(View.HPADDING, baseline, View.HPADDING + width, baseline, color);
        baseline += this.getText().getLineHeight();
      }
    }

    [JavaFlags(4)]
    public override void drawHighlight(Canvas canvas, int maxWidth)
    {
      int y = this.getBaseline() - TextField.style.getAscent();
      CursorPosition cursorPosition1 = this.selection.from();
      CursorPosition cursorPosition2 = this.selection.to();
      string[] displayLines = this.textContent.getDisplayLines();
      int displayFromLine = this.textContent.getDisplayFromLine();
      int num1 = displayFromLine + displayLines.Length;
      for (int forLine = displayFromLine; forLine <= num1; ++forLine)
      {
        if (forLine >= cursorPosition1.getLine() && forLine <= cursorPosition2.getLine())
        {
          string text = this.textContent.getText(forLine);
          int num2 = 0;
          int num3 = TextField.style.stringWidth(text);
          if (cursorPosition1.getLine() == forLine)
          {
            int num4 = Math.min(cursorPosition1.getCharacter(), StringImpl.length(text));
            num2 = TextField.style.stringWidth(StringImpl.substring(text, 0, num4));
          }
          if (cursorPosition2.getLine() == forLine)
          {
            int num5 = Math.min(cursorPosition2.getCharacter(), StringImpl.length(text));
            num3 = TextField.style.stringWidth(StringImpl.substring(text, 0, num5));
          }
          canvas.drawSolidRectangle(num2 + View.HPADDING, y, num3 - num2, this.getText().getLineHeight(), Style.PRIMARY3);
        }
        y += this.getText().getLineHeight();
      }
    }

    [JavaFlags(4)]
    public override void drawText(Canvas canvas, Color textColor, int width)
    {
      int baseline = this.getBaseline();
      string[] displayLines = this.textContent.getDisplayLines();
      int num1 = this.cursor.getLine() - this.textContent.getDisplayFromLine();
      for (int index = 0; index < displayLines.Length; ++index)
      {
        string text = displayLines[index];
        if (text == null)
          throw new NakedObjectRuntimeException();
        if (StringImpl.endsWith(text, "\n"))
          throw new RuntimeException();
        if (this.hasFocus() && this.canChangeValue() && num1 == index)
        {
          int num2 = Math.min(this.cursor.getCharacter(), StringImpl.length(text));
          int num3 = TextField.style.stringWidth(StringImpl.substring(text, 0, num2)) + View.HPADDING;
          canvas.drawLine(num3, baseline + TextField.style.getDescent(), num3, baseline - TextField.style.getAscent(), Style.PRIMARY1);
        }
        canvas.drawText(text, View.HPADDING, baseline, textColor, TextField.style);
        baseline += this.getText().getLineHeight();
      }
    }

    [JavaFlags(4)]
    public override bool enter()
    {
      this.textContent.breakBlock(this.cursor);
      this.cursor.lineDown();
      this.cursor.home();
      this.markDamaged();
      return true;
    }

    public virtual void setNoLines(int noLines) => this.textContent.setNoDisplayLines(noLines);

    public override void setSize(Size size)
    {
      base.setSize(size);
      this.textContent.setNoDisplayLines(size.getHeight() / TextField.style.getLineHeight());
    }

    public override void setMaximumSize(Size size)
    {
      int noLines = Math.max(1, size.getHeight() / this.getText().getLineHeight());
      this.setNoLines(noLines);
      int width = Math.max(180, size.getWidth() - View.HPADDING);
      this.setMaxWidth(width);
      if (WrappedTextField.LOG.isDebugEnabled())
        WrappedTextField.LOG.debug((object) new StringBuffer().append(noLines).append(" x ").append(width).ToString());
      this.invalidateLayout();
    }

    [JavaFlags(4)]
    public override void align()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static WrappedTextField()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
