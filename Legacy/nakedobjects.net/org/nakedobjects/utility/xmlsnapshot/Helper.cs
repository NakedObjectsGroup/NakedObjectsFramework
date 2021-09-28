// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.Helper
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.w3c.dom;

namespace org.nakedobjects.utility.xmlsnapshot
{
  public sealed class Helper
  {
    [JavaFlags(0)]
    public virtual string trailingSlash(string str) => StringImpl.endsWith(str, "/") ? str : new StringBuffer().append(str).append("/").ToString();

    [JavaFlags(0)]
    public virtual string classNameFor(string fullyQualifiedClassName)
    {
      int num = StringImpl.lastIndexOf(fullyQualifiedClassName, 46);
      return num > 0 && num < StringImpl.length(fullyQualifiedClassName) ? StringImpl.substring(fullyQualifiedClassName, num + 1) : fullyQualifiedClassName;
    }

    [JavaFlags(0)]
    public virtual string packageNameFor(string fullyQualifiedClassName)
    {
      int num = StringImpl.lastIndexOf(fullyQualifiedClassName, 46);
      return num > 0 ? StringImpl.substring(fullyQualifiedClassName, 0, num) : "default";
    }

    [JavaFlags(0)]
    public virtual Element rootElementFor(Element element)
    {
      Document ownerDocument = element.getOwnerDocument();
      return ownerDocument == null ? element : ownerDocument.getDocumentElement() ?? element;
    }

    [JavaFlags(0)]
    public virtual Document docFor(Element element) => element.getOwnerDocument();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Helper helper = this;
      ObjectImpl.clone((object) helper);
      return ((object) helper).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
