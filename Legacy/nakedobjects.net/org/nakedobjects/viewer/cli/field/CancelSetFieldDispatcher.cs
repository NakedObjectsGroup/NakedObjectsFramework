// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.CancelSetFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class CancelSetFieldDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => context.removeAgent();

    public virtual string getHelp() => "Cancel the current field setting";

    public virtual string getNames() => "cancel";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (AbstractFieldAgent)));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      CancelSetFieldDispatcher setFieldDispatcher = this;
      ObjectImpl.clone((object) setFieldDispatcher);
      return ((object) setFieldDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
