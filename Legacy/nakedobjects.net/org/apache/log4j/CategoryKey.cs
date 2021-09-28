// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.CategoryKey
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.apache.log4j
{
  [JavaFlags(32)]
  public class CategoryKey
  {
    [JavaFlags(0)]
    public string name;
    [JavaFlags(0)]
    public int hashCache;

    [JavaFlags(0)]
    public CategoryKey(string name)
    {
      this.name = StringImpl.intern(name);
      this.hashCache = StringImpl.hashCode(name);
    }

    [JavaFlags(17)]
    public override sealed int GetHashCode() => this.hashCache;

    [JavaFlags(17)]
    public override sealed bool Equals(object rArg) => this == rArg || rArg != null && Class.FromType(typeof (CategoryKey)) == ObjectImpl.getClass(rArg) && (object) this.name == (object) ((CategoryKey) rArg).name;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      CategoryKey categoryKey = this;
      ObjectImpl.clone((object) categoryKey);
      return ((object) categoryKey).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
