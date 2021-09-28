// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.SaveTransientObjectDispatcher
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
  public class SaveTransientObjectDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      ObjectAgent agent = (ObjectAgent) context.getAgent();
      Action objectAction = agent.getObject().getSpecification().getObjectAction(Action.USER, "save");
      NakedObjects.getObjectPersistor().startTransaction();
      if (objectAction != null)
      {
        Action action = objectAction;
        NakedObject nakedObject = agent.getObject();
        int length = 0;
        Naked[] parameters = length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
        action.execute((NakedReference) nakedObject, parameters);
      }
      else
        NakedObjects.getObjectPersistor().makePersistent(agent.getObject());
      NakedObjects.getObjectPersistor().endTransaction();
    }

    public virtual string getHelp() => "Save the current object";

    public virtual string getNames() => "save s";

    public virtual bool isAvailable(Context context)
    {
      if (context.currentAgentIs(Class.FromType(typeof (ObjectAgent))))
      {
        ObjectAgent agent = (ObjectAgent) context.getAgent();
        if (agent != null)
        {
          NakedObject nakedObject1 = agent.getObject();
          if (!nakedObject1.getResolveState().isTransient() || nakedObject1.persistable() != Persistable.USER_PERSISTABLE)
            return false;
          Action objectAction = agent.getObject().getSpecification().getObjectAction(Action.USER, "save");
          if (objectAction == null)
            return true;
          Action action = objectAction;
          NakedObject nakedObject2 = nakedObject1;
          int length = 0;
          Naked[] parameters = length >= 0 ? new Naked[length] : throw new NegativeArraySizeException();
          return action.isParameterSetValid((NakedReference) nakedObject2, parameters).isAllowed();
        }
      }
      return false;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SaveTransientObjectDispatcher objectDispatcher = this;
      ObjectImpl.clone((object) objectDispatcher);
      return ((object) objectDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
