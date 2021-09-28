// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.awt.TextFieldInput
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.cli.awt
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Input;")]
  public class TextFieldInput : Input
  {
    private readonly Entry entry;

    public TextFieldInput(Entry entry) => this.entry = entry;

    public virtual void connect()
    {
    }

    public virtual void disconnect()
    {
    }

    [JavaThrownExceptions("1;org/nakedobjects/viewer/cli/ConnectionException;")]
    public virtual string accept() => this.entry.get();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TextFieldInput textFieldInput = this;
      ObjectImpl.clone((object) textFieldInput);
      return ((object) textFieldInput).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
