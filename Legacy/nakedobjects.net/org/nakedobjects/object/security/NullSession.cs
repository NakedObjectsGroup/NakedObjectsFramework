// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.security.NullSession
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.security;

namespace org.nakedobjects.@object.security
{
  [JavaInterfaces("1;org/nakedobjects/object/Session;")]
  public class NullSession : Session
  {
    public virtual string getUserName() => "no name";

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NullSession nullSession = this;
      ObjectImpl.clone((object) nullSession);
      return ((object) nullSession).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
