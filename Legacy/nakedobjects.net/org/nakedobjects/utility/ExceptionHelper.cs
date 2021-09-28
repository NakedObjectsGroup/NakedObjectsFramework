// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.ExceptionHelper
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace org.nakedobjects.utility
{
  public class ExceptionHelper
  {
    public static string exceptionTraceAsString(Throwable exception)
    {
      ByteArrayOutputStream arrayOutputStream = (ByteArrayOutputStream) null;
      try
      {
        arrayOutputStream = new ByteArrayOutputStream();
        exception.printStackTrace(new PrintStream((OutputStream) arrayOutputStream));
        return arrayOutputStream.ToString();
      }
      finally
      {
        if (arrayOutputStream != null)
        {
          try
          {
            ((OutputStream) arrayOutputStream).close();
          }
          catch (IOException ex)
          {
          }
        }
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ExceptionHelper exceptionHelper = this;
      ObjectImpl.clone((object) exceptionHelper);
      return ((object) exceptionHelper).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
