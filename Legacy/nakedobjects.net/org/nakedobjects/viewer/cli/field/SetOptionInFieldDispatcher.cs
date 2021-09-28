// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.SetOptionInFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class SetOptionInFieldDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      if (!command.hasParameters())
        throw new IllegalDispatchException("No title specified");
      ((SetFieldAgent) context.getAgent()).setOption(context, view, command.getParameter(0));
    }

    public virtual string getHelp() => "Set the option with the specified title as the current parameter";

    public virtual string getNames() => "option opt o";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (SetFieldAgent))) && ((SetFieldAgent) context.getAgent()).isLookup();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SetOptionInFieldDispatcher inFieldDispatcher = this;
      ObjectImpl.clone((object) inFieldDispatcher);
      return ((object) inFieldDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
