// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.ShowOptionsForFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ShowOptionsForFieldDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => ((SetFieldAgent) context.getAgent()).listOptions(view);

    public virtual string getHelp() => "Show the available options for the current parameter";

    public virtual string getNames() => "options opts";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (SetFieldAgent))) && ((SetFieldAgent) context.getAgent()).isLookup();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ShowOptionsForFieldDispatcher forFieldDispatcher = this;
      ObjectImpl.clone((object) forFieldDispatcher);
      return ((object) forFieldDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
