// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.ParserAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using System.ComponentModel;

namespace org.xml.sax.helpers
{
  [JavaInterfaces("2;org/xml/sax/XMLReader;org/xml/sax/DocumentHandler;")]
  public class ParserAdapter : XMLReader, DocumentHandler
  {
    private const string FEATURES = "http://xml.org/sax/features/";
    private static readonly string NAMESPACES;
    private static readonly string NAMESPACE_PREFIXES;
    private static readonly string VALIDATION;
    private static readonly string EXTERNAL_GENERAL;
    private static readonly string EXTERNAL_PARAMETER;
    private NamespaceSupport nsSupport;
    private ParserAdapter.AttributeListAdapter attAdapter;
    private bool parsing;
    private string[] nameParts;
    private Parser parser;
    private AttributesImpl atts;
    private bool namespaces;
    private bool prefixes;
    [JavaFlags(0)]
    public Locator locator;
    [JavaFlags(0)]
    public EntityResolver entityResolver;
    [JavaFlags(0)]
    public DTDHandler dtdHandler;
    [JavaFlags(0)]
    public ContentHandler contentHandler;
    [JavaFlags(0)]
    public ErrorHandler errorHandler;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public ParserAdapter()
    {
      // ISSUE: unable to decompile the method.
    }

    public ParserAdapter(Parser parser)
    {
      this.parsing = false;
      int length = 3;
      this.nameParts = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      this.parser = (Parser) null;
      this.atts = (AttributesImpl) null;
      this.namespaces = true;
      this.prefixes = false;
      this.entityResolver = (EntityResolver) null;
      this.dtdHandler = (DTDHandler) null;
      this.contentHandler = (ContentHandler) null;
      this.errorHandler = (ErrorHandler) null;
      this.setup(parser);
    }

    private void setup(Parser parser)
    {
      this.parser = parser != null ? parser : throw new NullPointerException("Parser argument must not be null");
      this.atts = new AttributesImpl();
      this.nsSupport = new NamespaceSupport();
      this.attAdapter = new ParserAdapter.AttributeListAdapter(this);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual void setFeature(string name, bool state)
    {
      if (StringImpl.equals(name, (object) ParserAdapter.NAMESPACES))
      {
        this.checkNotParsing("feature", name);
        this.namespaces = state;
        if (this.namespaces || this.prefixes)
          return;
        this.prefixes = true;
      }
      else if (StringImpl.equals(name, (object) ParserAdapter.NAMESPACE_PREFIXES))
      {
        this.checkNotParsing("feature", name);
        this.prefixes = state;
        if (this.prefixes || this.namespaces)
          return;
        this.namespaces = true;
      }
      else
      {
        if (StringImpl.equals(name, (object) ParserAdapter.VALIDATION) || StringImpl.equals(name, (object) ParserAdapter.EXTERNAL_GENERAL) || StringImpl.equals(name, (object) ParserAdapter.EXTERNAL_PARAMETER))
          throw new SAXNotSupportedException(new StringBuffer().append("Feature: ").append(name).ToString());
        throw new SAXNotRecognizedException(new StringBuffer().append("Feature: ").append(name).ToString());
      }
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual bool getFeature(string name)
    {
      if (StringImpl.equals(name, (object) ParserAdapter.NAMESPACES))
        return this.namespaces;
      if (StringImpl.equals(name, (object) ParserAdapter.NAMESPACE_PREFIXES))
        return this.prefixes;
      if (StringImpl.equals(name, (object) ParserAdapter.VALIDATION) || StringImpl.equals(name, (object) ParserAdapter.EXTERNAL_GENERAL) || StringImpl.equals(name, (object) ParserAdapter.EXTERNAL_PARAMETER))
        throw new SAXNotSupportedException(new StringBuffer().append("Feature: ").append(name).ToString());
      throw new SAXNotRecognizedException(new StringBuffer().append("Feature: ").append(name).ToString());
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual void setProperty(string name, object value) => throw new SAXNotRecognizedException(new StringBuffer().append("Property: ").append(name).ToString());

    [JavaThrownExceptions("2;org/xml/sax/SAXNotRecognizedException;org/xml/sax/SAXNotSupportedException;")]
    public virtual object getProperty(string name) => throw new SAXNotRecognizedException(new StringBuffer().append("Property: ").append(name).ToString());

    public virtual void setEntityResolver(EntityResolver resolver) => this.entityResolver = resolver != null ? resolver : throw new NullPointerException("Null entity resolver");

    public virtual EntityResolver getEntityResolver() => this.entityResolver;

    public virtual void setDTDHandler(DTDHandler handler) => this.dtdHandler = handler != null ? handler : throw new NullPointerException("Null DTD handler");

    public virtual DTDHandler getDTDHandler() => this.dtdHandler;

    public virtual void setContentHandler(ContentHandler handler) => this.contentHandler = handler != null ? handler : throw new NullPointerException("Null content handler");

    public virtual ContentHandler getContentHandler() => this.contentHandler;

    public virtual void setErrorHandler(ErrorHandler handler) => this.errorHandler = handler != null ? handler : throw new NullPointerException("Null error handler");

    public virtual ErrorHandler getErrorHandler() => this.errorHandler;

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual void parse(string systemId) => this.parse(new InputSource(systemId));

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual void parse(InputSource input)
    {
      if (this.parsing)
        throw new SAXException("Parser is already in use");
      this.setupParser();
      this.parsing = true;
      try
      {
        this.parser.parse(input);
      }
      finally
      {
        this.parsing = false;
      }
      this.parsing = false;
    }

    public virtual void setDocumentLocator(Locator locator)
    {
      this.locator = locator;
      if (this.contentHandler == null)
        return;
      this.contentHandler.setDocumentLocator(locator);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startDocument()
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.startDocument();
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endDocument()
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.endDocument();
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void startElement(string qName, AttributeList qAtts)
    {
      Vector vector = (Vector) null;
      if (!this.namespaces)
      {
        if (this.contentHandler == null)
          return;
        this.attAdapter.setAttributeList(qAtts);
        this.contentHandler.startElement("", "", StringImpl.intern(qName), (Attributes) this.attAdapter);
      }
      else
      {
        this.nsSupport.pushContext();
        bool flag = false;
        this.atts.clear();
        int length1 = qAtts.getLength();
        for (int i = 0; i < length1; ++i)
        {
          string name = qAtts.getName(i);
          string type = qAtts.getType(i);
          string uri = qAtts.getValue(i);
          if (StringImpl.startsWith(name, "xmlns"))
          {
            int num = StringImpl.indexOf(name, 58);
            string prefix = num != -1 ? StringImpl.substring(name, num + 1) : "";
            if (!this.nsSupport.declarePrefix(prefix, uri))
              this.reportError(new StringBuffer().append("Illegal Namespace prefix: ").append(prefix).ToString());
            if (this.contentHandler != null)
              this.contentHandler.startPrefixMapping(prefix, uri);
            if (this.prefixes)
              this.atts.addAttribute("", "", StringImpl.intern(name), type, uri);
            flag = true;
          }
          else
          {
            try
            {
              string[] strArray = this.processName(name, true, true);
              this.atts.addAttribute(strArray[0], strArray[1], strArray[2], type, uri);
            }
            catch (SAXException ex)
            {
              if (vector == null)
                vector = new Vector();
              vector.addElement((object) ex);
              this.atts.addAttribute("", name, name, type, uri);
            }
          }
        }
        if (flag)
        {
          int length2 = this.atts.getLength();
          for (int index = 0; index < length2; ++index)
          {
            string qname = this.atts.getQName(index);
            if (!StringImpl.startsWith(qname, "xmlns"))
            {
              string[] strArray = this.processName(qname, true, false);
              this.atts.setURI(index, strArray[0]);
              this.atts.setLocalName(index, strArray[1]);
            }
          }
        }
        else if (vector != null && this.errorHandler != null)
        {
          for (int index = 0; index < vector.size(); ++index)
            this.errorHandler.error((SAXParseException) vector.elementAt(index));
        }
        if (this.contentHandler == null)
          return;
        string[] strArray1 = this.processName(qName, false, false);
        this.contentHandler.startElement(strArray1[0], strArray1[1], strArray1[2], (Attributes) this.atts);
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void endElement(string qName)
    {
      if (!this.namespaces)
      {
        if (this.contentHandler == null)
          return;
        this.contentHandler.endElement("", "", StringImpl.intern(qName));
      }
      else
      {
        string[] strArray = this.processName(qName, false, false);
        if (this.contentHandler != null)
        {
          this.contentHandler.endElement(strArray[0], strArray[1], strArray[2]);
          Enumeration declaredPrefixes = this.nsSupport.getDeclaredPrefixes();
          while (declaredPrefixes.hasMoreElements())
            this.contentHandler.endPrefixMapping(\u003CVerifierFix\u003E.genCastToString(declaredPrefixes.nextElement()));
        }
        this.nsSupport.popContext();
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void characters(char[] ch, int start, int length)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.characters(ch, start, length);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void ignorableWhitespace(char[] ch, int start, int length)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.ignorableWhitespace(ch, start, length);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void processingInstruction(string target, string data)
    {
      if (this.contentHandler == null)
        return;
      this.contentHandler.processingInstruction(target, data);
    }

    private void setupParser()
    {
      this.nsSupport.reset();
      if (this.entityResolver != null)
        this.parser.setEntityResolver(this.entityResolver);
      if (this.dtdHandler != null)
        this.parser.setDTDHandler(this.dtdHandler);
      if (this.errorHandler != null)
        this.parser.setErrorHandler(this.errorHandler);
      this.parser.setDocumentHandler((DocumentHandler) this);
      this.locator = (Locator) null;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private string[] processName(string qName, bool isAttribute, bool useException)
    {
      string[] strArray = this.nsSupport.processName(qName, this.nameParts, isAttribute);
      if (strArray == null)
      {
        int length = 3;
        strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
        strArray[2] = StringImpl.intern(qName);
        if (useException)
          throw this.makeException(new StringBuffer().append("Undeclared prefix: ").append(qName).ToString());
        this.reportError(new StringBuffer().append("Undeclared prefix: ").append(qName).ToString());
      }
      return strArray;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    [JavaFlags(0)]
    public virtual void reportError(string message)
    {
      if (this.errorHandler == null)
        return;
      this.errorHandler.error(this.makeException(message));
    }

    private SAXParseException makeException(string message) => this.locator != null ? new SAXParseException(message, this.locator) : new SAXParseException(message, (string) null, (string) null, -1, -1);

    [JavaThrownExceptions("1;org/xml/sax/SAXNotSupportedException;")]
    private void checkNotParsing(string type, string name)
    {
      if (this.parsing)
        throw new SAXNotSupportedException(new StringBuffer().append("Cannot change ").append(type).append(' ').append(name).append(" while parsing").ToString());
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ParserAdapter()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ParserAdapter parserAdapter = this;
      ObjectImpl.clone((object) parserAdapter);
      return ((object) parserAdapter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [Inner]
    [JavaFlags(48)]
    [JavaInterfaces("1;org/xml/sax/Attributes;")]
    public sealed class AttributeListAdapter : Attributes
    {
      private AttributeList qAtts;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ParserAdapter this\u00240;

      [JavaFlags(0)]
      public AttributeListAdapter(ParserAdapter _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(0)]
      public virtual void setAttributeList(AttributeList qAtts) => this.qAtts = qAtts;

      public virtual int getLength() => this.qAtts.getLength();

      public virtual string getURI(int i) => "";

      public virtual string getLocalName(int i) => "";

      public virtual string getQName(int i) => StringImpl.intern(this.qAtts.getName(i));

      public virtual string getType(int i) => StringImpl.intern(this.qAtts.getType(i));

      public virtual string getValue(int i) => this.qAtts.getValue(i);

      public virtual int getIndex(string uri, string localName) => -1;

      public virtual int getIndex(string qName)
      {
        int length = this.this\u00240.atts.getLength();
        for (int i = 0; i < length; ++i)
        {
          if (StringImpl.equals(this.qAtts.getName(i), (object) qName))
            return i;
        }
        return -1;
      }

      public virtual string getType(string uri, string localName) => (string) null;

      public virtual string getType(string qName) => StringImpl.intern(this.qAtts.getType(qName));

      public virtual string getValue(string uri, string localName) => (string) null;

      public virtual string getValue(string qName) => this.qAtts.getValue(qName);

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        ParserAdapter.AttributeListAdapter attributeListAdapter = this;
        ObjectImpl.clone((object) attributeListAdapter);
        return ((object) attributeListAdapter).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
