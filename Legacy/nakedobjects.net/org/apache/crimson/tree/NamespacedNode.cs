// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.tree.NamespacedNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.crimson.util;

namespace org.apache.crimson.tree
{
  public abstract class NamespacedNode : ParentNode
  {
    [JavaFlags(4)]
    public string qName;
    [JavaFlags(4)]
    public string namespaceURI;

    [JavaFlags(0)]
    public NamespacedNode(string namespaceURI, string qName)
    {
      this.namespaceURI = namespaceURI;
      this.qName = qName;
    }

    public override string getNamespaceURI() => this.namespaceURI;

    public override string getPrefix() => XmlNames.getPrefix(this.qName);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    public override void setPrefix(string prefix)
    {
      if (this.@readonly)
        throw new DomEx((short) 7);
      int num = StringImpl.indexOf(this.qName, 58);
      if (prefix == null)
      {
        if (num < 0)
          return;
        this.qName = StringImpl.substring(this.qName, num + 1);
      }
      else
      {
        if (!XmlNames.isUnqualifiedName(prefix))
          throw new DomEx((short) 5);
        if (this.namespaceURI == null || StringImpl.equals("xml", (object) prefix) && !StringImpl.equals("http://www.w3.org/XML/1998/namespace", (object) this.namespaceURI))
          throw new DomEx((short) 14);
        if (this.getNodeType() == (short) 2 && (StringImpl.equals("xmlns", (object) prefix) && !StringImpl.equals("http://www.w3.org/2000/xmlns/", (object) this.namespaceURI) || StringImpl.equals("xmlns", (object) this.qName)))
          throw new DomEx((short) 14);
        StringBuffer stringBuffer = new StringBuffer(prefix);
        stringBuffer.append(':');
        if (num < 0)
          stringBuffer.append(this.qName);
        else
          stringBuffer.append(StringImpl.substring(this.qName, num + 1));
        this.qName = stringBuffer.ToString();
      }
    }

    public override string getLocalName() => XmlNames.getLocalPart(this.qName);

    public override string getNodeName() => this.qName;
  }
}
