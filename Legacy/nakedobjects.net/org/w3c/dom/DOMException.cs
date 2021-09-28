// Decompiled with JetBrains decompiler
// Type: org.w3c.dom.DOMException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.w3c.dom
{
  public class DOMException : RuntimeException
  {
    public short code;
    public const short INDEX_SIZE_ERR = 1;
    public const short DOMSTRING_SIZE_ERR = 2;
    public const short HIERARCHY_REQUEST_ERR = 3;
    public const short WRONG_DOCUMENT_ERR = 4;
    public const short INVALID_CHARACTER_ERR = 5;
    public const short NO_DATA_ALLOWED_ERR = 6;
    public const short NO_MODIFICATION_ALLOWED_ERR = 7;
    public const short NOT_FOUND_ERR = 8;
    public const short NOT_SUPPORTED_ERR = 9;
    public const short INUSE_ATTRIBUTE_ERR = 10;
    public const short INVALID_STATE_ERR = 11;
    public const short SYNTAX_ERR = 12;
    public const short INVALID_MODIFICATION_ERR = 13;
    public const short NAMESPACE_ERR = 14;
    public const short INVALID_ACCESS_ERR = 15;

    public DOMException(short code, string message)
      : base(message)
    {
      this.code = code;
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      DOMException domException = this;
      ObjectImpl.clone((object) domException);
      return ((object) domException).MemberwiseClone();
    }
  }
}
