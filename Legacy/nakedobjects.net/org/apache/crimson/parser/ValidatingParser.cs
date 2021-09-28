// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.ValidatingParser
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.crimson.util;
using org.xml.sax;
using System.ComponentModel;

namespace org.apache.crimson.parser
{
  public class ValidatingParser : Parser2
  {
    private SimpleHashtable ids;
    private readonly ValidatingParser.EmptyValidator EMPTY;

    public ValidatingParser()
    {
      this.ids = new SimpleHashtable();
      this.EMPTY = new ValidatingParser.EmptyValidator(this);
      this.setIsValidating(true);
    }

    public ValidatingParser(bool rejectValidityErrors)
      : this()
    {
      if (!rejectValidityErrors)
        return;
      this.setErrorHandler((ErrorHandler) new ValidatingParser.\u0031(this));
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    [JavaFlags(0)]
    public override void afterRoot()
    {
      Enumeration enumeration = this.ids.keys();
      while (enumeration.hasMoreElements())
      {
        string key = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        Boolean boolean = (Boolean) this.ids.get(key);
        if (Boolean.FALSE == boolean)
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) key;
          this.error("V-024", parameters);
        }
      }
    }

    [JavaFlags(0)]
    public override void afterDocument() => this.ids.clear();

    [JavaFlags(0)]
    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override void validateAttributeSyntax(AttributeDecl attr, string value)
    {
      if ((object) "ID" == (object) attr.type)
      {
        if (!XmlNames.isName(value))
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) value;
          this.error("V-025", parameters);
        }
        Boolean nonInterned = (Boolean) this.ids.getNonInterned(value);
        if (nonInterned == null || nonInterned.Equals((object) Boolean.FALSE))
        {
          this.ids.put((object) StringImpl.intern(value), (object) Boolean.TRUE);
        }
        else
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) value;
          this.error("V-026", parameters);
        }
      }
      else if ((object) "IDREF" == (object) attr.type)
      {
        if (!XmlNames.isName(value))
        {
          int length = 1;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) value;
          this.error("V-027", parameters);
        }
        if ((Boolean) this.ids.getNonInterned(value) != null)
          return;
        this.ids.put((object) StringImpl.intern(value), (object) Boolean.FALSE);
      }
      else if ((object) "IDREFS" == (object) attr.type)
      {
        StringTokenizer stringTokenizer = new StringTokenizer(value);
        bool flag = false;
        while (stringTokenizer.hasMoreTokens())
        {
          value = stringTokenizer.nextToken();
          if (!XmlNames.isName(value))
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) value;
            this.error("V-027", parameters);
          }
          if ((Boolean) this.ids.getNonInterned(value) == null)
            this.ids.put((object) StringImpl.intern(value), (object) Boolean.FALSE);
          flag = true;
        }
        if (flag)
          return;
        this.error("V-039", (object[]) null);
      }
      else if ((object) "NMTOKEN" == (object) attr.type)
      {
        if (XmlNames.isNmtoken(value))
          return;
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) value;
        this.error("V-028", parameters);
      }
      else if ((object) "NMTOKENS" == (object) attr.type)
      {
        StringTokenizer stringTokenizer = new StringTokenizer(value);
        bool flag = false;
        while (stringTokenizer.hasMoreTokens())
        {
          value = stringTokenizer.nextToken();
          if (!XmlNames.isNmtoken(value))
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) value;
            this.error("V-028", parameters);
          }
          flag = true;
        }
        if (flag)
          return;
        this.error("V-032", (object[]) null);
      }
      else if ((object) "ENUMERATION" == (object) attr.type)
      {
        for (int index = 0; index < attr.values.Length; ++index)
        {
          if (StringImpl.equals(value, (object) attr.values[index]))
            return;
        }
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) value;
        this.error("V-029", parameters);
      }
      else if ((object) "NOTATION" == (object) attr.type)
      {
        for (int index = 0; index < attr.values.Length; ++index)
        {
          if (StringImpl.equals(value, (object) attr.values[index]))
            return;
        }
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) value;
        this.error("V-030", parameters);
      }
      else if ((object) "ENTITY" == (object) attr.type)
      {
        if (this.isUnparsedEntity(value))
          return;
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) value;
        this.error("V-031", parameters);
      }
      else if ((object) "ENTITIES" == (object) attr.type)
      {
        StringTokenizer stringTokenizer = new StringTokenizer(value);
        bool flag = false;
        while (stringTokenizer.hasMoreTokens())
        {
          value = stringTokenizer.nextToken();
          if (!this.isUnparsedEntity(value))
          {
            int length = 1;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) value;
            this.error("V-031", parameters);
          }
          flag = true;
        }
        if (flag)
          return;
        this.error("V-040", (object[]) null);
      }
      else if ((object) "CDATA" != (object) attr.type)
        throw new InternalError(attr.type);
    }

    [JavaFlags(0)]
    public override ContentModel newContentModel(string tag) => new ContentModel(tag);

    [JavaFlags(0)]
    public override ContentModel newContentModel(char type, ContentModel next) => new ContentModel(type, next);

    [JavaFlags(0)]
    public override ElementValidator newValidator(ElementDecl element)
    {
      if (element.validator != null)
        return element.validator;
      if (element.model != null)
        return (ElementValidator) new ValidatingParser.ChildrenValidator(this, element);
      element.validator = element.contentType == null || (object) "ANY" == (object) element.contentType ? ElementValidator.ANY : ((object) "EMPTY" != (object) element.contentType ? (ElementValidator) new ValidatingParser.MixedValidator(this, element) : (ElementValidator) this.EMPTY);
      return element.validator;
    }

    private bool isUnparsedEntity(string name)
    {
      object nonInterned = this.entities.getNonInterned(name);
      return nonInterned != null && nonInterned is ExternalEntity && ((ExternalEntity) nonInterned).notation != null;
    }

    [JavaFlags(32)]
    [Inner]
    public class EmptyValidator : ElementValidator
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ValidatingParser this\u00240;

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void consume(string token) => this.this\u00240.error("V-033", (object[]) null);

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void text() => this.this\u00240.error("V-033", (object[]) null);

      [JavaFlags(0)]
      public EmptyValidator(ValidatingParser _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }

    [Inner]
    [JavaFlags(32)]
    public class MixedValidator : ElementValidator
    {
      private ElementDecl element;
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ValidatingParser this\u00240;

      [JavaFlags(0)]
      public MixedValidator(ValidatingParser _param1, ElementDecl element)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.element = element;
      }

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void consume(string type)
      {
        string contentType = this.element.contentType;
        int num = 8;
        while ((num = StringImpl.indexOf(contentType, type, num + 1)) >= 9)
        {
          if (StringImpl.charAt(contentType, num - 1) == '|')
          {
            switch (StringImpl.charAt(contentType, num + StringImpl.length(type)))
            {
              case ')':
                return;
              case '|':
                return;
              default:
                continue;
            }
          }
        }
        ValidatingParser this0 = this.this\u00240;
        int length = 3;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) this.element.name;
        parameters[1] = (object) type;
        parameters[2] = (object) contentType;
        this0.error("V-034", parameters);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class ChildrenValidator : ElementValidator
    {
      private ContentModelState state;
      private string name;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private ValidatingParser this\u00240;

      [JavaFlags(0)]
      public ChildrenValidator(ValidatingParser _param1, ElementDecl element)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.state = new ContentModelState(element.model);
        this.name = element.name;
      }

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void consume(string token)
      {
        if (this.state == null)
        {
          ValidatingParser this0 = this.this\u00240;
          int length = 2;
          object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          parameters[0] = (object) this.name;
          parameters[1] = (object) token;
          this0.error("V-035", parameters);
        }
        else
        {
          try
          {
            this.state = this.state.advance(token);
          }
          catch (EndOfInputException ex)
          {
            ValidatingParser this0 = this.this\u00240;
            int length = 2;
            object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            parameters[0] = (object) this.name;
            parameters[1] = (object) token;
            this0.error("V-036", parameters);
          }
        }
      }

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void text()
      {
        ValidatingParser this0 = this.this\u00240;
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) this.name;
        this0.error("V-037", parameters);
      }

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void done()
      {
        if (this.state == null || this.state.terminate())
          return;
        ValidatingParser this0 = this.this\u00240;
        int length = 1;
        object[] parameters = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        parameters[0] = (object) this.name;
        this0.error("V-038", parameters);
      }
    }

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : HandlerBase
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private ValidatingParser this\u00240;

      [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
      public override void error(SAXParseException x) => throw x;

      public \u0031(ValidatingParser _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
