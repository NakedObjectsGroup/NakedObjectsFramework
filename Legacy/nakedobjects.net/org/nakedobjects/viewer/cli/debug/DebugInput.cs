// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.debug.DebugInput
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;

namespace org.nakedobjects.viewer.cli.debug
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Input;")]
  public class DebugInput : Input
  {
    private readonly PrintStream printer;
    private readonly Input input;

    public DebugInput(PrintStream printer, Input input)
    {
      this.printer = printer;
      this.input = input;
    }

    public virtual void connect()
    {
      this.printer.println("** CONNECTED **");
      this.input.connect();
    }

    public virtual void disconnect()
    {
      this.printer.println("** DISCONNECTED **");
      this.input.disconnect();
    }

    [JavaThrownExceptions("1;org/nakedobjects/viewer/cli/ConnectionException;")]
    public virtual string accept()
    {
      string str = this.input.accept();
      this.printer.println(str);
      return str;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugInput debugInput = this;
      ObjectImpl.clone((object) debugInput);
      return ((object) debugInput).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
