// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.ValueToSetFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ValueToSetFieldDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      SetFieldAgent agent = (SetFieldAgent) context.getAgent();
      string parameter = command.getParameter(0);
      agent.setValueField(context, view, parameter);
    }

    public virtual string getHelp() => "Sets the current object's field to the specified value";

    public virtual string getNames() => "value val v";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (SetFieldAgent)));

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ValueToSetFieldDispatcher setFieldDispatcher = this;
      ObjectImpl.clone((object) setFieldDispatcher);
      return ((object) setFieldDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
