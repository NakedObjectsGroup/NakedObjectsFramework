// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.ClearOutputDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ClearOutputDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => view.clear();

    public virtual string getHelp() => "Clears the console";

    public virtual string getNames() => "cls";

    public virtual bool isAvailable(Context context) => true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ClearOutputDispatcher outputDispatcher = this;
      ObjectImpl.clone((object) outputDispatcher);
      return ((object) outputDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
