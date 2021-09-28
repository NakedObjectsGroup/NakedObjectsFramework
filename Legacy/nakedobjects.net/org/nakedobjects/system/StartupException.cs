// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.StartupException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.system
{
  public class StartupException : NakedObjectRuntimeException
  {
    public StartupException()
    {
    }

    public StartupException(string s)
      : base(s)
    {
    }

    public StartupException(Throwable cause)
      : base(cause)
    {
    }

    public StartupException(string msg, Throwable cause)
      : base(msg, cause)
    {
    }
  }
}
