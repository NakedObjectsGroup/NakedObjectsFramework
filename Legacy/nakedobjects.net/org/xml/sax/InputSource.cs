// Decompiled with JetBrains decompiler
// Type: org.xml.sax.InputSource
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;

namespace org.xml.sax
{
  public class InputSource
  {
    private string publicId;
    private string systemId;
    private InputStream byteStream;
    private string encoding;
    private Reader characterStream;

    public InputSource()
    {
    }

    public InputSource(string systemId) => this.setSystemId(systemId);

    public InputSource(InputStream byteStream) => this.setByteStream(byteStream);

    public InputSource(Reader characterStream) => this.setCharacterStream(characterStream);

    public virtual void setPublicId(string publicId) => this.publicId = publicId;

    public virtual string getPublicId() => this.publicId;

    public virtual void setSystemId(string systemId) => this.systemId = systemId;

    public virtual string getSystemId() => this.systemId;

    public virtual void setByteStream(InputStream byteStream) => this.byteStream = byteStream;

    public virtual InputStream getByteStream() => this.byteStream;

    public virtual void setEncoding(string encoding) => this.encoding = encoding;

    public virtual string getEncoding() => this.encoding;

    public virtual void setCharacterStream(Reader characterStream) => this.characterStream = characterStream;

    public virtual Reader getCharacterStream() => this.characterStream;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      InputSource inputSource = this;
      ObjectImpl.clone((object) inputSource);
      return ((object) inputSource).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
