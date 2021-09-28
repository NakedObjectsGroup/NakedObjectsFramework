// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.ShowObjectDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.viewer.cli.@object;
using org.nakedobjects.viewer.cli.action;
using org.nakedobjects.viewer.cli.field;

namespace org.nakedobjects.viewer.cli.@object
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ShowObjectDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => context.getHistory().getCurrent()?.list(view, (string[]) null);

    public virtual string getHelp() => "Show the object being acted upon (ie running an action or setting a field).";

    public virtual string getNames() => "target tar";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ActionAgent))) || context.currentAgentIs(Class.FromType(typeof (SetFieldAgent)));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ShowObjectDispatcher objectDispatcher = this;
      ObjectImpl.clone((object) objectDispatcher);
      return ((object) objectDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
