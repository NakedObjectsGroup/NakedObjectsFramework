// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.DebugContext
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class DebugContext : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      DebugString debug = new DebugString();
      context.debug(debug);
      view.display(debug.ToString());
    }

    public virtual string getHelp() => "Show the context stack in debug mode";

    public virtual string getNames() => "debug";

    public virtual bool isAvailable(Context context) => true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugContext debugContext = this;
      ObjectImpl.clone((object) debugContext);
      return ((object) debugContext).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
