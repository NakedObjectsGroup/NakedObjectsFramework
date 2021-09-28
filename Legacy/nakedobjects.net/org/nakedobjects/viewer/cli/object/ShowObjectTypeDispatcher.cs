// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.ShowObjectTypeDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.@object
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ShowObjectTypeDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      NakedObject nakedObject = ((ObjectAgent) context.getAgent()).getObject();
      view.display(new StringBuffer().append("A ").append(nakedObject.getSpecification().getShortName()).ToString());
    }

    public virtual string getHelp() => "Show the type of the current object";

    public virtual string getNames() => "type";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ObjectAgent)));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ShowObjectTypeDispatcher objectTypeDispatcher = this;
      ObjectImpl.clone((object) objectTypeDispatcher);
      return ((object) objectTypeDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
