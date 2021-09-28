// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.security.User
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object.defaults;

namespace org.nakedobjects.system.security
{
  public class User
  {
    public User()
    {
    }

    public User(string @string)
    {
    }

    public virtual ApplicationContext getRootObject() => (ApplicationContext) null;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      User user = this;
      ObjectImpl.clone((object) user);
      return ((object) user).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
