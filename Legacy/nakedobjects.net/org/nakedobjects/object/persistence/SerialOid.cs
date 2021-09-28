// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.SerialOid
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("2;org/nakedobjects/object/Oid;java/io/Serializable;")]
  public class SerialOid : Oid, Serializable
  {
    private long serialNo;
    private SerialOid previous;
    private bool isNull;

    public SerialOid(long serialNo) => this.serialNo = serialNo;

    public override bool Equals(object obj) => obj == this || obj is SerialOid && ((SerialOid) obj).serialNo == this.serialNo;

    public virtual long getSerialNo() => this.serialNo;

    public override int GetHashCode() => 629 + (int) (this.serialNo ^ (long) ((ulong) this.serialNo >> 32));

    public override string ToString() => new StringBuffer().append("OID#").append(StringImpl.toUpperCase(Long.toHexString(this.serialNo))).ToString();

    public virtual bool hasPrevious() => this.previous != null;

    public virtual Oid getPrevious() => (Oid) this.previous;

    public virtual void copyFrom(Oid oid)
    {
      Assert.assertTrue(oid is SerialOid);
      this.serialNo = ((SerialOid) oid).serialNo;
    }

    public virtual void setPrevious(SerialOid previous)
    {
      Assert.assertNull((object) previous);
      this.previous = previous;
    }

    public virtual bool isNull() => this.isNull;

    public virtual void setNull(bool isNull) => this.isNull = isNull;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SerialOid serialOid = this;
      ObjectImpl.clone((object) serialOid);
      return ((object) serialOid).MemberwiseClone();
    }
  }
}
