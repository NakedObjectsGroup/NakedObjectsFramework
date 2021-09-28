// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.security.Authenticator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.system.security
{
  [JavaInterface]
  public interface Authenticator
  {
    [JavaThrownExceptions("1;org/nakedobjects/system/security/AuthenticationException;")]
    void validate(User user);
  }
}
