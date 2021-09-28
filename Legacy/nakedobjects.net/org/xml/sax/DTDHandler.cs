// Decompiled with JetBrains decompiler
// Type: org.xml.sax.DTDHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.xml.sax
{
  [JavaInterface]
  public interface DTDHandler
  {
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void notationDecl(string name, string publicId, string systemId);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void unparsedEntityDecl(string name, string publicId, string systemId, string notationName);
  }
}
