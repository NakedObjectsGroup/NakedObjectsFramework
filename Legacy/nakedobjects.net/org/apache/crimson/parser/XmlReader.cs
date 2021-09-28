// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.XmlReader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using System.ComponentModel;

namespace org.apache.crimson.parser
{
  [JavaFlags(48)]
  public sealed class XmlReader : Reader
  {
    private const int MAXPUSHBACK = 512;
    private Reader @in;
    private string assignedEncoding;
    private bool closed;
    private static readonly Hashtable charsets;

    [JavaThrownExceptions("1;java/io/IOException;")]
    public static Reader createReader(InputStream @in) => (Reader) new XmlReader(@in);

    [JavaThrownExceptions("1;java/io/IOException;")]
    public static Reader createReader(InputStream @in, string encoding)
    {
      if (encoding == null)
        return (Reader) new XmlReader(@in);
      if (StringImpl.equalsIgnoreCase("UTF-8", encoding) || StringImpl.equalsIgnoreCase("UTF8", encoding))
        return (Reader) new XmlReader.Utf8Reader(@in);
      if (StringImpl.equalsIgnoreCase("US-ASCII", encoding) || StringImpl.equalsIgnoreCase("ASCII", encoding))
        return (Reader) new XmlReader.AsciiReader(@in);
      return StringImpl.equalsIgnoreCase("ISO-8859-1", encoding) ? (Reader) new XmlReader.Iso8859_1Reader(@in) : (Reader) new InputStreamReader(@in, XmlReader.std2java(encoding));
    }

    private static string std2java(string encoding)
    {
      string upperCase = StringImpl.toUpperCase(encoding);
      return \u003CVerifierFix\u003E.genCastToString(XmlReader.charsets.get((object) upperCase)) ?? encoding;
    }

    public virtual string getEncoding() => this.assignedEncoding;

    [JavaThrownExceptions("1;java/io/IOException;")]
    private XmlReader(InputStream stream)
      : base((object) stream)
    {
      PushbackInputStream pb = new PushbackInputStream(stream, 512);
      int length = 4;
      sbyte[] numArray = length >= 0 ? new sbyte[length] : throw new NegativeArraySizeException();
      int num = ((FilterInputStream) pb).read(numArray);
      if (num > 0)
        pb.unread(numArray, 0, num);
      if (num == 4)
      {
        switch ((int) numArray[0] & (int) byte.MaxValue)
        {
          case 0:
            if (numArray[1] == (sbyte) 60 && numArray[2] == (sbyte) 0 && numArray[3] == (sbyte) 63)
            {
              this.setEncoding((InputStream) pb, "UnicodeBig");
              return;
            }
            break;
          case 60:
            switch ((int) numArray[1] & (int) byte.MaxValue)
            {
              case 0:
                if (numArray[2] == (sbyte) 63 && numArray[3] == (sbyte) 0)
                {
                  this.setEncoding((InputStream) pb, "UnicodeLittle");
                  return;
                }
                break;
              case 63:
                if (numArray[2] == (sbyte) 120 && numArray[3] == (sbyte) 109)
                {
                  this.useEncodingDecl(pb, "UTF8");
                  return;
                }
                break;
            }
            break;
          case 76:
            if (numArray[1] == (sbyte) 111 && ((int) byte.MaxValue & (int) numArray[2]) == 167 && ((int) byte.MaxValue & (int) numArray[3]) == 148)
            {
              this.useEncodingDecl(pb, "CP037");
              return;
            }
            break;
          case 254:
            if (((int) numArray[1] & (int) byte.MaxValue) == (int) byte.MaxValue)
            {
              this.setEncoding((InputStream) pb, "UTF-16");
              return;
            }
            break;
          case (int) byte.MaxValue:
            if (((int) numArray[1] & (int) byte.MaxValue) == 254)
            {
              this.setEncoding((InputStream) pb, "UTF-16");
              return;
            }
            break;
        }
      }
      this.setEncoding((InputStream) pb, "UTF-8");
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    private void useEncodingDecl(PushbackInputStream pb, string encoding)
    {
      int length = 512;
      sbyte[] numArray = length >= 0 ? new sbyte[length] : throw new NegativeArraySizeException();
      int num1 = pb.read(numArray, 0, numArray.Length);
      pb.unread(numArray, 0, num1);
      Reader reader = (Reader) new InputStreamReader((InputStream) new ByteArrayInputStream(numArray, 4, num1), encoding);
      if (reader.read() != 108)
      {
        this.setEncoding((InputStream) pb, "UTF-8");
      }
      else
      {
        StringBuffer stringBuffer1 = new StringBuffer();
        StringBuffer stringBuffer2 = (StringBuffer) null;
        string str = (string) null;
        bool flag1 = false;
        char ch = char.MinValue;
        bool flag2 = false;
        int num2;
        for (int index1 = 0; index1 < 507 && (num2 = reader.read()) != -1; ++index1)
        {
          if (num2 != 32 && num2 != 9 && num2 != 10 && num2 != 13)
          {
            if (index1 != 0)
            {
              if (num2 == 63)
                flag2 = true;
              else if (flag2)
              {
                if (num2 != 62)
                  flag2 = false;
                else
                  break;
              }
              if (str == null || !flag1)
              {
                if (stringBuffer2 == null)
                {
                  if (!Character.isWhitespace((char) num2))
                  {
                    stringBuffer2 = stringBuffer1;
                    stringBuffer1.setLength(0);
                    stringBuffer1.append((char) num2);
                    flag1 = false;
                  }
                }
                else if (Character.isWhitespace((char) num2))
                  str = stringBuffer2.ToString();
                else if (num2 == 61)
                {
                  if (str == null)
                    str = stringBuffer2.ToString();
                  flag1 = true;
                  stringBuffer2 = (StringBuffer) null;
                  ch = char.MinValue;
                }
                else
                  stringBuffer2.append((char) num2);
              }
              else if (!Character.isWhitespace((char) num2))
              {
                if (num2 == 34 || num2 == 39)
                {
                  if (ch == char.MinValue)
                  {
                    ch = (char) num2;
                    stringBuffer1.setLength(0);
                    continue;
                  }
                  if (num2 == (int) ch)
                  {
                    if (StringImpl.equals(nameof (encoding), (object) str))
                    {
                      this.assignedEncoding = stringBuffer1.ToString();
                      for (int index2 = 0; index2 < StringImpl.length(this.assignedEncoding); ++index2)
                      {
                        int num3 = (int) StringImpl.charAt(this.assignedEncoding, index2);
                        if ((num3 < 65 || num3 > 90) && (num3 < 97 || num3 > 122) && (index2 == 0 || index2 <= 0 || num3 != 45 && (num3 < 48 || num3 > 57) && num3 != 46 && num3 != 95))
                          goto label_38;
                      }
                      this.setEncoding((InputStream) pb, this.assignedEncoding);
                      return;
                    }
                    str = (string) null;
                    continue;
                  }
                }
                stringBuffer1.append((char) num2);
              }
            }
            else
              break;
          }
        }
label_38:
        this.setEncoding((InputStream) pb, "UTF-8");
      }
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    private void setEncoding(InputStream stream, string encoding)
    {
      this.assignedEncoding = encoding;
      this.@in = XmlReader.createReader(stream, encoding);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual int read(char[] buf, int off, int len)
    {
      if (this.closed)
        return -1;
      int num = this.@in.read(buf, off, len);
      if (num == -1)
        this.close();
      return num;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual int read()
    {
      if (this.closed)
        throw new IOException("closed");
      int num = this.@in.read();
      if (num == -1)
        this.close();
      return num;
    }

    public virtual bool markSupported() => this.@in != null && this.@in.markSupported();

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void mark(int value)
    {
      if (this.@in == null)
        return;
      this.@in.mark(value);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void reset()
    {
      if (this.@in == null)
        return;
      this.@in.reset();
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual long skip(long value) => this.@in == null ? 0L : this.@in.skip(value);

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual bool ready() => this.@in != null && this.@in.ready();

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void close()
    {
      if (this.closed)
        return;
      this.@in.close();
      this.@in = (Reader) null;
      this.closed = true;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static XmlReader()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      XmlReader xmlReader = this;
      ObjectImpl.clone((object) xmlReader);
      return ((object) xmlReader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(1064)]
    public abstract class BaseReader : Reader
    {
      [JavaFlags(4)]
      public InputStream instream;
      [JavaFlags(4)]
      public sbyte[] buffer;
      [JavaFlags(4)]
      public int start;
      [JavaFlags(4)]
      public int finish;

      [JavaFlags(0)]
      public BaseReader(InputStream stream)
        : base((object) stream)
      {
        this.instream = stream;
        int length = 8192;
        this.buffer = length >= 0 ? new sbyte[length] : throw new NegativeArraySizeException();
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public virtual bool ready() => this.instream == null || this.finish - this.start > 0 || this.instream.available() != 0;

      [JavaThrownExceptions("1;java/io/IOException;")]
      public virtual void close()
      {
        if (this.instream == null)
          return;
        this.instream.close();
        this.start = this.finish = 0;
        this.buffer = (sbyte[]) null;
        this.instream = (InputStream) null;
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public virtual object MemberwiseClone()
      {
        XmlReader.BaseReader baseReader = this;
        ObjectImpl.clone((object) baseReader);
        return ((object) baseReader).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public virtual string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(56)]
    public sealed class Utf8Reader : XmlReader.BaseReader
    {
      private char nextChar;

      [JavaFlags(0)]
      public Utf8Reader(InputStream stream)
        : base(stream)
      {
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public virtual int read(char[] buf, int offset, int len)
      {
        // ISSUE: unable to decompile the method.
      }
    }

    [JavaFlags(56)]
    public sealed class AsciiReader : XmlReader.BaseReader
    {
      [JavaFlags(0)]
      public AsciiReader(InputStream @in)
        : base(@in)
      {
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public virtual int read(char[] buf, int offset, int len)
      {
        if (this.instream == null)
          return -1;
        if (offset + len > buf.Length || offset < 0)
          throw new ArrayIndexOutOfBoundsException();
        int num1;
        for (num1 = 0; num1 < len; ++num1)
        {
          if (this.start >= this.finish)
          {
            this.start = 0;
            this.finish = this.instream.read(this.buffer, 0, this.buffer.Length);
            if (this.finish <= 0)
            {
              if (this.finish <= 0)
              {
                this.close();
                break;
              }
              break;
            }
          }
          sbyte[] buffer = this.buffer;
          int start;
          this.start = (start = this.start) + 1;
          int index = start;
          int num2 = (int) buffer[index];
          buf[offset + num1] = (num2 & 128) == 0 ? (char) num2 : throw new CharConversionException(new StringBuffer().append("Illegal ASCII character, 0x").append(Integer.toHexString(num2 & (int) byte.MaxValue)).ToString());
        }
        return num1 == 0 && this.finish <= 0 ? -1 : num1;
      }
    }

    [JavaFlags(56)]
    public sealed class Iso8859_1Reader : XmlReader.BaseReader
    {
      [JavaFlags(0)]
      public Iso8859_1Reader(InputStream @in)
        : base(@in)
      {
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public virtual int read(char[] buf, int offset, int len)
      {
        if (this.instream == null)
          return -1;
        if (offset + len > buf.Length || offset < 0)
          throw new ArrayIndexOutOfBoundsException();
        int num1;
        for (num1 = 0; num1 < len; ++num1)
        {
          if (this.start >= this.finish)
          {
            this.start = 0;
            this.finish = this.instream.read(this.buffer, 0, this.buffer.Length);
            if (this.finish <= 0)
            {
              if (this.finish <= 0)
              {
                this.close();
                break;
              }
              break;
            }
          }
          char[] chArray = buf;
          int index1 = offset + num1;
          sbyte[] buffer = this.buffer;
          int start;
          this.start = (start = this.start) + 1;
          int index2 = start;
          int num2 = (int) (ushort) ((int) byte.MaxValue & (int) buffer[index2]);
          chArray[index1] = (char) num2;
        }
        return num1 == 0 && this.finish <= 0 ? -1 : num1;
      }
    }
  }
}
