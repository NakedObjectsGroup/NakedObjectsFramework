// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.TreeWalker
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  public class TreeWalker
  {
    private Node startPoint;
    private Node current;

    public TreeWalker(Node initial)
    {
      if (initial == null)
        throw new IllegalArgumentException(XmlDocument.catalog.getMessage(Locale.getDefault(), "TW-004"));
      if (!(initial is NodeBase))
        throw new IllegalArgumentException(XmlDocument.catalog.getMessage(Locale.getDefault(), "TW-003"));
      this.startPoint = this.current = initial;
    }

    public virtual Node getCurrent() => this.current;

    public virtual Node getNext()
    {
      if (this.current == null)
        return (Node) null;
      switch (this.current.getNodeType())
      {
        case 1:
        case 5:
        case 9:
        case 11:
          Node firstChild = this.current.getFirstChild();
          if (firstChild != null)
          {
            this.current = firstChild;
            return firstChild;
          }
          goto case 2;
        case 2:
        case 3:
        case 4:
        case 6:
        case 7:
        case 8:
        case 10:
        case 12:
          for (Node node = this.current; node != null && node != this.startPoint; node = node.getParentNode())
          {
            Node nextSibling = node.getNextSibling();
            if (nextSibling != null)
            {
              this.current = nextSibling;
              return nextSibling;
            }
          }
          this.current = (Node) null;
          return (Node) null;
        default:
          NodeBase startPoint = (NodeBase) this.startPoint;
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) Short.toString(this.current.getNodeType());
          throw new InternalError(startPoint.getMessage("TW-000", parameters));
      }
    }

    public virtual Element getNextElement(string tag)
    {
      for (Node next = this.getNext(); next != null; next = this.getNext())
      {
        if (next.getNodeType() == (short) 1 && (tag == null || StringImpl.equals(tag, (object) next.getNodeName())))
          return (Element) next;
      }
      this.current = (Node) null;
      return (Element) null;
    }

    public virtual Element getNextElement(string nsURI, string localName)
    {
      for (Node next = this.getNext(); next != null; next = this.getNext())
      {
        if (next.getNodeType() == (short) 1 && (nsURI == null || StringImpl.equals(nsURI, (object) next.getNamespaceURI())) && (localName == null || StringImpl.equals(localName, (object) next.getLocalName())))
          return (Element) next;
      }
      this.current = (Node) null;
      return (Element) null;
    }

    public virtual void reset() => this.current = this.startPoint;

    public virtual Node removeCurrent()
    {
      Node oldChild = this.current != null ? this.current : throw new IllegalStateException(((NodeBase) this.startPoint).getMessage("TW-001"));
      Node parentNode = this.current.getParentNode();
      Node node1 = (Node) null;
      if (parentNode == null)
        throw new IllegalStateException(((NodeBase) this.startPoint).getMessage("TW-002"));
      for (Node node2 = this.current; node2 != null && node2 != this.startPoint; node2 = node2.getParentNode())
      {
        node1 = node2.getNextSibling();
        if (node1 != null)
        {
          this.current = node1;
          break;
        }
      }
      parentNode.removeChild(oldChild);
      return node1;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TreeWalker treeWalker = this;
      ObjectImpl.clone((object) treeWalker);
      return ((object) treeWalker).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
