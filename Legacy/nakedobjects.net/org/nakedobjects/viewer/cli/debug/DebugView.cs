// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.debug.DebugView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.debug
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/View;")]
  public class DebugView : View
  {
    private readonly PrintStream printer;
    private readonly View view;

    public DebugView(PrintStream printer, View view)
    {
      this.printer = printer;
      this.view = view;
    }

    public virtual void clear()
    {
      this.printer.println("** CLEARED **");
      this.view.clear();
    }

    public virtual void connect()
    {
      this.printer.println("** CONNECTED **");
      this.view.connect();
    }

    public virtual void disconnect()
    {
      this.printer.println("** DISCONNECTED **");
      this.printer.close();
      this.view.disconnect();
    }

    public virtual void display(string message)
    {
      this.printer.println(message);
      this.view.display(message);
    }

    public virtual void displayEntry(string entry)
    {
      this.printer.println(entry);
      this.view.displayEntry(entry);
    }

    public virtual void prompt(string prompt)
    {
      this.printer.print(new StringBuffer().append(prompt).append("> ").ToString());
      this.view.prompt(prompt);
    }

    public virtual void display(NakedObject instance)
    {
      this.printer.println(instance != null ? instance.titleString() : "");
      this.view.display(instance);
    }

    public virtual void error(string message) => this.display(new StringBuffer().append("ERROR: ").append(message).ToString());

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugView debugView = this;
      ObjectImpl.clone((object) debugView);
      return ((object) debugView).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
