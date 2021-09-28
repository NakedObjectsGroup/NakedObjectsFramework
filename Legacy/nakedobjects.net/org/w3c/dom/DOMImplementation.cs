// Decompiled with JetBrains decompiler
// Type: org.w3c.dom.DOMImplementation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.w3c.dom
{
  [JavaInterface]
  public interface DOMImplementation
  {
    bool hasFeature(string feature, string version);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    DocumentType createDocumentType(
      string qualifiedName,
      string publicId,
      string systemId);

    [JavaThrownExceptions("1;org/w3c/dom/DOMException;")]
    Document createDocument(
      string namespaceURI,
      string qualifiedName,
      DocumentType doctype);
  }
}
