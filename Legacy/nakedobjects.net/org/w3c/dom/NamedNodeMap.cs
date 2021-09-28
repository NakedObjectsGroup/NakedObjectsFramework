// Decompiled with JetBrains decompiler
// Type: org.w3c.dom.NamedNodeMap
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.w3c.dom
{
  [JavaInterface]
  public interface NamedNodeMap
  {
    Node getNamedItem(string name);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node setNamedItem(Node arg);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node removeNamedItem(string name);

    Node item(int index);

    int getLength();

    Node getNamedItemNS(string namespaceURI, string localName);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node setNamedItemNS(Node arg);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node removeNamedItemNS(string namespaceURI, string localName);
  }
}
