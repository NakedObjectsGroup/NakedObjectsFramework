// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.ValueParseException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;

namespace org.nakedobjects.application
{
  public class ValueParseException : ApplicationException
  {
    public ValueParseException(string message)
      : base(message)
    {
    }

    public ValueParseException(Throwable cause)
      : base("Could not parse", cause)
    {
    }

    public ValueParseException(string message, Throwable cause)
      : base(message, cause)
    {
    }
  }
}
