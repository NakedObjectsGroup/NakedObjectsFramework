// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.text.ConsoleInput
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace org.nakedobjects.viewer.cli.text
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Input;")]
  public class ConsoleInput : Input
  {
    public virtual void connect()
    {
    }

    public virtual string accept()
    {
      StringBuffer stringBuffer = new StringBuffer();
      while (true)
      {
        int num;
        try
        {
          num = ((InputStream) System.@in).read();
        }
        catch (IOException ex)
        {
          throw new ConnectionException((Throwable) ex);
        }
        if (num != 10)
          stringBuffer.append((char) num);
        else
          break;
      }
      return StringImpl.trim(stringBuffer.ToString());
    }

    public virtual void disconnect()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ConsoleInput consoleInput = this;
      ObjectImpl.clone((object) consoleInput);
      return ((object) consoleInput).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
