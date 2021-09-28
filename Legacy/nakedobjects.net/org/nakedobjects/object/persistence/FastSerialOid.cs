// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.FastSerialOid
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("1;org/nakedobjects/object/Oid;")]
  public class FastSerialOid : Oid
  {
    private readonly string asString;
    private readonly int hashCode;
    private readonly long serialNo;
    private bool isNull;

    public FastSerialOid(long serialNo)
    {
      this.serialNo = serialNo;
      this.asString = new StringBuffer().append("OID#").append(StringImpl.toUpperCase(Long.toHexString(serialNo))).ToString();
      this.hashCode = 629 + (int) (serialNo ^ (long) ((ulong) serialNo >> 32));
    }

    public virtual bool equals(FastSerialOid otherOid) => otherOid == this || otherOid.serialNo == this.serialNo;

    public override bool Equals(object obj) => obj == this || obj is FastSerialOid && ((FastSerialOid) obj).serialNo == this.serialNo;

    public virtual long getSerialNo() => this.serialNo;

    public override int GetHashCode() => this.hashCode;

    public override string ToString() => this.asString;

    public virtual bool hasPrevious() => false;

    public virtual Oid getPrevious() => (Oid) null;

    public virtual void copyFrom(Oid oid)
    {
    }

    public virtual bool isNull() => this.isNull;

    public virtual void setNull(bool isNull) => this.isNull = isNull;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      FastSerialOid fastSerialOid = this;
      ObjectImpl.clone((object) fastSerialOid);
      return ((object) fastSerialOid).MemberwiseClone();
    }
  }
}
