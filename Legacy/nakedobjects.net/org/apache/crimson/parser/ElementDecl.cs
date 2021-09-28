// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.ElementDecl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.apache.crimson.parser
{
  [JavaFlags(32)]
  public class ElementDecl
  {
    [JavaFlags(0)]
    public string name;
    [JavaFlags(0)]
    public string id;
    [JavaFlags(0)]
    public string contentType;
    [JavaFlags(0)]
    public ElementValidator validator;
    [JavaFlags(0)]
    public ContentModel model;
    [JavaFlags(0)]
    public bool ignoreWhitespace;
    [JavaFlags(0)]
    public bool isFromInternalSubset;
    [JavaFlags(0)]
    public SimpleHashtable attributes;

    [JavaFlags(0)]
    public ElementDecl(string s)
    {
      this.attributes = new SimpleHashtable();
      this.name = s;
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ElementDecl elementDecl = this;
      ObjectImpl.clone((object) elementDecl);
      return ((object) elementDecl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
