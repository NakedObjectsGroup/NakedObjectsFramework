// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.VectorWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;

namespace org.apache.log4j.spi
{
  [JavaFlags(32)]
  public class VectorWriter : PrintWriter
  {
    private Vector v;

    [JavaFlags(0)]
    public VectorWriter()
      : base((Writer) new NullWriter())
    {
      this.v = new Vector();
    }

    public virtual void print(object o) => this.v.addElement((object) o.ToString());

    public virtual void print(char[] chars) => this.v.addElement((object) StringImpl.createString(chars));

    public virtual void print(string s) => this.v.addElement((object) s);

    public virtual void println(object o) => this.v.addElement((object) o.ToString());

    public virtual void println(char[] chars) => this.v.addElement((object) StringImpl.createString(chars));

    public virtual void println(string s) => this.v.addElement((object) s);

    public virtual void write(char[] chars) => this.v.addElement((object) StringImpl.createString(chars));

    public virtual void write(char[] chars, int off, int len) => this.v.addElement((object) StringImpl.createString(chars, off, len));

    public virtual void write(string s, int off, int len) => this.v.addElement((object) StringImpl.substring(s, off, off + len));

    public virtual void write(string s) => this.v.addElement((object) s);

    public virtual string[] toStringArray()
    {
      int num = this.v.size();
      int length = num;
      string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < num; ++index)
        strArray[index] = \u003CVerifierFix\u003E.genCastToString(this.v.elementAt(index));
      return strArray;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      VectorWriter vectorWriter = this;
      ObjectImpl.clone((object) vectorWriter);
      return ((object) vectorWriter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
