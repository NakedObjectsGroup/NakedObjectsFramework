// Decompiled with JetBrains decompiler
// Type: javax.xml.parsers.DocumentBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.w3c.dom;
using org.xml.sax;

namespace javax.xml.parsers
{
  public abstract class DocumentBuilder
  {
    [JavaFlags(4)]
    public DocumentBuilder()
    {
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual Document parse(InputStream @is) => @is != null ? this.parse(new InputSource(@is)) : throw new IllegalArgumentException("InputStream cannot be null");

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual Document parse(InputStream @is, string systemId)
    {
      InputSource is1 = @is != null ? new InputSource(@is) : throw new IllegalArgumentException("InputStream cannot be null");
      is1.setSystemId(systemId);
      return this.parse(is1);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual Document parse(string uri) => uri != null ? this.parse(new InputSource(uri)) : throw new IllegalArgumentException("URI cannot be null");

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual Document parse(File f)
    {
      string systemId = f != null ? new StringBuffer().append("file:").append(f.getAbsolutePath()).ToString() : throw new IllegalArgumentException("File cannot be null");
      if ((int) File.separatorChar == 92)
        systemId = StringImpl.replace(systemId, '\\', '/');
      return this.parse(new InputSource(systemId));
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public abstract Document parse(InputSource @is);

    public abstract bool isNamespaceAware();

    public abstract bool isValidating();

    public abstract void setEntityResolver(EntityResolver er);

    public abstract void setErrorHandler(ErrorHandler eh);

    public abstract Document newDocument();

    public abstract DOMImplementation getDOMImplementation();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DocumentBuilder documentBuilder = this;
      ObjectImpl.clone((object) documentBuilder);
      return ((object) documentBuilder).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
