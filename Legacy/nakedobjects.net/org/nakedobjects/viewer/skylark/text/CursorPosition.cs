// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.text.CursorPosition
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark.text
{
  public class CursorPosition
  {
    private int character;
    private int line;
    private readonly TextContent textContent;

    public CursorPosition(TextContent content, CursorPosition pos)
      : this(content, pos.line, pos.character)
    {
    }

    public CursorPosition(TextContent content, int line, int afterCharacter)
    {
      this.textContent = content;
      this.line = line;
      this.character = afterCharacter;
    }

    public virtual void asFor(CursorPosition pos)
    {
      this.line = pos.line;
      this.character = pos.character;
    }

    public virtual void bottom()
    {
      this.line = this.textContent.getNoLinesOfContent() - 1;
      this.textContent.alignDisplay(this.line);
      this.end();
    }

    public virtual void cursorAt(Location atLocation)
    {
      this.line = this.textContent.cursorAtLine(atLocation);
      this.character = this.textContent.cursorAtCharacter(atLocation, this.line);
      if (this.line < this.textContent.getNoLinesOfContent())
        return;
      this.line = this.textContent.getNoLinesOfContent() - 1;
      this.end();
    }

    public virtual void end()
    {
      string text = this.textContent.getText(this.line);
      this.character = text != null ? StringImpl.length(text) : 0;
    }

    public virtual int getCharacter() => this.character;

    public virtual int getLine() => this.line;

    public virtual void home() => this.character = 0;

    public virtual void left()
    {
      if (this.line == 0 && this.character == 0)
        return;
      this.character += -1;
      if (this.character >= 0)
        return;
      this.line += -1;
      this.textContent.alignDisplay(this.line);
      this.end();
    }

    public virtual void lineDown() => this.moveDown(1);

    public virtual void lineUp() => this.moveUp(1);

    public virtual bool isAtStart() => this.character == 0;

    public virtual bool isAtEnd()
    {
      string text = this.textContent.getText(this.line);
      return text == null || this.character == StringImpl.length(text);
    }

    private void moveDown(int byLines)
    {
      int noLinesOfContent = this.textContent.getNoLinesOfContent();
      if (this.line >= noLinesOfContent - 1)
        return;
      this.line += byLines;
      this.line = Math.min(noLinesOfContent - 1, this.line);
      this.character = Math.min(this.character, StringImpl.length(this.textContent.getText(this.line)));
      this.textContent.alignDisplay(this.line);
    }

    private void moveUp(int byLines)
    {
      if (this.line <= 0)
        return;
      this.line -= byLines;
      this.line = Math.max(0, this.line);
      this.textContent.alignDisplay(this.line);
    }

    public virtual void pageDown() => this.moveDown(this.textContent.getNoDisplayLines() - 1);

    public virtual void pageUp() => this.moveUp(this.textContent.getNoDisplayLines() - 1);

    public virtual void right() => this.right(1);

    public virtual void right(int characters)
    {
      int num = StringImpl.length(this.textContent.getText(this.line));
      if (this.character + characters > num)
      {
        if (this.line + 1 >= this.textContent.getNoLinesOfContent())
          return;
        ++this.line;
        this.textContent.alignDisplay(this.line);
        int characters1 = this.character + characters - num;
        this.character = 0;
        this.right(characters1);
      }
      else
        this.character += characters;
    }

    public virtual void top()
    {
      this.line = 0;
      this.character = 0;
      this.textContent.alignDisplay(this.line);
    }

    public override string ToString() => new StringBuffer().append("CursorPosition [line=").append(this.line).append(",character=").append(this.character).append("]").ToString();

    public virtual void wordLeft()
    {
      if (this.line == 0 && this.character == 0)
        return;
      if (this.character == 0)
      {
        this.line += -1;
        this.end();
      }
      string text = this.textContent.getText(this.line);
      do
      {
        this.character += -1;
      }
      while (this.character >= 0 && StringImpl.charAt(text, this.character) == ' ');
      while (this.character >= 0 && StringImpl.charAt(text, this.character) != ' ')
        this.character += -1;
      ++this.character;
    }

    public virtual void wordRight()
    {
      string text = this.textContent.getText(this.line);
      int num = StringImpl.length(text);
      if (this.line == this.textContent.getNoLinesOfContent() - 1 && this.character == num - 1)
        return;
      while (this.character < num && StringImpl.charAt(text, this.character) == ' ')
        ++this.character;
      while (this.character < num && StringImpl.charAt(text, this.character) != ' ')
        ++this.character;
      while (this.character < num && StringImpl.charAt(text, this.character) == ' ')
        ++this.character;
      if (this.character < num || this.line + 1 >= this.textContent.getNoLinesOfContent())
        return;
      ++this.line;
      this.character = 0;
    }

    public virtual bool samePosition(CursorPosition positionToCompare) => this.line == positionToCompare.line && this.character == positionToCompare.character;

    public virtual bool isBefore(CursorPosition positionToCompare) => this.line < positionToCompare.line || this.line == positionToCompare.line && this.character < positionToCompare.character;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      CursorPosition cursorPosition = this;
      ObjectImpl.clone((object) cursorPosition);
      return ((object) cursorPosition).MemberwiseClone();
    }
  }
}
