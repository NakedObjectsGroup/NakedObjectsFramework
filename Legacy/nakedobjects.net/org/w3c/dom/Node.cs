// Decompiled with JetBrains decompiler
// Type: org.w3c.dom.Node
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.w3c.dom
{
  [JavaInterface]
  public interface Node
  {
    const short ELEMENT_NODE = 1;
    const short ATTRIBUTE_NODE = 2;
    const short TEXT_NODE = 3;
    const short CDATA_SECTION_NODE = 4;
    const short ENTITY_REFERENCE_NODE = 5;
    const short ENTITY_NODE = 6;
    const short PROCESSING_INSTRUCTION_NODE = 7;
    const short COMMENT_NODE = 8;
    const short DOCUMENT_NODE = 9;
    const short DOCUMENT_TYPE_NODE = 10;
    const short DOCUMENT_FRAGMENT_NODE = 11;
    const short NOTATION_NODE = 12;

    string getNodeName();

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    string getNodeValue();

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void setNodeValue(string nodeValue);

    short getNodeType();

    Node getParentNode();

    NodeList getChildNodes();

    Node getFirstChild();

    Node getLastChild();

    Node getPreviousSibling();

    Node getNextSibling();

    NamedNodeMap getAttributes();

    Document getOwnerDocument();

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node insertBefore(Node newChild, Node refChild);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node replaceChild(Node newChild, Node oldChild);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node removeChild(Node oldChild);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Node appendChild(Node newChild);

    bool hasChildNodes();

    Node cloneNode(bool deep);

    void normalize();

    bool isSupported(string feature, string version);

    string getNamespaceURI();

    string getPrefix();

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    void setPrefix(string prefix);

    string getLocalName();

    bool hasAttributes();
  }
}
