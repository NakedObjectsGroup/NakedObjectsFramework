// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.AttributeNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.apache.crimson.util;
using org.w3c.dom;

namespace org.apache.crimson.tree
{
  [JavaInterfaces("1;org/w3c/dom/Attr;")]
  public class AttributeNode : NamespacedNode, Attr
  {
    private string value;
    private bool specified;
    private string defaultValue;
    private Element ownerElement;

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public AttributeNode(
      string namespaceURI,
      string qName,
      string value,
      bool specified,
      string defaultValue)
      : base(namespaceURI, qName)
    {
      this.value = value;
      this.specified = specified;
      this.defaultValue = defaultValue;
    }

    [JavaFlags(0)]
    public virtual AttributeNode makeClone()
    {
      AttributeNode attributeNode = new AttributeNode(this.namespaceURI, this.qName, this.value, this.specified, this.defaultValue);
      attributeNode.ownerDocument = this.ownerDocument;
      return attributeNode;
    }

    [JavaFlags(8)]
    [JavaThrownExceptions("1;org/apache/crimson/tree/DomEx;")]
    public static void checkArguments(string namespaceURI, string qualifiedName)
    {
      int num = qualifiedName != null ? StringImpl.indexOf(qualifiedName, 58) : throw new DomEx((short) 14);
      if (num <= 0)
      {
        if (!XmlNames.isUnqualifiedName(qualifiedName))
          throw new DomEx((short) 5);
        if (StringImpl.equals("xmlns", (object) qualifiedName) && !StringImpl.equals("http://www.w3.org/2000/xmlns/", (object) namespaceURI))
          throw new DomEx((short) 14);
      }
      else
      {
        string str1 = StringImpl.lastIndexOf(qualifiedName, 58) == num ? StringImpl.substring(qualifiedName, 0, num) : throw new DomEx((short) 14);
        string str2 = StringImpl.substring(qualifiedName, num + 1);
        if (!XmlNames.isUnqualifiedName(str1) || !XmlNames.isUnqualifiedName(str2))
          throw new DomEx((short) 5);
        if (namespaceURI == null || StringImpl.equals("xml", (object) str1) && !StringImpl.equals("http://www.w3.org/XML/1998/namespace", (object) namespaceURI))
          throw new DomEx((short) 14);
      }
    }

    [JavaFlags(0)]
    public virtual string getDefaultValue() => this.defaultValue;

    public virtual Element getOwnerElement() => this.ownerElement;

    [JavaFlags(0)]
    public virtual void setOwnerElement(Element element)
    {
      if (element != null && this.ownerElement != null)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) element.getTagName();
        throw new IllegalStateException(this.getMessage("A-000", parameters));
      }
      this.ownerElement = element;
    }

    public override short getNodeType() => 2;

    public virtual string getName() => this.qName;

    public virtual string getValue() => this.value;

    public virtual void setValue(string value) => this.setNodeValue(value);

    public override string getNodeValue() => this.value;

    public virtual bool getSpecified() => this.specified;

    public override void setNodeValue(string value)
    {
      if (this.isReadonly())
        throw new DomEx((short) 7);
      this.value = value;
      this.specified = true;
    }

    [JavaFlags(0)]
    public virtual void setSpecified(bool specified) => this.specified = specified;

    public override Node getParentNode() => (Node) null;

    public override Node getNextSibling() => (Node) null;

    public override Node getPreviousSibling() => (Node) null;

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      writer.write(this.qName);
      writer.write("=\"");
      this.writeChildrenXml(context);
      writer.write(34);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public override void writeChildrenXml(XmlWriteContext context)
    {
      Writer writer = context.getWriter();
      for (int index = 0; index < StringImpl.length(this.value); ++index)
      {
        int num = (int) StringImpl.charAt(this.value, index);
        switch (num)
        {
          case 34:
            writer.write("&quot;");
            break;
          case 38:
            writer.write("&amp;");
            break;
          case 39:
            writer.write("&apos;");
            break;
          case 60:
            writer.write("&lt;");
            break;
          case 62:
            writer.write("&gt;");
            break;
          default:
            writer.write(num);
            break;
        }
      }
    }

    public override Node cloneNode(bool deep)
    {
      AttributeNode attributeNode = this.cloneAttributeNode(deep);
      attributeNode.specified = true;
      return (Node) attributeNode;
    }

    [JavaFlags(0)]
    public virtual AttributeNode cloneAttributeNode(bool deep)
    {
      try
      {
        AttributeNode attributeNode = this.makeClone();
        if (deep)
        {
          Node node;
          for (int i = 0; (node = this.item(i)) != null; ++i)
          {
            Node newChild = node.cloneNode(true);
            attributeNode.appendChild(newChild);
          }
        }
        return attributeNode;
      }
      catch (DOMException ex)
      {
        throw new RuntimeException(this.getMessage("A-002"));
      }
    }

    [JavaFlags(0)]
    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override void checkChildType(int type)
    {
      switch (type)
      {
        case 3:
          break;
        case 5:
          break;
        default:
          throw new DomEx((short) 3);
      }
    }
  }
}
