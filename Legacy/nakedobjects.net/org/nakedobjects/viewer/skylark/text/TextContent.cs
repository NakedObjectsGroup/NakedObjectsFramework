// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.text.TextContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.text
{
  public class TextContent
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly org.apache.log4j.Logger UI_LOG;
    public const int NO_WRAPPING = 1;
    public const int WRAPPING = 0;
    private Vector blocks;
    private TextBlockTarget target;
    private int displayFromLine;
    private int noDisplayLines;
    private readonly int wrap;

    public TextContent(TextBlockTarget target, int noLines, int wrap)
    {
      this.noDisplayLines = 1;
      this.target = target;
      this.wrap = wrap;
      this.blocks = new Vector();
      this.noDisplayLines = noLines;
      this.displayFromLine = 0;
      this.addBlock("");
      this.alignDisplay(0);
    }

    private void addBlock(string text)
    {
      TextBlock textBlock = new TextBlock(this.target, text, this.wrap == 0);
      if (TextContent.LOG.isDebugEnabled())
        TextContent.LOG.debug((object) new StringBuffer().append("add block ").append((object) textBlock).ToString());
      this.blocks.addElement((object) textBlock);
    }

    public virtual int getNoDisplayLines() => this.noDisplayLines;

    public virtual void alignDisplay(int line)
    {
      int noLinesOfContent = this.getNoLinesOfContent();
      int num1 = noLinesOfContent - 1;
      int num2 = Math.min(this.displayFromLine + this.noDisplayLines, noLinesOfContent);
      if (noLinesOfContent <= this.noDisplayLines)
      {
        this.displayFromLine = 0;
      }
      else
      {
        if (line >= num2)
        {
          num2 = Math.min(line + 3, num1);
          this.displayFromLine = num2 - this.noDisplayLines + 1;
          this.displayFromLine = Math.max(this.displayFromLine, 0);
        }
        if (line < this.displayFromLine)
        {
          this.displayFromLine = line;
          num2 = this.displayFromLine + this.noDisplayLines - 1;
          if (num2 >= noLinesOfContent)
          {
            num2 = num1;
            this.displayFromLine = Math.max(0, num2 - this.noDisplayLines);
          }
        }
      }
      if (!TextContent.LOG.isDebugEnabled())
        return;
      TextContent.LOG.debug((object) new StringBuffer().append("display line ").append(line).append(" ").append(this.displayFromLine).append("~").append(num2).ToString());
    }

    public virtual void breakBlock(CursorPosition cursorAt)
    {
      TextContent.TextBlockReference blockFor = this.getBlockFor(cursorAt.getLine());
      this.blocks.insertElementAt((object) blockFor.block.breakBlock(blockFor.line, cursorAt.getCharacter()), blockFor.blockIndex + 1);
    }

    public virtual void delete(TextSelection selection)
    {
      CursorPosition cursorPosition1 = selection.from();
      CursorPosition cursorPosition2 = selection.to();
      TextContent.TextBlockReference textBlockReference = cursorPosition1.getLine() == cursorPosition2.getLine() ? this.getBlockFor(cursorPosition1.getLine()) : throw new NotImplementedException();
      textBlockReference.block.delete(textBlockReference.line, cursorPosition1.getCharacter(), cursorPosition2.getCharacter());
    }

    public virtual void deleteLeft(CursorPosition cursorAt)
    {
      TextContent.TextBlockReference blockFor = this.getBlockFor(cursorAt.getLine());
      if (blockFor == null || blockFor.block == null)
        throw new NakedObjectRuntimeException(new StringBuffer().append("invalid block ").append((object) blockFor).append(" for line ").append(cursorAt.getLine()).ToString());
      blockFor.block.deleteLeft(blockFor.line, cursorAt.getCharacter());
    }

    public virtual void deleteRight(CursorPosition cursorAt)
    {
      TextContent.TextBlockReference blockFor = this.getBlockFor(cursorAt.getLine());
      blockFor.block.deleteRight(blockFor.line, cursorAt.getCharacter());
    }

    private TextContent.TextBlockReference getBlockFor(int line)
    {
      int line1 = line >= 0 ? line : throw new IllegalArgumentException(new StringBuffer().append("Line must be greater than, or equal to, zero: ").append(line).ToString());
      for (int blockIndex = 0; blockIndex < this.blocks.size(); ++blockIndex)
      {
        TextBlock block = (TextBlock) this.blocks.elementAt(blockIndex);
        int num = block.noLines();
        if (line1 < num)
        {
          if (TextContent.UI_LOG.isDebugEnabled())
            TextContent.UI_LOG.debug((object) new StringBuffer().append("block ").append(blockIndex).append(", line ").append(line1).ToString());
          return new TextContent.TextBlockReference(blockIndex, block, line1);
        }
        line1 -= num;
      }
      return (TextContent.TextBlockReference) null;
    }

    public virtual string getText()
    {
      StringBuffer stringBuffer = new StringBuffer();
      Enumeration enumeration = this.blocks.elements();
      while (enumeration.hasMoreElements())
      {
        TextBlock textBlock = (TextBlock) enumeration.nextElement();
        if (stringBuffer.length() > 0)
          stringBuffer.append("\n");
        stringBuffer.append(textBlock.getText());
      }
      return stringBuffer.ToString();
    }

    public virtual string getText(int forLine)
    {
      TextContent.TextBlockReference blockFor = this.getBlockFor(forLine);
      return blockFor?.block.getLine(blockFor.line);
    }

    public virtual string getText(TextSelection selection)
    {
      CursorPosition cursorPosition1 = selection.from();
      CursorPosition cursorPosition2 = selection.to();
      int line = cursorPosition1.getLine();
      string text1 = this.getText(line);
      if (cursorPosition1.getLine() == cursorPosition2.getLine())
        return StringImpl.substring(text1, cursorPosition1.getCharacter(), cursorPosition2.getCharacter());
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(StringImpl.substring(text1, cursorPosition1.getCharacter()));
      for (int forLine = line + 1; forLine < line + (cursorPosition2.getLine() - cursorPosition1.getLine()); ++forLine)
      {
        string text2 = this.getText(forLine);
        stringBuffer.append(text2);
      }
      string text3 = this.getText(line + (cursorPosition2.getLine() - cursorPosition1.getLine()));
      stringBuffer.append(StringImpl.substring(text3, 0, cursorPosition2.getCharacter()));
      return stringBuffer.ToString();
    }

    public virtual void insert(CursorPosition cursorAt, string characters)
    {
      Assert.assertNotNull((object) cursorAt);
      TextContent.TextBlockReference blockFor = this.getBlockFor(cursorAt.getLine());
      Assert.assertNotNull(new StringBuffer().append("failed to get block for line ").append(cursorAt.getLine()).ToString(), (object) blockFor);
      blockFor.block.insert(blockFor.line, cursorAt.getCharacter(), characters);
    }

    public virtual int getNoLinesOfContent()
    {
      int num = 0;
      Enumeration enumeration = this.blocks.elements();
      while (enumeration.hasMoreElements())
        num += ((TextBlock) enumeration.nextElement()).noLines();
      return num;
    }

    public virtual void setText(string text)
    {
      this.blocks.removeAllElements();
      if (StringImpl.equals(text, (object) ""))
      {
        this.addBlock("");
      }
      else
      {
        StringTokenizer stringTokenizer = new StringTokenizer(text, "\n");
        while (stringTokenizer.hasMoreTokens())
          this.addBlock(stringTokenizer.nextToken());
      }
    }

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("field", (object) this.target);
      toString.append("lines", this.noDisplayLines);
      toString.append("blocks=", this.blocks.size());
      return toString.ToString();
    }

    public virtual string[] getDisplayLines()
    {
      int noDisplayLines = this.noDisplayLines;
      string[] strArray = noDisplayLines >= 0 ? new string[noDisplayLines] : throw new NegativeArraySizeException();
      int index = 0;
      int displayFromLine = this.displayFromLine;
      while (index < strArray.Length)
      {
        string text = this.getText(displayFromLine);
        strArray[index] = text ?? "";
        ++index;
        ++displayFromLine;
      }
      return strArray;
    }

    public virtual int getDisplayFromLine() => this.displayFromLine;

    public virtual void setNoDisplayLines(int noDisplayLines) => this.noDisplayLines = noDisplayLines;

    public virtual void increaseDepth() => ++this.noDisplayLines;

    public virtual bool decreaseDepth()
    {
      if (this.noDisplayLines <= 1)
        return false;
      this.noDisplayLines += -1;
      return true;
    }

    [JavaFlags(0)]
    public virtual int cursorAtLine(Location atLocation)
    {
      if (TextContent.LOG.isDebugEnabled())
        TextContent.LOG.debug((object) new StringBuffer().append("pointer at ").append((object) atLocation).ToString());
      return Math.max(this.displayFromLine + atLocation.getY() / this.target.getText().getLineHeight(), 0);
    }

    [JavaFlags(0)]
    public virtual int cursorAtCharacter(Location atLocation, int lineOffset)
    {
      string text1 = this.getText(lineOffset);
      if (text1 == null)
      {
        for (int forLine = lineOffset; forLine >= 0; forLine += -1)
        {
          string text2 = this.getText(forLine);
          if (text2 != null)
          {
            int num = StringImpl.length(text2);
            if (TextContent.LOG.isDebugEnabled())
              TextContent.LOG.debug((object) new StringBuffer().append("character at ").append(num).append(" line ").append(lineOffset).ToString());
            return num;
          }
        }
      }
      int num1 = atLocation.getX() - 3;
      int num2 = 0;
      int num3 = StringImpl.length(text1);
      for (int index = 0; num2 < num3 && num1 > index; ++num2)
        index += this.target.getText().charWidth(StringImpl.charAt(text1, num2));
      if (TextContent.LOG.isDebugEnabled())
        TextContent.LOG.debug((object) new StringBuffer().append("character at ").append(num2).append(" line ").append(lineOffset).ToString());
      return num2;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static TextContent()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TextContent textContent = this;
      ObjectImpl.clone((object) textContent);
      return ((object) textContent).MemberwiseClone();
    }

    [JavaFlags(42)]
    private class TextBlockReference
    {
      [JavaFlags(0)]
      public TextBlock block;
      [JavaFlags(0)]
      public int blockIndex;
      [JavaFlags(0)]
      public int line;

      public TextBlockReference(int blockIndex, TextBlock block, int line)
      {
        this.blockIndex = blockIndex;
        this.block = block;
        this.line = line;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        TextContent.TextBlockReference textBlockReference = this;
        ObjectImpl.clone((object) textBlockReference);
        return ((object) textBlockReference).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
