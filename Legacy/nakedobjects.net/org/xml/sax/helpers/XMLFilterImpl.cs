// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.XMLFilterImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.xml.sax.helpers
{
  [JavaInterfaces("5;org/xml/sax/XMLFilter;org/xml/sax/EntityResolver;org/xml/sax/DTDHandler;org/xml/sax/ContentHandler;org/xml/sax/ErrorHandler;")]
  public class XMLFilterImpl : XMLFilter, EntityResolver, DTDHandler, ContentHandler, ErrorHandler
  {
    private XMLReader parent;
    private Locator locator;
    private EntityResolver entityResolver;
    private DTDHandler dtdHandler;
    private ContentHandler contentHandler;
    private ErrorHandler errorHandler;

    public XMLFilterImpl()
    {
      this.parent = (XMLReader) null;
      this.locator = (Locator) null;
      this.entityResolver = (EntityResolver) null;
      this.dtdHandler = (DTDHandler) null;
      this.contentHandler = (ContentHandler) null;
      this.errorHandler = (ErrorHandler) null;
    }

    public XMLFilterImpl(XMLReader parent)
    {
      this.parent = (XMLReader) null;
      this.locator = (Locator) null;
      this.entityResolver = (EntityResolver) null;
      this.dtdHandler = (DTDHandler) null;
      this.contentHandler = (ContentHandler) null;
      this.errorHandler = (ErrorHandler) null;
      this.setParent(parent);
    }

    public virtual void setParent(XMLReader parent) => this.parent = parent != null ? parent : throw new NullPointerException("Null parent");

    public virtual XMLReader getParent() => this.parent;

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual void setFeature(string name, bool state)
    {
      if (this.parent == null)
        throw new SAXNotRecognizedException(new StringBuffer().append("Feature: ").append(name).ToString());
      this.parent.setFeature(name, state);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual bool getFeature(string name) => this.parent != null ? this.parent.getFeature(name) : throw new SAXNotRecognizedException(new StringBuffer().append("Feature: ").append(name).ToString());

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual void setProperty(string name, object value)
    {
      if (this.parent == null)
        throw new SAXNotRecognizedException(new StringBuffer().append("Property: ").append(name).ToString());
      this.parent.setProperty(name, value);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual object getProperty(string name) => this.parent != null ? this.parent.getProperty(name) : throw new SAXNotRecognizedException(new StringBuffer().append("Property: ").append(name).ToString());

    public virtual void setEntityResolver(EntityResolver resolver) => this.entityResolver = resolver != null ? resolver : throw new NullPointerException("Null entity resolver");

    public virtual EntityResolver getEntityResolver() => this.entityResolver;

    public virtual void setDTDHandler(DTDHandler handler) => this.dtdHandler = handler != null ? handler : throw new NullPointerException("Null DTD handler");

    public virtual DTDHandler getDTDHandler() => this.dtdHandler;

    public virtual void setContentHandler(ContentHandler handler) => this.contentHandler = handler != null ? handler : throw new NullPointerException("Null content handler");

    public virtual ContentHandler getContentHandler() => this.contentHandler;

    public virtual void setErrorHandler(ErrorHandler handler) => this.errorHandler = handler != null ? handler : throw new NullPointerException("Null error handler");

    public virtual ErrorHandler getErrorHandler() => this.errorHandler;

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(InputSource input)
    {
      this.setupParse();
      this.parent.parse(input);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(string systemId) => this.parse(new InputSource(systemId));

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual InputSource resolveEntity(string publicId, string systemId) => this.entityResolver != null ? this.entityResolver.resolveEntity(publicId, systemId) : (InputSource) null;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void notationDecl(string name, string publicId, string systemId)
    {
      if (this.dtdHandler == null)
        return;
      this.dtdHandler.notationDecl(name, publicId, systemId);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void unparsedEntityDecl(
      string name,
      string publicId,
      string systemId,
      string notationName)
    {
      if (this.dtdHandler == null)
        return;
      this.dtdHandler.unparsedEntityDecl(name, publicId, systemId, notationName);
    }

    public virtual void setDocumentLocator(Locator locator)
    {
      this.locator = locator;
      if (this.contentHandler == null)
        return;
      this.contentHandler.setDocumentLocator(locator);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startDocument()
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.startDocument();
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endDocument()
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.endDocument();
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startPrefixMapping(string prefix, string uri)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.startPrefixMapping(prefix, uri);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endPrefixMapping(string prefix)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.endPrefixMapping(prefix);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startElement(string uri, string localName, string qName, Attributes atts)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.startElement(uri, localName, qName, atts);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endElement(string uri, string localName, string qName)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.endElement(uri, localName, qName);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void characters(char[] ch, int start, int length)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.characters(ch, start, length);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void ignorableWhitespace(char[] ch, int start, int length)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.ignorableWhitespace(ch, start, length);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void processingInstruction(string target, string data)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.processingInstruction(target, data);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void skippedEntity(string name)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.skippedEntity(name);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void warning(SAXParseException e)
    {
      if (this.errorHandler == null)
        return;
      this.errorHandler.warning(e);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void error(SAXParseException e)
    {
      if (this.errorHandler == null)
        return;
      this.errorHandler.error(e);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void fatalError(SAXParseException e)
    {
      if (this.errorHandler == null)
        return;
      this.errorHandler.fatalError(e);
    }

    private void setupParse()
    {
      if (this.parent == null)
        throw new NullPointerException("No parent for filter");
      this.parent.setEntityResolver((EntityResolver) this);
      this.parent.setDTDHandler((DTDHandler) this);
      this.parent.setContentHandler((ContentHandler) this);
      this.parent.setErrorHandler((ErrorHandler) this);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      XMLFilterImpl xmlFilterImpl = this;
      ObjectImpl.clone((object) xmlFilterImpl);
      return ((object) xmlFilterImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
