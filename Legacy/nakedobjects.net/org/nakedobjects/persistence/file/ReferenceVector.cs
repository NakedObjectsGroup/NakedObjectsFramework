// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.ReferenceVector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.persistence.file
{
  public class ReferenceVector
  {
    private readonly SerialOid oid;
    private readonly Vector elements;

    public ReferenceVector(SerialOid oid)
    {
      this.elements = new Vector();
      this.oid = oid;
    }

    public virtual void add(SerialOid oid) => this.elements.addElement((object) oid);

    public virtual void remove(SerialOid oid) => this.elements.removeElement((object) oid);

    public virtual SerialOid getOid() => this.oid;

    public virtual int size() => this.elements.size();

    public virtual SerialOid elementAt(int index) => (SerialOid) this.elements.elementAt(index);

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;
      return obj is ReferenceVector && ((ReferenceVector) obj).elements.Equals((object) this.elements);
    }

    public override int GetHashCode() => 37 * 17 + this.elements.GetHashCode();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("oid", (object) this.oid);
      toString.append("refs", (object) this.elements);
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ReferenceVector referenceVector = this;
      ObjectImpl.clone((object) referenceVector);
      return ((object) referenceVector).MemberwiseClone();
    }
  }
}
