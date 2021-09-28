// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.LongNumberVersion
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.@object.persistence
{
  public class LongNumberVersion : AbstractVersion
  {
    private readonly long versionNumber;

    public LongNumberVersion(long number, string user, Date time)
      : base(user, time)
    {
      this.versionNumber = number;
    }

    public override bool different(Version version) => version is LongNumberVersion && this.versionNumber != ((LongNumberVersion) version).versionNumber;

    public virtual long getSequence() => this.versionNumber;

    [JavaFlags(4)]
    public override AbstractVersion next() => (AbstractVersion) new LongNumberVersion(this.versionNumber + 1L, (string) null, (Date) null);

    public override string ToString() => new StringBuffer().append("LongNumberVersion#").append(this.versionNumber).append(" ").append(org.nakedobjects.utility.ToString.timestamp(this.time)).ToString();
  }
}
