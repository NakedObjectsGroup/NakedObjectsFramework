// Decompiled with JetBrains decompiler
// Type: javax.xml.parsers.SAXParserFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace javax.xml.parsers
{
  public abstract class SAXParserFactory
  {
    private bool validating;
    private bool namespaceAware;

    [JavaFlags(4)]
    public SAXParserFactory()
    {
      this.validating = false;
      this.namespaceAware = false;
    }

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryConfigurationError;")]
    public static SAXParserFactory newInstance()
    {
      try
      {
        return (SAXParserFactory) FactoryFinder.find("javax.xml.parsers.SAXParserFactory", "org.apache.crimson.jaxp.SAXParserFactoryImpl");
      }
      catch (FactoryFinder.ConfigurationError ex)
      {
        throw new FactoryConfigurationError(ex.getException(), ((Throwable) ex).getMessage());
      }
    }

    [JavaThrownExceptions("2;javax/xml/parsers/ParserConfigurationException;org/xml/sax/SAXException;")]
    public abstract SAXParser newSAXParser();

    public virtual void setNamespaceAware(bool awareness) => this.namespaceAware = awareness;

    public virtual void setValidating(bool validating) => this.validating = validating;

    public virtual bool isNamespaceAware() => this.namespaceAware;

    public virtual bool isValidating() => this.validating;

    [JavaThrownExceptions("3;javax/xml/parsers/ParserConfigurationException;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public abstract void setFeature(string name, bool value);

    [JavaThrownExceptions("3;javax/xml/parsers/ParserConfigurationException;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public abstract bool getFeature(string name);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SAXParserFactory saxParserFactory = this;
      ObjectImpl.clone((object) saxParserFactory);
      return ((object) saxParserFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
