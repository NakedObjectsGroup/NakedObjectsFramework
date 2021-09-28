// Decompiled with JetBrains decompiler
// Type: org.xml.sax.XMLReader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.xml.sax
{
  [JavaInterface]
  public interface XMLReader
  {
    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    bool getFeature(string name);

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    void setFeature(string name, bool value);

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    object getProperty(string name);

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    void setProperty(string name, object value);

    void setEntityResolver(EntityResolver resolver);

    EntityResolver getEntityResolver();

    void setDTDHandler(DTDHandler handler);

    DTDHandler getDTDHandler();

    void setContentHandler(ContentHandler handler);

    ContentHandler getContentHandler();

    void setErrorHandler(ErrorHandler handler);

    ErrorHandler getErrorHandler();

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    void parse(InputSource input);

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    void parse(string systemId);
  }
}
