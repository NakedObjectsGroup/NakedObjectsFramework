// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.CollectionData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.persistence.file
{
  public class CollectionData : Data
  {
    private ReferenceVector elements;

    public CollectionData(NakedObjectSpecification type, SerialOid oid)
      : base(type, oid)
    {
      this.elements = new ReferenceVector(oid);
    }

    public virtual void addElement(SerialOid elementOid) => this.elements.add(elementOid);

    public virtual void removeElement(SerialOid elementOid) => this.elements.remove(elementOid);

    public virtual ReferenceVector references() => this.elements;

    public override string ToString() => new StringBuffer().append("CollectionData[type=").append(this.getClassName()).append(",elements=").append((object) this.elements).append("]").ToString();
  }
}
