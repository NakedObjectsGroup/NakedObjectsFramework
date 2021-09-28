// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.java.JavaReferenceData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.java
{
  [JavaInterfaces("1;org/nakedobjects/distribution/IdentityData;")]
  public class JavaReferenceData : IdentityData
  {
    private readonly Oid oid;
    private readonly string type;
    private readonly Version version;

    public JavaReferenceData(string type, Oid oid, Version version)
    {
      this.type = type;
      this.oid = oid;
      this.version = version;
    }

    public virtual Oid getOid() => this.oid;

    public virtual string getType() => this.type;

    public virtual Version getVersion() => this.version;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("type", this.type);
      toString.append("oid", (object) this.oid);
      toString.append("version", (object) this.version);
      return toString.ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaReferenceData javaReferenceData = this;
      ObjectImpl.clone((object) javaReferenceData);
      return ((object) javaReferenceData).MemberwiseClone();
    }
  }
}
