// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.AbstractVersion
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("1;org/nakedobjects/object/Version;")]
  public abstract class AbstractVersion : Version
  {
    [JavaFlags(4)]
    public string user;
    [JavaFlags(4)]
    public Date time;

    public AbstractVersion(string user, Date time)
    {
      this.user = user;
      this.time = time;
    }

    public virtual Date getTime() => this.time;

    public virtual string getUser() => this.user;

    public virtual Version next(string user, Date time)
    {
      AbstractVersion abstractVersion = this.next();
      abstractVersion.user = user;
      abstractVersion.time = time;
      return (Version) abstractVersion;
    }

    [JavaFlags(1028)]
    public abstract AbstractVersion next();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractVersion abstractVersion = this;
      ObjectImpl.clone((object) abstractVersion);
      return ((object) abstractVersion).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract bool different(Version version);
  }
}
