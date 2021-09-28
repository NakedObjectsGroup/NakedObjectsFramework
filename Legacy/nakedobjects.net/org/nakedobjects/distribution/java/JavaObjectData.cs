// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.java.JavaObjectData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.java
{
  [JavaInterfaces("1;org/nakedobjects/distribution/ObjectData;")]
  public class JavaObjectData : ObjectData
  {
    private Data[] fieldContent;
    private readonly Oid oid;
    private readonly bool resolved;
    private readonly string type;
    private readonly Version version;

    public JavaObjectData(Oid oid, string type, bool resolved, Version version)
    {
      this.oid = oid;
      this.type = type;
      this.resolved = resolved;
      this.version = version;
    }

    public virtual Data[] getFieldContent() => this.fieldContent;

    public virtual Oid getOid() => this.oid;

    public virtual string getType() => this.type;

    public virtual Version getVersion() => this.version;

    public virtual bool hasCompleteData() => this.resolved;

    public virtual void setFieldContent(Data[] fieldContent) => this.fieldContent = fieldContent;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("type", this.type);
      toString.append("oid", (object) this.oid);
      toString.append("version", (object) this.version);
      toString.append(",fields=");
      for (int index = 0; this.fieldContent != null && index < this.fieldContent.Length; ++index)
      {
        if (index > 0)
          toString.append(";");
        if (this.fieldContent[index] == null)
        {
          toString.append("null");
        }
        else
        {
          string name = ObjectImpl.getClass((object) this.fieldContent[index]).getName();
          toString.append(StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1));
        }
      }
      return toString.ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaObjectData javaObjectData = this;
      ObjectImpl.clone((object) javaObjectData);
      return ((object) javaObjectData).MemberwiseClone();
    }
  }
}
