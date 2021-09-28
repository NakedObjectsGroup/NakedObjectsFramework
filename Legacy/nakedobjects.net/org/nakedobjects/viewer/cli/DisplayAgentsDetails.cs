// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.DisplayAgentsDetails
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class DisplayAgentsDetails : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      if (command.hasParameters())
      {
        Agent agent = context.findAgent(command.getParameter(0));
        int num = command.getNumberOfParameters() - 1;
        int length = num;
        string[] layout = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
        for (int index = 0; index < num; ++index)
          layout[index] = command.getParameterAsLowerCase(index + 1);
        agent.list(view, layout);
      }
      else
        context.getAgent().list(view, (string[]) null);
    }

    public virtual string getHelp() => "Display the details of the current context";

    public virtual string getNames() => "list l";

    public virtual bool isAvailable(Context context) => true;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DisplayAgentsDetails displayAgentsDetails = this;
      ObjectImpl.clone((object) displayAgentsDetails);
      return ((object) displayAgentsDetails).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
