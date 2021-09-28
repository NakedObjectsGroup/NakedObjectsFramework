// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.AttributeDecl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.apache.crimson.parser
{
  [JavaFlags(32)]
  public class AttributeDecl
  {
    [JavaFlags(0)]
    public string name;
    [JavaFlags(0)]
    public string type;
    [JavaFlags(0)]
    public string[] values;
    [JavaFlags(0)]
    public string defaultValue;
    [JavaFlags(0)]
    public bool isRequired;
    [JavaFlags(0)]
    public bool isFixed;
    [JavaFlags(0)]
    public bool isFromInternalSubset;
    [JavaFlags(24)]
    public const string CDATA = "CDATA";
    [JavaFlags(24)]
    public const string ID = "ID";
    [JavaFlags(24)]
    public const string IDREF = "IDREF";
    [JavaFlags(24)]
    public const string IDREFS = "IDREFS";
    [JavaFlags(24)]
    public const string ENTITY = "ENTITY";
    [JavaFlags(24)]
    public const string ENTITIES = "ENTITIES";
    [JavaFlags(24)]
    public const string NMTOKEN = "NMTOKEN";
    [JavaFlags(24)]
    public const string NMTOKENS = "NMTOKENS";
    [JavaFlags(24)]
    public const string NOTATION = "NOTATION";
    [JavaFlags(24)]
    public const string ENUMERATION = "ENUMERATION";
    [JavaFlags(0)]
    public string valueDefault;
    [JavaFlags(24)]
    public const string IMPLIED = "#IMPLIED";
    [JavaFlags(24)]
    public const string REQUIRED = "#REQUIRED";
    [JavaFlags(24)]
    public const string FIXED = "#FIXED";

    [JavaFlags(0)]
    public AttributeDecl(string s) => this.name = s;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AttributeDecl attributeDecl = this;
      ObjectImpl.clone((object) attributeDecl);
      return ((object) attributeDecl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
