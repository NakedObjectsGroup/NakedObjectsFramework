// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.ElementNode2
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.apache.crimson.util;
using org.w3c.dom;
using System.ComponentModel;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/apache/crimson/tree/ElementEx;")]
  public class ElementNode2 : NamespacedNode, ElementEx
  {
    [JavaFlags(4)]
    public AttributeSet attributes;
    private string idAttributeName;
    private object userObject;
    private static readonly char[] tagStart;
    private static readonly char[] tagEnd;

    [JavaThrownExceptions("1;org/apache/crimson/tree/DomEx;")]
    public ElementNode2(string namespaceURI, string qName)
      : base(namespaceURI, qName)
    {
    }

    [JavaFlags(0)]
    public virtual ElementNode2 makeClone()
    {
      ElementNode2 elementNode2 = new ElementNode2(this.namespaceURI, this.qName);
      if (this.attributes != null)
      {
        elementNode2.attributes = new AttributeSet(this.attributes, true);
        elementNode2.attributes.setOwnerElement((Element) elementNode2);
      }
      elementNode2.idAttributeName = this.idAttributeName;
      elementNode2.userObject = this.userObject;
      elementNode2.ownerDocument = this.ownerDocument;
      return elementNode2;
    }

    [JavaFlags(0)]
    public virtual ElementNode2 createCopyForImportNode(bool deep)
    {
      ElementNode2 elementNode2 = new ElementNode2(this.namespaceURI, this.qName);
      if (this.attributes != null)
      {
        elementNode2.attributes = new AttributeSet(this.attributes);
        elementNode2.attributes.setOwnerElement((Element) elementNode2);
      }
      elementNode2.userObject = this.userObject;
      if (deep)
      {
        elementNode2.ownerDocument = this.ownerDocument;
        int i = 0;
        while (true)
        {
          Node node = this.item(i);
          if (node != null)
          {
            if (node is ElementNode2)
              elementNode2.appendChild((Node) ((ElementNode2) node).createCopyForImportNode(true));
            else
              elementNode2.appendChild(node.cloneNode(true));
            ++i;
          }
          else
            break;
        }
      }
      return elementNode2;
    }

    [JavaThrownExceptions("1;org/apache/crimson/tree/DomEx;")]
    [JavaFlags(8)]
    public static void checkArguments(string namespaceURI, string qualifiedName)
    {
      int num = qualifiedName != null ? StringImpl.indexOf(qualifiedName, 58) : throw new DomEx((short) 14);
      if (num <= 0)
      {
        if (!XmlNames.isUnqualifiedName(qualifiedName))
          throw new DomEx((short) 5);
      }
      else
      {
        string str1 = StringImpl.lastIndexOf(qualifiedName, 58) == num ? StringImpl.substring(qualifiedName, 0, num) : throw new DomEx((short) 14);
        string str2 = StringImpl.substring(qualifiedName, num + 1);
        if (!XmlNames.isUnqualifiedName(str1) || !XmlNames.isUnqualifiedName(str2))
          throw new DomEx((short) 5);
        if (namespaceURI == null || StringImpl.equals(str1, (object) "xml") && !StringImpl.equals("http://www.w3.org/XML/1998/namespace", (object) namespaceURI))
          throw new DomEx((short) 14);
      }
    }

    public override void trimToSize()
    {
      base.trimToSize();
      if (this.attributes == null)
        return;
      this.attributes.trimToSize();
    }

    [JavaFlags(0)]
    public virtual void setAttributes(AttributeSet a)
    {
      AttributeSet attributes = this.attributes;
      if (attributes != null && attributes.isReadonly())
        throw new DomEx((short) 7);
      a?.setOwnerElement((Element) this);
      this.attributes = a;
      attributes?.setOwnerElement((Element) null);
    }

    [JavaFlags(0)]
    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
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

    public override void setReadonly(bool deep)
    {
      if (this.attributes != null)
        this.attributes.setReadonly();
      base.setReadonly(deep);
    }

    public override NamedNodeMap getAttributes()
    {
      if (this.attributes == null)
        this.attributes = new AttributeSet((Element) this);
      return (NamedNodeMap) this.attributes;
    }

    public override bool hasAttributes() => this.attributes != null;

    public override string ToString()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      if (this.qName == null)
        throw new IllegalStateException(this.getMessage("EN-002"));
      writer.write(ElementNode2.tagStart, 0, 1);
      writer.write(this.qName);
      if (this.attributes != null)
        this.attributes.writeXml(context);
      if (!this.hasChildNodes())
      {
        writer.write(ElementNode2.tagEnd, 0, 3);
      }
      else
      {
        writer.write(ElementNode2.tagEnd, 2, 1);
        this.writeChildrenXml(context);
        writer.write(ElementNode2.tagStart, 0, 2);
        writer.write(this.qName);
        writer.write(ElementNode2.tagEnd, 2, 1);
      }
    }

    public virtual void setIdAttributeName(string attName)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      this.idAttributeName = attName;
    }

    public virtual string getIdAttributeName() => this.idAttributeName;

    public virtual void setUserObject(object userObject) => this.userObject = userObject;

    public virtual object getUserObject() => this.userObject;

    public override short getNodeType() => 1;

    public virtual string getTagName() => this.qName;

    public virtual bool hasAttribute(string name) => this.getAttributeNode(name) != null;

    public virtual bool hasAttributeNS(string namespaceURI, string localName) => this.getAttributeNodeNS(namespaceURI, localName) != null;

    public virtual string getAttribute(string name) => this.attributes == null ? "" : this.attributes.getValue(name);

    public virtual string getAttributeNS(string namespaceURI, string localName)
    {
      if (this.attributes == null)
        return "";
      Attr attributeNodeNs = this.getAttributeNodeNS(namespaceURI, localName);
      return attributeNodeNs == null ? "" : attributeNodeNs.getValue();
    }

    public virtual Attr getAttributeNodeNS(string namespaceURI, string localName)
    {
      if (localName == null)
        return (Attr) null;
      if (this.attributes == null)
        return (Attr) null;
      int index = 0;
      AttributeNode attributeNode;
      while (true)
      {
        attributeNode = (AttributeNode) this.attributes.item(index);
        if (attributeNode != null)
        {
          if (!StringImpl.equals(localName, (object) attributeNode.getLocalName()) || (object) attributeNode.getNamespaceURI() != (object) namespaceURI && !StringImpl.equals(attributeNode.getNamespaceURI(), (object) namespaceURI))
            ++index;
          else
            goto label_8;
        }
        else
          break;
      }
      return (Attr) null;
label_8:
      return (Attr) attributeNode;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual void setAttribute(string name, string value)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      if (!XmlNames.isName(name))
        throw new DomEx((short) 5);
      if (this.attributes == null)
        this.attributes = new AttributeSet((Element) this);
      NodeBase namedItem;
      if ((namedItem = (NodeBase) this.attributes.getNamedItem(name)) != null)
      {
        namedItem.setNodeValue(value);
      }
      else
      {
        NodeBase nodeBase = (NodeBase) new AttributeNode1(name, value, true, (string) null);
        nodeBase.setOwnerDocument((XmlDocument) this.getOwnerDocument());
        this.attributes.setNamedItem((Node) nodeBase);
      }
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual void setAttributeNS(string namespaceURI, string qualifiedName, string value)
    {
      AttributeNode.checkArguments(namespaceURI, qualifiedName);
      Attr attributeNodeNs = this.getAttributeNodeNS(namespaceURI, XmlNames.getLocalPart(qualifiedName));
      if (attributeNodeNs == null)
      {
        AttributeNode attributeNode = new AttributeNode(namespaceURI, qualifiedName, value, true, (string) null);
        attributeNode.setOwnerDocument((XmlDocument) this.getOwnerDocument());
        this.setAttributeNodeNS((Attr) attributeNode);
      }
      else
      {
        attributeNodeNs.setValue(value);
        attributeNodeNs.setPrefix(XmlNames.getPrefix(qualifiedName));
      }
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Attr setAttributeNodeNS(Attr newAttr)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      if (newAttr.getOwnerDocument() != this.getOwnerDocument())
        throw new DomEx((short) 4);
      if (this.attributes == null)
        this.attributes = new AttributeSet((Element) this);
      return (Attr) this.attributes.setNamedItemNS((Node) newAttr);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual void removeAttribute(string name)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      if (this.attributes == null)
        return;
      try
      {
        this.attributes.removeNamedItem(name);
      }
      catch (DOMException ex)
      {
        DOMException domException1 = ex;
        if (domException1.code == (short) 8)
          return;
        DOMException domException2 = domException1;
        if (domException2 != ex)
          throw domException2;
        throw;
      }
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual void removeAttributeNS(string namespaceURI, string localName)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      try
      {
        this.attributes.removeNamedItemNS(namespaceURI, localName);
      }
      catch (DOMException ex)
      {
        DOMException domException1 = ex;
        if (domException1.code == (short) 8)
          return;
        DOMException domException2 = domException1;
        if (domException2 != ex)
          throw domException2;
        throw;
      }
    }

    public virtual Attr getAttributeNode(string name) => this.attributes != null ? (Attr) this.attributes.getNamedItem(name) : (Attr) null;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Attr setAttributeNode(Attr newAttr)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      if (!(newAttr is AttributeNode))
        throw new DomEx((short) 4);
      if (this.attributes == null)
        this.attributes = new AttributeSet((Element) this);
      return (Attr) this.attributes.setNamedItem((Node) newAttr);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Attr removeAttributeNode(Attr oldAttr)
    {
      if (this.isReadonly())
        throw new DomEx((short) 7);
      Attr attributeNode = this.getAttributeNode(oldAttr.getNodeName());
      if (attributeNode == null)
        throw new DomEx((short) 8);
      this.removeAttribute(attributeNode.getNodeName());
      return attributeNode;
    }

    public override Node cloneNode(bool deep)
    {
      try
      {
        ElementNode2 elementNode2 = this.makeClone();
        if (deep)
        {
          int i = 0;
          while (true)
          {
            Node node = this.item(i);
            if (node != null)
            {
              elementNode2.appendChild(node.cloneNode(true));
              ++i;
            }
            else
              break;
          }
        }
        return (Node) elementNode2;
      }
      catch (DOMException ex)
      {
        throw new RuntimeException(this.getMessage("EN-001"));
      }
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(Writer @out) => this.writeXml(new XmlWriteContext(@out));

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ElementNode2()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
