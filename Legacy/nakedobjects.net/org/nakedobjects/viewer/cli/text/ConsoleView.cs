// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.text.ConsoleView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;

namespace org.nakedobjects.viewer.cli.text
{
  public class ConsoleView : AbstractTextView
  {
    [JavaFlags(4)]
    public override void appendln(string text)
    {
      ((PrintStream) System.@out).println(text);
      ((PrintStream) System.@out).flush();
    }

    public override void clear()
    {
    }

    public override void prompt(string prompt) => ((PrintStream) System.@out).print(new StringBuffer().append(prompt).append("> ").ToString());
  }
}
