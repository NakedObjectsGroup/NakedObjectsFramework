// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.XmlWriteContext
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;

namespace org.apache.crimson.tree
{
  public class XmlWriteContext
  {
    private Writer writer;
    private int indentLevel;
    private bool prettyOutput;

    public XmlWriteContext(Writer @out) => this.writer = @out;

    public XmlWriteContext(Writer @out, int level)
    {
      this.writer = @out;
      this.prettyOutput = true;
      this.indentLevel = level;
    }

    public virtual Writer getWriter() => this.writer;

    public virtual bool isEntityDeclared(string name) => StringImpl.equals("amp", (object) name) || StringImpl.equals("lt", (object) name) || StringImpl.equals("gt", (object) name) || StringImpl.equals("quot", (object) name) || StringImpl.equals("apos", (object) name);

    public virtual int getIndentLevel() => this.indentLevel;

    public virtual void setIndentLevel(int level) => this.indentLevel = level;

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void printIndent()
    {
      int num1 = this.indentLevel;
      if (!this.prettyOutput)
        return;
      this.writer.write(XmlDocument.eol);
      while (true)
      {
        int num2;
        num1 = (num2 = num1) - 1;
        if (num2 > 0)
          this.writer.write(32);
        else
          break;
      }
    }

    public virtual bool isPrettyOutput() => this.prettyOutput;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      XmlWriteContext xmlWriteContext = this;
      ObjectImpl.clone((object) xmlWriteContext);
      return ((object) xmlWriteContext).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
