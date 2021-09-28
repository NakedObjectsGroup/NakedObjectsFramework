// Decompiled with JetBrains decompiler
// Type: org.xml.sax.ext.LexicalHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.xml.sax.ext
{
  [JavaInterface]
  public interface LexicalHandler
  {
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void startDTD(string name, string publicId, string systemId);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void endDTD();

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void startEntity(string name);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void endEntity(string name);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void startCDATA();

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void endCDATA();

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void comment(char[] ch, int start, int length);
  }
}
