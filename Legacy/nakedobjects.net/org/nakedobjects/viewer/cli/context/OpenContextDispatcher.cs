// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.context.OpenContextDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.cli.context
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class OpenContextDispatcher : Dispatcher
  {
    private ContextManager manager;

    public OpenContextDispatcher(ContextManager manager) => this.manager = manager;

    public virtual void execute(Command command, Context context, View view)
    {
      if (!command.hasParameters())
        this.manager.newContext();
      else
        this.manager.newContext(context.findAgent(command.getParameter(0)));
      view.display("New context");
    }

    public virtual string getHelp() => "Open a new context work within";

    public virtual string getNames() => "open";

    public virtual bool isAvailable(Context context) => true;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      OpenContextDispatcher contextDispatcher = this;
      ObjectImpl.clone((object) contextDispatcher);
      return ((object) contextDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
