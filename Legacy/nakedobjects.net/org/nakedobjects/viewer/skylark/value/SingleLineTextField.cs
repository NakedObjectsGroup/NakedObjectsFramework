// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.SingleLineTextField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.metal;

namespace org.nakedobjects.viewer.skylark.value
{
  public class SingleLineTextField : TextField
  {
    private const int LIMIT = 20;
    private int offset;

    public SingleLineTextField(
      Content content,
      ViewSpecification specification,
      ViewAxis axis,
      bool showLines,
      int width)
      : base(content, specification, axis, showLines, width, 1)
    {
      this.offset = 0;
    }

    [JavaFlags(4)]
    public override void align()
    {
      string text = this.textContent.getText(0);
      if (text == null)
        return;
      int maxWidth = this.getMaxWidth();
      int num1 = this.offset + 20;
      int num2 = this.offset + maxWidth - 20;
      if (this.cursor.getCharacter() > StringImpl.length(text))
        this.cursor.end();
      int num3 = TextField.style.stringWidth(StringImpl.substring(text, 0, this.cursor.getCharacter()));
      if (num3 > num2)
      {
        this.offset += num3 - num2;
        this.offset = Math.min(TextField.style.stringWidth(text), this.offset);
      }
      else
      {
        if (num3 >= num1)
          return;
        this.offset -= num1 - num3;
        this.offset = Math.max(0, this.offset);
      }
    }

    [JavaFlags(4)]
    public override void drawHighlight(Canvas canvas, int maxWidth)
    {
      int y = this.getBaseline() - TextField.style.getAscent();
      int character1 = this.selection.from().getCharacter();
      int character2 = this.selection.to().getCharacter();
      string text = this.textContent.getText(0);
      if (text == null)
        return;
      int num1 = TextField.style.stringWidth(StringImpl.substring(text, 0, character1));
      int num2 = TextField.style.stringWidth(StringImpl.substring(text, 0, character2));
      canvas.drawSolidRectangle(num1 + View.HPADDING, y, num2 - num1, TextField.style.getLineHeight(), Style.PRIMARY3);
    }

    [JavaFlags(4)]
    public override void drawLines(Canvas canvas, Color color, int width)
    {
      int baseline = this.getBaseline();
      canvas.drawLine(View.HPADDING, baseline, View.HPADDING + width, baseline, color);
    }

    [JavaFlags(4)]
    public override void drawText(Canvas canvas, Color textColor, int width)
    {
      string[] displayLines = this.textContent.getDisplayLines();
      string text = displayLines.Length <= 1 ? displayLines[0] : throw new NakedObjectRuntimeException(new StringBuffer().append("Single line text field should contain a string that contains no line breaks; contains ").append(displayLines.Length).ToString());
      if (text == null)
        throw new NakedObjectRuntimeException();
      if (StringImpl.endsWith(text, "\n"))
        throw new RuntimeException();
      int baseline = this.getBaseline();
      if (this.hasFocus() && this.canChangeValue())
      {
        int num1 = Math.min(this.cursor.getCharacter(), StringImpl.length(text));
        int num2 = TextField.style.stringWidth(StringImpl.substring(text, 0, num1)) - this.offset + View.HPADDING;
        canvas.drawLine(num2, baseline + TextField.style.getDescent(), num2, baseline - TextField.style.getAscent(), Style.PRIMARY1);
      }
      canvas.drawText(text, View.HPADDING - this.offset, baseline, textColor, TextField.style);
    }

    public override void setMaximumSize(Size size)
    {
      this.setMaxWidth(Math.max(180, size.getWidth() - View.HPADDING));
      this.invalidateLayout();
    }

    public override void keyPressed(KeyboardAction key)
    {
      if (this.getFocusManager() is TableFocusManager && !this.isModifierKeyPressed(key))
      {
        int keyCode = key.getKeyCode();
        switch (keyCode)
        {
          case 38:
            ((TableFocusManager) this.getFocusManager()).focusUpOneRow();
            key.consume();
            return;
          case 40:
            ((TableFocusManager) this.getFocusManager()).focusDownOneRow();
            key.consume();
            return;
          default:
            if (keyCode == 37 && this.getCursor().isAtStart())
            {
              ((TableFocusManager) this.getFocusManager()).focusPreviousView();
              key.consume();
              return;
            }
            if (keyCode == 39 && this.getCursor().isAtEnd())
            {
              ((TableFocusManager) this.getFocusManager()).focusNextView();
              key.consume();
              return;
            }
            break;
        }
      }
      base.keyPressed(key);
    }

    private bool isModifierKeyPressed(KeyboardAction key)
    {
      int modifiers = key.getModifiers();
      bool flag1 = (modifiers & 8) > 0;
      bool flag2 = (modifiers & 1) > 0;
      bool flag3 = (modifiers & 2) > 0;
      return flag1 || flag2 || flag3;
    }
  }
}
