// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.persistence.file.PersistorException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.persistence.file
{
  public class PersistorException : NakedObjectRuntimeException
  {
    public PersistorException()
    {
    }

    public PersistorException(string message)
      : base(message)
    {
    }

    public PersistorException(string message, Throwable cause)
      : base(message, cause)
    {
    }

    public PersistorException(Throwable cause)
      : base(cause)
    {
    }
  }
}
