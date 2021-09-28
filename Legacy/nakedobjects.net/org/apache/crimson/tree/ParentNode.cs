// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.ParentNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.w3c.dom;
using System.ComponentModel;

namespace org.apache.crimson.tree
{
  [JavaFlags(1056)]
  public abstract class ParentNode : NodeBase
  {
    private NodeBase[] children;
    private int length;

    [JavaFlags(0)]
    public ParentNode()
    {
    }

    public virtual void trimToSize()
    {
      if (this.length == 0)
      {
        this.children = (NodeBase[]) null;
      }
      else
      {
        if (this.children.Length == this.length)
          return;
        int length = this.length;
        NodeBase[] nodeBaseArray = length >= 0 ? new NodeBase[length] : throw new NegativeArraySizeException();
        java.lang.System.arraycopy((object) this.children, 0, (object) nodeBaseArray, 0, this.length);
        this.children = nodeBaseArray;
      }
    }

    [JavaFlags(0)]
    public virtual void reduceWaste()
    {
      if (this.children == null || this.children.Length - this.length <= 6)
        return;
      this.trimToSize();
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeChildrenXml(XmlWriteContext context)
    {
      if (this.children == null)
        return;
      int level = 0;
      bool flag1 = true;
      bool flag2 = true;
      if (this.getNodeType() == (short) 1)
      {
        flag1 = StringImpl.equals("preserve", (object) this.getInheritedAttribute("xml:space"));
        level = context.getIndentLevel();
      }
      try
      {
        if (!flag1)
          context.setIndentLevel(level + 2);
        for (int index = 0; index < this.length; ++index)
        {
          if (!flag1 && this.children[index].getNodeType() != (short) 3)
          {
            context.printIndent();
            flag2 = false;
          }
          this.children[index].writeXml(context);
        }
      }
      finally
      {
        if (!flag1)
        {
          context.setIndentLevel(level);
          if (!flag2)
            context.printIndent();
        }
      }
    }

    [JavaFlags(1024)]
    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public abstract void checkChildType(int type);

    [JavaFlags(17)]
    public override sealed bool hasChildNodes() => this.length > 0;

    [JavaFlags(17)]
    public override sealed Node getFirstChild() => this.length == 0 ? (Node) null : (Node) this.children[0];

    [JavaFlags(17)]
    public override sealed Node getLastChild() => this.length == 0 ? (Node) null : (Node) this.children[this.length - 1];

    [JavaFlags(17)]
    public override sealed int getLength() => this.length;

    [JavaFlags(17)]
    public override sealed Node item(int i)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    private NodeBase checkDocument(Node newChild)
    {
      if (newChild == null)
        throw new DomEx((short) 3);
      Document document = newChild is NodeBase ? newChild.getOwnerDocument() : throw new DomEx((short) 4);
      XmlDocument doc = this.ownerDocument;
      NodeBase nodeBase = (NodeBase) newChild;
      if (doc == null && this is XmlDocument)
        doc = (XmlDocument) this;
      if (document != null && document != doc)
        throw new DomEx((short) 4);
      if (document == null)
        nodeBase.setOwnerDocument(doc);
      if (nodeBase.hasChildNodes())
      {
        int i = 0;
        while (true)
        {
          Node node = nodeBase.item(i);
          if (node != null)
          {
            if (node.getOwnerDocument() == null)
              ((NodeBase) node).setOwnerDocument(doc);
            else if (node.getOwnerDocument() != doc)
              break;
            ++i;
          }
          else
            goto label_18;
        }
        throw new DomEx((short) 4);
      }
label_18:
      return nodeBase;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    private void checkNotAncestor(Node newChild)
    {
      if (!newChild.hasChildNodes())
        return;
      for (Node node = (Node) this; node != null; node = node.getParentNode())
      {
        if (newChild == node)
          throw new DomEx((short) 3);
      }
    }

    private void mutated()
    {
      XmlDocument xmlDocument = this.ownerDocument;
      if (xmlDocument == null && this is XmlDocument)
        xmlDocument = (XmlDocument) this;
      if (xmlDocument == null)
        return;
      ++xmlDocument.mutationCount;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    private void consumeFragment(Node fragment, Node before)
    {
      ParentNode parentNode = (ParentNode) fragment;
      Node newChild1;
      for (int i = 0; (newChild1 = parentNode.item(i)) != null; ++i)
      {
        this.checkNotAncestor(newChild1);
        this.checkChildType((int) newChild1.getNodeType());
      }
      Node newChild2;
      while ((newChild2 = parentNode.item(0)) != null)
        this.insertBefore(newChild2, before);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override Node appendChild(Node newChild)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      NodeBase nodeBase1 = this.checkDocument(newChild);
      if (newChild.getNodeType() == (short) 11)
      {
        this.consumeFragment(newChild, (Node) null);
        return newChild;
      }
      this.checkNotAncestor(newChild);
      this.checkChildType((int) nodeBase1.getNodeType());
      if (this.children == null)
      {
        int length = 3;
        this.children = length >= 0 ? new NodeBase[length] : throw new NegativeArraySizeException();
      }
      else if (this.children.Length == this.length)
      {
        int length = this.length * 2;
        NodeBase[] nodeBaseArray = length >= 0 ? new NodeBase[length] : throw new NegativeArraySizeException();
        java.lang.System.arraycopy((object) this.children, 0, (object) nodeBaseArray, 0, this.length);
        this.children = nodeBaseArray;
      }
      nodeBase1.setParentNode(this, this.length);
      NodeBase[] children = this.children;
      int length1;
      this.length = (length1 = this.length) + 1;
      int index = length1;
      NodeBase nodeBase2 = nodeBase1;
      children[index] = nodeBase2;
      this.mutated();
      return (Node) nodeBase1;
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override Node insertBefore(Node newChild, Node refChild)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      if (refChild == null)
        return this.appendChild(newChild);
      if (this.length == 0)
        throw new DomEx((short) 8);
      NodeBase nodeBase = this.checkDocument(newChild);
      if (newChild.getNodeType() == (short) 11)
      {
        this.consumeFragment(newChild, refChild);
        return newChild;
      }
      this.checkNotAncestor(newChild);
      this.checkChildType((int) newChild.getNodeType());
      for (int index = 0; index < this.length; ++index)
      {
        if (this.children[index] == newChild)
        {
          this.removeChild(newChild);
          break;
        }
      }
      if (this.children.Length == this.length)
      {
        int length = this.length * 2;
        NodeBase[] nodeBaseArray = length >= 0 ? new NodeBase[length] : throw new NegativeArraySizeException();
        java.lang.System.arraycopy((object) this.children, 0, (object) nodeBaseArray, 0, this.length);
        this.children = nodeBaseArray;
      }
      for (int index = 0; index < this.length; ++index)
      {
        if (this.children[index] == refChild)
        {
          nodeBase.setParentNode(this, index);
          java.lang.System.arraycopy((object) this.children, index, (object) this.children, index + 1, this.length - index);
          this.children[index] = nodeBase;
          ++this.length;
          this.mutated();
          return newChild;
        }
      }
      throw new DomEx((short) 8);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override Node replaceChild(Node newChild, Node refChild)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      if (newChild == null || refChild == null)
        throw new DomEx((short) 3);
      if (this.children == null)
        throw new DomEx((short) 8);
      NodeBase nodeBase = this.checkDocument(newChild);
      if (newChild.getNodeType() == (short) 11)
      {
        this.consumeFragment(newChild, refChild);
        return this.removeChild(refChild);
      }
      this.checkNotAncestor(newChild);
      this.checkChildType((int) newChild.getNodeType());
      for (int index = 0; index < this.length; ++index)
      {
        if (this.children[index] == newChild)
        {
          this.removeChild(newChild);
          break;
        }
      }
      for (int index = 0; index < this.length; ++index)
      {
        if (this.children[index] == refChild)
        {
          nodeBase.setParentNode(this, index);
          this.children[index] = nodeBase;
          ((NodeBase) refChild).setParentNode((ParentNode) null, -1);
          this.mutated();
          return refChild;
        }
      }
      throw new DomEx((short) 8);
    }

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override Node removeChild(Node oldChild)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      NodeBase nodeBase = oldChild is NodeBase ? (NodeBase) oldChild : throw new DomEx((short) 8);
      for (int index = 0; index < this.length; ++index)
      {
        if (this.children[index] == nodeBase)
        {
          if (index + 1 != this.length)
            java.lang.System.arraycopy((object) this.children, index + 1, (object) this.children, index, this.length - 1 - index);
          this.length += -1;
          this.children[this.length] = (NodeBase) null;
          nodeBase.setParentNode((ParentNode) null, -1);
          this.mutated();
          return oldChild;
        }
      }
      throw new DomEx((short) 8);
    }

    public virtual NodeList getElementsByTagName(string tagname)
    {
      if (StringImpl.equals("*", (object) tagname))
        tagname = (string) null;
      return (NodeList) new ParentNode.TagList(this, tagname);
    }

    public virtual NodeList getElementsByTagNameNS(string namespaceURI, string localName)
    {
      if (StringImpl.equals("*", (object) namespaceURI))
        namespaceURI = (string) null;
      if (StringImpl.equals("*", (object) localName))
        localName = (string) null;
      return (NodeList) new ParentNode.TagListNS(this, namespaceURI, localName);
    }

    [JavaFlags(17)]
    public override sealed int getIndexOf(Node maybeChild)
    {
      for (int index = 0; index < this.length; ++index)
      {
        if (this.children[index] == maybeChild)
          return index;
      }
      return -1;
    }

    public override void normalize()
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      int i = 0;
      while (true)
      {
        Node node1 = this.item(i);
        if (node1 != null)
        {
          switch (node1.getNodeType())
          {
            case 1:
              node1.normalize();
              break;
            case 3:
              Node node2 = this.item(i + 1);
              if (node2 != null && node2.getNodeType() == (short) 3)
              {
                ((TextNode) node1).joinNextText();
                i += -1;
                break;
              }
              break;
          }
          ++i;
        }
        else
          break;
      }
    }

    public virtual int removeWhiteSpaces(char[] buf)
    {
      int num1 = 0;
      int index1 = 0;
label_7:
      while (index1 < buf.Length)
      {
        bool flag = false;
        char[] chArray1 = buf;
        int num2;
        index1 = (num2 = index1) + 1;
        int index2 = num2;
        char ch = chArray1[index2];
        switch (ch)
        {
          case '\t':
          case '\n':
          case '\r':
          case ' ':
            ch = ' ';
            flag = true;
            break;
        }
        char[] chArray2 = buf;
        int num3;
        num1 = (num3 = num1) + 1;
        int index3 = num3;
        int num4 = (int) ch;
        chArray2[index3] = (char) num4;
        if (flag)
        {
          for (; index1 < buf.Length; ++index1)
          {
            switch (buf[index1])
            {
              case '\t':
              case '\n':
              case '\r':
              case ' ':
                continue;
              default:
                goto label_7;
            }
          }
        }
      }
      return num1;
    }

    [Inner]
    [JavaInterfaces("1;org/w3c/dom/NodeList;")]
    [JavaFlags(32)]
    public class TagList : NodeList
    {
      [JavaFlags(4)]
      public string tag;
      [JavaFlags(4)]
      public int lastMutationCount;
      [JavaFlags(4)]
      public int lastIndex;
      [JavaFlags(4)]
      public TreeWalker lastWalker;
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ParentNode this\u00240;

      [JavaFlags(4)]
      public virtual int getLastMutationCount()
      {
        XmlDocument ownerDocument = (XmlDocument) this.this\u00240.getOwnerDocument();
        return ownerDocument == null ? 0 : ownerDocument.mutationCount;
      }

      [JavaFlags(0)]
      public TagList(ParentNode _param1, string tag)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.tag = tag;
      }

      public virtual Node item(int i)
      {
        if (i < 0)
          return (Node) null;
        int lastMutationCount = this.getLastMutationCount();
        if (this.lastWalker != null && (i < this.lastIndex || lastMutationCount != this.lastMutationCount))
          this.lastWalker = (TreeWalker) null;
        if (this.lastWalker == null)
        {
          this.lastWalker = new TreeWalker((Node) this.this\u00240);
          this.lastIndex = -1;
          this.lastMutationCount = lastMutationCount;
        }
        if (i == this.lastIndex)
          return this.lastWalker.getCurrent();
        Node node = (Node) null;
        while (i > this.lastIndex && (Element) (node = (Node) this.lastWalker.getNextElement(this.tag)) != null)
          ++this.lastIndex;
        if (node == null)
          this.lastWalker = (TreeWalker) null;
        return node;
      }

      public virtual int getLength()
      {
        TreeWalker treeWalker = new TreeWalker((Node) this.this\u00240);
        Node node = (Node) null;
        int num = 0;
        while ((Element) (node = (Node) treeWalker.getNextElement(this.tag)) != null)
          ++num;
        return num;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ParentNode.TagList tagList = this;
        ObjectImpl.clone((object) tagList);
        return ((object) tagList).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [Inner]
    public class TagListNS : ParentNode.TagList
    {
      private string namespaceURI;
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ParentNode this\u00240;

      [JavaFlags(0)]
      public TagListNS(ParentNode _param1, string namespaceURI, string localName)
        : base(_param1, localName)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.namespaceURI = namespaceURI;
      }

      public override Node item(int i)
      {
        if (i < 0)
          return (Node) null;
        int lastMutationCount = this.getLastMutationCount();
        if (this.lastWalker != null && (i < this.lastIndex || lastMutationCount != this.lastMutationCount))
          this.lastWalker = (TreeWalker) null;
        if (this.lastWalker == null)
        {
          this.lastWalker = new TreeWalker((Node) this.this\u00240);
          this.lastIndex = -1;
          this.lastMutationCount = lastMutationCount;
        }
        if (i == this.lastIndex)
          return this.lastWalker.getCurrent();
        Node node = (Node) null;
        while (i > this.lastIndex && (Element) (node = (Node) this.lastWalker.getNextElement(this.namespaceURI, this.tag)) != null)
          ++this.lastIndex;
        if (node == null)
          this.lastWalker = (TreeWalker) null;
        return node;
      }

      public override int getLength()
      {
        TreeWalker treeWalker = new TreeWalker((Node) this.this\u00240);
        int num = 0;
        while (treeWalker.getNextElement(this.namespaceURI, this.tag) != null)
          ++num;
        return num;
      }
    }
  }
}
