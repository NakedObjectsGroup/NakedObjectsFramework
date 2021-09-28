// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.XMLReaderFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.xml.sax.helpers
{
  public sealed class XMLReaderFactory
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    private static Class XMLReaderFactory\u0024ClassObject;

    private XMLReaderFactory()
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static XMLReader createXMLReader()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static XMLReader createXMLReader(string className)
    {
      // ISSUE: unable to decompile the method.
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static XMLReaderFactory()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      XMLReaderFactory xmlReaderFactory = this;
      ObjectImpl.clone((object) xmlReaderFactory);
      return ((object) xmlReaderFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
