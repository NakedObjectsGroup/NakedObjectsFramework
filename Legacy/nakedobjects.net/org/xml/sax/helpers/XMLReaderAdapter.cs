// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.XMLReaderAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using System.ComponentModel;

namespace org.xml.sax.helpers
{
  [JavaInterfaces("2;org/xml/sax/Parser;org/xml/sax/ContentHandler;")]
  public class XMLReaderAdapter : Parser, ContentHandler
  {
    [JavaFlags(0)]
    public XMLReader xmlReader;
    [JavaFlags(0)]
    public DocumentHandler documentHandler;
    [JavaFlags(0)]
    public XMLReaderAdapter.AttributesAdapter qAtts;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public XMLReaderAdapter() => this.setup(XMLReaderFactory.createXMLReader());

    public XMLReaderAdapter(XMLReader xmlReader) => this.setup(xmlReader);

    private void setup(XMLReader xmlReader)
    {
      this.xmlReader = xmlReader != null ? xmlReader : throw new NullPointerException("XMLReader must not be null");
      this.qAtts = new XMLReaderAdapter.AttributesAdapter(this);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void setLocale(Locale locale) => throw new SAXNotSupportedException("setLocale not supported");

    public virtual void setEntityResolver(EntityResolver resolver) => this.xmlReader.setEntityResolver(resolver);

    public virtual void setDTDHandler(DTDHandler handler) => this.xmlReader.setDTDHandler(handler);

    public virtual void setDocumentHandler(DocumentHandler handler) => this.documentHandler = handler;

    public virtual void setErrorHandler(ErrorHandler handler) => this.xmlReader.setErrorHandler(handler);

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual void parse(string systemId) => this.parse(new InputSource(systemId));

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual void parse(InputSource input)
    {
      this.setupXMLReader();
      this.xmlReader.parse(input);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private void setupXMLReader()
    {
      this.xmlReader.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
      try
      {
        this.xmlReader.setFeature("http://xml.org/sax/features/namespaces", false);
      }
      catch (SAXException ex)
      {
      }
      this.xmlReader.setContentHandler((ContentHandler) this);
    }

    public virtual void setDocumentLocator(Locator locator)
    {
      if (this.documentHandler == null)
        return;
      this.documentHandler.setDocumentLocator(locator);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startDocument()
    {
      if (this.documentHandler == null)
        return;
      this.documentHandler.startDocument();
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endDocument()
    {
      if (this.documentHandler == null)
        return;
      this.documentHandler.endDocument();
    }

    public virtual void startPrefixMapping(string prefix, string uri)
    {
    }

    public virtual void endPrefixMapping(string prefix)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startElement(string uri, string localName, string qName, Attributes atts)
    {
      if (this.documentHandler == null)
        return;
      this.qAtts.setAttributes(atts);
      this.documentHandler.startElement(qName, (AttributeList) this.qAtts);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endElement(string uri, string localName, string qName)
    {
      if (this.documentHandler == null)
        return;
      this.documentHandler.endElement(qName);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void characters(char[] ch, int start, int length)
    {
      if (this.documentHandler == null)
        return;
      this.documentHandler.characters(ch, start, length);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void ignorableWhitespace(char[] ch, int start, int length)
    {
      if (this.documentHandler == null)
        return;
      this.documentHandler.ignorableWhitespace(ch, start, length);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void processingInstruction(string target, string data)
    {
      if (this.documentHandler == null)
        return;
      this.documentHandler.processingInstruction(target, data);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void skippedEntity(string name)
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      XMLReaderAdapter xmlReaderAdapter = this;
      ObjectImpl.clone((object) xmlReaderAdapter);
      return ((object) xmlReaderAdapter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaInterfaces("1;org/xml/sax/AttributeList;")]
    [Inner]
    [JavaFlags(48)]
    public sealed class AttributesAdapter : AttributeList
    {
      private Attributes attributes;
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private XMLReaderAdapter this\u00240;

      [JavaFlags(0)]
      public AttributesAdapter(XMLReaderAdapter _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(0)]
      public virtual void setAttributes(Attributes attributes) => this.attributes = attributes;

      public virtual int getLength() => this.attributes.getLength();

      public virtual string getName(int i) => this.attributes.getQName(i);

      public virtual string getType(int i) => this.attributes.getType(i);

      public virtual string getValue(int i) => this.attributes.getValue(i);

      public virtual string getType(string qName) => this.attributes.getType(qName);

      public virtual string getValue(string qName) => this.attributes.getValue(qName);

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        XMLReaderAdapter.AttributesAdapter attributesAdapter = this;
        ObjectImpl.clone((object) attributesAdapter);
        return ((object) attributesAdapter).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
