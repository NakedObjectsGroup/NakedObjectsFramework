// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.CommandLineViewer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using java.util;
using org.nakedobjects.@event;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.controller;
using org.nakedobjects.viewer.cli.text;

namespace org.nakedobjects.viewer.cli
{
  public class CommandLineViewer : NakedObjectsViewer
  {
    private UserContext applicationContext;
    private CommandLineController controller;
    private ObjectViewingMechanismListener shutdownListener;
    private string title;

    public CommandLineViewer()
    {
      this.controller = new CommandLineController();
      this.controller.setView((View) new ConsoleView());
      this.controller.setInput((Input) new ConsoleInput());
    }

    public override void init()
    {
      if (this.applicationContext == null)
        throw new NullPointerException(new StringBuffer().append("No application context set for ").append((object) this).ToString());
      if (this.getView() == null)
        throw new NullPointerException(new StringBuffer().append("No View set for ").append((object) this).ToString());
      if (this.getInput() == null)
        throw new NullPointerException(new StringBuffer().append("No Input set for ").append((object) this).ToString());
      this.controller.setShutdownListener(this.shutdownListener);
      this.setResources(this.controller);
      this.controller.init();
    }

    public override void setApplication(UserContext applicationContext) => this.applicationContext = applicationContext;

    public override void setExploration(bool inExplorationMode)
    {
    }

    public override void setTitle(string title) => this.title = title;

    public override void setShutdownListener(ObjectViewingMechanismListener shutdownListener) => this.shutdownListener = shutdownListener;

    public virtual void setView(View view) => this.controller.setView(view);

    public virtual View getView() => this.controller.getView();

    public virtual void setInput(Input input) => this.controller.setInput(input);

    public virtual Input getInput() => this.controller.getInput();

    public override void shutdown()
    {
    }

    private void setResources(CommandLineController controller)
    {
      Vector classes1 = this.applicationContext.getClasses();
      int length = classes1.size() - 1;
      NakedClass[] classes2 = length >= 0 ? new NakedClass[length] : throw new NegativeArraySizeException();
      if (classes1 != null)
      {
        Enumeration enumeration = classes1.elements();
        for (int index = 0; index < classes2.Length && enumeration.hasMoreElements(); ++index)
          classes2[index] = (NakedClass) enumeration.nextElement();
      }
      controller.setResources(classes2);
    }
  }
}
