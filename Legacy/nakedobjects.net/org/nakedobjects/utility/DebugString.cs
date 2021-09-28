// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.DebugString
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using System.ComponentModel;

namespace org.nakedobjects.utility
{
  public class DebugString
  {
    private const int COLUMN_SPACING = 15;
    private const string SPACES = "                                                                            ";
    private const string LINE = "-------------------------------------------------------------------------------";
    private static readonly int MAX_LINE_LENGTH;
    private static readonly int MAX_SPACES_LENGTH;
    private const int INDENT_WIDTH = 3;
    private readonly StringBuffer @string;
    private int indent;

    public virtual void append(DebugInfo debug) => debug.debugData(this);

    public virtual void append(int number, int width)
    {
      int len = this.@string.length();
      this.@string.append(number);
      this.regularizeWidth(width, len);
    }

    public virtual void append(object @object) => this.@string.append(@object);

    public virtual void append(object @object, int width)
    {
      int len = this.@string.length();
      this.@string.append(@object);
      this.regularizeWidth(width, len);
    }

    public virtual void appendAsHexln(string label, long value) => this.appendln(label, (object) new StringBuffer().append("#").append(Long.toHexString(value)).ToString());

    public virtual void appendln() => this.@string.append('\n');

    public virtual void appendln(string label, bool value) => this.appendln(label, (object) StringImpl.valueOf(value));

    public virtual void appendln(string label, double value) => this.appendln(label, (object) StringImpl.valueOf(value));

    public virtual void appendln(string label, long value) => this.appendln(label, (object) StringImpl.valueOf(value));

    public virtual void appendln(string label, object @object)
    {
      this.indent(this.indent);
      this.@string.append(label);
      int num = 15 - StringImpl.length(label);
      this.@string.append(new StringBuffer().append(": ").append(this.spaces(num <= 0 ? 0 : num)).ToString());
      this.@string.append(@object);
      this.@string.append('\n');
    }

    public virtual void appendln(string label, object[] @object)
    {
      if (@object.Length == 0)
      {
        this.appendln(label, (object) "empty array");
      }
      else
      {
        this.appendln(label, @object[0]);
        for (int index = 1; index < @object.Length; ++index)
        {
          this.@string.append(this.spaces(17));
          this.@string.append(@object[index]);
          this.@string.append('\n');
        }
      }
    }

    public virtual void appendln(string text)
    {
      this.indent(this.indent);
      this.append((object) text);
      this.appendln();
    }

    public virtual void appendTitle(string title)
    {
      this.@string.append(title);
      this.@string.append('\n');
      this.@string.append(StringImpl.substring("-------------------------------------------------------------------------------", 0, Math.min(DebugString.MAX_LINE_LENGTH, StringImpl.length(title))));
      this.@string.append('\n');
    }

    public virtual void blankLine()
    {
      if (this.@string.length() <= 0)
        return;
      this.@string.append('\n');
    }

    public virtual void exception(Exception e)
    {
      ByteArrayOutputStream arrayOutputStream;
      PrintStream printStream = new PrintStream((OutputStream) (arrayOutputStream = new ByteArrayOutputStream()));
      ((Throwable) e).printStackTrace(printStream);
      this.appendln(((Throwable) e).getMessage());
      this.appendln(StringImpl.createString(arrayOutputStream.toByteArray()));
      printStream.close();
    }

    public virtual void indent() => ++this.indent;

    public virtual void unindent()
    {
      if (this.indent <= 0)
        return;
      this.indent += -1;
    }

    private void indent(int indent) => this.@string.append(this.spaces(Math.min(DebugString.MAX_SPACES_LENGTH, indent * 3)));

    private string spaces(int spaces) => StringImpl.substring("                                                                            ", 0, spaces);

    private void regularizeWidth(int width, int len)
    {
      if (width <= 0)
        return;
      int num = this.@string.length() - len;
      if (num > width)
      {
        this.@string.setLength(len + width - 3);
        this.@string.append("...");
      }
      else
        this.@string.append(StringImpl.substring("                                                                            ", 0, Math.max(0, width - num)));
    }

    public override string ToString() => this.@string.ToString();

    public DebugString()
    {
      this.@string = new StringBuffer();
      this.indent = 0;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static DebugString()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugString debugString = this;
      ObjectImpl.clone((object) debugString);
      return ((object) debugString).MemberwiseClone();
    }
  }
}
