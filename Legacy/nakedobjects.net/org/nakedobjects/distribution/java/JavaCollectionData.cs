// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.java.JavaCollectionData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.java
{
  [JavaInterfaces("1;org/nakedobjects/distribution/CollectionData;")]
  public class JavaCollectionData : CollectionData
  {
    private readonly ReferenceData[] elements;
    private readonly Oid oid;
    private readonly string type;
    private readonly Version version;
    private readonly bool hasAllElements;

    public JavaCollectionData(
      Oid oid,
      string type,
      ReferenceData[] elements,
      bool hasAllElements,
      Version version)
    {
      this.oid = oid;
      this.type = type;
      this.elements = elements;
      this.hasAllElements = hasAllElements;
      this.version = version;
    }

    public virtual ReferenceData[] getElements() => this.elements;

    public virtual Oid getOid() => this.oid;

    public virtual string getType() => this.type;

    public virtual Version getVersion() => this.version;

    public virtual bool hasAllElements() => this.hasAllElements;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("type", this.type);
      toString.append("oid", (object) this.oid);
      toString.append("version", (object) this.version);
      toString.append(",elements=");
      for (int index = 0; this.elements != null && index < this.elements.Length; ++index)
      {
        if (index > 0)
          toString.append(";");
        if (this.elements[index] == null)
        {
          toString.append("null");
        }
        else
        {
          string name = ObjectImpl.getClass((object) this.elements[index]).getName();
          toString.append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1));
        }
      }
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      JavaCollectionData javaCollectionData = this;
      ObjectImpl.clone((object) javaCollectionData);
      return ((object) javaCollectionData).MemberwiseClone();
    }
  }
}
