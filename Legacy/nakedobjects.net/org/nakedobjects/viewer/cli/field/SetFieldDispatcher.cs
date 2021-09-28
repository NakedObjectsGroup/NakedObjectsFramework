// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.SetFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.field
{
  public class SetFieldDispatcher : FieldDispatcher
  {
    [JavaFlags(4)]
    public override void execute(
      Command command,
      Context context,
      View view,
      NakedObjectField field,
      NakedObject @object,
      string name)
    {
      if (field.isCollection())
        view.error("Can't set a collection field");
      else
        this.setField(command, context, view, @object, field.getName(), field);
    }

    private void setField(
      Command command,
      Context context,
      View view,
      NakedObject @object,
      string name,
      NakedObjectField field)
    {
      SetFieldAgent setFieldAgent = new SetFieldAgent(name, @object, field);
      context.addSubject((Agent) setFieldAgent);
      if (command.getNumberOfParameters() <= 1)
        return;
      string parameter = command.getParameter(1);
      if (StringImpl.length(parameter) <= 0)
        return;
      setFieldAgent.setField(context, view, parameter);
    }

    public override string getHelp() => "Set the specified field, either using the specified value, or a reference to be subsequently found";

    public override string getNames() => "set s";

    public virtual bool isValid(Command command, Context context) => command.getNumberOfParameters() >= 1;
  }
}
