// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.controller.CommandLineController
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@event;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.cli.@object;
using org.nakedobjects.viewer.cli.action;
using org.nakedobjects.viewer.cli.classes;
using org.nakedobjects.viewer.cli.context;
using org.nakedobjects.viewer.cli.field;
using System;
using System.ComponentModel;

namespace org.nakedobjects.viewer.cli.controller
{
  [JavaInterfaces("1;java/lang/Runnable;")]
  public class CommandLineController : Runnable
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly bool debug;
    private ObjectViewingMechanismListener shutdownListener;
    private ContextManager manager;
    private Hashtable dispatchers;
    private Input input;
    private bool isConnected;
    private View view;

    private void addDispatcher(org.nakedobjects.viewer.cli.Dispatcher dispatcher)
    {
      StringTokenizer stringTokenizer = new StringTokenizer(dispatcher.getNames(), " ");
      while (stringTokenizer.hasMoreTokens())
      {
        object obj = stringTokenizer.nextElement();
        Vector vector = (Vector) this.dispatchers.get(obj);
        if (vector == null)
        {
          vector = new Vector();
          this.dispatchers.put(obj, (object) vector);
        }
        vector.addElement((object) dispatcher);
      }
    }

    private Command awaitInput()
    {
      this.view.prompt(this.getContext().getPrompt());
      string entry = this.input.accept();
      Command command = !this.getContext().isValueEntry() ? (Command) new QuotedCommand(entry) : (!StringImpl.startsWith(entry, ">") ? (Command) new ValueCommand(entry) : (Command) new QuotedCommand(StringImpl.substring(entry, 1)));
      this.view.displayEntry(entry);
      if (CommandLineController.LOG.isDebugEnabled())
        CommandLineController.LOG.debug((object) new StringBuffer().append("input ").append((object) command).ToString());
      return command;
    }

    private void dispatch(Command command)
    {
      Vector dispatchers = (Vector) this.dispatchers.get((object) StringImpl.toLowerCase(command.getName()));
      if (dispatchers != null)
        this.dispatch(command, dispatchers, this.view);
      else
        this.view.error(new StringBuffer().append("unknown command ").append(command.getName()).ToString());
    }

    public virtual void dispatch(Command command, Vector dispatchers, View view)
    {
      try
      {
        Context context = this.getContext();
        Enumeration enumeration = dispatchers.elements();
        while (enumeration.hasMoreElements())
        {
          org.nakedobjects.viewer.cli.Dispatcher dispatcher = (org.nakedobjects.viewer.cli.Dispatcher) enumeration.nextElement();
          if (dispatcher.isAvailable(context))
          {
            if (CommandLineController.LOG.isDebugEnabled())
              CommandLineController.LOG.debug((object) new StringBuffer().append("request handled by ").append((object) dispatcher).ToString());
            dispatcher.execute(command, context, view);
            return;
          }
        }
        view.error("can't invoke this command at this time");
      }
      catch (IllegalDispatchException ex)
      {
        view.error(((Throwable) ex).getMessage());
      }
    }

    private void dispatchers()
    {
      DebugString debugString = new DebugString();
      Enumeration enumeration1 = this.dispatchers.elements();
      Context context = this.getContext();
      while (enumeration1.hasMoreElements())
      {
        Enumeration enumeration2 = ((Vector) enumeration1.nextElement()).elements();
        while (enumeration2.hasMoreElements())
        {
          org.nakedobjects.viewer.cli.Dispatcher dispatcher = (org.nakedobjects.viewer.cli.Dispatcher) enumeration2.nextElement();
          string name = ObjectImpl.getClass((object) dispatcher).getName();
          string str = StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
          debugString.append(!dispatcher.isAvailable(context) ? (object) " " : (object) "*");
          debugString.append((object) str);
          debugString.append((object) " (");
          debugString.append((object) dispatcher.getNames());
          debugString.append((object) ") - ");
          debugString.appendln(dispatcher.getHelp());
        }
      }
      this.view.display(debugString.ToString());
    }

    public virtual View getView() => this.view;

    public virtual void init()
    {
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new AboutDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new CommandLineController.QuitDispatcher(this));
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new CommandHelpDispatcher(this.dispatchers));
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new OpenContextDispatcher(this.manager));
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new CloseContextDispatcher(this.manager));
      if (CommandLineController.debug)
        this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new DebugContext());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowPromptDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new DisplayAgentsDetails());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ClearOutputDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ClassesDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new InstancesDispather());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new NewInstanceDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowObjectHistoryDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new GotoObjectDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new PreviousHistoryEntryDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new NextHistoryEntryDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowObjectTypeDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowObjectDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new SetFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ClearFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new SelectReferenceToSetFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ValueToSetFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new CancelSetFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new AddToFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new SelectReferenceToAddToFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new RemoveFromFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new SelectReferenceToRemoveFromFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowOptionsForFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new SetOptionInFieldDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ObjectActionDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ClassActionListDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ClassActionSummaryDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ObjectActionListDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ObjectActionSummaryDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowOptionsForParameterDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new SetOptionAsParameterDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ExecuteActionDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new UseDefaultForParameterDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new CancelActionDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ClearParameterDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ReferenceParameterDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ValueParameterDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowParameterTypeDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new ShowParameterStateDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new NextParameterDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new PreviousParameterDispatcher());
      this.addDispatcher((org.nakedobjects.viewer.cli.Dispatcher) new SaveTransientObjectDispatcher());
      new Thread((Runnable) this).start();
    }

    private void quit()
    {
      if (CommandLineController.LOG.isDebugEnabled())
        CommandLineController.LOG.debug((object) "quitting");
      this.isConnected = false;
    }

    public virtual void run()
    {
      this.view.connect();
      this.isConnected = true;
      this.processRequests();
      this.view.disconnect();
      this.input.disconnect();
      if (this.shutdownListener == null)
        return;
      this.shutdownListener.viewerClosing();
    }

    private void processRequests()
    {
      while (this.isConnected)
      {
        try
        {
          Command command = this.awaitInput();
          if (StringImpl.equals(command.getName(), (object) "dispatchers"))
            this.dispatchers();
          else if (!StringImpl.equals(command.getName(), (object) ""))
            this.dispatch(command);
        }
        catch (Exception ex)
        {
          Throwable t = ThrowableWrapper.wrapThrowable(ex);
          this.view.error(new StringBuffer().append(t.getMessage()).append("\nsee log for trace").ToString());
          CommandLineController.LOG.error((object) "Failed command", t);
        }
      }
    }

    public virtual void setInput(Input input) => this.input = input;

    public virtual Input getInput() => this.input;

    public virtual void setResources(NakedClass[] classes) => this.manager = new ContextManager(new ClassesAgent(classes));

    public virtual void setView(View view) => this.view = view;

    private Context getContext() => this.manager.current();

    public virtual void shutdown() => this.isConnected = false;

    public virtual void setShutdownListener(ObjectViewingMechanismListener shutdownListener) => this.shutdownListener = shutdownListener;

    public CommandLineController() => this.dispatchers = new Hashtable();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static CommandLineController()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      CommandLineController commandLineController = this;
      ObjectImpl.clone((object) commandLineController);
      return ((object) commandLineController).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
    [JavaFlags(34)]
    [Inner]
    private class QuitDispatcher : org.nakedobjects.viewer.cli.Dispatcher
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private CommandLineController this\u00240;

      public virtual void execute(Command command, Context context, View view) => this.this\u00240.quit();

      public virtual string getHelp() => "Quit application";

      public virtual string getNames() => "quit";

      public virtual bool isAvailable(Context context) => true;

      [JavaFlags(2)]
      public QuitDispatcher(CommandLineController _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        CommandLineController.QuitDispatcher quitDispatcher = this;
        ObjectImpl.clone((object) quitDispatcher);
        return ((object) quitDispatcher).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
