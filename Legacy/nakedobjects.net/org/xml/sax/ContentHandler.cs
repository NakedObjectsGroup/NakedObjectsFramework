// Decompiled with JetBrains decompiler
// Type: org.xml.sax.ContentHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.xml.sax
{
  [JavaInterface]
  public interface ContentHandler
  {
    void setDocumentLocator(Locator locator);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void startDocument();

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void endDocument();

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void startPrefixMapping(string prefix, string uri);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void endPrefixMapping(string prefix);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void startElement(string namespaceURI, string localName, string qName, Attributes atts);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void endElement(string namespaceURI, string localName, string qName);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void characters(char[] ch, int start, int length);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void ignorableWhitespace(char[] ch, int start, int length);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void processingInstruction(string target, string data);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void skippedEntity(string name);
  }
}
