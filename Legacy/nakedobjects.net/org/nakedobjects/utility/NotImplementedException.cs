// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.NotImplementedException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

namespace org.nakedobjects.utility
{
  public class NotImplementedException : NakedObjectRuntimeException
  {
    public NotImplementedException()
      : base("This method is not implemented yet")
    {
    }

    public NotImplementedException(string arg0)
      : base(arg0)
    {
    }
  }
}
