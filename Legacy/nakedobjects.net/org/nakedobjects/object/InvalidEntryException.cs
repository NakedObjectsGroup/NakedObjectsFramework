// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.InvalidEntryException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object
{
  public class InvalidEntryException : NakedObjectRuntimeException
  {
    public InvalidEntryException()
      : this("Invalid value")
    {
    }

    public InvalidEntryException(string message)
      : base(message)
    {
    }

    public InvalidEntryException(Throwable cause)
      : this("Invalid value", cause)
    {
    }

    public InvalidEntryException(string message, Throwable cause)
      : base(message, cause)
    {
    }
  }
}
