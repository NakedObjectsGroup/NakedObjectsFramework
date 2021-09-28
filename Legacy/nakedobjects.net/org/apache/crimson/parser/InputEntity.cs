// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.InputEntity
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using java.util;
using org.apache.crimson.util;
using org.xml.sax;
using System.ComponentModel;

namespace org.apache.crimson.parser
{
  [JavaInterfaces("1;org/xml/sax/Locator;")]
  [JavaFlags(48)]
  public sealed class InputEntity : Locator
  {
    private int start;
    private int finish;
    private char[] buf;
    private int lineNumber;
    private bool returnedFirstHalf;
    private bool maybeInCRLF;
    private string name;
    private InputEntity next;
    private InputSource input;
    private Reader reader;
    private bool isClosed;
    private ErrorHandler errHandler;
    private Locale locale;
    private StringBuffer rememberedText;
    private int startRemember;
    private bool isPE;
    private const int BUFSIZ = 8193;
    private static readonly char[] newline;

    public static InputEntity getInputEntity(ErrorHandler h, Locale l) => new InputEntity()
    {
      errHandler = h,
      locale = l
    };

    private InputEntity()
    {
      this.lineNumber = 1;
      this.returnedFirstHalf = false;
      this.maybeInCRLF = false;
    }

    public virtual bool isInternal() => this.reader == null;

    public virtual bool isDocument() => this.next == null;

    public virtual bool isParameterEntity() => this.isPE;

    public virtual string getName() => this.name;

    private static string convertToFileURL(string filename)
    {
      string str = new File(filename).getAbsolutePath();
      if ((int) File.separatorChar != 47)
        str = StringImpl.replace(str, (char) File.separatorChar, '/');
      if (!StringImpl.startsWith(str, "/"))
        str = new StringBuffer().append("/").append(str).ToString();
      return new StringBuffer().append("file:").append(str).ToString();
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual void init(InputSource @in, string name, InputEntity stack, bool isPE)
    {
      this.input = @in;
      this.isPE = isPE;
      this.reader = @in.getCharacterStream();
      if (this.reader == null)
      {
        if (@in.getByteStream() == null)
        {
          string systemId = @in.getSystemId();
          URL url;
          try
          {
            url = new URL(systemId);
          }
          catch (MalformedURLException ex)
          {
            string fileUrl = InputEntity.convertToFileURL(systemId);
            @in.setSystemId(fileUrl);
            url = new URL(fileUrl);
          }
          this.reader = XmlReader.createReader(url.openStream());
        }
        else
          this.reader = @in.getEncoding() == null ? XmlReader.createReader(@in.getByteStream()) : XmlReader.createReader(@in.getByteStream(), @in.getEncoding());
      }
      this.next = stack;
      int length = 8193;
      this.buf = length >= 0 ? new char[length] : throw new NegativeArraySizeException();
      this.name = name;
      this.checkRecursion(stack);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public virtual void init(char[] b, string name, InputEntity stack, bool isPE)
    {
      this.next = stack;
      this.buf = b;
      this.finish = b.Length;
      this.name = name;
      this.isPE = isPE;
      this.checkRecursion(stack);
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private void checkRecursion(InputEntity stack)
    {
      if (stack == null)
        return;
      for (stack = stack.next; stack != null; stack = stack.next)
      {
        if (stack.name != null && StringImpl.equals(stack.name, (object) this.name))
        {
          int length = 1;
          object[] @params = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          @params[0] = (object) this.name;
          this.fatal("P-069", @params);
        }
      }
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual InputEntity pop()
    {
      this.close();
      return this.next;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual bool isEOF()
    {
      if (this.start < this.finish)
        return false;
      this.fillbuf();
      return this.start >= this.finish;
    }

    public virtual string getEncoding()
    {
      if (this.reader == null)
        return (string) null;
      if (this.reader is XmlReader)
        return ((XmlReader) this.reader).getEncoding();
      return this.reader is InputStreamReader ? ((InputStreamReader) this.reader).getEncoding() : (string) null;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual char getNameChar()
    {
      if (this.finish <= this.start)
        this.fillbuf();
      if (this.finish > this.start)
      {
        char[] buf = this.buf;
        int start;
        this.start = (start = this.start) + 1;
        int index = start;
        char c = buf[index];
        if (XmlChars.isNameChar(c))
          return c;
        this.start += -1;
      }
      return char.MinValue;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual char getc()
    {
      if (this.finish <= this.start)
        this.fillbuf();
      if (this.finish > this.start)
      {
        char[] buf = this.buf;
        int start;
        this.start = (start = this.start) + 1;
        int index = start;
        char ch = buf[index];
        if (this.returnedFirstHalf)
        {
          if (ch >= '\uDC00' && ch <= '\uDFFF')
          {
            this.returnedFirstHalf = false;
            return ch;
          }
          int length = 1;
          object[] @params = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          @params[0] = (object) Integer.toHexString((int) ch);
          this.fatal("P-070", @params);
        }
        if (ch >= ' ' && ch <= '\uD7FF' || ch == '\t' || ch >= '\uE000' && ch <= '�')
          return ch;
        if (ch == '\r' && !this.isInternal())
        {
          this.maybeInCRLF = true;
          if (this.getc() != '\n')
            this.ungetc();
          this.maybeInCRLF = false;
          ++this.lineNumber;
          return '\n';
        }
        if (ch == '\n' || ch == '\r')
        {
          if (!this.isInternal() && !this.maybeInCRLF)
            ++this.lineNumber;
          return ch;
        }
        if (ch >= '\uD800' && ch < '\uDC00')
        {
          this.returnedFirstHalf = true;
          return ch;
        }
        int length1 = 1;
        object[] params1 = length1 >= 0 ? new object[length1] : throw new NegativeArraySizeException();
        params1[0] = (object) Integer.toHexString((int) ch);
        this.fatal("P-071", params1);
      }
      throw new EndOfInputException();
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual bool peekc(char c)
    {
      if (this.finish <= this.start)
        this.fillbuf();
      if (this.finish <= this.start || (int) this.buf[this.start] != (int) c)
        return false;
      ++this.start;
      return true;
    }

    public virtual void ungetc()
    {
      if (this.start == 0)
        throw new InternalError(nameof (ungetc));
      this.start += -1;
      if (this.buf[this.start] == '\n' || this.buf[this.start] == '\r')
      {
        if (this.isInternal())
          return;
        this.lineNumber += -1;
      }
      else
      {
        if (!this.returnedFirstHalf)
          return;
        this.returnedFirstHalf = false;
      }
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual bool maybeWhitespace()
    {
      bool flag1 = false;
      bool flag2 = false;
      while (true)
      {
        char ch;
        do
        {
          do
          {
            if (this.finish <= this.start)
              this.fillbuf();
            if (this.finish <= this.start)
              return flag1;
            char[] buf = this.buf;
            int start;
            this.start = (start = this.start) + 1;
            int index = start;
            ch = buf[index];
            switch (ch)
            {
              case '\t':
              case '\n':
              case '\r':
              case ' ':
                flag1 = true;
                continue;
              default:
                goto label_11;
            }
          }
          while (ch != '\n' && ch != '\r' || this.isInternal());
          if (ch != '\n' || !flag2)
          {
            ++this.lineNumber;
            flag2 = false;
          }
        }
        while (ch != '\r');
        flag2 = true;
      }
label_11:
      this.start += -1;
      return flag1;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual bool parsedContent(ContentHandler contentHandler, ElementValidator validator)
    {
      int offset;
      int start = offset = this.start;
      bool flag = false;
      while (true)
      {
        if (offset >= this.finish)
        {
          if (offset > start)
          {
            validator.text();
            contentHandler.characters(this.buf, start, offset - start);
            flag = true;
            this.start = offset;
          }
          if (!this.isEOF())
          {
            start = this.start;
            offset = start - 1;
          }
          else
            break;
        }
        else
        {
          char ch = this.buf[offset];
          if ((ch <= ']' || ch > '\uD7FF') && (ch >= '&' || ch < ' ') && (ch <= '<' || ch >= ']') && (ch <= '&' || ch >= '<') && ch != '\t' && (ch < '\uE000' || ch > '�'))
          {
            if (ch != '<' && ch != '&')
            {
              switch (ch)
              {
                case '\n':
                  if (!this.isInternal())
                  {
                    ++this.lineNumber;
                    break;
                  }
                  break;
                case '\r':
                  if (!this.isInternal())
                  {
                    contentHandler.characters(this.buf, start, offset - start);
                    contentHandler.characters(InputEntity.newline, 0, 1);
                    flag = true;
                    ++this.lineNumber;
                    if (this.finish > offset + 1 && this.buf[offset + 1] == '\n')
                      ++offset;
                    start = this.start = offset + 1;
                    break;
                  }
                  break;
                case ']':
                  switch (this.finish - offset)
                  {
                    case 1:
                      if (this.reader != null && !this.isClosed)
                      {
                        if (offset != start)
                        {
                          int num = offset - 1;
                          if (num > start)
                          {
                            validator.text();
                            contentHandler.characters(this.buf, start, num - start);
                            flag = true;
                            this.start = num;
                          }
                          this.fillbuf();
                          start = offset = this.start;
                          break;
                        }
                        goto label_20;
                      }
                      else
                        break;
                    case 2:
                      if (this.buf[offset + 1] == ']')
                        goto case 1;
                      else
                        break;
                    default:
                      if (this.buf[offset + 1] == ']' && this.buf[offset + 2] == '>')
                      {
                        this.fatal("P-072", (object[]) null);
                        break;
                      }
                      break;
                  }
                  break;
                default:
                  if (ch >= '\uD800' && ch <= '\uDFFF')
                  {
                    if (offset + 1 >= this.finish)
                    {
                      if (offset > start)
                      {
                        validator.text();
                        contentHandler.characters(this.buf, start, offset - start);
                        flag = true;
                        this.start = offset + 1;
                      }
                      if (this.isEOF())
                      {
                        int length = 1;
                        if (length >= 0)
                        {
                          object[] @params = new object[length];
                          @params[0] = (object) Integer.toHexString((int) ch);
                          this.fatal("P-081", @params);
                        }
                        else
                          goto label_32;
                      }
                      start = this.start;
                      offset = start;
                      break;
                    }
                    if (this.checkSurrogatePair(offset))
                    {
                      ++offset;
                      break;
                    }
                    goto label_37;
                  }
                  else
                  {
                    int length = 1;
                    if (length >= 0)
                    {
                      object[] @params = new object[length];
                      @params[0] = (object) Integer.toHexString((int) ch);
                      this.fatal("P-071", @params);
                      break;
                    }
                    goto label_39;
                  }
              }
            }
            else
              goto label_42;
          }
        }
        ++offset;
      }
      return flag;
label_20:
      throw new InternalError("fillbuf");
label_32:
      throw new NegativeArraySizeException();
label_37:
      offset += -1;
      goto label_42;
label_39:
      throw new NegativeArraySizeException();
label_42:
      if (offset == start)
        return flag;
      validator.text();
      contentHandler.characters(this.buf, start, offset - start);
      this.start = offset;
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual void unparsedContent(
      ContentHandler contentHandler,
      ElementValidator validator,
      bool ignorableWhitespace,
      string whitespaceInvalidMessage)
    {
      while (true)
      {
        do
        {
          bool flag1 = false;
          bool flag2 = ignorableWhitespace;
          int start;
          for (start = this.start; start < this.finish; ++start)
          {
            char ch = this.buf[start];
            if (!XmlChars.isChar((int) ch))
            {
              flag2 = false;
              if (ch >= '\uD800' && ch <= '\uDFFF')
              {
                if (this.checkSurrogatePair(start))
                {
                  ++start;
                  continue;
                }
                start += -1;
                break;
              }
              int length = 1;
              object[] @params = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
              @params[0] = (object) Integer.toHexString((int) this.buf[start]);
              this.fatal("P-071", @params);
            }
            switch (ch)
            {
              case '\t':
              case ' ':
                continue;
              case '\n':
                if (!this.isInternal())
                {
                  ++this.lineNumber;
                  continue;
                }
                continue;
              case '\r':
                if (!this.isInternal())
                {
                  if (flag2)
                  {
                    if (whitespaceInvalidMessage != null)
                      this.errHandler.error(new SAXParseException(Parser2.messages.getMessage(this.locale, whitespaceInvalidMessage), (Locator) this));
                    contentHandler.ignorableWhitespace(this.buf, this.start, start - this.start);
                    contentHandler.ignorableWhitespace(InputEntity.newline, 0, 1);
                  }
                  else
                  {
                    validator.text();
                    contentHandler.characters(this.buf, this.start, start - this.start);
                    contentHandler.characters(InputEntity.newline, 0, 1);
                  }
                  ++this.lineNumber;
                  if (this.finish > start + 1 && this.buf[start + 1] == '\n')
                    ++start;
                  this.start = start + 1;
                  continue;
                }
                continue;
              case ']':
                if (start + 2 < this.finish)
                {
                  if (this.buf[start + 1] == ']' && this.buf[start + 2] == '>')
                  {
                    flag1 = true;
                    goto label_29;
                  }
                  else
                  {
                    flag2 = false;
                    continue;
                  }
                }
                else
                  goto label_29;
              default:
                flag2 = false;
                continue;
            }
          }
label_29:
          if (flag2)
          {
            if (whitespaceInvalidMessage != null)
              this.errHandler.error(new SAXParseException(Parser2.messages.getMessage(this.locale, whitespaceInvalidMessage), (Locator) this));
            contentHandler.ignorableWhitespace(this.buf, this.start, start - this.start);
          }
          else
          {
            validator.text();
            contentHandler.characters(this.buf, this.start, start - this.start);
          }
          if (flag1)
          {
            this.start = start + 3;
            return;
          }
          this.start = start;
          this.fillbuf();
        }
        while (!this.isEOF());
        this.fatal("P-073", (object[]) null);
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private bool checkSurrogatePair(int offset)
    {
      if (offset + 1 >= this.finish)
        return false;
      char[] buf = this.buf;
      int num;
      offset = (num = offset) + 1;
      int index = num;
      char ch1 = buf[index];
      char ch2 = this.buf[offset];
      if (ch1 >= '\uD800' && ch1 < '\uDC00' && ch2 >= '\uDC00' && ch2 <= '\uDFFF')
        return true;
      int length = 2;
      object[] @params = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
      @params[0] = (object) Integer.toHexString((int) ch1 & (int) ushort.MaxValue);
      @params[1] = (object) Integer.toHexString((int) ch2 & (int) ushort.MaxValue);
      this.fatal("P-074", @params);
      return false;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual bool ignorableWhitespace(ContentHandler handler)
    {
      bool flag = false;
      int start1 = this.start;
      while (true)
      {
        if (this.finish <= this.start)
        {
          if (flag)
            handler.ignorableWhitespace(this.buf, start1, this.start - start1);
          this.fillbuf();
          start1 = this.start;
        }
        if (this.finish > this.start)
        {
          char[] buf = this.buf;
          int start2;
          this.start = (start2 = this.start) + 1;
          int index = start2;
          switch (buf[index])
          {
            case '\t':
            case ' ':
              flag = true;
              continue;
            case '\n':
              if (!this.isInternal())
              {
                ++this.lineNumber;
                goto case '\t';
              }
              else
                goto case '\t';
            case '\r':
              flag = true;
              if (!this.isInternal())
                ++this.lineNumber;
              handler.ignorableWhitespace(this.buf, start1, this.start - 1 - start1);
              handler.ignorableWhitespace(InputEntity.newline, 0, 1);
              if (this.start < this.finish && this.buf[this.start] == '\n')
                ++this.start;
              start1 = this.start;
              continue;
            default:
              goto label_16;
          }
        }
        else
          break;
      }
      return flag;
label_16:
      this.ungetc();
      if (flag)
        handler.ignorableWhitespace(this.buf, start1, this.start - start1);
      return flag;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    public virtual bool peek(string next, char[] chars)
    {
      int num = chars == null ? StringImpl.length(next) : chars.Length;
      if (this.finish <= this.start || this.finish - this.start < num)
        this.fillbuf();
      if (this.finish <= this.start)
        return false;
      int index;
      if (chars != null)
      {
        for (index = 0; index < num && this.start + index < this.finish; ++index)
        {
          if ((int) this.buf[this.start + index] != (int) chars[index])
            return false;
        }
      }
      else
      {
        for (index = 0; index < num && this.start + index < this.finish; ++index)
        {
          if ((int) this.buf[this.start + index] != (int) StringImpl.charAt(next, index))
            return false;
        }
      }
      if (index < num)
      {
        if (this.reader == null || this.isClosed)
          return false;
        if (num > this.buf.Length)
        {
          int length = 1;
          object[] @params = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          @params[0] = (object) new Integer(this.buf.Length);
          this.fatal("P-077", @params);
        }
        this.fillbuf();
        return this.peek(next, chars);
      }
      this.start += num;
      return true;
    }

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    [JavaFlags(0)]
    public virtual bool isXmlDeclOrTextDeclPrefix()
    {
      string str = "<?xml";
      int num1 = StringImpl.length(str);
      int num2 = num1 + 1;
      if (this.finish <= this.start || this.finish - this.start < num2)
        this.fillbuf();
      if (this.finish <= this.start)
        return false;
      int index;
      for (index = 0; index < num1 && this.start + index < this.finish; ++index)
      {
        if ((int) this.buf[this.start + index] != (int) StringImpl.charAt(str, index))
          return false;
      }
      if (index < num1)
      {
        if (this.reader == null || this.isClosed)
          return false;
        this.fillbuf();
        return this.isXmlDeclOrTextDeclPrefix();
      }
      return XmlChars.isSpace(this.buf[index]);
    }

    public virtual void startRemembering() => this.startRemember = this.startRemember == 0 ? this.start : throw new InternalError();

    public virtual string rememberText()
    {
      string str;
      if (this.rememberedText != null)
      {
        this.rememberedText.append(this.buf, this.startRemember, this.start - this.startRemember);
        str = this.rememberedText.ToString();
      }
      else
        str = StringImpl.createString(this.buf, this.startRemember, this.start - this.startRemember);
      this.startRemember = 0;
      this.rememberedText = (StringBuffer) null;
      return str;
    }

    private Locator getLocator()
    {
      InputEntity inputEntity = this;
      while (inputEntity != null && inputEntity.input == null)
        inputEntity = inputEntity.next;
      return (Locator) inputEntity ?? (Locator) this;
    }

    public virtual string getPublicId()
    {
      Locator locator = this.getLocator();
      return locator == this ? this.input.getPublicId() : locator.getPublicId();
    }

    public virtual string getSystemId()
    {
      Locator locator = this.getLocator();
      return locator == this ? this.input.getSystemId() : locator.getSystemId();
    }

    public virtual int getLineNumber()
    {
      Locator locator = this.getLocator();
      return locator == this ? this.lineNumber : locator.getLineNumber();
    }

    public virtual int getColumnNumber() => -1;

    [JavaThrownExceptions("2;java/io/IOException;org/xml/sax/SAXException;")]
    private void fillbuf()
    {
      if (this.reader == null || this.isClosed)
        return;
      if (this.startRemember != 0)
      {
        if (this.rememberedText == null)
          this.rememberedText = new StringBuffer(this.buf.Length);
        this.rememberedText.append(this.buf, this.startRemember, this.start - this.startRemember);
      }
      bool flag = this.finish > 0 && this.start > 0;
      if (flag)
        this.start += -1;
      int num = this.finish - this.start;
      java.lang.System.arraycopy((object) this.buf, this.start, (object) this.buf, 0, num);
      this.start = 0;
      this.finish = num;
      try
      {
        num = this.buf.Length - num;
        num = this.reader.read(this.buf, this.finish, num);
      }
      catch (UnsupportedEncodingException ex)
      {
        int length = 1;
        object[] @params = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        @params[0] = (object) ((Throwable) ex).getMessage();
        this.fatal("P-075", @params);
      }
      catch (CharConversionException ex)
      {
        int length = 1;
        object[] @params = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        @params[0] = (object) ((Throwable) ex).getMessage();
        this.fatal("P-076", @params);
      }
      if (num >= 0)
        this.finish += num;
      else
        this.close();
      if (flag)
        ++this.start;
      if (this.startRemember == 0)
        return;
      this.startRemember = 1;
    }

    public virtual void close()
    {
      try
      {
        if (this.reader != null && !this.isClosed)
          this.reader.close();
        this.isClosed = true;
      }
      catch (IOException ex)
      {
      }
    }

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    private void fatal(string messageId, object[] @params)
    {
      SAXParseException exception = new SAXParseException(Parser2.messages.getMessage(this.locale, messageId, @params), (Locator) this);
      this.close();
      this.errHandler.fatalError(exception);
      throw exception;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static InputEntity()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      InputEntity inputEntity = this;
      ObjectImpl.clone((object) inputEntity);
      return ((object) inputEntity).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
