// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.ElementValidator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using System.ComponentModel;

namespace org.apache.crimson.parser
{
  [JavaFlags(32)]
  public class ElementValidator
  {
    [JavaFlags(24)]
    public static readonly ElementValidator ANY;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void consume(string type)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void text()
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void done()
    {
    }

    [JavaFlags(0)]
    public ElementValidator()
    {
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ElementValidator()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ElementValidator elementValidator = this;
      ObjectImpl.clone((object) elementValidator);
      return ((object) elementValidator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
