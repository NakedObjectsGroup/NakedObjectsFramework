// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.jaxp.SAXParserImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using javax.xml.parsers;
using org.apache.crimson.parser;
using org.xml.sax;
using org.xml.sax.helpers;

namespace org.apache.crimson.jaxp
{
  public class SAXParserImpl : SAXParser
  {
    private XMLReader xmlReader;
    private Parser parser;
    private bool validating;
    private bool namespaceAware;

    [JavaFlags(0)]
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public SAXParserImpl(SAXParserFactory spf, Hashtable features)
    {
      this.parser = (Parser) null;
      this.validating = false;
      this.namespaceAware = false;
      this.xmlReader = (XMLReader) new XMLReaderImpl();
      this.validating = spf.isValidating();
      string name = "http://xml.org/sax/features/validation";
      if (this.validating)
        this.xmlReader.setErrorHandler((ErrorHandler) new DefaultValidationErrorHandler());
      this.xmlReader.setFeature(name, this.validating);
      this.namespaceAware = spf.isNamespaceAware();
      this.xmlReader.setFeature("http://xml.org/sax/features/namespaces", this.namespaceAware);
      this.setFeatures(features);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotSupportedException;org/xml/sax/SAXNotRecognizedException;")]
    private void setFeatures(Hashtable features)
    {
      if (features == null)
        return;
      Enumeration enumeration = features.keys();
      while (enumeration.hasMoreElements())
      {
        string name = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        bool flag = ((Boolean) features.get((object) name)).booleanValue();
        this.xmlReader.setFeature(name, flag);
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override Parser getParser()
    {
      if (this.parser == null)
      {
        this.parser = (Parser) new XMLReaderAdapter(this.xmlReader);
        this.parser.setDocumentHandler((DocumentHandler) new HandlerBase());
      }
      return this.parser;
    }

    public override XMLReader getXMLReader() => this.xmlReader;

    public override bool isNamespaceAware() => this.namespaceAware;

    public override bool isValidating() => this.validating;

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public override void setProperty(string name, object value) => this.xmlReader.setProperty(name, value);

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public override object getProperty(string name) => this.xmlReader.getProperty(name);
  }
}
