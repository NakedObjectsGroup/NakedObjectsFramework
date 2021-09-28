// Decompiled with JetBrains decompiler
// Type: org.xml.sax.DocumentHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using System;

namespace org.xml.sax
{
  [JavaInterface]
  [Obsolete(null, false)]
  public interface DocumentHandler
  {
    void setDocumentLocator(Locator locator);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void startDocument();

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void endDocument();

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void startElement(string name, AttributeList atts);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void endElement(string name);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void characters(char[] ch, int start, int length);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void ignorableWhitespace(char[] ch, int start, int length);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void processingInstruction(string target, string data);
  }
}
