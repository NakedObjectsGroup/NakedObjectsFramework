// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.XMLReaderImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.xml.sax;
using org.xml.sax.ext;
using System.ComponentModel;

namespace org.apache.crimson.parser
{
  [JavaInterfaces("1;org/xml/sax/XMLReader;")]
  public class XMLReaderImpl : XMLReader
  {
    private const string FEATURES = "http://xml.org/sax/features/";
    private static readonly string NAMESPACES;
    private static readonly string NAMESPACE_PREFIXES;
    private static readonly string STRING_INTERNING;
    private static readonly string VALIDATION;
    private static readonly string EXTERNAL_GENERAL;
    private static readonly string EXTERNAL_PARAMETER;
    private static readonly string LEXICAL_PARAMETER_ENTITIES;
    private const string PROPERTIES = "http://xml.org/sax/properties/";
    private static readonly string LEXICAL_HANDLER;
    private static readonly string DECLARATION_HANDLER;
    private bool namespaces;
    private bool prefixes;
    private bool validation;
    private LexicalHandler lexicalHandler;
    private DeclHandler declHandler;
    private ContentHandler contentHandler;
    private DTDHandler dtdHandler;
    private ErrorHandler errorHandler;
    private EntityResolver entityResolver;
    private Parser2 parser;
    private bool parsing;

    public XMLReaderImpl()
    {
      this.namespaces = true;
      this.prefixes = false;
      this.validation = false;
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual bool getFeature(string name)
    {
      if (StringImpl.equals(name, (object) XMLReaderImpl.NAMESPACES))
        return this.namespaces;
      if (StringImpl.equals(name, (object) XMLReaderImpl.NAMESPACE_PREFIXES))
        return this.prefixes;
      if (StringImpl.equals(name, (object) XMLReaderImpl.VALIDATION))
        return this.validation;
      if (StringImpl.equals(name, (object) XMLReaderImpl.STRING_INTERNING) || StringImpl.equals(name, (object) XMLReaderImpl.EXTERNAL_GENERAL) || StringImpl.equals(name, (object) XMLReaderImpl.EXTERNAL_PARAMETER))
        return true;
      if (StringImpl.equals(name, (object) XMLReaderImpl.LEXICAL_PARAMETER_ENTITIES))
        return false;
      throw new SAXNotRecognizedException(new StringBuffer().append("Feature: ").append(name).ToString());
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual void setFeature(string name, bool state)
    {
      if (StringImpl.equals(name, (object) XMLReaderImpl.NAMESPACES))
      {
        this.checkNotParsing("feature", name);
        this.namespaces = state;
        if (this.namespaces || this.prefixes)
          return;
        this.prefixes = true;
      }
      else if (StringImpl.equals(name, (object) XMLReaderImpl.NAMESPACE_PREFIXES))
      {
        this.checkNotParsing("feature", name);
        this.prefixes = state;
        if (this.prefixes || this.namespaces)
          return;
        this.namespaces = true;
      }
      else if (StringImpl.equals(name, (object) XMLReaderImpl.VALIDATION))
      {
        this.checkNotParsing("feature", name);
        if (this.validation != state)
          this.parser = (Parser2) null;
        this.validation = state;
      }
      else if (StringImpl.equals(name, (object) XMLReaderImpl.STRING_INTERNING))
      {
        if (!state)
          throw new SAXNotSupportedException(new StringBuffer().append("Feature: ").append(name).append(" State: false").ToString());
      }
      else
      {
        if (StringImpl.equals(name, (object) XMLReaderImpl.EXTERNAL_GENERAL) || StringImpl.equals(name, (object) XMLReaderImpl.EXTERNAL_PARAMETER) || StringImpl.equals(name, (object) XMLReaderImpl.LEXICAL_PARAMETER_ENTITIES))
          throw new SAXNotSupportedException(new StringBuffer().append("Feature: ").append(name).ToString());
        throw new SAXNotRecognizedException(new StringBuffer().append("Feature: ").append(name).ToString());
      }
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual object getProperty(string name)
    {
      if (StringImpl.equals(name, (object) XMLReaderImpl.LEXICAL_HANDLER))
        return (object) this.lexicalHandler;
      if (StringImpl.equals(name, (object) XMLReaderImpl.DECLARATION_HANDLER))
        return (object) this.declHandler;
      throw new SAXNotRecognizedException(new StringBuffer().append("Property: ").append(name).ToString());
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual void setProperty(string name, object value)
    {
      string message = new StringBuffer().append("Property: ").append(name).ToString();
      if (StringImpl.equals(name, (object) XMLReaderImpl.LEXICAL_HANDLER))
      {
        this.lexicalHandler = value is LexicalHandler ? (LexicalHandler) value : throw new SAXNotSupportedException(message);
      }
      else
      {
        if (!StringImpl.equals(name, (object) XMLReaderImpl.DECLARATION_HANDLER))
          throw new SAXNotRecognizedException(new StringBuffer().append("Property: ").append(name).ToString());
        this.declHandler = value is DeclHandler ? (DeclHandler) value : throw new SAXNotSupportedException(message);
      }
    }

    public virtual void setEntityResolver(EntityResolver resolver)
    {
      this.entityResolver = resolver != null ? resolver : throw new NullPointerException("Null entity resolver");
      if (this.parser == null)
        return;
      this.parser.setEntityResolver(resolver);
    }

    public virtual EntityResolver getEntityResolver() => this.entityResolver;

    public virtual void setDTDHandler(DTDHandler handler)
    {
      this.dtdHandler = handler != null ? handler : throw new NullPointerException("Null DTD handler");
      if (this.parser == null)
        return;
      this.parser.setDTDHandler(this.dtdHandler);
    }

    public virtual DTDHandler getDTDHandler() => this.dtdHandler;

    public virtual void setContentHandler(ContentHandler handler)
    {
      this.contentHandler = handler != null ? handler : throw new NullPointerException("Null content handler");
      if (this.parser == null)
        return;
      this.parser.setContentHandler(handler);
    }

    public virtual ContentHandler getContentHandler() => this.contentHandler;

    public virtual void setErrorHandler(ErrorHandler handler)
    {
      this.errorHandler = handler != null ? handler : throw new NullPointerException("Null error handler");
      if (this.parser == null)
        return;
      this.parser.setErrorHandler(this.errorHandler);
    }

    public virtual ErrorHandler getErrorHandler() => this.errorHandler;

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual void parse(string systemId) => this.parse(new InputSource(systemId));

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual void parse(InputSource input)
    {
      this.parsing = !this.parsing ? true : throw new SAXException("Parser is already in use");
      if (this.parser == null)
        this.parser = !this.validation ? new Parser2() : (Parser2) new ValidatingParser();
      this.parser.setNamespaceFeatures(this.namespaces, this.prefixes);
      this.parser.setContentHandler(this.contentHandler);
      this.parser.setDTDHandler(this.dtdHandler);
      this.parser.setErrorHandler(this.errorHandler);
      this.parser.setEntityResolver(this.entityResolver);
      this.parser.setLexicalHandler(this.lexicalHandler);
      this.parser.setDeclHandler(this.declHandler);
      try
      {
        this.parser.parse(input);
      }
      finally
      {
        this.parsing = false;
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXNotSupportedException;")]
    private void checkNotParsing(string type, string name)
    {
      if (this.parsing)
        throw new SAXNotSupportedException(new StringBuffer().append("Cannot change ").append(type).append(' ').append(name).append(" while parsing").ToString());
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static XMLReaderImpl()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      XMLReaderImpl xmlReaderImpl = this;
      ObjectImpl.clone((object) xmlReaderImpl);
      return ((object) xmlReaderImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
