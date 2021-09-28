// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.XmlDocument
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.apache.crimson.util;
using org.w3c.dom;
using org.xml.sax;
using org.xml.sax.helpers;
using System;
using System.ComponentModel;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/apache/crimson/tree/DocumentEx;")]
  public class XmlDocument : ParentNode, DocumentEx
  {
    [JavaFlags(8)]
    public static string eol;
    [JavaFlags(24)]
    public static readonly MessageCatalog catalog;
    private Locale locale;
    private string systemId;
    private ElementFactory factory;
    [JavaFlags(0)]
    public int mutationCount;
    [JavaFlags(0)]
    public bool replaceRootElement;

    public XmlDocument() => this.locale = Locale.getDefault();

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    [Obsolete(null, false)]
    public static XmlDocument createXmlDocument(string documentURI, bool doValidate) => XmlDocument.createXmlDocument(new InputSource(documentURI), doValidate);

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    [Obsolete(null, false)]
    public static XmlDocument createXmlDocument(string documentURI) => XmlDocument.createXmlDocument(new InputSource(documentURI), false);

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    [Obsolete(null, false)]
    public static XmlDocument createXmlDocument(InputStream @in, bool doValidate) => XmlDocument.createXmlDocument(new InputSource(@in), doValidate);

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    [Obsolete(null, false)]
    public static XmlDocument createXmlDocument(InputSource @in, bool doValidate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual Locale getLocale() => this.locale;

    public virtual void setLocale(Locale locale)
    {
      if (locale == null)
        locale = Locale.getDefault();
      this.locale = locale;
    }

    public virtual Locale chooseLocale(string[] languages)
    {
      Locale locale = XmlDocument.catalog.chooseLocale(languages);
      if (locale != null)
        this.setLocale(locale);
      return locale;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(OutputStream @out) => this.write((Writer) new OutputStreamWriter(@out, "UTF8"), "UTF-8");

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(Writer @out)
    {
      string encoding = (string) null;
      if (@out is OutputStreamWriter)
        encoding = XmlDocument.java2std(((OutputStreamWriter) @out).getEncoding());
      this.write(@out, encoding);
    }

    [JavaFlags(8)]
    public static string java2std(string encodingName)
    {
      if (encodingName == null)
        return (string) null;
      if (StringImpl.startsWith(encodingName, "ISO8859_"))
        return new StringBuffer().append("ISO-8859-").append(StringImpl.substring(encodingName, 8)).ToString();
      if (StringImpl.startsWith(encodingName, "8859_"))
        return new StringBuffer().append("ISO-8859-").append(StringImpl.substring(encodingName, 5)).ToString();
      if (StringImpl.equalsIgnoreCase("ASCII7", encodingName) || StringImpl.equalsIgnoreCase("ASCII", encodingName))
        return "US-ASCII";
      if (StringImpl.equalsIgnoreCase("UTF8", encodingName))
        return "UTF-8";
      if (StringImpl.startsWith(encodingName, "Unicode"))
        return "UTF-16";
      if (StringImpl.equalsIgnoreCase("SJIS", encodingName))
        return "Shift_JIS";
      if (StringImpl.equalsIgnoreCase("JIS", encodingName))
        return "ISO-2022-JP";
      return StringImpl.equalsIgnoreCase("EUCJIS", encodingName) ? "EUC-JP" : encodingName;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(Writer @out, string encoding)
    {
      @out.write("<?xml version=\"1.0\"");
      if (encoding != null)
      {
        @out.write(" encoding=\"");
        @out.write(encoding);
        @out.write(34);
      }
      @out.write("?>");
      @out.write(XmlDocument.eol);
      @out.write(XmlDocument.eol);
      this.writeChildrenXml(this.createWriteContext(@out, 0));
      @out.write(XmlDocument.eol);
      @out.flush();
    }

    public virtual XmlWriteContext createWriteContext(Writer @out) => (XmlWriteContext) new XmlDocument.ExtWriteContext(this, @out);

    public virtual XmlWriteContext createWriteContext(Writer @out, int level) => (XmlWriteContext) new XmlDocument.ExtWriteContext(this, @out, level);

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      string str = (string) null;
      if (writer is OutputStreamWriter)
        str = XmlDocument.java2std(((OutputStreamWriter) writer).getEncoding());
      writer.write("<?xml version=\"1.0\"");
      if (str != null)
      {
        writer.write(" encoding=\"");
        writer.write(str);
        writer.write(34);
      }
      writer.write("?>");
      writer.write(XmlDocument.eol);
      writer.write(XmlDocument.eol);
      this.writeChildrenXml(context);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeChildrenXml(XmlWriteContext context)
    {
      int length = this.getLength();
      Writer writer = context.getWriter();
      if (length == 0)
        return;
      for (int i = 0; i < length; ++i)
      {
        ((NodeBase) this.item(i)).writeXml(context);
        writer.write(XmlDocument.eol);
      }
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    [JavaFlags(0)]
    public override void checkChildType(int type)
    {
      switch (type)
      {
        case 1:
          break;
        case 7:
          break;
        case 8:
          break;
        case 10:
          break;
        default:
          throw new DomEx((short) 3);
      }
    }

    [JavaFlags(17)]
    public void setSystemId(string uri) => this.systemId = uri;

    [JavaFlags(17)]
    public string getSystemId() => this.systemId;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override Node appendChild(Node n)
    {
      switch (n)
      {
        case Element _ when this.getDocumentElement() != null:
          throw new DomEx((short) 3);
        case DocumentType _ when this.getDoctype() != null:
          throw new DomEx((short) 3);
        default:
          return base.appendChild(n);
      }
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override Node insertBefore(Node n, Node refNode)
    {
      if (!this.replaceRootElement && n is Element && this.getDocumentElement() != null)
        throw new DomEx((short) 3);
      return this.replaceRootElement || !(n is DocumentType) || this.getDoctype() == null ? base.insertBefore(n, refNode) : throw new DomEx((short) 3);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override Node replaceChild(Node newChild, Node refChild)
    {
      if (newChild is DocumentFragment)
      {
        int num1 = 0;
        int num2 = 0;
        this.replaceRootElement = false;
        ParentNode parentNode = (ParentNode) newChild;
        Node node;
        for (int i = 0; (node = parentNode.item(i)) != null; ++i)
        {
          switch (node)
          {
            case Element _:
              ++num1;
              break;
            case DocumentType _:
              ++num2;
              break;
          }
        }
        if (num1 > 1 || num2 > 1)
          throw new DomEx((short) 3);
        this.replaceRootElement = true;
      }
      return base.replaceChild(newChild, refChild);
    }

    [JavaFlags(17)]
    public override sealed short getNodeType() => 9;

    [JavaFlags(17)]
    public DocumentType getDoctype()
    {
      int i = 0;
      Node node;
      while (true)
      {
        node = this.item(i);
        if (node != null)
        {
          if (!(node is DocumentType))
            ++i;
          else
            goto label_4;
        }
        else
          break;
      }
      return (DocumentType) null;
label_4:
      return (DocumentType) node;
    }

    public virtual DocumentType setDoctype(
      string dtdPublicId,
      string dtdSystemId,
      string internalSubset)
    {
      Doctype doctype = (Doctype) this.getDoctype();
      if (doctype != null)
      {
        doctype.setPrintInfo(dtdPublicId, dtdSystemId, internalSubset);
      }
      else
      {
        doctype = new Doctype(dtdPublicId, dtdSystemId, internalSubset);
        doctype.setOwnerDocument(this);
        this.insertBefore((Node) doctype, this.getFirstChild());
      }
      return (DocumentType) doctype;
    }

    public virtual Element getDocumentElement()
    {
      int i = 0;
      Node node;
      while (true)
      {
        node = this.item(i);
        if (node != null)
        {
          if (!(node is Element))
            ++i;
          else
            goto label_4;
        }
        else
          break;
      }
      return (Element) null;
label_4:
      return (Element) node;
    }

    [Obsolete(null, false)]
    [JavaFlags(17)]
    public void setElementFactory(ElementFactory factory) => this.factory = factory;

    [Obsolete(null, false)]
    [JavaFlags(17)]
    public ElementFactory getElementFactory() => this.factory;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Element createElement(string tagName) => (Element) this.createElementEx(tagName);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Element createElementNS(string namespaceURI, string qualifiedName)
    {
      ElementNode2.checkArguments(namespaceURI, qualifiedName);
      ElementNode2 elementNode2 = new ElementNode2(namespaceURI, qualifiedName);
      elementNode2.setOwnerDocument(this);
      return (Element) elementNode2;
    }

    [Obsolete(null, false)]
    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    [JavaFlags(17)]
    public ElementEx createElementEx(string tagName)
    {
      if (!XmlNames.isName(tagName))
        throw new DomEx((short) 5);
      ElementNode elementNode;
      if (this.factory != null)
      {
        elementNode = (ElementNode) this.factory.createElementEx(tagName);
        elementNode.setTag(tagName);
      }
      else
        elementNode = new ElementNode(tagName);
      elementNode.setOwnerDocument(this);
      return (ElementEx) elementNode;
    }

    [Obsolete(null, false)]
    [JavaFlags(17)]
    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public ElementEx createElementEx(string uri, string tagName)
    {
      if (!XmlNames.isName(tagName))
        throw new DomEx((short) 5);
      ElementNode elementNode;
      if (this.factory != null)
      {
        elementNode = (ElementNode) this.factory.createElementEx(uri, tagName);
        elementNode.setTag(tagName);
      }
      else
        elementNode = new ElementNode(tagName);
      elementNode.setOwnerDocument(this);
      return (ElementEx) elementNode;
    }

    public virtual Text createTextNode(string text)
    {
      TextNode textNode = new TextNode();
      textNode.setOwnerDocument(this);
      if (text != null)
        textNode.setText(StringImpl.toCharArray(text));
      return (Text) textNode;
    }

    public virtual CDATASection createCDATASection(string text)
    {
      CDataNode cdataNode = new CDataNode();
      if (text != null)
        cdataNode.setText(StringImpl.toCharArray(text));
      cdataNode.setOwnerDocument(this);
      return (CDATASection) cdataNode;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    [JavaFlags(0)]
    public virtual TextNode newText(char[] buf, int offset, int len)
    {
      TextNode textNode = (TextNode) this.createTextNode((string) null);
      int length = len;
      char[] buf1 = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) buf, offset, (object) buf1, 0, len);
      textNode.setText(buf1);
      return textNode;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual ProcessingInstruction createProcessingInstruction(
      string target,
      string instructions)
    {
      PINode piNode = XmlNames.isName(target) ? new PINode(target, instructions) : throw new DomEx((short) 5);
      piNode.setOwnerDocument(this);
      return (ProcessingInstruction) piNode;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Attr createAttribute(string name)
    {
      AttributeNode1 attributeNode1 = XmlNames.isName(name) ? new AttributeNode1(name, "", true, (string) null) : throw new DomEx((short) 5);
      attributeNode1.setOwnerDocument(this);
      return (Attr) attributeNode1;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Attr createAttributeNS(string namespaceURI, string qualifiedName)
    {
      AttributeNode.checkArguments(namespaceURI, qualifiedName);
      AttributeNode attributeNode = new AttributeNode(namespaceURI, qualifiedName, "", true, (string) null);
      attributeNode.setOwnerDocument(this);
      return (Attr) attributeNode;
    }

    public virtual Comment createComment(string data)
    {
      CommentNode commentNode = new CommentNode(data);
      commentNode.setOwnerDocument(this);
      return (Comment) commentNode;
    }

    public virtual Document getOwnerDoc() => (Document) null;

    public virtual DOMImplementation getImplementation() => DOMImplementationImpl.getDOMImplementation();

    public virtual DocumentFragment createDocumentFragment()
    {
      XmlDocument.DocFragNode docFragNode = new XmlDocument.DocFragNode();
      docFragNode.setOwnerDocument(this);
      return (DocumentFragment) docFragNode;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual EntityReference createEntityReference(string name)
    {
      XmlDocument.EntityRefNode entityRefNode = XmlNames.isName(name) ? new XmlDocument.EntityRefNode(name) : throw new DomEx((short) 5);
      entityRefNode.setOwnerDocument(this);
      return (EntityReference) entityRefNode;
    }

    [JavaFlags(17)]
    public override sealed string getNodeName() => "#document";

    public override Node cloneNode(bool deep)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.systemId = this.systemId;
      if (deep)
      {
        Node node1;
        for (int i = 0; (node1 = this.item(i)) != null; ++i)
        {
          if (!(node1 is DocumentType))
          {
            Node node2 = node1.cloneNode(true);
            xmlDocument.changeNodeOwner(node2);
            xmlDocument.appendChild(node2);
          }
        }
      }
      return (Node) xmlDocument;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    [JavaFlags(17)]
    public void changeNodeOwner(Node node)
    {
      if (node.getOwnerDocument() == this)
        return;
      if (!(node is NodeBase))
        throw new DomEx((short) 4);
      switch (node.getNodeType())
      {
        case 6:
        case 9:
        case 10:
        case 12:
          throw new DomEx((short) 3);
        default:
          if (node is AttributeNode)
          {
            Element ownerElement = ((AttributeNode) node).getOwnerElement();
            if (ownerElement != null && ownerElement.getOwnerDocument() != this)
              throw new DomEx((short) 3);
          }
          ((NodeBase) node.getParentNode())?.removeChild(node);
          TreeWalker treeWalker = new TreeWalker(node);
          for (NodeBase nodeBase = (NodeBase) treeWalker.getCurrent(); nodeBase != null; nodeBase = (NodeBase) treeWalker.getNext())
          {
            nodeBase.setOwnerDocument(this);
            if (nodeBase is Element)
            {
              NamedNodeMap attributes = nodeBase.getAttributes();
              int length = attributes.getLength();
              for (int index = 0; index < length; ++index)
                this.changeNodeOwner(attributes.item(index));
            }
          }
          break;
      }
    }

    public virtual Element getElementById(string elementId) => (Element) this.getElementExById(elementId);

    [Obsolete(null, false)]
    public virtual ElementEx getElementExById(string id)
    {
      if (id == null)
        throw new IllegalArgumentException(this.getMessage("XD-000"));
      TreeWalker treeWalker = new TreeWalker((Node) this);
      ElementEx nextElement;
      while ((nextElement = (ElementEx) treeWalker.getNextElement((string) null)) != null)
      {
        string idAttributeName = nextElement.getIdAttributeName();
        if (idAttributeName != null && StringImpl.equals(nextElement.getAttribute(idAttributeName), (object) id))
          return nextElement;
      }
      return (ElementEx) null;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node importNode(Node importedNode, bool deep)
    {
      Node initial;
      switch (importedNode.getNodeType())
      {
        case 1:
          initial = (Node) ((ElementNode2) importedNode).createCopyForImportNode(deep);
          break;
        case 2:
          initial = importedNode.cloneNode(true);
          break;
        case 5:
          initial = importedNode.cloneNode(false);
          break;
        case 6:
          initial = importedNode.cloneNode(deep);
          break;
        case 9:
        case 10:
          throw new DomEx((short) 9);
        case 11:
          initial = !deep ? (Node) new XmlDocument.DocFragNode() : importedNode.cloneNode(true);
          break;
        default:
          initial = importedNode.cloneNode(false);
          break;
      }
      TreeWalker treeWalker = new TreeWalker(initial);
      for (NodeBase nodeBase = (NodeBase) treeWalker.getCurrent(); nodeBase != null; nodeBase = (NodeBase) treeWalker.getNext())
      {
        nodeBase.setOwnerDocument(this);
        if (nodeBase is Element)
        {
          NamedNodeMap attributes = nodeBase.getAttributes();
          int length = attributes.getLength();
          for (int index = 0; index < length; ++index)
            this.changeNodeOwner(attributes.item(index));
        }
      }
      return initial;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static XmlDocument()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(56)]
    [JavaInterfaces("1;org/w3c/dom/DocumentFragment;")]
    public sealed class DocFragNode : ParentNode, DocumentFragment
    {
      [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
      [JavaFlags(0)]
      public override void checkChildType(int type)
      {
        switch (type)
        {
          case 1:
            break;
          case 3:
            break;
          case 4:
            break;
          case 5:
            break;
          case 7:
            break;
          case 8:
            break;
          default:
            throw new DomEx((short) 3);
        }
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public override void writeXml(XmlWriteContext context) => this.writeChildrenXml(context);

      public override Node getParentNode() => (Node) null;

      public virtual void setParentNode(Node p)
      {
        if (p != null)
          throw new IllegalArgumentException();
      }

      public override short getNodeType() => 11;

      public override string getNodeName() => "#document-fragment";

      public override Node cloneNode(bool deep)
      {
        XmlDocument.DocFragNode docFragNode = new XmlDocument.DocFragNode();
        docFragNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
        if (deep)
        {
          Node node;
          for (int i = 0; (node = this.item(i)) != null; ++i)
          {
            Node newChild = node.cloneNode(true);
            docFragNode.appendChild(newChild);
          }
        }
        return (Node) docFragNode;
      }

      [JavaFlags(0)]
      public DocFragNode()
      {
      }
    }

    [JavaInterfaces("1;org/w3c/dom/EntityReference;")]
    [JavaFlags(56)]
    public sealed class EntityRefNode : ParentNode, EntityReference
    {
      private string entity;

      [JavaFlags(0)]
      public EntityRefNode(string name) => this.entity = name != null ? name : throw new IllegalArgumentException(this.getMessage("XD-002"));

      [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
      [JavaFlags(0)]
      public override void checkChildType(int type)
      {
        switch (type)
        {
          case 1:
            break;
          case 3:
            break;
          case 4:
            break;
          case 5:
            break;
          case 7:
            break;
          case 8:
            break;
          default:
            throw new DomEx((short) 3);
        }
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public override void writeXml(XmlWriteContext context)
      {
        if (!context.isEntityDeclared(this.entity))
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) this.entity;
          throw new IOException(this.getMessage("XD-003", parameters));
        }
        Writer writer = context.getWriter();
        writer.write(38);
        writer.write(this.entity);
        writer.write(59);
      }

      public override short getNodeType() => 5;

      public override string getNodeName() => this.entity;

      public override Node cloneNode(bool deep)
      {
        XmlDocument.EntityRefNode entityRefNode = new XmlDocument.EntityRefNode(this.entity);
        entityRefNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
        if (deep)
        {
          Node node;
          for (int i = 0; (node = this.item(i)) != null; ++i)
          {
            Node newChild = node.cloneNode(true);
            entityRefNode.appendChild(newChild);
          }
        }
        return (Node) entityRefNode;
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class ExtWriteContext : XmlWriteContext
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private XmlDocument this\u00240;

      [JavaFlags(0)]
      public ExtWriteContext(XmlDocument _param1, Writer @out)
        : base(@out)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(0)]
      public ExtWriteContext(XmlDocument _param1, Writer @out, int level)
        : base(@out, level)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      public override bool isEntityDeclared(string name)
      {
        if (base.isEntityDeclared(name))
          return true;
        DocumentType doctype = this.this\u00240.getDoctype();
        return doctype != null && doctype.getEntities().getNamedItem(name) != null;
      }
    }

    [JavaFlags(40)]
    public class Catalog : MessageCatalog
    {
      [JavaFlags(0)]
      public Catalog()
        : base(Class.FromType(typeof (XmlDocument.Catalog)))
      {
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class \u0031 : DefaultHandler
    {
      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void error(SAXParseException e) => throw e;
    }
  }
}
