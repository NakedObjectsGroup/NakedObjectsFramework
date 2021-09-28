// Decompiled with JetBrains decompiler
// Type: org.w3c.dom.Document
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.w3c.dom
{
  [JavaInterface]
  [JavaInterfaces("1;org/w3c/dom/Node;")]
  public interface Document : Node
  {
    DocumentType getDoctype();

    DOMImplementation getImplementation();

    Element getDocumentElement();

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Element createElement(string tagName);

    DocumentFragment createDocumentFragment();

    Text createTextNode(string data);

    Comment createComment(string data);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    CDATASection createCDATASection(string data);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    ProcessingInstruction createProcessingInstruction(
      string target,
      string data);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Attr createAttribute(string name);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    EntityReference createEntityReference(string name);

    NodeList getElementsByTagName(string tagname);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node importNode(Node importedNode, bool deep);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Element createElementNS(string namespaceURI, string qualifiedName);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Attr createAttributeNS(string namespaceURI, string qualifiedName);

    NodeList getElementsByTagNameNS(string namespaceURI, string localName);

    Element getElementById(string elementId);
  }
}
