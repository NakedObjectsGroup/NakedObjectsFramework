// Decompiled with JetBrains decompiler
// Type: javax.xml.parsers.SAXParser
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.xml.sax;
using org.xml.sax.helpers;

namespace javax.xml.parsers
{
  public abstract class SAXParser
  {
    [JavaFlags(4)]
    public SAXParser()
    {
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(InputStream @is, HandlerBase hb)
    {
      if (@is == null)
        throw new IllegalArgumentException("InputStream cannot be null");
      this.parse(new InputSource(@is), hb);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(InputStream @is, HandlerBase hb, string systemId)
    {
      InputSource is1 = @is != null ? new InputSource(@is) : throw new IllegalArgumentException("InputStream cannot be null");
      is1.setSystemId(systemId);
      this.parse(is1, hb);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(InputStream @is, DefaultHandler dh)
    {
      if (@is == null)
        throw new IllegalArgumentException("InputStream cannot be null");
      this.parse(new InputSource(@is), dh);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(InputStream @is, DefaultHandler dh, string systemId)
    {
      InputSource is1 = @is != null ? new InputSource(@is) : throw new IllegalArgumentException("InputStream cannot be null");
      is1.setSystemId(systemId);
      this.parse(is1, dh);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(string uri, HandlerBase hb)
    {
      if (uri == null)
        throw new IllegalArgumentException("uri cannot be null");
      this.parse(new InputSource(uri), hb);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(string uri, DefaultHandler dh)
    {
      if (uri == null)
        throw new IllegalArgumentException("uri cannot be null");
      this.parse(new InputSource(uri), dh);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(File f, HandlerBase hb)
    {
      string systemId = f != null ? new StringBuffer().append("file:").append(f.getAbsolutePath()).ToString() : throw new IllegalArgumentException("File cannot be null");
      if ((int) File.separatorChar == 92)
        systemId = StringImpl.replace(systemId, '\\', '/');
      this.parse(new InputSource(systemId), hb);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(File f, DefaultHandler dh)
    {
      string systemId = f != null ? new StringBuffer().append("file:").append(f.getAbsolutePath()).ToString() : throw new IllegalArgumentException("File cannot be null");
      if ((int) File.separatorChar == 92)
        systemId = StringImpl.replace(systemId, '\\', '/');
      this.parse(new InputSource(systemId), dh);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(InputSource @is, HandlerBase hb)
    {
      if (@is == null)
        throw new IllegalArgumentException("InputSource cannot be null");
      Parser parser = this.getParser();
      if (hb != null)
      {
        parser.setDocumentHandler((DocumentHandler) hb);
        parser.setEntityResolver((EntityResolver) hb);
        parser.setErrorHandler((ErrorHandler) hb);
        parser.setDTDHandler((DTDHandler) hb);
      }
      parser.parse(@is);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(InputSource @is, DefaultHandler dh)
    {
      if (@is == null)
        throw new IllegalArgumentException("InputSource cannot be null");
      XMLReader xmlReader = this.getXMLReader();
      if (dh != null)
      {
        xmlReader.setContentHandler((ContentHandler) dh);
        xmlReader.setEntityResolver((EntityResolver) dh);
        xmlReader.setErrorHandler((ErrorHandler) dh);
        xmlReader.setDTDHandler((DTDHandler) dh);
      }
      xmlReader.parse(@is);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public abstract Parser getParser();

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public abstract XMLReader getXMLReader();

    public abstract bool isNamespaceAware();

    public abstract bool isValidating();

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public abstract void setProperty(string name, object value);

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public abstract object getProperty(string name);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SAXParser saxParser = this;
      ObjectImpl.clone((object) saxParser);
      return ((object) saxParser).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
