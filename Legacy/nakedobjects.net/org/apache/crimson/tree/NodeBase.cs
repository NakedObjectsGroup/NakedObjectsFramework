// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.NodeBase
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaFlags(1056)]
  [JavaInterfaces("4;org/w3c/dom/Node;org/apache/crimson/tree/NodeEx;org/w3c/dom/NodeList;org/apache/crimson/tree/XmlWritable;")]
  public abstract class NodeBase : Node, NodeEx, NodeList, XmlWritable
  {
    private ParentNode parent;
    private int parentIndex;
    [JavaFlags(0)]
    public XmlDocument ownerDocument;
    [JavaFlags(0)]
    public bool @readonly;

    [JavaFlags(0)]
    public NodeBase() => this.parentIndex = -1;

    [JavaFlags(0)]
    public virtual ParentNode getParentImpl() => this.parent;

    public virtual bool isReadonly() => this.@readonly;

    public virtual void setReadonly(bool deep)
    {
      this.@readonly = true;
      if (!deep)
        return;
      TreeWalker treeWalker = new TreeWalker((Node) this);
      Node next;
      while ((next = treeWalker.getNext()) != null)
        ((NodeBase) next).setReadonly(false);
    }

    public virtual string getLanguage() => this.getInheritedAttribute("xml:lang");

    public virtual string getInheritedAttribute(string name)
    {
      NodeBase nodeBase = this;
      Attr attr = (Attr) null;
      while (!(nodeBase is ElementNode2) || (attr = ((ElementNode2) nodeBase).getAttributeNode(name)) == null)
      {
        nodeBase = (NodeBase) nodeBase.getParentImpl();
        if (nodeBase == null)
          break;
      }
      return attr?.getValue();
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void writeChildrenXml(XmlWriteContext context)
    {
    }

    public virtual Node getParentNode() => (Node) this.parent;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    [JavaFlags(0)]
    public virtual void setParentNode(ParentNode arg, int index)
    {
      if (this.parent != null && arg != null)
        this.parent.removeChild((Node) this);
      this.parent = arg;
      this.parentIndex = index;
    }

    [JavaFlags(4)]
    public virtual void setOwnerDocument(XmlDocument doc) => this.ownerDocument = doc;

    public virtual Document getOwnerDocument() => (Document) this.ownerDocument;

    public virtual bool hasChildNodes() => false;

    public virtual void setNodeValue(string value)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
    }

    public virtual string getNodeValue() => (string) null;

    public virtual Node getFirstChild() => (Node) null;

    public virtual int getLength() => 0;

    public virtual Node item(int i) => (Node) null;

    public virtual NodeList getChildNodes() => (NodeList) this;

    public virtual Node getLastChild() => (Node) null;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node appendChild(Node newChild) => throw new DomEx((short) 3);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node insertBefore(Node newChild, Node refChild) => throw new DomEx((short) 3);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node replaceChild(Node newChild, Node refChild) => throw new DomEx((short) 3);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual Node removeChild(Node oldChild) => throw new DomEx((short) 3);

    public virtual Node getNextSibling()
    {
      if (this.parent == null)
        return (Node) null;
      if (this.parentIndex < 0 || this.parent.item(this.parentIndex) != this)
        this.parentIndex = this.parent.getIndexOf((Node) this);
      return this.parent.item(this.parentIndex + 1);
    }

    public virtual Node getPreviousSibling()
    {
      if (this.parent == null)
        return (Node) null;
      if (this.parentIndex < 0 || this.parent.item(this.parentIndex) != this)
        this.parentIndex = this.parent.getIndexOf((Node) this);
      return this.parent.item(this.parentIndex - 1);
    }

    public virtual NamedNodeMap getAttributes() => (NamedNodeMap) null;

    public virtual void normalize()
    {
    }

    public virtual bool isSupported(string feature, string version) => DOMImplementationImpl.hasFeature0(feature, version);

    public virtual string getNamespaceURI() => (string) null;

    public virtual string getPrefix() => (string) null;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public virtual void setPrefix(string prefix) => throw new DomEx((short) 14);

    public virtual string getLocalName() => (string) null;

    public virtual bool hasAttributes() => false;

    public virtual int getIndexOf(Node maybeChild) => -1;

    [JavaFlags(0)]
    public virtual string getMessage(string messageId) => this.getMessage(messageId, (object[]) null);

    [JavaFlags(0)]
    public virtual string getMessage(string messageId, object[] parameters)
    {
      Locale locale = !(this is XmlDocument) ? (this.ownerDocument != null ? this.ownerDocument.getLocale() : Locale.getDefault()) : ((XmlDocument) this).getLocale();
      return XmlDocument.catalog.getMessage(locale, messageId, parameters);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NodeBase nodeBase = this;
      ObjectImpl.clone((object) nodeBase);
      return ((object) nodeBase).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract Node cloneNode(bool deep);

    public abstract string getNodeName();

    public abstract short getNodeType();

    [JavaThrownExceptions("1;java/io/IOException;")]
    public abstract void writeXml(XmlWriteContext context);
  }
}
