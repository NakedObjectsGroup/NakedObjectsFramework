// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.context.CloseContextDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.viewer.cli.action;
using org.nakedobjects.viewer.cli.field;

namespace org.nakedobjects.viewer.cli.context
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class CloseContextDispatcher : Dispatcher
  {
    private ContextManager manager;

    public CloseContextDispatcher(ContextManager manager) => this.manager = manager;

    public virtual void execute(Command command, Context context, View view)
    {
      context.closeContext();
      this.manager.removeContext();
      view.display("Context closed");
    }

    public virtual string getHelp() => "Open the current context";

    public virtual string getNames() => "close x";

    public virtual bool isAvailable(Context context) => this.manager.canRemoveContext() && !context.currentAgentIs(Class.FromType(typeof (ActionAgent))) && !context.currentAgentIs(Class.FromType(typeof (AbstractFieldAgent)));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      CloseContextDispatcher contextDispatcher = this;
      ObjectImpl.clone((object) contextDispatcher);
      return ((object) contextDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
