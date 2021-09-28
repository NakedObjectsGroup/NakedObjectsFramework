// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.security.SimplePasswordAuthenticator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.system.security
{
  [JavaInterfaces("1;org/nakedobjects/system/security/PasswordAuthenticator;")]
  public class SimplePasswordAuthenticator : PasswordAuthenticator
  {
    private string password;

    public virtual void setPassword(string password) => this.password = password;

    [JavaThrownExceptions("1;org/nakedobjects/system/security/AuthenticationException;")]
    public virtual void validate(User user)
    {
    }

    public virtual string getPassword() => this.password;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SimplePasswordAuthenticator passwordAuthenticator = this;
      ObjectImpl.clone((object) passwordAuthenticator);
      return ((object) passwordAuthenticator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
