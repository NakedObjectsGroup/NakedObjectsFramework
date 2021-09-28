// Decompiled with JetBrains decompiler
// Type: org.xml.sax.ext.DeclHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.xml.sax.ext
{
  [JavaInterface]
  public interface DeclHandler
  {
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void elementDecl(string name, string model);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void attributeDecl(
      string eName,
      string aName,
      string type,
      string valueDefault,
      string value);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void internalEntityDecl(string name, string value);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void externalEntityDecl(string name, string publicId, string systemId);
  }
}
