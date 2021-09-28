// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.XmlDocumentBuilder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.crimson.parser;
using org.w3c.dom;
using org.xml.sax;
using org.xml.sax.ext;
using System;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("4;org/xml/sax/ContentHandler;org/xml/sax/ext/LexicalHandler;org/xml/sax/ext/DeclHandler;org/xml/sax/DTDHandler;")]
  public class XmlDocumentBuilder : ContentHandler, LexicalHandler, DeclHandler, DTDHandler
  {
    [JavaFlags(4)]
    public XmlDocument document;
    [JavaFlags(4)]
    public Locator locator;
    private Locale locale;
    private ElementFactory factory;
    private Vector attrTmp;
    [JavaFlags(4)]
    public ParentNode[] elementStack;
    [JavaFlags(4)]
    public int topOfStack;
    private bool inDTD;
    private bool inCDataSection;
    private Doctype doctype;
    private bool disableNamespaces;
    private bool ignoreWhitespace;
    private bool expandEntityRefs;
    private bool ignoreComments;
    private bool putCDATAIntoText;

    public XmlDocumentBuilder()
    {
      this.locale = Locale.getDefault();
      this.attrTmp = new Vector();
      this.disableNamespaces = true;
      this.ignoreWhitespace = false;
      this.expandEntityRefs = true;
      this.ignoreComments = false;
      this.putCDATAIntoText = false;
    }

    public virtual bool isIgnoringLexicalInfo() => this.ignoreWhitespace && this.expandEntityRefs && this.ignoreComments && this.putCDATAIntoText;

    public virtual void setIgnoringLexicalInfo(bool value)
    {
      this.ignoreWhitespace = value;
      this.expandEntityRefs = value;
      this.ignoreComments = value;
      this.putCDATAIntoText = value;
    }

    public virtual void setIgnoreWhitespace(bool value) => this.ignoreWhitespace = value;

    public virtual void setExpandEntityReferences(bool value) => this.expandEntityRefs = value;

    public virtual void setIgnoreComments(bool value) => this.ignoreComments = value;

    public virtual void setPutCDATAIntoText(bool value) => this.putCDATAIntoText = value;

    public virtual bool getDisableNamespaces() => this.disableNamespaces;

    public virtual void setDisableNamespaces(bool value) => this.disableNamespaces = value;

    public virtual XmlDocument getDocument() => this.document;

    public virtual Locale getLocale() => this.locale;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void setLocale(Locale locale)
    {
      if (locale == null)
        locale = Locale.getDefault();
      this.locale = locale;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual Locale chooseLocale(string[] languages)
    {
      Locale locale = XmlDocument.catalog.chooseLocale(languages);
      if (locale != null)
        this.setLocale(locale);
      return locale;
    }

    [JavaFlags(0)]
    public virtual string getMessage(string messageId) => this.getMessage(messageId, (object[]) null);

    [JavaFlags(0)]
    public virtual string getMessage(string messageId, object[] parameters)
    {
      if (this.locale == null)
        this.getLocale();
      return XmlDocument.catalog.getMessage(this.locale, messageId, parameters);
    }

    public virtual void setDocumentLocator(Locator locator) => this.locator = locator;

    public virtual XmlDocument createDocument()
    {
      XmlDocument xmlDocument = new XmlDocument();
      if (this.factory != null)
        xmlDocument.setElementFactory(this.factory);
      return xmlDocument;
    }

    [JavaFlags(17)]
    [Obsolete(null, false)]
    public void setElementFactory(ElementFactory factory) => this.factory = factory;

    [Obsolete(null, false)]
    [JavaFlags(17)]
    public ElementFactory getElementFactory() => this.factory;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startDocument()
    {
      this.document = this.createDocument();
      if (this.locator != null)
        this.document.setSystemId(this.locator.getSystemId());
      int length = 200;
      this.elementStack = length >= 0 ? new ParentNode[length] : throw new NegativeArraySizeException();
      this.topOfStack = 0;
      this.elementStack[this.topOfStack] = (ParentNode) this.document;
      this.inDTD = false;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endDocument()
    {
      if (this.topOfStack != 0)
        throw new IllegalStateException(this.getMessage("XDB-000"));
      this.document.trimToSize();
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startPrefixMapping(string prefix, string uri)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endPrefixMapping(string prefix)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startElement(
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
          a = AttributeSet.createAttributeSet1(attributes);
        }
        catch (DOMException ex)
        {
          int length2 = 1;
          object[] parameters = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
          parameters[0] = (object) ((Throwable) ex).getMessage();
          throw new SAXParseException(this.getMessage("XDB-002", parameters), this.locator, (Exception) ex);
        }
      }
      ElementNode elementEx;
      try
      {
        elementEx = (ElementNode) this.document.createElementEx(qName);
      }
      catch (DOMException ex)
      {
        int length3 = 1;
        object[] parameters = length3 >= 0 ? new object[length3] : throw new NegativeArraySizeException();
        parameters[0] = (object) ((Throwable) ex).getMessage();
        throw new SAXParseException(this.getMessage("XDB-004", parameters), this.locator, (Exception) ex);
      }
      if (attributes is AttributesEx)
        elementEx.setIdAttributeName(((AttributesEx) attributes).getIdAttributeName());
      if (length1 != 0)
        elementEx.setAttributes(a);
      ParentNode[] elementStack = this.elementStack;
      int topOfStack;
      this.topOfStack = (topOfStack = this.topOfStack) + 1;
      int index = topOfStack;
      elementStack[index].appendChild((Node) elementEx);
      this.elementStack[this.topOfStack] = (ParentNode) elementEx;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endElement(string namespaceURI, string localName, string qName)
    {
      ParentNode element = this.elementStack[this.topOfStack];
      ParentNode[] elementStack = this.elementStack;
      int topOfStack;
      this.topOfStack = (topOfStack = this.topOfStack) - 1;
      int index = topOfStack;
      elementStack[index] = (ParentNode) null;
      element.reduceWaste();
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void characters(char[] buf, int offset, int len)
    {
      ParentNode element = this.elementStack[this.topOfStack];
      if (this.inCDataSection)
      {
        string str = StringImpl.createString(buf, offset, len);
        ((CharacterData) element.getLastChild()).appendData(str);
      }
      else
      {
        try
        {
          NodeBase lastChild = (NodeBase) element.getLastChild();
          if (lastChild != null && ObjectImpl.getClass((object) lastChild) == Class.FromType(typeof (TextNode)))
          {
            string newData = StringImpl.createString(buf, offset, len);
            ((DataNode) lastChild).appendData(newData);
          }
          else
          {
            TextNode textNode = this.document.newText(buf, offset, len);
            element.appendChild((Node) textNode);
          }
        }
        catch (DOMException ex)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) ((Throwable) ex).getMessage();
          throw new SAXParseException(this.getMessage("XDB-004", parameters), this.locator, (Exception) ex);
        }
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void ignorableWhitespace(char[] buf, int offset, int len)
    {
      if (this.ignoreWhitespace)
        return;
      this.characters(buf, offset, len);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void processingInstruction(string name, string instruction)
    {
      if (this.inDTD)
        return;
      ParentNode element = this.elementStack[this.topOfStack];
      try
      {
        PINode processingInstruction = (PINode) this.document.createProcessingInstruction(name, instruction);
        element.appendChild((Node) processingInstruction);
      }
      catch (DOMException ex)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) ((Throwable) ex).getMessage();
        throw new SAXParseException(this.getMessage("XDB-004", parameters), this.locator, (Exception) ex);
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void skippedEntity(string name)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startDTD(string name, string publicId, string systemId)
    {
      this.doctype = (Doctype) this.document.getImplementation().createDocumentType(name, publicId, systemId);
      this.doctype.setOwnerDocument(this.document);
      this.inDTD = true;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endDTD()
    {
      this.document.appendChild((Node) this.doctype);
      this.inDTD = false;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startEntity(string name)
    {
      if (this.expandEntityRefs || this.inDTD)
        return;
      EntityReference entityReference = this.document.createEntityReference(name);
      ParentNode[] elementStack = this.elementStack;
      int topOfStack;
      this.topOfStack = (topOfStack = this.topOfStack) + 1;
      int index = topOfStack;
      elementStack[index].appendChild((Node) entityReference);
      this.elementStack[this.topOfStack] = (ParentNode) entityReference;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endEntity(string name)
    {
      if (this.inDTD)
        return;
      ParentNode element = this.elementStack[this.topOfStack];
      if (!(element is EntityReference))
        return;
      element.setReadonly(true);
      ParentNode[] elementStack = this.elementStack;
      int topOfStack;
      this.topOfStack = (topOfStack = this.topOfStack) - 1;
      int index = topOfStack;
      elementStack[index] = (ParentNode) null;
      if (StringImpl.equals(name, (object) element.getNodeName()))
        return;
      int length = 2;
      object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
      parameters[0] = (object) name;
      parameters[1] = (object) element.getNodeName();
      throw new SAXParseException(this.getMessage("XDB-011", parameters), this.locator);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startCDATA()
    {
      if (this.putCDATAIntoText)
        return;
      CDATASection cdataSection = this.document.createCDATASection("");
      ParentNode element = this.elementStack[this.topOfStack];
      try
      {
        this.inCDataSection = true;
        element.appendChild((Node) cdataSection);
      }
      catch (DOMException ex)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) ((Throwable) ex).getMessage();
        throw new SAXParseException(this.getMessage("XDB-004", parameters), this.locator, (Exception) ex);
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endCDATA() => this.inCDataSection = false;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void comment(char[] ch, int start, int length)
    {
      if (this.ignoreComments || this.inDTD)
        return;
      Comment comment = this.document.createComment(StringImpl.createString(ch, start, length));
      ParentNode element = this.elementStack[this.topOfStack];
      try
      {
        element.appendChild((Node) comment);
      }
      catch (DOMException ex)
      {
        int length1 = 1;
        object[] parameters = length1 >= 0 ? new object[length1] : throw new NegativeArraySizeException();
        parameters[0] = (object) ((Throwable) ex).getMessage();
        throw new SAXParseException(this.getMessage("XDB-004", parameters), this.locator, (Exception) ex);
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void elementDecl(string name, string model)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void attributeDecl(
      string eName,
      string aName,
      string type,
      string valueDefault,
      string value)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void internalEntityDecl(string name, string value)
    {
      if (StringImpl.startsWith(name, "%"))
        return;
      this.doctype.addEntityNode(name, value);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void externalEntityDecl(string name, string publicId, string systemId)
    {
      if (StringImpl.startsWith(name, "%"))
        return;
      this.doctype.addEntityNode(name, publicId, systemId, (string) null);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void notationDecl(string n, string p, string s) => this.doctype.addNotation(n, p, s);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void unparsedEntityDecl(
      string name,
      string publicId,
      string systemId,
      string notation)
    {
      this.doctype.addEntityNode(name, publicId, systemId, notation);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      XmlDocumentBuilder xmlDocumentBuilder = this;
      ObjectImpl.clone((object) xmlDocumentBuilder);
      return ((object) xmlDocumentBuilder).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
