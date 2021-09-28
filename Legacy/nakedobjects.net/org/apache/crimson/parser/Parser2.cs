// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.Parser2
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.apache.crimson.util;
using org.xml.sax;
using org.xml.sax.ext;
using org.xml.sax.helpers;
using System.ComponentModel;

namespace org.apache.crimson.parser
{
  public class Parser2
  {
    private InputEntity @in;
    private AttributesExImpl attTmp;
    private StringBuffer strTmp;
    private char[] nameTmp;
    private Parser2.NameCache nameCache;
    private char[] charTmp;
    private string[] namePartsTmp;
    private bool seenNSDecl;
    private NamespaceSupport nsSupport;
    private Vector nsAttTmp;
    private bool isValidating;
    private bool fastStandalone;
    private bool isInAttribute;
    private bool namespaces;
    private bool prefixes;
    private bool inExternalPE;
    private bool doLexicalPE;
    private bool donePrologue;
    private bool isStandalone;
    private string rootElementName;
    private bool ignoreDeclarations;
    private SimpleHashtable elements;
    private SimpleHashtable @params;
    [JavaFlags(0)]
    public Hashtable notations;
    [JavaFlags(0)]
    public SimpleHashtable entities;
    private ContentHandler contentHandler;
    private DTDHandler dtdHandler;
    private EntityResolver resolver;
    private ErrorHandler errHandler;
    private Locale locale;
    private Locator locator;
    private DeclHandler declHandler;
    private LexicalHandler lexicalHandler;
    private const bool supportValidation = true;
    [JavaFlags(24)]
    public const string strANY = "ANY";
    [JavaFlags(24)]
    public const string strEMPTY = "EMPTY";
    private static readonly Parser2.NullHandler nullHandler;
    private const string XmlLang = "xml:lang";
    [JavaFlags(24)]
    public static readonly Parser2.Catalog messages;

    public Parser2()
    {
      int length1 = 2;
      this.charTmp = length1 >= 0 ? new char[length1] : throw new NegativeArraySizeException();
      int length2 = 3;
      this.namePartsTmp = length2 >= 0 ? new string[length2] : throw new NegativeArraySizeException();
      this.isValidating = false;
      this.fastStandalone = false;
      this.isInAttribute = false;
      this.elements = new SimpleHashtable(47);
      this.@params = new SimpleHashtable(7);
      this.notations = new Hashtable(7);
      this.entities = new SimpleHashtable(17);
      this.locator = (Locator) new Parser2.DocLocator(this);
      this.setHandlers();
    }

    [JavaFlags(0)]
    public virtual void setNamespaceFeatures(bool namespaces, bool prefixes)
    {
      this.namespaces = namespaces;
      this.prefixes = prefixes;
    }

    [JavaFlags(0)]
    public virtual void setEntityResolver(EntityResolver resolver) => this.resolver = resolver;

    public virtual void setDTDHandler(DTDHandler handler) => this.dtdHandler = handler;

    [JavaFlags(0)]
    public virtual void setContentHandler(ContentHandler handler) => this.contentHandler = handler;

    [JavaFlags(0)]
    public virtual void setErrorHandler(ErrorHandler handler) => this.errHandler = handler;

    [JavaFlags(0)]
    public virtual void setLexicalHandler(LexicalHandler handler) => this.lexicalHandler = handler;

    [JavaFlags(0)]
    public virtual void setDeclHandler(DeclHandler handler) => this.declHandler = handler;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void setLocale(Locale l)
    {
      if (l != null && !Parser2.messages.isLocaleSupported(l.ToString()))
      {
        Parser2.Catalog messages = Parser2.messages;
        Locale locale = this.locale;
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) l;
        throw new SAXException(messages.getMessage(locale, "P-078", parameters));
      }
      this.locale = l;
    }

    public virtual Locale getLocale() => this.locale;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual Locale chooseLocale(string[] languages)
    {
      Locale l = Parser2.messages.chooseLocale(languages);
      if (l != null)
        this.setLocale(l);
      return l;
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    public virtual void parse(InputSource @in)
    {
      this.init();
      this.parseInternal(@in);
    }

    public virtual void setFastStandalone(bool value) => this.fastStandalone = value && !this.isValidating;

    public virtual bool isFastStandalone() => this.fastStandalone;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void pushInputBuffer(char[] buf, int offset, int len)
    {
      if (len <= 0)
        return;
      if (offset != 0 || len != buf.Length)
      {
        int length = len;
        char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
        java.lang.System.arraycopy((object) buf, offset, (object) chArray, 0, len);
        buf = chArray;
      }
      this.pushReader(buf, (string) null, false);
    }

    [JavaFlags(0)]
    public virtual void setIsValidating(bool value)
    {
      this.isValidating = value;
      if (!value)
        return;
      this.fastStandalone = false;
    }

    private void init()
    {
      this.@in = (InputEntity) null;
      this.attTmp = new AttributesExImpl();
      this.strTmp = new StringBuffer();
      int length = 20;
      this.nameTmp = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      this.nameCache = new Parser2.NameCache();
      if (this.namespaces)
      {
        this.nsSupport = new NamespaceSupport();
        if (this.isValidating && !this.prefixes)
          this.nsAttTmp = new Vector();
      }
      this.isStandalone = false;
      this.rootElementName = (string) null;
      this.isInAttribute = false;
      this.inExternalPE = false;
      this.doLexicalPE = false;
      this.donePrologue = false;
      this.entities.clear();
      this.notations.clear();
      this.@params.clear();
      this.elements.clear();
      this.ignoreDeclarations = false;
      this.builtin("amp", "&#38;");
      this.builtin("lt", "&#60;");
      this.builtin("gt", ">");
      this.builtin("quot", "\"");
      this.builtin("apos", "'");
      if (this.locale == null)
        this.locale = Locale.getDefault();
      if (this.resolver == null)
        this.resolver = (EntityResolver) new Resolver();
      this.setHandlers();
    }

    private void setHandlers()
    {
      if (this.contentHandler == null)
        this.contentHandler = (ContentHandler) Parser2.nullHandler;
      if (this.errHandler == null)
        this.errHandler = (ErrorHandler) Parser2.nullHandler;
      if (this.dtdHandler == null)
        this.dtdHandler = (DTDHandler) Parser2.nullHandler;
      if (this.lexicalHandler == null)
        this.lexicalHandler = (LexicalHandler) Parser2.nullHandler;
      if (this.declHandler != null)
        return;
      this.declHandler = (DeclHandler) Parser2.nullHandler;
    }

    private void builtin(string entityName, string entityValue)
    {
      InternalEntity internalEntity = new InternalEntity(entityName, StringImpl.toCharArray(entityValue));
      this.entities.put((object) entityName, (object) internalEntity);
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    private void parseInternal(InputSource input)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(0)]
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void afterRoot()
    {
    }

    [JavaFlags(0)]
    public virtual void afterDocument()
    {
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void whitespace(string roleId)
    {
      if (this.maybeWhitespace())
        return;
      int length = 1;
      object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
      parameters[0] = (object) Parser2.messages.getMessage(this.locale, roleId);
      this.fatal("P-004", parameters);
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeWhitespace()
    {
      if (!this.inExternalPE || !this.doLexicalPE)
        return this.@in.maybeWhitespace();
      char ch = this.getc();
      bool flag = false;
      for (; ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r'; ch = this.getc())
      {
        flag = true;
        if (this.@in.isEOF() && !this.@in.isInternal())
          return flag;
      }
      this.ungetc();
      return flag;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private string maybeGetName() => this.maybeGetNameCacheEntry()?.name;

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private Parser2.NameCacheEntry maybeGetNameCacheEntry()
    {
      char c = this.getc();
      if (XmlChars.isLetter(c) || c == ':' || c == '_')
        return this.nameCharString(c);
      this.ungetc();
      return (Parser2.NameCacheEntry) null;
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    private string getNmtoken()
    {
      char c = this.getc();
      if (!XmlChars.isNameChar(c))
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) new Character(c);
        this.fatal("P-006", parameters);
      }
      return this.nameCharString(c).name;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private Parser2.NameCacheEntry nameCharString(char c)
    {
      int len = 1;
      this.nameTmp[0] = c;
      while ((c = this.@in.getNameChar()) != char.MinValue)
      {
        if (len >= this.nameTmp.Length)
        {
          int length = this.nameTmp.Length + 10;
          char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
          java.lang.System.arraycopy((object) this.nameTmp, 0, (object) chArray, 0, this.nameTmp.Length);
          this.nameTmp = chArray;
        }
        char[] nameTmp = this.nameTmp;
        int num1;
        len = (num1 = len) + 1;
        int index = num1;
        int num2 = (int) c;
        nameTmp[index] = (char) num2;
      }
      return this.nameCache.lookupEntry(this.nameTmp, len);
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void parseLiteral(bool isEntityValue)
    {
      bool doLexicalPe = this.doLexicalPE;
      char ch1 = this.getc();
      InputEntity inputEntity = this.@in;
      if (ch1 != '\'' && ch1 != '"')
        this.fatal("P-007");
      this.isInAttribute = ((isEntityValue ? 1 : 0) ^ 1) != 0;
      this.strTmp = new StringBuffer();
      while (true)
      {
        while (this.@in == inputEntity || !this.@in.isEOF())
        {
          char ch2;
          if ((int) (ch2 = this.getc()) != (int) ch1 || this.@in != inputEntity)
          {
            switch (ch2)
            {
              case '%':
                if (isEntityValue)
                {
                  string name = this.maybeGetName();
                  if (name != null)
                  {
                    this.nextChar(';', "F-021", name);
                    if (this.inExternalPE)
                    {
                      this.expandEntityInLiteral(name, this.@params, isEntityValue);
                      continue;
                    }
                    int length = 1;
                    object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
                    parameters[0] = (object) name;
                    this.fatal("P-010", parameters);
                    continue;
                  }
                  this.fatal("P-011");
                  break;
                }
                break;
              case '&':
                string name1 = this.maybeGetName();
                if (name1 != null)
                {
                  this.nextChar(';', "F-020", name1);
                  if (isEntityValue)
                  {
                    this.strTmp.append('&');
                    this.strTmp.append(name1);
                    this.strTmp.append(';');
                    continue;
                  }
                  this.expandEntityInLiteral(name1, this.entities, isEntityValue);
                  continue;
                }
                if (this.getc() == '#')
                {
                  int charNumber = this.parseCharNumber();
                  if (charNumber > (int) ushort.MaxValue)
                  {
                    int charTmp = this.surrogatesToCharTmp(charNumber);
                    this.strTmp.append(this.charTmp[0]);
                    if (charTmp == 2)
                    {
                      this.strTmp.append(this.charTmp[1]);
                      continue;
                    }
                    continue;
                  }
                  this.strTmp.append((char) charNumber);
                  continue;
                }
                this.fatal("P-009");
                continue;
            }
            if (!isEntityValue)
            {
              switch (ch2)
              {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                  this.strTmp.append(' ');
                  continue;
                case '<':
                  this.fatal("P-012");
                  break;
              }
            }
            this.strTmp.append(ch2);
          }
          else
          {
            this.isInAttribute = false;
            return;
          }
        }
        this.@in = this.@in.pop();
      }
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    private void expandEntityInLiteral(string name, SimpleHashtable table, bool isEntityValue)
    {
      object obj = table.get(name);
      switch (obj)
      {
        case InternalEntity _:
          InternalEntity internalEntity = (InternalEntity) obj;
          if (this.isValidating && this.isStandalone && !internalEntity.isFromInternalSubset)
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) name;
            this.error("V-002", parameters);
          }
          this.pushReader(internalEntity.buf, name, ((internalEntity.isPE ? 1 : 0) ^ 1) != 0);
          break;
        case ExternalEntity _:
          if (!isEntityValue)
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) name;
            this.fatal("P-013", parameters);
          }
          this.pushReader((ExternalEntity) obj);
          break;
        case null:
          string message = table != this.@params ? "P-014" : "V-022";
          int length1 = 1;
          object[] parameters1 = length1 >= 0 ? new object[length1] : throw new NegativeArraySizeException();
          parameters1[0] = (object) name;
          this.fatal(message, parameters1);
          break;
      }
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private string getQuotedString(string type, string extra)
    {
      char ch1 = this.@in.getc();
      switch (ch1)
      {
        case '"':
        case '\'':
          this.strTmp = new StringBuffer();
          char ch2;
          while ((int) (ch2 = this.@in.getc()) != (int) ch1)
            this.strTmp.append(ch2);
          return this.strTmp.ToString();
        default:
          int length1 = 1;
          object[] parameters1 = length1 >= 0 ? new object[length1] : throw new NegativeArraySizeException();
          Parser2.Catalog messages = Parser2.messages;
          Locale locale = this.locale;
          string messageId = type;
          int length2 = 1;
          object[] parameters2 = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
          parameters2[0] = (object) extra;
          parameters1[0] = (object) messages.getMessage(locale, messageId, parameters2);
          this.fatal("P-015", parameters1);
          goto case '"';
      }
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private string parsePublicId()
    {
      string quotedString = this.getQuotedString("F-033", (string) null);
      for (int index = 0; index < StringImpl.length(quotedString); ++index)
      {
        char ch = StringImpl.charAt(quotedString, index);
        if (StringImpl.indexOf(" \r\n-'()+,./:=?;!*#@$_%0123456789", (int) ch) == -1 && (ch < 'A' || ch > 'Z') && (ch < 'a' || ch > 'z'))
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) new Character(ch);
          this.fatal("P-016", parameters);
        }
      }
      this.strTmp = new StringBuffer();
      this.strTmp.append(quotedString);
      return this.normalize(false);
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeComment(bool skipStart)
    {
      if (!this.@in.peek(!skipStart ? "<!--" : "!--", (char[]) null))
        return false;
      bool doLexicalPe = this.doLexicalPE;
      this.doLexicalPE = false;
      bool flag = this.lexicalHandler != Parser2.nullHandler;
      if (flag)
        this.strTmp = new StringBuffer();
      while (true)
      {
        try
        {
          while (true)
          {
            int num;
            do
            {
              num = (int) this.getc();
              if (num == 45)
              {
                if (this.getc() != '-')
                {
                  if (flag)
                    this.strTmp.append('-');
                  this.ungetc();
                }
                else
                {
                  this.nextChar('>', "F-022", (string) null);
                  goto label_18;
                }
              }
            }
            while (!flag);
            this.strTmp.append((char) num);
          }
        }
        catch (EndOfInputException ex)
        {
          if (this.inExternalPE || !this.donePrologue && this.@in.isInternal())
          {
            if (this.isValidating)
              this.error("V-021", (object[]) null);
            this.@in = this.@in.pop();
          }
          else
            this.fatal("P-017");
        }
      }
label_18:
      this.doLexicalPE = doLexicalPe;
      if (flag)
      {
        int length1 = this.strTmp.length();
        int length2 = length1;
        char[] ch = length2 >= 0 ? new char[length2] : throw new NegativeArraySizeException();
        if (length1 != 0)
          this.strTmp.getChars(0, length1, ch, 0);
        this.lexicalHandler.comment(ch, 0, length1);
      }
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybePI(bool skipStart)
    {
      bool doLexicalPe = this.doLexicalPE;
      if (!this.@in.peek(!skipStart ? "<?" : "?", (char[]) null))
        return false;
      this.doLexicalPE = false;
      string name = this.maybeGetName();
      if (name == null)
        this.fatal("P-018");
      if (StringImpl.equals("xml", (object) name))
        this.fatal("P-019");
      if (StringImpl.equalsIgnoreCase("xml", name))
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) name;
        this.fatal("P-020", parameters);
      }
      if (this.maybeWhitespace())
      {
        this.strTmp = new StringBuffer();
        try
        {
          while (true)
          {
            char ch = this.@in.getc();
            if (ch == '?')
            {
              if (this.@in.peekc('>'))
                break;
            }
            this.strTmp.append(ch);
          }
        }
        catch (EndOfInputException ex)
        {
          this.fatal("P-021");
        }
        this.contentHandler.processingInstruction(name, this.strTmp.ToString());
      }
      else
      {
        if (!this.@in.peek("?>", (char[]) null))
          this.fatal("P-022");
        this.contentHandler.processingInstruction(name, "");
      }
      this.doLexicalPE = doLexicalPe;
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void maybeXmlDecl()
    {
      if (!this.@in.isXmlDeclOrTextDeclPrefix())
        return;
      this.peek("<?xml");
      this.readVersion(true, "1.0");
      this.readEncoding(false);
      this.readStandalone();
      this.maybeWhitespace();
      if (this.peek("?>"))
        return;
      char ch = this.getc();
      int length = 2;
      object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
      parameters[0] = (object) Integer.toHexString((int) ch);
      parameters[1] = (object) new Character(ch);
      this.fatal("P-023", parameters);
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private string maybeReadAttribute(string name, bool must)
    {
      if (!this.maybeWhitespace())
      {
        if (!must)
          return (string) null;
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) name;
        this.fatal("P-024", parameters);
      }
      if (!this.peek(name))
      {
        if (must)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) name;
          this.fatal("P-024", parameters);
        }
        else
        {
          this.ungetc();
          return (string) null;
        }
      }
      this.maybeWhitespace();
      this.nextChar('=', "F-023", (string) null);
      this.maybeWhitespace();
      return this.getQuotedString("F-035", name);
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void readVersion(bool must, string versionNum)
    {
      string str = this.maybeReadAttribute("version", must);
      if (must && str == null)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) versionNum;
        this.fatal("P-025", parameters);
      }
      if (str != null)
      {
        int num = StringImpl.length(str);
        for (int index = 0; index < num; ++index)
        {
          char ch = StringImpl.charAt(str, index);
          if ((ch < '0' || ch > '9') && ch != '_' && ch != '.' && (ch < 'a' || ch > 'z') && (ch < 'A' || ch > 'Z') && ch != ':' && ch != '-')
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) str;
            this.fatal("P-026", parameters);
          }
        }
      }
      if (str == null || StringImpl.equals(str, (object) versionNum))
        return;
      int length1 = 2;
      object[] parameters1 = length1 >= 0 ? new object[length1] : throw new NegativeArraySizeException();
      parameters1[0] = (object) versionNum;
      parameters1[1] = (object) str;
      this.error("P-027", parameters1);
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void maybeMisc(bool eofOK)
    {
      do
        ;
      while ((!eofOK || !this.@in.isEOF()) && (this.maybeComment(false) || this.maybePI(false) || this.maybeWhitespace()));
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private string getMarkupDeclname(string roleId, bool qname)
    {
      this.whitespace(roleId);
      string name = this.maybeGetName();
      if (name == null)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) Parser2.messages.getMessage(this.locale, roleId);
        this.fatal("P-005", parameters);
      }
      return name;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeDoctypeDecl()
    {
      if (!this.peek("<!DOCTYPE"))
        return false;
      ExternalEntity next = (ExternalEntity) null;
      this.rootElementName = this.getMarkupDeclname("F-014", true);
      if (this.maybeWhitespace() && (next = this.maybeExternalID()) != null)
      {
        this.lexicalHandler.startDTD(this.rootElementName, next.publicId, next.verbatimSystemId);
        this.maybeWhitespace();
      }
      else
        this.lexicalHandler.startDTD(this.rootElementName, (string) null, (string) null);
      if (this.@in.peekc('['))
      {
        while (true)
        {
          while (!this.@in.isEOF() || this.@in.isDocument())
          {
            if (!this.maybeMarkupDecl() && !this.maybePEReference() && !this.maybeWhitespace())
            {
              if (this.peek("<!["))
              {
                this.fatal("P-028");
              }
              else
              {
                this.nextChar(']', "F-024", (string) null);
                this.maybeWhitespace();
                goto label_12;
              }
            }
          }
          this.@in = this.@in.pop();
        }
      }
label_12:
      this.nextChar('>', "F-025", (string) null);
      if (next != null)
      {
        next.name = "[dtd]";
        next.isPE = true;
        this.externalParameterEntity(next);
      }
      this.@params.clear();
      this.lexicalHandler.endDTD();
      Vector vector = new Vector();
      Enumeration enumeration = this.notations.keys();
      while (enumeration.hasMoreElements())
      {
        string str = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        object obj = this.notations.get((object) str);
        if (obj == Boolean.TRUE)
        {
          if (this.isValidating)
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) str;
            this.error("V-003", parameters);
          }
          vector.addElement((object) str);
        }
        else if (\u003CVerifierFix\u003E.isInstanceOfString(obj))
        {
          if (this.isValidating)
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) str;
            this.error("V-004", parameters);
          }
          vector.addElement((object) str);
        }
      }
      while (!vector.isEmpty())
      {
        object obj = vector.firstElement();
        vector.removeElement(obj);
        this.notations.remove(obj);
      }
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeMarkupDecl() => this.maybeElementDecl() || this.maybeAttlistDecl() || this.maybeEntityDecl() || this.maybeNotationDecl() || this.maybePI(false) || this.maybeComment(false);

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void readStandalone()
    {
      string str = this.maybeReadAttribute("standalone", false);
      if (str == null || StringImpl.equals("no", (object) str))
        return;
      if (StringImpl.equals("yes", (object) str))
      {
        this.isStandalone = true;
      }
      else
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) str;
        this.fatal("P-029", parameters);
      }
    }

    private bool isXmlLang(string value)
    {
      if (StringImpl.length(value) < 2)
        return false;
      char ch1 = StringImpl.charAt(value, 1);
      char ch2;
      int num;
      if (ch1 == '-')
      {
        ch2 = StringImpl.charAt(value, 0);
        switch (ch2)
        {
          case 'I':
          case 'X':
          case 'i':
          case 'x':
            num = 1;
            break;
          default:
            return false;
        }
      }
      else
      {
        if ((ch1 < 'a' || ch1 > 'z') && (ch1 < 'A' || ch1 > 'Z'))
          return false;
        ch2 = StringImpl.charAt(value, 0);
        if ((ch2 < 'a' || ch2 > 'z') && (ch2 < 'A' || ch2 > 'Z'))
          return false;
        num = 2;
      }
      while (num < StringImpl.length(value))
      {
        ch2 = StringImpl.charAt(value, num);
        if (ch2 == '-')
        {
          while (++num < StringImpl.length(value))
          {
            ch2 = StringImpl.charAt(value, num);
            if ((ch2 < 'a' || ch2 > 'z') && (ch2 < 'A' || ch2 > 'Z'))
              break;
          }
        }
        else
          break;
      }
      return StringImpl.length(value) == num && ch2 != '-';
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeElement(ElementValidator validator)
    {
      bool flag1 = false;
      bool flag2 = true;
      Parser2.NameCacheEntry nameCacheEntry = this.maybeGetNameCacheEntry();
      if (nameCacheEntry == null)
        return false;
      validator?.consume(nameCacheEntry.name);
      ElementDecl element = (ElementDecl) this.elements.get(nameCacheEntry.name);
      if (this.isValidating)
      {
        if (element == null || element.contentType == null)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) nameCacheEntry.name;
          this.error("V-005", parameters);
          element = new ElementDecl(nameCacheEntry.name);
          element.contentType = "ANY";
          this.elements.put((object) nameCacheEntry.name, (object) element);
        }
        if (validator == null && this.rootElementName != null && !StringImpl.equals(this.rootElementName, (object) nameCacheEntry.name))
        {
          int length = 2;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) nameCacheEntry.name;
          parameters[1] = (object) this.rootElementName;
          this.error("V-006", parameters);
        }
      }
      int lineNumber = this.@in.getLineNumber();
      bool flag3 = this.@in.maybeWhitespace();
      Vector exceptions = (Vector) null;
      if (this.namespaces)
      {
        this.nsSupport.pushContext();
        this.seenNSDecl = false;
      }
      while (!this.@in.peekc('>'))
      {
        if (this.@in.peekc('/'))
        {
          flag2 = false;
          break;
        }
        if (!flag3)
          this.fatal("P-030");
        string name = this.maybeGetName();
        if (name == null)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) new Character(this.getc());
          this.fatal("P-031", parameters);
        }
        if (this.attTmp.getValue(name) != null)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) name;
          this.fatal("P-032", parameters);
        }
        this.@in.maybeWhitespace();
        this.nextChar('=', "F-026", name);
        this.@in.maybeWhitespace();
        this.doLexicalPE = false;
        this.parseLiteral(false);
        flag3 = this.@in.maybeWhitespace();
        AttributeDecl attr = element != null ? (AttributeDecl) element.attributes.get(name) : (AttributeDecl) null;
        string str;
        if (attr == null)
        {
          if (this.isValidating)
          {
            int length = 2;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) name;
            parameters[1] = (object) nameCacheEntry.name;
            this.error("V-007", parameters);
          }
          str = this.strTmp.ToString();
        }
        else
        {
          if (!StringImpl.equals("CDATA", (object) attr.type))
          {
            str = this.normalize(((attr.isFromInternalSubset ? 1 : 0) ^ 1) != 0);
            if (this.isValidating)
              this.validateAttributeSyntax(attr, str);
          }
          else
            str = this.strTmp.ToString();
          if (this.isValidating && attr.isFixed && !StringImpl.equals(str, (object) attr.defaultValue))
          {
            int length = 3;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) name;
            parameters[1] = (object) nameCacheEntry.name;
            parameters[2] = (object) attr.defaultValue;
            this.error("V-008", parameters);
          }
        }
        if (StringImpl.equals("xml:lang", (object) name) && !this.isXmlLang(str))
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) str;
          this.error("P-033", parameters);
        }
        string type = attr != null ? attr.type : "CDATA";
        string defaultValue = attr?.defaultValue;
        if (this.namespaces)
          exceptions = this.processAttributeNS(name, type, str, defaultValue, true, false, exceptions);
        else
          this.attTmp.addAttribute("", "", name, type, str, defaultValue, true);
        flag1 = true;
      }
      if (element != null)
        this.attTmp.setIdAttributeName(element.id);
      if (element != null && element.attributes.size() != 0)
        flag1 = this.defaultAttributes(element) || flag1;
      if (this.seenNSDecl)
      {
        int length = this.attTmp.getLength();
        for (int index = 0; index < length; ++index)
        {
          string qname = this.attTmp.getQName(index);
          if (!StringImpl.startsWith(qname, "xmlns") || StringImpl.length(qname) != 5 && StringImpl.charAt(qname, 5) != ':')
          {
            string[] strArray = this.processName(qname, true, false);
            this.attTmp.setURI(index, strArray[0]);
            this.attTmp.setLocalName(index, strArray[1]);
          }
        }
      }
      else if (exceptions != null && this.errHandler != null)
      {
        for (int index = 0; index < exceptions.size(); ++index)
          this.errHandler.error((SAXParseException) exceptions.elementAt(index));
      }
      if (this.namespaces)
      {
        string[] strArray = this.processName(nameCacheEntry.name, false, false);
        this.contentHandler.startElement(strArray[0], strArray[1], strArray[2], (Attributes) this.attTmp);
      }
      else
        this.contentHandler.startElement("", "", nameCacheEntry.name, (Attributes) this.attTmp);
      if (flag1)
      {
        this.attTmp.clear();
        if (this.isValidating && this.namespaces && !this.prefixes)
          this.nsAttTmp.removeAllElements();
      }
      validator = this.newValidator(element);
      if (flag2)
      {
        this.content(element, false, validator);
        if (!this.@in.peek(nameCacheEntry.name, nameCacheEntry.chars))
        {
          int length = 2;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) nameCacheEntry.name;
          parameters[1] = (object) new Integer(lineNumber);
          this.fatal("P-034", parameters);
        }
        this.@in.maybeWhitespace();
      }
      this.nextChar('>', "F-027", nameCacheEntry.name);
      validator.done();
      if (this.namespaces)
      {
        string[] strArray = this.processName(nameCacheEntry.name, false, false);
        this.contentHandler.endElement(strArray[0], strArray[1], strArray[2]);
        Enumeration declaredPrefixes = this.nsSupport.getDeclaredPrefixes();
        while (declaredPrefixes.hasMoreElements())
          this.contentHandler.endPrefixMapping(\u003CVerifierFix\u003E.genCastToString(declaredPrefixes.nextElement()));
        this.nsSupport.popContext();
      }
      else
        this.contentHandler.endElement("", "", nameCacheEntry.name);
      return true;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private Vector processAttributeNS(
      string attQName,
      string type,
      string value,
      string defaultValue,
      bool isSpecified,
      bool isDefaulting,
      Vector exceptions)
    {
      if (StringImpl.startsWith(attQName, "xmlns"))
      {
        bool flag = StringImpl.length(attQName) == 5;
        if (!flag)
        {
          if (StringImpl.charAt(attQName, 5) != ':')
            goto label_12;
        }
        string str = !flag ? StringImpl.substring(attQName, 6) : "";
        if (!this.nsSupport.declarePrefix(str, value))
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) str;
          this.error("P-083", parameters);
        }
        this.contentHandler.startPrefixMapping(str, value);
        if (this.prefixes)
          this.attTmp.addAttribute("", str, StringImpl.intern(attQName), type, value, defaultValue, isSpecified);
        else if (this.isValidating && !isDefaulting)
          this.nsAttTmp.addElement((object) attQName);
        this.seenNSDecl = true;
        return exceptions;
      }
label_12:
      try
      {
        string[] strArray = this.processName(attQName, true, true);
        this.attTmp.addAttribute(strArray[0], strArray[1], strArray[2], type, value, defaultValue, isSpecified);
      }
      catch (SAXException ex)
      {
        if (exceptions == null)
          exceptions = new Vector();
        exceptions.addElement((object) ex);
        this.attTmp.addAttribute("", attQName, attQName, type, value, defaultValue, isSpecified);
      }
      return exceptions;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private string[] processName(string qName, bool isAttribute, bool useException)
    {
      string[] strArray = this.nsSupport.processName(qName, this.namePartsTmp, isAttribute);
      if (strArray == null)
      {
        int length1 = 3;
        strArray = length1 >= 0 ? new string[length1] : throw new NegativeArraySizeException();
        strArray[0] = "";
        string localPart = XmlNames.getLocalPart(qName);
        strArray[1] = localPart == null ? "" : StringImpl.intern(localPart);
        strArray[2] = StringImpl.intern(qName);
        string messageId = "P-084";
        int length2 = 1;
        object[] objArray = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
        objArray[0] = (object) qName;
        object[] parameters = objArray;
        if (useException)
          throw new SAXParseException(Parser2.messages.getMessage(this.locale, messageId, parameters), this.locator);
        this.error(messageId, parameters);
      }
      return strArray;
    }

    [JavaFlags(0)]
    public virtual ElementValidator newValidator(ElementDecl element) => ElementValidator.ANY;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    [JavaFlags(0)]
    public virtual void validateAttributeSyntax(AttributeDecl attr, string value)
    {
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private bool defaultAttributes(ElementDecl element)
    {
      bool flag = false;
      Enumeration enumeration = element.attributes.keys();
      while (enumeration.hasMoreElements())
      {
        string str = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        if (this.attTmp.getValue(str) == null)
        {
          AttributeDecl attributeDecl = (AttributeDecl) element.attributes.get(str);
          if (this.isValidating && attributeDecl.isRequired)
          {
            if (!this.namespaces || this.prefixes || !this.nsAttTmp.contains((object) str))
            {
              int length = 1;
              object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
              parameters[0] = (object) str;
              this.error("V-009", parameters);
            }
            else
              continue;
          }
          string defaultValue = attributeDecl.defaultValue;
          if (defaultValue != null)
          {
            if (this.isValidating && this.isStandalone && !attributeDecl.isFromInternalSubset)
            {
              int length = 1;
              object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
              parameters[0] = (object) str;
              this.error("V-010", parameters);
            }
            if (this.namespaces)
              this.processAttributeNS(str, attributeDecl.type, defaultValue, defaultValue, false, true, (Vector) null);
            else
              this.attTmp.addAttribute("", "", str, attributeDecl.type, defaultValue, defaultValue, false);
            flag = true;
          }
        }
      }
      return flag;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void content(ElementDecl element, bool allowEOF, ElementValidator validator)
    {
      do
      {
        do
        {
          if (this.@in.peekc('<'))
          {
            if (!this.maybeElement(validator))
            {
              if (this.@in.peekc('/'))
                return;
              if (!this.maybeComment(true) && !this.maybePI(true))
              {
                if (this.@in.peek("![CDATA[", (char[]) null))
                {
                  this.lexicalHandler.startCDATA();
                  this.@in.unparsedContent(this.contentHandler, validator, element != null && element.ignoreWhitespace, !this.isStandalone || !this.isValidating || element.isFromInternalSubset ? (string) null : "V-023");
                  this.lexicalHandler.endCDATA();
                  continue;
                }
                char ch = this.getc();
                int length = 2;
                object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
                parameters[0] = (object) Integer.toHexString((int) ch);
                parameters[1] = (object) new Character(ch);
                this.fatal("P-079", parameters);
              }
              else
                continue;
            }
            else
              continue;
          }
          if (element != null && element.ignoreWhitespace && this.@in.ignorableWhitespace(this.contentHandler))
          {
            if (this.isValidating && this.isStandalone && !element.isFromInternalSubset)
            {
              int length = 1;
              object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
              parameters[0] = (object) element.name;
              this.error("V-011", parameters);
            }
          }
        }
        while (this.@in.parsedContent(this.contentHandler, validator));
        if (this.@in.isEOF())
          goto label_21;
      }
      while (this.maybeReferenceInContent(element, validator));
      throw new InternalError();
label_21:
      if (allowEOF)
        return;
      this.fatal("P-035");
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeElementDecl()
    {
      InputEntity inputEntity = this.peekDeclaration("!ELEMENT");
      if (inputEntity == null)
        return false;
      string markupDeclname = this.getMarkupDeclname("F-015", true);
      ElementDecl element = (ElementDecl) this.elements.get(markupDeclname);
      bool flag = false;
      if (element != null)
      {
        if (element.contentType != null)
        {
          if (this.isValidating && element.contentType != null)
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) markupDeclname;
            this.error("V-012", parameters);
          }
          element = new ElementDecl(markupDeclname);
        }
      }
      else
      {
        element = new ElementDecl(markupDeclname);
        if (!this.ignoreDeclarations)
        {
          this.elements.put((object) element.name, (object) element);
          flag = true;
        }
      }
      element.isFromInternalSubset = ((this.inExternalPE ? 1 : 0) ^ 1) != 0;
      this.whitespace("F-000");
      if (this.peek("EMPTY"))
      {
        element.contentType = "EMPTY";
        element.ignoreWhitespace = true;
      }
      else if (this.peek("ANY"))
      {
        element.contentType = "ANY";
        element.ignoreWhitespace = false;
      }
      else
        element.contentType = this.getMixedOrChildren(element);
      this.maybeWhitespace();
      char ch = this.getc();
      if (ch != '>')
      {
        int length = 2;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) markupDeclname;
        parameters[1] = (object) new Character(ch);
        this.fatal("P-036", parameters);
      }
      if (this.isValidating && inputEntity != this.@in)
        this.error("V-013", (object[]) null);
      if (flag)
        this.declHandler.elementDecl(element.name, element.contentType);
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private string getMixedOrChildren(ElementDecl element)
    {
      this.strTmp = new StringBuffer();
      this.nextChar('(', "F-028", element.name);
      InputEntity start = this.@in;
      this.maybeWhitespace();
      this.strTmp.append('(');
      if (this.peek("#PCDATA"))
      {
        this.strTmp.append("#PCDATA");
        this.getMixed(element.name, start);
        element.ignoreWhitespace = false;
      }
      else
      {
        element.model = this.getcps(element.name, start);
        element.ignoreWhitespace = true;
      }
      return this.strTmp.ToString();
    }

    [JavaFlags(0)]
    public virtual ContentModel newContentModel(string tag) => (ContentModel) null;

    [JavaFlags(0)]
    public virtual ContentModel newContentModel(char type, ContentModel next) => (ContentModel) null;

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private ContentModel getcps(string element, InputEntity start)
    {
      bool flag = false;
      char type = char.MinValue;
      // ISSUE: variable of the null type
      __Null local;
      ContentModel next = (ContentModel) (local = null);
      ContentModel contentModel = (ContentModel) local;
      ContentModel original = (ContentModel) local;
      do
      {
        string name = this.maybeGetName();
        if (name != null)
        {
          this.strTmp.append(name);
          next = this.getFrequency(this.newContentModel(name));
        }
        else if (this.peek("("))
        {
          InputEntity start1 = this.@in;
          this.strTmp.append('(');
          this.maybeWhitespace();
          next = this.getFrequency(this.getcps(element, start1));
        }
        else
        {
          string message;
          switch (type)
          {
            case char.MinValue:
              message = "P-039";
              break;
            case ',':
              message = "P-037";
              break;
            default:
              message = "P-038";
              break;
          }
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) new Character(this.getc());
          this.fatal(message, parameters);
        }
        this.maybeWhitespace();
        if (flag)
        {
          char ch = this.getc();
          if (contentModel != null)
          {
            contentModel.next = this.newContentModel(type, next);
            contentModel = contentModel.next;
          }
          if ((int) ch == (int) type)
          {
            this.strTmp.append(type);
            this.maybeWhitespace();
            goto label_26;
          }
          else if (ch == ')')
          {
            this.ungetc();
            goto label_26;
          }
          else
          {
            string message = type != char.MinValue ? "P-040" : "P-041";
            int length = 2;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) new Character(ch);
            parameters[1] = (object) new Character(type);
            this.fatal(message, parameters);
          }
        }
        else
        {
          type = this.getc();
          switch (type)
          {
            case ',':
            case '|':
              flag = true;
              original = contentModel = this.newContentModel(type, next);
              this.strTmp.append(type);
              break;
            default:
              original = contentModel = next;
              this.ungetc();
              goto label_26;
          }
        }
        this.maybeWhitespace();
label_26:;
      }
      while (!this.peek(")"));
      if (this.isValidating && this.@in != start)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) element;
        this.error("V-014", parameters);
      }
      this.strTmp.append(')');
      return this.getFrequency(original);
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private ContentModel getFrequency(ContentModel original)
    {
      char type = this.getc();
      switch (type)
      {
        case '*':
        case '+':
        case '?':
          this.strTmp.append(type);
          if (original == null)
            return (ContentModel) null;
          if (original.type != char.MinValue)
            return this.newContentModel(type, original);
          original.type = type;
          return original;
        default:
          this.ungetc();
          return original;
      }
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void getMixed(string element, InputEntity start)
    {
      this.maybeWhitespace();
      if (this.peek(")*") || this.peek(")"))
      {
        if (this.isValidating && this.@in != start)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) element;
          this.error("V-014", parameters);
        }
        this.strTmp.append(')');
      }
      else
      {
        Vector vector = (Vector) null;
        if (this.isValidating)
          vector = new Vector();
        while (this.peek("|"))
        {
          this.strTmp.append('|');
          this.maybeWhitespace();
          string name = this.maybeGetName();
          if (name == null)
          {
            int length = 2;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) element;
            parameters[1] = (object) Integer.toHexString((int) this.getc());
            this.fatal("P-042", parameters);
          }
          if (this.isValidating)
          {
            if (vector.contains((object) name))
            {
              int length = 1;
              object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
              parameters[0] = (object) name;
              this.error("V-015", parameters);
            }
            else
              vector.addElement((object) name);
          }
          this.strTmp.append(name);
          this.maybeWhitespace();
        }
        if (!this.peek(")*"))
        {
          int length = 2;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) element;
          parameters[1] = (object) new Character(this.getc());
          this.fatal("P-043", parameters);
        }
        if (this.isValidating && this.@in != start)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) element;
          this.error("V-014", parameters);
        }
        this.strTmp.append(')');
      }
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeAttlistDecl()
    {
      InputEntity inputEntity = this.peekDeclaration("!ATTLIST");
      if (inputEntity == null)
        return false;
      string markupDeclname = this.getMarkupDeclname("F-016", true);
      ElementDecl elementDecl = (ElementDecl) this.elements.get(markupDeclname);
      if (elementDecl == null)
      {
        elementDecl = new ElementDecl(markupDeclname);
        if (!this.ignoreDeclarations)
          this.elements.put((object) markupDeclname, (object) elementDecl);
      }
      this.maybeWhitespace();
      while (!this.peek(">"))
      {
        string name1 = this.maybeGetName();
        if (name1 == null)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) new Character(this.getc());
          this.fatal("P-044", parameters);
        }
        this.whitespace("F-001");
        AttributeDecl attr = new AttributeDecl(name1);
        attr.isFromInternalSubset = ((this.inExternalPE ? 1 : 0) ^ 1) != 0;
        if (this.peek("CDATA"))
          attr.type = "CDATA";
        else if (this.peek("IDREFS"))
          attr.type = "IDREFS";
        else if (this.peek("IDREF"))
          attr.type = "IDREF";
        else if (this.peek("ID"))
        {
          attr.type = "ID";
          if (elementDecl.id != null)
          {
            if (this.isValidating)
            {
              int length = 1;
              object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
              parameters[0] = (object) elementDecl.id;
              this.error("V-016", parameters);
            }
          }
          else
            elementDecl.id = name1;
        }
        else if (this.peek("ENTITY"))
          attr.type = "ENTITY";
        else if (this.peek("ENTITIES"))
          attr.type = "ENTITIES";
        else if (this.peek("NMTOKENS"))
          attr.type = "NMTOKENS";
        else if (this.peek("NMTOKEN"))
          attr.type = "NMTOKEN";
        else if (this.peek("NOTATION"))
        {
          attr.type = "NOTATION";
          this.whitespace("F-002");
          this.nextChar('(', "F-029", (string) null);
          this.maybeWhitespace();
          Vector vector = new Vector();
          do
          {
            string name2;
            if ((name2 = this.maybeGetName()) == null)
              this.fatal("P-068");
            if (this.isValidating && this.notations.get((object) name2) == null)
              this.notations.put((object) name2, (object) name2);
            vector.addElement((object) name2);
            this.maybeWhitespace();
            if (this.peek("|"))
              this.maybeWhitespace();
          }
          while (!this.peek(")"));
          AttributeDecl attributeDecl = attr;
          int length = vector.size();
          string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
          attributeDecl.values = strArray;
          for (int index = 0; index < vector.size(); ++index)
            attr.values[index] = \u003CVerifierFix\u003E.genCastToString(vector.elementAt(index));
        }
        else if (this.peek("("))
        {
          attr.type = "ENUMERATION";
          this.maybeWhitespace();
          Vector vector = new Vector();
          do
          {
            string nmtoken = this.getNmtoken();
            vector.addElement((object) nmtoken);
            this.maybeWhitespace();
            if (this.peek("|"))
              this.maybeWhitespace();
          }
          while (!this.peek(")"));
          AttributeDecl attributeDecl = attr;
          int length = vector.size();
          string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
          attributeDecl.values = strArray;
          for (int index = 0; index < vector.size(); ++index)
            attr.values[index] = \u003CVerifierFix\u003E.genCastToString(vector.elementAt(index));
        }
        else
        {
          int length = 2;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) name1;
          parameters[1] = (object) new Character(this.getc());
          this.fatal("P-045", parameters);
        }
        this.whitespace("F-003");
        if (this.peek("#REQUIRED"))
        {
          attr.valueDefault = "#REQUIRED";
          attr.isRequired = true;
        }
        else if (this.peek("#FIXED"))
        {
          if (this.isValidating && (object) attr.type == (object) "ID")
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) attr.name;
            this.error("V-017", parameters);
          }
          attr.valueDefault = "#FIXED";
          attr.isFixed = true;
          this.whitespace("F-004");
          this.doLexicalPE = false;
          this.parseLiteral(false);
          this.doLexicalPE = true;
          attr.defaultValue = (object) attr.type == (object) "CDATA" ? this.strTmp.ToString() : this.normalize(false);
          if ((object) attr.type != (object) "CDATA")
            this.validateAttributeSyntax(attr, attr.defaultValue);
        }
        else if (this.peek("#IMPLIED"))
        {
          attr.valueDefault = "#IMPLIED";
        }
        else
        {
          if (this.isValidating && (object) attr.type == (object) "ID")
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) attr.name;
            this.error("V-018", parameters);
          }
          this.doLexicalPE = false;
          this.parseLiteral(false);
          this.doLexicalPE = true;
          attr.defaultValue = (object) attr.type == (object) "CDATA" ? this.strTmp.ToString() : this.normalize(false);
          if ((object) attr.type != (object) "CDATA")
            this.validateAttributeSyntax(attr, attr.defaultValue);
        }
        if (StringImpl.equals("xml:lang", (object) attr.name) && attr.defaultValue != null && !this.isXmlLang(attr.defaultValue))
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) attr.defaultValue;
          this.error("P-033", parameters);
        }
        if (!this.ignoreDeclarations && elementDecl.attributes.get(attr.name) == null)
        {
          elementDecl.attributes.put((object) attr.name, (object) attr);
          string type;
          if ((object) attr.type == (object) "ENUMERATION" || (object) attr.type == (object) "NOTATION")
          {
            StringBuffer stringBuffer = new StringBuffer();
            if ((object) attr.type == (object) "NOTATION")
            {
              stringBuffer.append(attr.type);
              stringBuffer.append(" ");
            }
            if (attr.values.Length > 1)
              stringBuffer.append("(");
            for (int index = 0; index < attr.values.Length; ++index)
            {
              stringBuffer.append(attr.values[index]);
              if (index + 1 < attr.values.Length)
                stringBuffer.append("|");
            }
            if (attr.values.Length > 1)
              stringBuffer.append(")");
            type = stringBuffer.ToString();
          }
          else
            type = attr.type;
          this.declHandler.attributeDecl(elementDecl.name, attr.name, type, attr.valueDefault, attr.defaultValue);
        }
        this.maybeWhitespace();
      }
      if (this.isValidating && inputEntity != this.@in)
        this.error("V-013", (object[]) null);
      return true;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private string normalize(bool invalidIfNeeded)
    {
      string str1 = this.strTmp.ToString();
      string str2 = StringImpl.trim(str1);
      bool flag = false;
      if ((object) str1 != (object) str2)
      {
        str1 = str2;
        str2 = (string) null;
        flag = true;
      }
      this.strTmp = new StringBuffer();
      for (int index = 0; index < StringImpl.length(str1); ++index)
      {
        char c = StringImpl.charAt(str1, index);
        if (!XmlChars.isSpace(c))
        {
          this.strTmp.append(c);
        }
        else
        {
          this.strTmp.append(' ');
          while (++index < StringImpl.length(str1) && XmlChars.isSpace(StringImpl.charAt(str1, index)))
            flag = true;
          index += -1;
        }
      }
      if (this.isValidating && this.isStandalone && invalidIfNeeded && (str2 == null || flag))
        this.error("V-019", (object[]) null);
      return flag ? this.strTmp.ToString() : str1;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeConditionalSect()
    {
      if (!this.peek("<!["))
        return false;
      InputEntity inputEntity = this.@in;
      this.maybeWhitespace();
      string name;
      if ((name = this.maybeGetName()) == null)
        this.fatal("P-046");
      this.maybeWhitespace();
      this.nextChar('[', "F-030", (string) null);
      if (StringImpl.equals("INCLUDE", (object) name))
      {
        while (true)
        {
          while (!this.@in.isEOF() || this.@in == inputEntity)
          {
            if (this.@in.isEOF())
            {
              if (this.isValidating)
                this.error("V-020", (object[]) null);
              this.@in = this.@in.pop();
            }
            if (!this.peek("]]>"))
            {
              this.doLexicalPE = false;
              if (!this.maybeWhitespace() && !this.maybePEReference())
              {
                this.doLexicalPE = true;
                if (!this.maybeMarkupDecl() && !this.maybeConditionalSect())
                  this.fatal("P-047");
              }
            }
            else
              goto label_26;
          }
          this.@in = this.@in.pop();
        }
      }
      else if (StringImpl.equals("IGNORE", (object) name))
      {
        int num = 1;
        this.doLexicalPE = false;
        while (num > 0)
        {
          switch (this.getc())
          {
            case '<':
              if (this.peek("!["))
              {
                ++num;
                continue;
              }
              continue;
            case ']':
              if (this.peek("]>"))
              {
                num += -1;
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      else
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) name;
        this.fatal("P-048", parameters);
      }
label_26:
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeReferenceInContent(ElementDecl element, ElementValidator validator)
    {
      if (!this.@in.peekc('&'))
        return false;
      if (!this.@in.peekc('#'))
      {
        string name = this.maybeGetName();
        if (name == null)
          this.fatal("P-009");
        this.nextChar(';', "F-020", name);
        this.expandEntityInContent(element, name, validator);
        return true;
      }
      validator.text();
      this.contentHandler.characters(this.charTmp, 0, this.surrogatesToCharTmp(this.parseCharNumber()));
      return true;
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    private int parseCharNumber()
    {
      int num = 0;
      if (this.getc() != 'x')
      {
        this.ungetc();
        while (true)
        {
          char ch = this.getc();
          if (ch >= '0' && ch <= '9')
            num = num * 10 + ((int) ch - 48);
          else if (ch != ';')
            this.fatal("P-049");
          else
            break;
        }
        return num;
      }
      while (true)
      {
        char ch = this.getc();
        if (ch >= '0' && ch <= '9')
          num = (num << 4) + ((int) ch - 48);
        else if (ch >= 'a' && ch <= 'f')
          num = (num << 4) + (10 + ((int) ch - 97));
        else if (ch >= 'A' && ch <= 'F')
          num = (num << 4) + (10 + ((int) ch - 65));
        else if (ch != ';')
          this.fatal("P-050");
        else
          break;
      }
      return num;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private int surrogatesToCharTmp(int ucs4)
    {
      if (ucs4 <= (int) ushort.MaxValue)
      {
        if (XmlChars.isChar(ucs4))
        {
          this.charTmp[0] = (char) ucs4;
          return 1;
        }
      }
      else if (ucs4 <= 1114111)
      {
        ucs4 -= 65536;
        this.charTmp[0] = (char) (55296 | ucs4 >> 10 & 1023);
        this.charTmp[1] = (char) (56320 | ucs4 & 1023);
        return 2;
      }
      int length = 1;
      object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
      parameters[0] = (object) Integer.toHexString(ucs4);
      this.fatal("P-051", parameters);
      return -1;
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    private void expandEntityInContent(
      ElementDecl element,
      string name,
      ElementValidator validator)
    {
      object obj = this.entities.get(name);
      InputEntity inputEntity = this.@in;
      if (obj == null)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) name;
        this.fatal("P-014", parameters);
      }
      if (obj is InternalEntity)
      {
        InternalEntity internalEntity = (InternalEntity) obj;
        if (this.isValidating && this.isStandalone && !internalEntity.isFromInternalSubset)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) name;
          this.error("V-002", parameters);
        }
        this.pushReader(internalEntity.buf, name, true);
        this.content(element, true, validator);
        if (this.@in != inputEntity && !this.@in.isEOF())
        {
          while (this.@in.isInternal())
            this.@in = this.@in.pop();
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) name;
          this.fatal("P-052", parameters);
        }
        this.lexicalHandler.endEntity(name);
        this.@in = this.@in.pop();
      }
      else
      {
        ExternalEntity next = obj is ExternalEntity ? (ExternalEntity) obj : throw new InternalError(name);
        if (next.notation != null)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) name;
          this.fatal("P-053", parameters);
        }
        if (this.isValidating && this.isStandalone && !next.isFromInternalSubset)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) name;
          this.error("V-002", parameters);
        }
        this.externalParsedEntity(element, next, validator);
      }
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybePEReference()
    {
      if (!this.@in.peekc('%'))
        return false;
      string name = this.maybeGetName();
      if (name == null)
        this.fatal("P-011");
      this.nextChar(';', "F-021", name);
      object obj = this.@params.get(name);
      switch (obj)
      {
        case InternalEntity _:
          this.pushReader(((InternalEntity) obj).buf, name, false);
          break;
        case ExternalEntity _:
          this.externalParameterEntity((ExternalEntity) obj);
          break;
        case null:
          this.ignoreDeclarations = true;
          if (this.isValidating)
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) name;
            this.error("V-022", parameters);
            break;
          }
          int length1 = 1;
          object[] parameters1 = length1 >= 0 ? new object[length1] : throw new NegativeArraySizeException();
          parameters1[0] = (object) name;
          this.warning("V-022", parameters1);
          break;
      }
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeEntityDecl()
    {
      InputEntity inputEntity = this.peekDeclaration("!ENTITY");
      if (inputEntity == null)
        return false;
      this.doLexicalPE = false;
      this.whitespace("F-005");
      SimpleHashtable entities;
      if (this.@in.peekc('%'))
      {
        this.whitespace("F-006");
        entities = this.@params;
      }
      else
        entities = this.entities;
      this.ungetc();
      this.doLexicalPE = true;
      string markupDeclname = this.getMarkupDeclname("F-017", false);
      this.whitespace("F-007");
      ExternalEntity externalEntity = this.maybeExternalID();
      bool flag1 = entities.get(markupDeclname) == null;
      if (!flag1 && entities == this.entities)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) markupDeclname;
        this.warning("P-054", parameters);
      }
      bool flag2 = ((flag1 ? 1 : 0) & ((this.ignoreDeclarations ? 1 : 0) ^ 1)) != 0;
      if (externalEntity == null)
      {
        this.doLexicalPE = false;
        this.parseLiteral(true);
        this.doLexicalPE = true;
        if (flag2)
        {
          int length = this.strTmp.length();
          char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
          if (chArray.Length != 0)
            this.strTmp.getChars(0, chArray.Length, chArray, 0);
          InternalEntity internalEntity = new InternalEntity(markupDeclname, chArray);
          internalEntity.isPE = entities == this.@params;
          internalEntity.isFromInternalSubset = ((this.inExternalPE ? 1 : 0) ^ 1) != 0;
          entities.put((object) markupDeclname, (object) internalEntity);
          if (entities == this.@params)
            markupDeclname = new StringBuffer().append("%").append(markupDeclname).ToString();
          this.declHandler.internalEntityDecl(markupDeclname, StringImpl.createString(chArray));
        }
      }
      else
      {
        if (entities == this.entities && this.maybeWhitespace() && this.peek("NDATA"))
        {
          externalEntity.notation = this.getMarkupDeclname("F-018", false);
          if (this.isValidating && this.notations.get((object) externalEntity.notation) == null)
            this.notations.put((object) externalEntity.notation, (object) Boolean.TRUE);
        }
        externalEntity.name = markupDeclname;
        externalEntity.isPE = entities == this.@params;
        externalEntity.isFromInternalSubset = ((this.inExternalPE ? 1 : 0) ^ 1) != 0;
        if (flag2)
        {
          entities.put((object) markupDeclname, (object) externalEntity);
          if (externalEntity.notation != null)
          {
            this.dtdHandler.unparsedEntityDecl(markupDeclname, externalEntity.publicId, externalEntity.systemId, externalEntity.notation);
          }
          else
          {
            if (entities == this.@params)
              markupDeclname = new StringBuffer().append("%").append(markupDeclname).ToString();
            this.declHandler.externalEntityDecl(markupDeclname, externalEntity.publicId, externalEntity.systemId);
          }
        }
      }
      this.maybeWhitespace();
      this.nextChar('>', "F-031", markupDeclname);
      if (this.isValidating && inputEntity != this.@in)
        this.error("V-013", (object[]) null);
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private ExternalEntity maybeExternalID()
    {
      string str = (string) null;
      if (this.peek("PUBLIC"))
      {
        this.whitespace("F-009");
        str = this.parsePublicId();
      }
      else if (!this.peek("SYSTEM"))
        return (ExternalEntity) null;
      ExternalEntity externalEntity = new ExternalEntity((Locator) this.@in);
      externalEntity.publicId = str;
      this.whitespace("F-008");
      externalEntity.verbatimSystemId = this.getQuotedString("F-034", (string) null);
      externalEntity.systemId = this.resolveURI(externalEntity.verbatimSystemId);
      return externalEntity;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private string parseSystemId() => this.resolveURI(this.getQuotedString("F-034", (string) null));

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private string resolveURI(string uri)
    {
      int num1 = StringImpl.indexOf(uri, 58);
      if (num1 == -1 || StringImpl.indexOf(uri, 47) < num1)
      {
        string systemId = this.@in.getSystemId();
        if (systemId == null)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) uri;
          this.fatal("P-055", parameters);
        }
        if (StringImpl.length(uri) == 0)
          uri = ".";
        string str = StringImpl.substring(systemId, 0, StringImpl.lastIndexOf(systemId, 47) + 1);
        if (StringImpl.charAt(uri, 0) != '/')
        {
          uri = new StringBuffer().append(str).append(uri).ToString();
        }
        else
        {
          int num2 = StringImpl.indexOf(str, 58);
          uri = new StringBuffer().append(num2 != -1 ? StringImpl.substring(str, 0, num2 + 1) : "file:").append(uri).ToString();
        }
      }
      if (StringImpl.indexOf(uri, 35) != -1)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) uri;
        this.error("P-056", parameters);
      }
      return uri;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void maybeTextDecl()
    {
      if (!this.@in.isXmlDeclOrTextDeclPrefix())
        return;
      this.peek("<?xml");
      this.readVersion(false, "1.0");
      this.readEncoding(true);
      this.maybeWhitespace();
      if (this.peek("?>"))
        return;
      this.fatal("P-057");
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool externalParsedEntity(
      ElementDecl element,
      ExternalEntity next,
      ElementValidator validator)
    {
      if (!this.pushReader(next))
      {
        if (!this.isInAttribute)
          this.lexicalHandler.endEntity(next.name);
        return false;
      }
      this.maybeTextDecl();
      this.content(element, true, validator);
      if (!this.@in.isEOF())
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) next.name;
        this.fatal("P-058", parameters);
      }
      this.@in = this.@in.pop();
      if (!this.isInAttribute)
        this.lexicalHandler.endEntity(next.name);
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void externalParameterEntity(ExternalEntity next)
    {
      if (this.isStandalone && this.fastStandalone)
        return;
      this.inExternalPE = true;
      try
      {
        this.pushReader(next);
      }
      catch (IOException ex)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) next.systemId;
        this.fatal("P-082", parameters, (Exception) ex);
      }
      InputEntity inputEntity = this.@in;
      try
      {
        this.maybeTextDecl();
      }
      catch (IOException ex)
      {
        this.@in = this.@in.pop();
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) next.systemId;
        this.fatal("P-082", parameters, (Exception) ex);
      }
      while (!inputEntity.isEOF())
      {
        if (this.@in.isEOF())
        {
          this.@in = this.@in.pop();
        }
        else
        {
          this.doLexicalPE = false;
          if (!this.maybeWhitespace() && !this.maybePEReference())
          {
            this.doLexicalPE = true;
            if (!this.maybeMarkupDecl() && !this.maybeConditionalSect())
              break;
          }
        }
      }
      if (!inputEntity.isEOF())
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) this.@in.getName();
        this.fatal("P-059", parameters);
      }
      this.@in = this.@in.pop();
      this.inExternalPE = ((this.@in.isDocument() ? 1 : 0) ^ 1) != 0;
      this.doLexicalPE = false;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void readEncoding(bool must)
    {
      string str = this.maybeReadAttribute("encoding", must);
      if (str == null)
        return;
      for (int index = 0; index < StringImpl.length(str); ++index)
      {
        char ch = StringImpl.charAt(str, index);
        if ((ch < 'A' || ch > 'Z') && (ch < 'a' || ch > 'z') && (index == 0 || (ch < '0' || ch > '9') && ch != '-' && ch != '_' && ch != '.'))
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) new Character(ch);
          this.fatal("P-060", parameters);
        }
      }
      string encoding = this.@in.getEncoding();
      if (encoding == null || StringImpl.equalsIgnoreCase(str, encoding))
        return;
      int length1 = 2;
      object[] parameters1 = length1 >= 0 ? new object[length1] : throw new NegativeArraySizeException();
      parameters1[0] = (object) str;
      parameters1[1] = (object) encoding;
      this.warning("P-061", parameters1);
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool maybeNotationDecl()
    {
      InputEntity inputEntity = this.peekDeclaration("!NOTATION");
      if (inputEntity == null)
        return false;
      string markupDeclname = this.getMarkupDeclname("F-019", false);
      ExternalEntity externalEntity = new ExternalEntity((Locator) this.@in);
      this.whitespace("F-011");
      if (this.peek("PUBLIC"))
      {
        this.whitespace("F-009");
        externalEntity.publicId = this.parsePublicId();
        if (this.maybeWhitespace())
        {
          if (!this.peek(">"))
            externalEntity.systemId = this.parseSystemId();
          else
            this.ungetc();
        }
      }
      else if (this.peek("SYSTEM"))
      {
        this.whitespace("F-008");
        externalEntity.systemId = this.parseSystemId();
      }
      else
        this.fatal("P-062");
      this.maybeWhitespace();
      this.nextChar('>', "F-032", markupDeclname);
      if (this.isValidating && inputEntity != this.@in)
        this.error("V-013", (object[]) null);
      if (externalEntity.systemId != null && StringImpl.indexOf(externalEntity.systemId, 35) != -1)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) externalEntity.systemId;
        this.error("P-056", parameters);
      }
      object obj = this.notations.get((object) markupDeclname);
      if (obj != null && obj is ExternalEntity)
      {
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) markupDeclname;
        this.warning("P-063", parameters);
      }
      else if (!this.ignoreDeclarations)
      {
        this.notations.put((object) markupDeclname, (object) externalEntity);
        this.dtdHandler.notationDecl(markupDeclname, externalEntity.publicId, externalEntity.systemId);
      }
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private char getc()
    {
      if (!this.inExternalPE || !this.doLexicalPE)
      {
        char ch = this.@in.getc();
        if (ch == '%' && this.doLexicalPE)
          this.fatal("P-080");
        return ch;
      }
      while (this.@in.isEOF())
      {
        if (this.@in.isInternal() || this.doLexicalPE && !this.@in.isDocument())
        {
          this.@in = this.@in.pop();
        }
        else
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) this.@in.getName();
          this.fatal("P-064", parameters);
        }
      }
      char ch1;
      if ((ch1 = this.@in.getc()) != '%' || !this.doLexicalPE)
        return ch1;
      string name = this.maybeGetName();
      if (name == null)
        this.fatal("P-011");
      this.nextChar(';', "F-021", name);
      object obj = this.@params.get(name);
      this.pushReader(StringImpl.toCharArray(" "), (string) null, false);
      switch (obj)
      {
        case InternalEntity _:
          this.pushReader(((InternalEntity) obj).buf, name, false);
          break;
        case ExternalEntity _:
          this.pushReader((ExternalEntity) obj);
          break;
        case null:
          this.fatal("V-022");
          break;
        default:
          throw new InternalError();
      }
      this.pushReader(StringImpl.toCharArray(" "), (string) null, false);
      return this.@in.getc();
    }

    private void ungetc() => this.@in.ungetc();

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private bool peek(string s) => this.@in.peek(s, (char[]) null);

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private InputEntity peekDeclaration(string s)
    {
      if (!this.@in.peekc('<'))
        return (InputEntity) null;
      InputEntity inputEntity = this.@in;
      if (this.@in.peek(s, (char[]) null))
        return inputEntity;
      this.@in.ungetc();
      return (InputEntity) null;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void nextChar(char c, string location, string near)
    {
      while (this.@in.isEOF() && !this.@in.isDocument())
        this.@in = this.@in.pop();
      if (this.@in.peekc(c))
        return;
      int length = 3;
      object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
      parameters[0] = (object) new Character(c);
      parameters[1] = (object) Parser2.messages.getMessage(this.locale, location);
      parameters[2] = near != null ? (object) new StringBuffer().append('"').append(near).append('"').ToString() : (object) "";
      this.fatal("P-008", parameters);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private void pushReader(char[] buf, string name, bool isGeneral)
    {
      if (isGeneral && !this.isInAttribute)
        this.lexicalHandler.startEntity(name);
      InputEntity inputEntity = InputEntity.getInputEntity(this.errHandler, this.locale);
      inputEntity.init(buf, name, this.@in, ((isGeneral ? 1 : 0) ^ 1) != 0);
      this.@in = inputEntity;
    }

    [JavaThrownExceptions("2;org/xml/sax/SAXException;java/io/IOException;")]
    private bool pushReader(ExternalEntity next)
    {
      if (!next.isPE && !this.isInAttribute)
        this.lexicalHandler.startEntity(next.name);
      InputEntity inputEntity = InputEntity.getInputEntity(this.errHandler, this.locale);
      InputSource inputSource = next.getInputSource(this.resolver);
      inputEntity.init(inputSource, next.name, this.@in, next.isPE);
      this.@in = inputEntity;
      return true;
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private void warning(string messageId, object[] parameters) => this.errHandler.warning(new SAXParseException(Parser2.messages.getMessage(this.locale, messageId, parameters), this.locator));

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    [JavaFlags(0)]
    public virtual void error(string messageId, object[] parameters) => this.errHandler.error(new SAXParseException(Parser2.messages.getMessage(this.locale, messageId, parameters), this.locator));

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private void fatal(string message) => this.fatal(message, (object[]) null, (Exception) null);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private void fatal(string message, object[] parameters) => this.fatal(message, parameters, (Exception) null);

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private void fatal(string messageId, object[] parameters, Exception e)
    {
      SAXParseException exception = new SAXParseException(Parser2.messages.getMessage(this.locale, messageId, parameters), this.locator, e);
      this.errHandler.fatalError(exception);
      throw exception;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Parser2()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Parser2 parser2 = this;
      ObjectImpl.clone((object) parser2);
      return ((object) parser2).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;org/xml/sax/Locator;")]
    public class DocLocator : Locator
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private Parser2 this\u00240;

      public virtual string getPublicId() => this.this\u00240.@in == null ? (string) null : this.this\u00240.@in.getPublicId();

      public virtual string getSystemId() => this.this\u00240.@in == null ? (string) null : this.this\u00240.@in.getSystemId();

      public virtual int getLineNumber() => this.this\u00240.@in == null ? -1 : this.this\u00240.@in.getLineNumber();

      public virtual int getColumnNumber() => this.this\u00240.@in == null ? -1 : this.this\u00240.@in.getColumnNumber();

      [JavaFlags(0)]
      public DocLocator(Parser2 _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        Parser2.DocLocator docLocator = this;
        ObjectImpl.clone((object) docLocator);
        return ((object) docLocator).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(40)]
    public class NameCache
    {
      [JavaFlags(0)]
      public Parser2.NameCacheEntry[] hashtable;

      [JavaFlags(0)]
      public virtual string lookup(char[] value, int len) => this.lookupEntry(value, len).name;

      [JavaFlags(0)]
      public virtual Parser2.NameCacheEntry lookupEntry(char[] value, int len)
      {
        int num = 0;
        for (int index = 0; index < len; ++index)
          num = num * 31 + (int) value[index];
        int index1 = (num & int.MaxValue) % this.hashtable.Length;
        for (Parser2.NameCacheEntry next = this.hashtable[index1]; next != null; next = next.next)
        {
          if (next.matches(value, len))
            return next;
        }
        Parser2.NameCacheEntry nameCacheEntry1 = new Parser2.NameCacheEntry();
        Parser2.NameCacheEntry nameCacheEntry2 = nameCacheEntry1;
        int length = len;
        char[] chArray = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
        nameCacheEntry2.chars = chArray;
        java.lang.System.arraycopy((object) value, 0, (object) nameCacheEntry1.chars, 0, len);
        nameCacheEntry1.name = StringImpl.createString(nameCacheEntry1.chars);
        nameCacheEntry1.name = StringImpl.intern(nameCacheEntry1.name);
        nameCacheEntry1.next = this.hashtable[index1];
        this.hashtable[index1] = nameCacheEntry1;
        return nameCacheEntry1;
      }

      [JavaFlags(0)]
      public NameCache()
      {
        int length = 541;
        this.hashtable = length >= 0 ? new Parser2.NameCacheEntry[length] : throw new NegativeArraySizeException();
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        Parser2.NameCache nameCache = this;
        ObjectImpl.clone((object) nameCache);
        return ((object) nameCache).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(40)]
    public class NameCacheEntry
    {
      [JavaFlags(0)]
      public string name;
      [JavaFlags(0)]
      public char[] chars;
      [JavaFlags(0)]
      public Parser2.NameCacheEntry next;

      [JavaFlags(0)]
      public virtual bool matches(char[] value, int len)
      {
        if (this.chars.Length != len)
          return false;
        for (int index = 0; index < len; ++index)
        {
          if ((int) value[index] != (int) this.chars[index])
            return false;
        }
        return true;
      }

      [JavaFlags(0)]
      public NameCacheEntry()
      {
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        Parser2.NameCacheEntry nameCacheEntry = this;
        ObjectImpl.clone((object) nameCacheEntry);
        return ((object) nameCacheEntry).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(42)]
    [JavaInterfaces("2;org/xml/sax/ext/LexicalHandler;org/xml/sax/ext/DeclHandler;")]
    private class NullHandler : DefaultHandler, LexicalHandler, DeclHandler
    {
      public virtual void startDTD(string name, string publicId, string systemId)
      {
      }

      public virtual void endDTD()
      {
      }

      public virtual void startEntity(string name)
      {
      }

      public virtual void endEntity(string name)
      {
      }

      public virtual void startCDATA()
      {
      }

      public virtual void endCDATA()
      {
      }

      public virtual void comment(char[] ch, int start, int length)
      {
      }

      public virtual void elementDecl(string name, string model)
      {
      }

      public virtual void attributeDecl(
        string eName,
        string aName,
        string type,
        string valueDefault,
        string value)
      {
      }

      public virtual void internalEntityDecl(string name, string value)
      {
      }

      public virtual void externalEntityDecl(string name, string publicId, string systemId)
      {
      }

      [JavaFlags(2)]
      [JavaFlags(2)]
      public NullHandler()
      {
      }
    }

    [JavaFlags(56)]
    public sealed class Catalog : MessageCatalog
    {
      [JavaFlags(0)]
      public Catalog()
        : base(Class.FromType(typeof (Parser2)))
      {
      }
    }
  }
}
