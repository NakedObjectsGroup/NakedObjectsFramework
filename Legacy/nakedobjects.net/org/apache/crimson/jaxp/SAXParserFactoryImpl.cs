// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.jaxp.SAXParserFactoryImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using javax.xml.parsers;
using org.xml.sax;

namespace org.apache.crimson.jaxp
{
  public class SAXParserFactoryImpl : SAXParserFactory
  {
    private Hashtable features;

    [JavaThrownExceptions("1;javax/xml/parsers/ParserConfigurationException;")]
    public override SAXParser newSAXParser()
    {
      try
      {
        return (SAXParser) new SAXParserImpl((SAXParserFactory) this, this.features);
      }
      catch (SAXException ex)
      {
        throw new ParserConfigurationException(ex.getMessage());
      }
    }

    [JavaThrownExceptions("3;javax/xml/parsers/ParserConfigurationException;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    private SAXParserImpl newSAXParserImpl()
    {
      try
      {
        return new SAXParserImpl((SAXParserFactory) this, this.features);
      }
      catch (SAXNotSupportedException ex)
      {
        SAXNotSupportedException supportedException = ex;
        if (supportedException != ex)
          throw supportedException;
        throw;
      }
      catch (SAXNotRecognizedException ex)
      {
        SAXNotRecognizedException recognizedException = ex;
        if (recognizedException != ex)
          throw recognizedException;
        throw;
      }
      catch (SAXException ex)
      {
        throw new ParserConfigurationException(ex.getMessage());
      }
    }

    [JavaThrownExceptions("3;javax/xml/parsers/ParserConfigurationException;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public override void setFeature(string name, bool value)
    {
      if (this.features == null)
        this.features = new Hashtable();
      this.features.put((object) name, (object) new Boolean(value));
      try
      {
        this.newSAXParserImpl();
      }
      catch (SAXNotSupportedException ex)
      {
        SAXNotSupportedException supportedException1 = ex;
        this.features.remove((object) name);
        SAXNotSupportedException supportedException2 = supportedException1;
        if (supportedException2 != ex)
          throw supportedException2;
        throw;
      }
      catch (SAXNotRecognizedException ex)
      {
        SAXNotRecognizedException recognizedException1 = ex;
        this.features.remove((object) name);
        SAXNotRecognizedException recognizedException2 = recognizedException1;
        if (recognizedException2 != ex)
          throw recognizedException2;
        throw;
      }
    }

    [JavaThrownExceptions("3;javax/xml/parsers/ParserConfigurationException;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public override bool getFeature(string name) => this.newSAXParserImpl().getXMLReader().getFeature(name);
  }
}
