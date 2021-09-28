// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.text.TextSelection
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark.text
{
  public class TextSelection
  {
    private readonly CursorPosition cursor;
    private readonly CursorPosition start;

    public TextSelection(TextContent content, CursorPosition cursor)
    {
      this.cursor = cursor;
      this.start = new CursorPosition(content, 0, 0);
    }

    private bool backwardSelection() => this.cursor.isBefore(this.start);

    public virtual void extendTo(CursorPosition pos) => this.cursor.asFor(pos);

    public virtual void extendTo(Location at) => this.cursor.cursorAt(at);

    public virtual CursorPosition from() => this.backwardSelection() ? this.cursor : this.start;

    public virtual bool hasSelection() => ((this.cursor.samePosition(this.start) ? 1 : 0) ^ 1) != 0;

    public virtual void resetTo(CursorPosition pos)
    {
      this.start.asFor(pos);
      this.cursor.asFor(pos);
    }

    public virtual void selectSentence()
    {
      this.resetTo(this.cursor);
      this.start.home();
      this.cursor.end();
    }

    public virtual void selectWord()
    {
      this.resetTo(this.cursor);
      this.start.wordLeft();
      this.cursor.wordRight();
    }

    public virtual CursorPosition to() => this.backwardSelection() ? this.start : this.cursor;

    public override string ToString() => new StringBuffer().append("Selection [from=").append(this.start.getLine()).append(":").append(this.start.getCharacter()).append(",to=").append(this.cursor.getLine()).append(":").append(this.cursor.getCharacter()).append("]").ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TextSelection textSelection = this;
      ObjectImpl.clone((object) textSelection);
      return ((object) textSelection).MemberwiseClone();
    }
  }
}
