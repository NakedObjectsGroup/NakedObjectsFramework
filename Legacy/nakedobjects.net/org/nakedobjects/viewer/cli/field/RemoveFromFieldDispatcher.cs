// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.RemoveFromFieldDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.field
{
  public class RemoveFromFieldDispatcher : FieldDispatcher
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
      if (field.isValue())
        view.error("Can't add to a value field");
      else if (field.isObject() || field.isValue())
      {
        view.error("Can't add to a reference field");
      }
      else
      {
        RemoveFromFieldAgent removeFromFieldAgent = new RemoveFromFieldAgent(name, @object, field);
        context.addSubject((Agent) removeFromFieldAgent);
      }
    }

    public override string getHelp() => "Remove an element from the collection in the specified field with a reference to be subsequently found";

    public override string getNames() => "remove rem r";
  }
}
