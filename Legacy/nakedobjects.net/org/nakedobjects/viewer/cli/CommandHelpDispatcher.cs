// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.CommandHelpDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class CommandHelpDispatcher : Dispatcher
  {
    private readonly Hashtable dispatchers;

    public CommandHelpDispatcher(Hashtable dispatchers) => this.dispatchers = dispatchers;

    public virtual void execute(Command command, Context context, View view)
    {
      if (command.hasParameters())
        this.showHelpForCommand(command, view, context);
      else
        this.showCommandsList(view, context);
    }

    public virtual string getHelp() => "Help about a command";

    public virtual string getNames() => "command com";

    public virtual bool isAvailable(Context context) => true;

    private void showCommandsList(View view, Context context)
    {
      Help help = new Help(true);
      Enumeration enumeration1 = this.dispatchers.elements();
      while (enumeration1.hasMoreElements())
      {
        Enumeration enumeration2 = ((Vector) enumeration1.nextElement()).elements();
        while (enumeration2.hasMoreElements())
        {
          Dispatcher dispatcher = (Dispatcher) enumeration2.nextElement();
          if (dispatcher.isAvailable(context))
          {
            string names = dispatcher.getNames();
            int num = StringImpl.indexOf(names, 32);
            help.append(num <= 0 ? names : StringImpl.substring(names, 0, num));
          }
        }
      }
      view.display(help.getText());
    }

    private void showHelpForCommand(Command command, View view, Context context)
    {
      Help help = new Help(false);
      for (int index = 0; index < command.getNumberOfParameters(); ++index)
      {
        string parameterAsLowerCase = command.getParameterAsLowerCase(index);
        Vector vector = (Vector) this.dispatchers.get((object) parameterAsLowerCase);
        if (vector == null)
        {
          help.append(new StringBuffer().append("No command ").append(parameterAsLowerCase).ToString());
        }
        else
        {
          Enumeration enumeration = vector.elements();
          while (enumeration.hasMoreElements())
          {
            Dispatcher dispatcher = (Dispatcher) enumeration.nextElement();
            if (dispatcher.isAvailable(context))
            {
              string names = dispatcher.getNames();
              int num = StringImpl.indexOf(names, 32);
              help.append(num <= 0 ? names : StringImpl.substring(names, 0, num), num <= 0 ? "" : StringImpl.substring(names, num + 1), dispatcher.getHelp());
            }
          }
        }
      }
      view.display(help.getText());
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      CommandHelpDispatcher commandHelpDispatcher = this;
      ObjectImpl.clone((object) commandHelpDispatcher);
      return ((object) commandHelpDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
