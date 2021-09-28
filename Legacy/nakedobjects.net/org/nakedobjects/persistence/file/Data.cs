// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.Data
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.persistence.file
{
  public abstract class Data
  {
    private readonly string type;
    private readonly SerialOid oid;

    [JavaFlags(0)]
    public Data(NakedObjectSpecification type, SerialOid oid)
    {
      this.type = type.getFullName();
      this.oid = oid;
    }

    public virtual SerialOid getOid() => this.oid;

    public virtual string getClassName() => this.type;

    public override bool Equals(object obj) => obj == this || obj is Data && StringImpl.equals(((Data) obj).type, (object) this.type) && ((Data) obj).oid.Equals((object) this.oid);

    public override int GetHashCode() => 37 * (37 * 17 + StringImpl.hashCode(this.type)) + this.oid.GetHashCode();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Data data = this;
      ObjectImpl.clone((object) data);
      return ((object) data).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
