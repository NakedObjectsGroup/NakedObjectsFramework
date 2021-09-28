// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.controller.ValueCommand
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.cli.controller
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Command;")]
  public class ValueCommand : Command
  {
    private readonly string entry;

    public ValueCommand(string entry) => this.entry = entry;

    public virtual string getName() => "value";

    public virtual int getNumberOfParameters() => 1;

    public virtual string getParameter(int index) => this.entry;

    public virtual string getParameterAsLowerCase(int index) => throw new NotImplementedException();

    public virtual bool hasParameters() => true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ValueCommand valueCommand = this;
      ObjectImpl.clone((object) valueCommand);
      return ((object) valueCommand).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
