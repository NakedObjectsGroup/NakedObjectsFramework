// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.awt.TextAreaView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.awt;
using java.lang;

namespace org.nakedobjects.viewer.cli.awt
{
  public class TextAreaView : AbstractTextView
  {
    private readonly ConsoleWindow consoleFrame;

    public TextAreaView(ConsoleWindow consoleFrame) => this.consoleFrame = consoleFrame;

    public override void displayEntry(string entry) => this.appendln(entry);

    [JavaFlags(4)]
    public override void appendln(string text)
    {
      this.consoleFrame.append(text);
      this.consoleFrame.append("\n");
    }

    public override void clear() => this.consoleFrame.clear();

    public override void disconnect()
    {
      ((Window) this.consoleFrame).hide();
      this.consoleFrame.dispose();
    }

    public override void prompt(string prompt) => this.consoleFrame.append(new StringBuffer().append(prompt).append("> ").ToString());
  }
}
