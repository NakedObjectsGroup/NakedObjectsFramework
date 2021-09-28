// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.XmlDocumentBuilderNS
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.crimson.parser;
using org.w3c.dom;
using org.xml.sax;

namespace org.apache.crimson.tree
{
  public class XmlDocumentBuilderNS : XmlDocumentBuilder
  {
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override void startElement(
      string namespaceURI,
      string localName,
      string qName,
      Attributes attributes)
    {
      AttributeSet a = (AttributeSet) null;
      int length1 = attributes.getLength();
      if (length1 != 0)
      {
        try
        {
          a = AttributeSet.createAttributeSet2(attributes);
        }
        catch (DOMException ex)
        {
          int length2 = 1;
          object[] parameters = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
          parameters[0] = (object) ((Throwable) ex).getMessage();
          throw new SAXParseException(this.getMessage("XDB-002", parameters), this.locator, (Exception) ex);
        }
      }
      ElementNode2 elementNs;
      try
      {
        if (StringImpl.equals("", (object) namespaceURI))
          namespaceURI = (string) null;
        elementNs = (ElementNode2) this.document.createElementNS(namespaceURI, qName);
      }
      catch (DOMException ex)
      {
        int length3 = 1;
        object[] parameters = length3 >= 0 ? new object[length3] : throw new NegativeArraySizeException();
        parameters[0] = (object) ((Throwable) ex).getMessage();
        throw new SAXParseException(this.getMessage("XDB-004", parameters), this.locator, (Exception) ex);
      }
      if (attributes is AttributesEx)
        elementNs.setIdAttributeName(((AttributesEx) attributes).getIdAttributeName());
      if (length1 != 0)
        elementNs.setAttributes(a);
      ParentNode[] elementStack = this.elementStack;
      int topOfStack;
      this.topOfStack = (topOfStack = this.topOfStack) + 1;
      int index = topOfStack;
      elementStack[index].appendChild((Node) elementNs);
      this.elementStack[this.topOfStack] = (ParentNode) elementNs;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override void processingInstruction(string name, string instruction)
    {
      if (StringImpl.indexOf(name, 58) != -1)
        throw new SAXParseException(this.getMessage("XDB-010"), this.locator);
      base.processingInstruction(name, instruction);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override void internalEntityDecl(string name, string value)
    {
      if (StringImpl.indexOf(name, 58) != -1)
        throw new SAXParseException(this.getMessage("XDB-012"), this.locator);
      base.internalEntityDecl(name, value);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override void externalEntityDecl(string name, string publicId, string systemId)
    {
      if (StringImpl.indexOf(name, 58) != -1)
        throw new SAXParseException(this.getMessage("XDB-012"), this.locator);
      base.externalEntityDecl(name, publicId, systemId);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override void notationDecl(string n, string p, string s)
    {
      if (StringImpl.indexOf(n, 58) != -1)
        throw new SAXParseException(this.getMessage("XDB-013"), this.locator);
      base.notationDecl(n, p, s);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override void unparsedEntityDecl(
      string name,
      string publicId,
      string systemId,
      string notation)
    {
      if (StringImpl.indexOf(name, 58) != -1)
        throw new SAXParseException(this.getMessage("XDB-012"), this.locator);
      base.unparsedEntityDecl(name, publicId, systemId, notation);
    }
  }
}
