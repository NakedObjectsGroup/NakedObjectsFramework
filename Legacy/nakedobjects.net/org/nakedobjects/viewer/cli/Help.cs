// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.Help
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;

namespace org.nakedobjects.viewer.cli
{
  public class Help
  {
    private readonly Vector help;
    private readonly bool isOnOneLine;

    public Help(bool isOnOneLine)
    {
      this.help = new Vector();
      this.isOnOneLine = isOnOneLine;
    }

    public virtual string getText()
    {
      StringBuffer stringBuffer = new StringBuffer();
      for (int index = 0; index < this.help.size(); ++index)
      {
        if (index > 0)
          stringBuffer.append(!this.isOnOneLine ? "\n" : ", ");
        stringBuffer.append(this.help.elementAt(index));
      }
      return stringBuffer.ToString();
    }

    public virtual void append(string text)
    {
      int num;
      for (num = 0; num < this.help.size(); ++num)
      {
        string str = this.help.elementAt(num).ToString();
        if (StringImpl.compareTo(text, str) == 0)
          return;
        if (StringImpl.compareTo(text, str) < 0)
          break;
      }
      this.help.insertElementAt((object) text, num);
    }

    public virtual void append(string name, string alternatives, string help) => this.append(new StringBuffer().append(name).append(!StringImpl.equals(alternatives, (object) "") ? new StringBuffer().append(" (").append(alternatives).append(")").ToString() : "").append(" - ").append(help).ToString());

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Help help = this;
      ObjectImpl.clone((object) help);
      return ((object) help).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
