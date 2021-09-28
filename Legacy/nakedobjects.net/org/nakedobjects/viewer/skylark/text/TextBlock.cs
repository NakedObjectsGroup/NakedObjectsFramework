// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.text.TextBlock
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.text
{
  [JavaFlags(32)]
  public class TextBlock
  {
    private static readonly Logger LOG;
    private static readonly Logger UI_LOG;
    private readonly TextBlockTarget forField;
    private string text;
    private int[] lineBreaks;
    private bool isFormatted;
    private int lineCount;
    private bool canWrap;

    [JavaFlags(0)]
    public TextBlock(TextBlockTarget forField, string text, bool canWrap)
    {
      this.forField = forField;
      this.text = text;
      this.isFormatted = false;
      this.canWrap = canWrap;
    }

    public virtual string getLine(int line)
    {
      if (line < 0 || line > this.lineCount)
        throw new IllegalArgumentException(new StringBuffer().append("line outside of block ").append(line).ToString());
      this.format();
      return StringImpl.substring(this.text, this.lineStart(line), this.lineEnd(line));
    }

    public virtual string getText() => this.text;

    public virtual void deleteLeft(int line, int character)
    {
      int num = this.pos(line, character);
      if (num <= 0)
        return;
      this.text = new StringBuffer().append(StringImpl.substring(this.text, 0, num - 1)).append(StringImpl.substring(this.text, num)).ToString();
      this.isFormatted = false;
    }

    public virtual void delete(int fromLine, int fromCharacter, int toLine, int toCharacter)
    {
      this.format();
      this.delete(toLine, 0, toCharacter);
      this.delete(fromLine, fromCharacter, this.lineEnd(fromLine));
      int num = toLine - 1;
      while (num > fromLine)
        num += -1;
    }

    public virtual void deleteRight(int line, int character)
    {
      int num = this.pos(line, character);
      if (num >= StringImpl.length(this.text))
        return;
      this.text = new StringBuffer().append(StringImpl.substring(this.text, 0, num)).append(StringImpl.substring(this.text, num + 1)).ToString();
      this.isFormatted = false;
    }

    public virtual void delete(int line, int fromCharacter, int toCharacter)
    {
      int num1 = this.pos(line, fromCharacter);
      int num2 = this.pos(line, toCharacter);
      this.text = new StringBuffer().append(StringImpl.substring(this.text, 0, num1)).append(StringImpl.substring(this.text, num2)).ToString();
      this.isFormatted = false;
    }

    public virtual int noLines()
    {
      this.format();
      return this.lineCount + 1;
    }

    private void breakAt(int breakAt)
    {
      this.lineBreaks[this.lineCount] = breakAt;
      ++this.lineCount;
    }

    private void format()
    {
      if (!this.canWrap || this.isFormatted)
        return;
      int length = 100;
      this.lineBreaks = length >= 0 ? new int[length] : throw new NegativeArraySizeException();
      this.lineCount = 0;
      int num1 = StringImpl.length(this.text);
      int num2 = 0;
      int num3 = -1;
      for (int index = 0; index < num1; ++index)
      {
        char c = StringImpl.charAt(this.text, index);
        if (c == '\n')
          throw new IllegalStateException("Block must not contain newline characters");
        num2 += this.forField.getText().charWidth(c);
        if (num2 > this.forField.getMaxWidth())
        {
          int breakAt = num3 != -1 ? num3 : index - 1;
          this.breakAt(breakAt);
          num2 = this.forField.getText().stringWidth(StringImpl.substring(this.text, breakAt - 1, index + 1));
          num3 = -1;
        }
        else if (c == ' ')
          num3 = index + 1;
      }
      this.isFormatted = true;
    }

    public virtual void insert(int line, int character, string characters)
    {
      if (StringImpl.indexOf(characters, 10) >= 0)
        throw new IllegalArgumentException("Insert characters cannot contain newline");
      int num = this.pos(line, character);
      this.text = new StringBuffer().append(StringImpl.substring(this.text, 0, num)).append(characters).append(StringImpl.substring(this.text, num)).ToString();
      this.isFormatted = false;
    }

    private int pos(int line, int character)
    {
      int num = this.lineStart(line) + character;
      if (TextBlock.LOG.isDebugEnabled())
        TextBlock.LOG.debug((object) new StringBuffer().append("position ").append(num).ToString());
      return num;
    }

    private int lineStart(int line)
    {
      int num = line != 0 ? this.lineBreaks[line - 1] : 0;
      if (TextBlock.UI_LOG.isDebugEnabled())
        TextBlock.UI_LOG.debug((object) new StringBuffer().append("line ").append(line).append(" starts at ").append(num).ToString());
      return num;
    }

    private int lineEnd(int line)
    {
      int num = line < this.lineCount ? this.lineBreaks[line] : StringImpl.length(this.text);
      if (TextBlock.UI_LOG.isDebugEnabled())
        TextBlock.UI_LOG.debug((object) new StringBuffer().append("line ").append(line).append(" ends at ").append(num).ToString());
      return num;
    }

    public virtual TextBlock breakBlock(int line, int character)
    {
      this.format();
      int num = this.pos(line, character);
      TextBlock textBlock = new TextBlock(this.forField, StringImpl.substring(this.text, num), this.canWrap);
      this.text = StringImpl.substring(this.text, 0, num);
      this.isFormatted = false;
      return textBlock;
    }

    public virtual void setCanWrap(bool canWrap) => this.canWrap = canWrap;

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append("TextBlock [");
      stringBuffer.append("formatted=");
      stringBuffer.append(this.isFormatted);
      stringBuffer.append(",lines=");
      stringBuffer.append(this.lineCount);
      stringBuffer.append(",text=");
      stringBuffer.append(this.text);
      stringBuffer.append(",breaks=");
      if (this.lineBreaks == null)
      {
        stringBuffer.append("none");
      }
      else
      {
        for (int index = 0; index < this.lineBreaks.Length; ++index)
        {
          stringBuffer.append(index != 0 ? "," : "");
          stringBuffer.append(this.lineBreaks[index]);
        }
      }
      stringBuffer.append("]");
      return stringBuffer.ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TextBlock()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TextBlock textBlock = this;
      ObjectImpl.clone((object) textBlock);
      return ((object) textBlock).MemberwiseClone();
    }
  }
}
