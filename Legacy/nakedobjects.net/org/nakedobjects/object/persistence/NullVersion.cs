// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.NullVersion
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("1;org/nakedobjects/object/Version;")]
  public class NullVersion : Version
  {
    public virtual bool different(Version version) => false;

    public virtual Version next(string user, Date time) => throw new UnexpectedCallException();

    public virtual string getUser() => "";

    public virtual Date getTime() => new Date();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NullVersion nullVersion = this;
      ObjectImpl.clone((object) nullVersion);
      return ((object) nullVersion).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
