// Decompiled with JetBrains decompiler
// Type: org.w3c.dom.Element
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.w3c.dom
{
  [JavaInterface]
  [JavaInterfaces("1;org/w3c/dom/Node;")]
  public interface Element : Node
  {
    string getTagName();

    string getAttribute(string name);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void setAttribute(string name, string value);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void removeAttribute(string name);

    Attr getAttributeNode(string name);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Attr setAttributeNode(Attr newAttr);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Attr removeAttributeNode(Attr oldAttr);

    NodeList getElementsByTagName(string name);

    string getAttributeNS(string namespaceURI, string localName);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void setAttributeNS(string namespaceURI, string qualifiedName, string value);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void removeAttributeNS(string namespaceURI, string localName);

    Attr getAttributeNodeNS(string namespaceURI, string localName);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Attr setAttributeNodeNS(Attr newAttr);

    NodeList getElementsByTagNameNS(string namespaceURI, string localName);

    bool hasAttribute(string name);

    bool hasAttributeNS(string namespaceURI, string localName);
  }
}
