// Decompiled with JetBrains decompiler
// Type: javax.xml.parsers.DocumentBuilderFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace javax.xml.parsers
{
  public abstract class DocumentBuilderFactory
  {
    private bool validating;
    private bool namespaceAware;
    private bool whitespace;
    private bool expandEntityRef;
    private bool ignoreComments;
    private bool coalescing;

    [JavaFlags(4)]
    public DocumentBuilderFactory()
    {
      this.validating = false;
      this.namespaceAware = false;
      this.whitespace = false;
      this.expandEntityRef = true;
      this.ignoreComments = false;
      this.coalescing = false;
    }

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryConfigurationError;")]
    public static DocumentBuilderFactory newInstance()
    {
      try
      {
        return (DocumentBuilderFactory) FactoryFinder.find("javax.xml.parsers.DocumentBuilderFactory", "org.apache.crimson.jaxp.DocumentBuilderFactoryImpl");
      }
      catch (FactoryFinder.ConfigurationError ex)
      {
        throw new FactoryConfigurationError(ex.getException(), ((Throwable) ex).getMessage());
      }
    }

    [JavaThrownExceptions("1;javax/xml/parsers/ParserConfigurationException;")]
    public abstract DocumentBuilder newDocumentBuilder();

    public virtual void setNamespaceAware(bool awareness) => this.namespaceAware = awareness;

    public virtual void setValidating(bool validating) => this.validating = validating;

    public virtual void setIgnoringElementContentWhitespace(bool whitespace) => this.whitespace = whitespace;

    public virtual void setExpandEntityReferences(bool expandEntityRef) => this.expandEntityRef = expandEntityRef;

    public virtual void setIgnoringComments(bool ignoreComments) => this.ignoreComments = ignoreComments;

    public virtual void setCoalescing(bool coalescing) => this.coalescing = coalescing;

    public virtual bool isNamespaceAware() => this.namespaceAware;

    public virtual bool isValidating() => this.validating;

    public virtual bool isIgnoringElementContentWhitespace() => this.whitespace;

    public virtual bool isExpandEntityReferences() => this.expandEntityRef;

    public virtual bool isIgnoringComments() => this.ignoreComments;

    public virtual bool isCoalescing() => this.coalescing;

    [JavaThrownExceptions("1;java/lang/IllegalArgumentException;")]
    public abstract void setAttribute(string name, object value);

    [JavaThrownExceptions("1;java/lang/IllegalArgumentException;")]
    public abstract object getAttribute(string name);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DocumentBuilderFactory documentBuilderFactory = this;
      ObjectImpl.clone((object) documentBuilderFactory);
      return ((object) documentBuilderFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
