// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.SelectReferenceToRemoveFromFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class SelectReferenceToRemoveFromFieldDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      RemoveFromFieldAgent agent = (RemoveFromFieldAgent) context.getAgent();
      NakedObject associate = (command.getNumberOfParameters() != 0 ? (ObjectAgent) context.findAgent(command.getParameterAsLowerCase(0)) : (ObjectAgent) context.getHistory().last()).getObject();
      agent.removeReferenceFromCollection(context, view, associate);
    }

    public virtual string getHelp() => "Sets the current object to be added to the collection field previously specified";

    public virtual string getNames() => "use u";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (RemoveFromFieldAgent)));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SelectReferenceToRemoveFromFieldDispatcher fromFieldDispatcher = this;
      ObjectImpl.clone((object) fromFieldDispatcher);
      return ((object) fromFieldDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
