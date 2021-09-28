// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.EntityDecl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.apache.crimson.parser
{
  [JavaFlags(32)]
  public class EntityDecl
  {
    [JavaFlags(0)]
    public string name;
    [JavaFlags(0)]
    public bool isFromInternalSubset;
    [JavaFlags(0)]
    public bool isPE;

    [JavaFlags(0)]
    public EntityDecl()
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      EntityDecl entityDecl = this;
      ObjectImpl.clone((object) entityDecl);
      return ((object) entityDecl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
