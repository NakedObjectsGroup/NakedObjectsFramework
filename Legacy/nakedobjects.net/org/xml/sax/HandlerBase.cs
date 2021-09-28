// Decompiled with JetBrains decompiler
// Type: org.xml.sax.HandlerBase
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using System;

namespace org.xml.sax
{
  [JavaInterfaces("4;org/xml/sax/EntityResolver;org/xml/sax/DTDHandler;org/xml/sax/DocumentHandler;org/xml/sax/ErrorHandler;")]
  [Obsolete(null, false)]
  public class HandlerBase : EntityResolver, DTDHandler, DocumentHandler, ErrorHandler
  {
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual InputSource resolveEntity(string publicId, string systemId) => (InputSource) null;

    public virtual void notationDecl(string name, string publicId, string systemId)
    {
    }

    public virtual void unparsedEntityDecl(
      string name,
      string publicId,
      string systemId,
      string notationName)
    {
    }

    public virtual void setDocumentLocator(Locator locator)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startDocument()
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endDocument()
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startElement(string name, AttributeList attributes)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endElement(string name)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void characters(char[] ch, int start, int length)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void ignorableWhitespace(char[] ch, int start, int length)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void processingInstruction(string target, string data)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void warning(SAXParseException e)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void error(SAXParseException e)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void fatalError(SAXParseException e) => throw e;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      HandlerBase handlerBase = this;
      ObjectImpl.clone((object) handlerBase);
      return ((object) handlerBase).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
