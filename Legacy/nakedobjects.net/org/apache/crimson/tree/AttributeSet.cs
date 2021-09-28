// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.AttributeSet
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.apache.crimson.parser;
using org.apache.crimson.util;
using org.w3c.dom;
using org.xml.sax;

namespace org.apache.crimson.tree
{
  [JavaFlags(48)]
  [JavaInterfaces("2;org/w3c/dom/NamedNodeMap;org/apache/crimson/tree/XmlWritable;")]
  public sealed class AttributeSet : NamedNodeMap, XmlWritable
  {
    private bool @readonly;
    private Vector list;
    private Element ownerElement;

    private AttributeSet()
    {
    }

    [JavaFlags(0)]
    public AttributeSet(Element ownerElement)
    {
      this.list = new Vector(5);
      this.ownerElement = ownerElement;
    }

    [JavaFlags(0)]
    public AttributeSet(AttributeSet original, bool deep)
    {
      int length = original.getLength();
      this.list = new Vector(length);
      for (int index = 0; index < length; ++index)
      {
        Node node = original.item(index);
        if (!(node is AttributeNode))
          throw new IllegalArgumentException(((NodeBase) node).getMessage("A-003"));
        this.list.addElement((object) ((AttributeNode) node).cloneAttributeNode(deep));
      }
    }

    [JavaFlags(0)]
    public AttributeSet(AttributeSet original)
    {
      int length = original.getLength();
      this.list = new Vector(length);
      for (int index = 0; index < length; ++index)
      {
        Node node = original.item(index);
        AttributeNode attributeNode = node is AttributeNode ? (AttributeNode) node : throw new IllegalArgumentException(((NodeBase) node).getMessage("A-003"));
        if (attributeNode.getSpecified())
          this.list.addElement((object) attributeNode.cloneAttributeNode(true));
      }
      this.list.trimToSize();
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    [JavaFlags(8)]
    public static AttributeSet createAttributeSet2(Attributes source)
    {
      AttributeSet attributeSet = new AttributeSet();
      int length = source.getLength();
      AttributesEx attributesEx = (AttributesEx) null;
      attributeSet.list = new Vector(length);
      if (source is AttributesEx)
        attributesEx = (AttributesEx) source;
      for (int index = 0; index < length; ++index)
      {
        string qname = source.getQName(index);
        string namespaceURI;
        if (StringImpl.equals("xmlns", (object) qname) || StringImpl.equals("xmlns", (object) XmlNames.getPrefix(qname)))
        {
          namespaceURI = "http://www.w3.org/2000/xmlns/";
        }
        else
        {
          namespaceURI = source.getURI(index);
          if (StringImpl.equals("", (object) namespaceURI))
            namespaceURI = (string) null;
        }
        AttributeNode attributeNode = new AttributeNode(namespaceURI, qname, source.getValue(index), attributesEx == null || attributesEx.isSpecified(index), attributesEx?.getDefault(index));
        attributeSet.list.addElement((object) attributeNode);
      }
      return attributeSet;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    [JavaFlags(8)]
    public static AttributeSet createAttributeSet1(Attributes source)
    {
      AttributeSet attributeSet = new AttributeSet();
      int length = source.getLength();
      AttributesEx attributesEx = (AttributesEx) null;
      attributeSet.list = new Vector(length);
      if (source is AttributesEx)
        attributesEx = (AttributesEx) source;
      for (int index = 0; index < length; ++index)
      {
        AttributeNode1 attributeNode1 = new AttributeNode1(source.getQName(index), source.getValue(index), attributesEx == null || attributesEx.isSpecified(index), attributesEx?.getDefault(index));
        attributeSet.list.addElement((object) attributeNode1);
      }
      return attributeSet;
    }

    [JavaFlags(0)]
    public virtual void trimToSize() => this.list.trimToSize();

    public virtual void setReadonly()
    {
      this.@readonly = true;
      for (int index = 0; index < this.list.size(); ++index)
        ((NodeBase) this.list.elementAt(index)).setReadonly(true);
    }

    public virtual bool isReadonly()
    {
      if (this.@readonly)
        return true;
      for (int index = 0; index < this.list.size(); ++index)
      {
        if (((NodeBase) this.list.elementAt(index)).isReadonly())
          return true;
      }
      return false;
    }

    [JavaFlags(0)]
    public virtual void setOwnerElement(Element e)
    {
      this.ownerElement = e == null || this.ownerElement == null ? e : throw new IllegalStateException(((NodeBase) e).getMessage("A-004"));
      int num = this.list.size();
      for (int index = 0; index < num; ++index)
      {
        AttributeNode attributeNode = (AttributeNode) this.list.elementAt(index);
        attributeNode.setOwnerElement((Element) null);
        attributeNode.setOwnerElement(e);
      }
    }

    [JavaFlags(0)]
    public virtual string getValue(string name)
    {
      Attr namedItem = (Attr) this.getNamedItem(name);
      return namedItem == null ? "" : namedItem.getValue();
    }

    public virtual Node getNamedItem(string name)
    {
      int num = this.list.size();
      for (int index = 0; index < num; ++index)
      {
        Node node = this.item(index);
        if (StringImpl.equals(node.getNodeName(), (object) name))
          return node;
      }
      return (Node) null;
    }

    public virtual Node getNamedItemNS(string namespaceURI, string localName)
    {
      if (localName == null)
        return (Node) null;
      for (int index = 0; index < this.list.size(); ++index)
      {
        Node node = this.item(index);
        string localName1 = node.getLocalName();
        if (StringImpl.equals(localName, (object) localName1))
        {
          string namespaceUri = node.getNamespaceURI();
          if ((object) namespaceURI == (object) namespaceUri || namespaceURI != null && StringImpl.equals(namespaceURI, (object) namespaceUri))
            return node;
        }
      }
      return (Node) null;
    }

    public virtual int getLength() => this.list.size();

    public virtual Node item(int index) => index < 0 || index >= this.list.size() ? (Node) null : (Node) this.list.elementAt(index);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node removeNamedItem(string name)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      for (int index = 0; index < this.list.size(); ++index)
      {
        Node node = (Node) this.list.elementAt(index);
        if (StringImpl.equals(node.getNodeName(), (object) name))
        {
          this.list.removeElementAt(index);
          AttributeNode attributeNode1 = (AttributeNode) node;
          string defaultValue = attributeNode1.getDefaultValue();
          if (defaultValue != null)
          {
            AttributeNode attributeNode2 = attributeNode1.cloneAttributeNode(true);
            attributeNode2.setOwnerElement(attributeNode1.getOwnerElement());
            attributeNode2.setValue(defaultValue);
            attributeNode2.setSpecified(false);
            this.list.addElement((object) attributeNode2);
          }
          attributeNode1.setOwnerElement((Element) null);
          return (Node) attributeNode1;
        }
      }
      throw new DomEx((short) 8);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node removeNamedItemNS(string namespaceURI, string localName)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      if (localName == null)
        throw new DomEx((short) 8);
      for (int index = 0; index < this.list.size(); ++index)
      {
        Node node = (Node) this.list.elementAt(index);
        string localName1 = node.getLocalName();
        if (StringImpl.equals(localName, (object) localName1))
        {
          string namespaceUri = node.getNamespaceURI();
          if ((object) namespaceURI == (object) namespaceUri || namespaceURI != null && StringImpl.equals(namespaceURI, (object) namespaceUri))
          {
            this.list.removeElementAt(index);
            AttributeNode attributeNode1 = (AttributeNode) node;
            string defaultValue = attributeNode1.getDefaultValue();
            if (defaultValue != null)
            {
              AttributeNode attributeNode2 = attributeNode1.cloneAttributeNode(true);
              attributeNode2.setOwnerElement(attributeNode1.getOwnerElement());
              attributeNode2.setValue(defaultValue);
              attributeNode2.setSpecified(false);
              this.list.addElement((object) attributeNode2);
            }
            attributeNode1.setOwnerElement((Element) null);
            return (Node) attributeNode1;
          }
        }
      }
      throw new DomEx((short) 8);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node setNamedItem(Node value)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      AttributeNode attributeNode1 = value is AttributeNode && value.getOwnerDocument() == this.ownerElement.getOwnerDocument() ? (AttributeNode) value : throw new DomEx((short) 4);
      if (attributeNode1.getOwnerElement() != null)
        throw new DomEx((short) 10);
      int num = this.list.size();
      for (int index = 0; index < num; ++index)
      {
        AttributeNode attributeNode2 = (AttributeNode) this.item(index);
        if (StringImpl.equals(attributeNode2.getNodeName(), (object) value.getNodeName()))
        {
          if (attributeNode2.isReadonly())
            throw new DomEx((short) 7);
          attributeNode1.setOwnerElement(this.ownerElement);
          this.list.setElementAt((object) attributeNode1, index);
          attributeNode2.setOwnerElement((Element) null);
          return (Node) attributeNode2;
        }
      }
      attributeNode1.setOwnerElement(this.ownerElement);
      this.list.addElement((object) value);
      return (Node) null;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node setNamedItemNS(Node arg)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      AttributeNode attributeNode1 = arg is AttributeNode && arg.getOwnerDocument() == this.ownerElement.getOwnerDocument() ? (AttributeNode) arg : throw new DomEx((short) 4);
      string str = attributeNode1.getOwnerElement() == null ? attributeNode1.getLocalName() : throw new DomEx((short) 10);
      string namespaceUri1 = attributeNode1.getNamespaceURI();
      int num = this.list.size();
      for (int index = 0; index < num; ++index)
      {
        AttributeNode attributeNode2 = (AttributeNode) this.item(index);
        string localName = attributeNode2.getLocalName();
        string namespaceUri2 = attributeNode2.getNamespaceURI();
        if (((object) str == (object) localName || str != null && StringImpl.equals(str, (object) localName)) && ((object) namespaceUri1 == (object) namespaceUri2 || namespaceUri1 != null && StringImpl.equals(namespaceUri1, (object) namespaceUri2)))
        {
          if (attributeNode2.isReadonly())
            throw new DomEx((short) 7);
          attributeNode1.setOwnerElement(this.ownerElement);
          this.list.setElementAt((object) attributeNode1, index);
          attributeNode2.setOwnerElement((Element) null);
          return (Node) attributeNode2;
        }
      }
      attributeNode1.setOwnerElement(this.ownerElement);
      this.list.addElement((object) attributeNode1);
      return (Node) null;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      int num = this.list.size();
      for (int index = 0; index < num; ++index)
      {
        AttributeNode attributeNode = (AttributeNode) this.list.elementAt(index);
        if (attributeNode.getSpecified())
        {
          writer.write(32);
          attributeNode.writeXml(context);
        }
      }
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void writeChildrenXml(XmlWriteContext context)
    {
    }

    public override string ToString()
    {
      try
      {
        CharArrayWriter charArrayWriter = new CharArrayWriter();
        this.writeXml(new XmlWriteContext((Writer) charArrayWriter));
        return charArrayWriter.ToString();
      }
      catch (IOException ex)
      {
        return ObjectImpl.jloToString((object) this);
      }
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AttributeSet attributeSet = this;
      ObjectImpl.clone((object) attributeSet);
      return ((object) attributeSet).MemberwiseClone();
    }
  }
}
