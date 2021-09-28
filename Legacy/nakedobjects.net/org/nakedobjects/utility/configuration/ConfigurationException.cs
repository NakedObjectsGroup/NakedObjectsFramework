// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.configuration.ConfigurationException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;

namespace org.nakedobjects.utility.configuration
{
  public class ConfigurationException : NakedObjectRuntimeException
  {
    public ConfigurationException()
    {
    }

    public ConfigurationException(string s)
      : base(s)
    {
    }

    public ConfigurationException(string msg, Throwable cause)
      : base(msg, cause)
    {
    }

    public ConfigurationException(Throwable cause)
      : base(cause)
    {
    }
  }
}
