// Decompiled with JetBrains decompiler
// Type: org.xml.sax.Parser
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using System;

namespace org.xml.sax
{
  [Obsolete(null, false)]
  [JavaInterface]
  public interface Parser
  {
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    void setLocale(Locale locale);

    void setEntityResolver(EntityResolver resolver);

    void setDTDHandler(DTDHandler handler);

    void setDocumentHandler(DocumentHandler handler);

    void setErrorHandler(ErrorHandler handler);

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    void parse(InputSource source);

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    void parse(string systemId);
  }
}
