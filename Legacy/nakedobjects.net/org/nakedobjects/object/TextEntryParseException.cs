// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.TextEntryParseException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.@object
{
  public class TextEntryParseException : NakedObjectApplicationException
  {
    public TextEntryParseException()
    {
    }

    public TextEntryParseException(string message)
      : base(message)
    {
    }

    public TextEntryParseException(Throwable cause)
      : base(cause)
    {
    }

    public TextEntryParseException(string message, Throwable cause)
      : base(message, cause)
    {
    }
  }
}
