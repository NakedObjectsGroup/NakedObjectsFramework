// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.classes.NewInstanceDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.classes
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class NewInstanceDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      NakedObject @object = ((ClassAgent) context.getAgent()).newInstance();
      view.display(@object);
      context.addObject((Agent) new ObjectAgent(@object));
    }

    public virtual string getHelp() => "Creates a new instance of the current class";

    public virtual string getNames() => "new";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ClassAgent)));

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      NewInstanceDispatcher instanceDispatcher = this;
      ObjectImpl.clone((object) instanceDispatcher);
      return ((object) instanceDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
