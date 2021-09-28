// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.jaxp.DocumentBuilderImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using javax.xml.parsers;
using org.apache.crimson.parser;
using org.apache.crimson.tree;
using org.w3c.dom;
using org.xml.sax;
using org.xml.sax.helpers;

namespace org.apache.crimson.jaxp
{
  public class DocumentBuilderImpl : DocumentBuilder
  {
    private DocumentBuilderFactory dbf;
    private EntityResolver er;
    private ErrorHandler eh;
    private XMLReader xmlReader;
    private XmlDocumentBuilder builder;
    private bool namespaceAware;
    private bool validating;

    [JavaFlags(0)]
    [JavaThrownExceptions("1;javax/xml/parsers/ParserConfigurationException;")]
    public DocumentBuilderImpl(DocumentBuilderFactory dbf)
    {
      this.er = (EntityResolver) null;
      this.eh = (ErrorHandler) null;
      this.xmlReader = (XMLReader) null;
      this.builder = (XmlDocumentBuilder) null;
      this.namespaceAware = false;
      this.validating = false;
      this.dbf = dbf;
      this.namespaceAware = dbf.isNamespaceAware();
      this.xmlReader = (XMLReader) new XMLReaderImpl();
      try
      {
        this.validating = dbf.isValidating();
        this.xmlReader.setFeature("http://xml.org/sax/features/validation", this.validating);
        if (this.validating)
          this.setErrorHandler((ErrorHandler) new DefaultValidationErrorHandler());
        this.xmlReader.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
        this.xmlReader.setFeature("http://xml.org/sax/features/namespaces", this.namespaceAware);
        this.builder = !this.namespaceAware ? new XmlDocumentBuilder() : (XmlDocumentBuilder) new XmlDocumentBuilderNS();
        this.xmlReader.setContentHandler((ContentHandler) this.builder);
        this.xmlReader.setProperty("http://xml.org/sax/properties/lexical-handler", (object) this.builder);
        this.xmlReader.setProperty("http://xml.org/sax/properties/declaration-handler", (object) this.builder);
        this.xmlReader.setDTDHandler((DTDHandler) this.builder);
      }
      catch (SAXException ex)
      {
        throw new ParserConfigurationException(ex.getMessage());
      }
      this.builder.setIgnoreWhitespace(dbf.isIgnoringElementContentWhitespace());
      this.builder.setExpandEntityReferences(dbf.isExpandEntityReferences());
      this.builder.setIgnoreComments(dbf.isIgnoringComments());
      this.builder.setPutCDATAIntoText(dbf.isCoalescing());
    }

    public override Document newDocument() => (Document) new XmlDocument();

    public override DOMImplementation getDOMImplementation() => DOMImplementationImpl.getDOMImplementation();

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public override Document parse(InputSource @is)
    {
      if (@is == null)
        throw new IllegalArgumentException("InputSource cannot be null");
      if (this.er != null)
        this.xmlReader.setEntityResolver(this.er);
      if (this.eh != null)
        this.xmlReader.setErrorHandler(this.eh);
      this.xmlReader.parse(@is);
      return (Document) this.builder.getDocument();
    }

    public override bool isNamespaceAware() => this.namespaceAware;

    public override bool isValidating() => this.validating;

    public override void setEntityResolver(EntityResolver er) => this.er = er;

    public override void setErrorHandler(ErrorHandler eh) => this.eh = eh != null ? eh : (ErrorHandler) new DefaultHandler();
  }
}
