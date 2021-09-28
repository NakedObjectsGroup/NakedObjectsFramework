// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.AbstractTextView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/View;")]
  public abstract class AbstractTextView : View
  {
    [JavaFlags(1028)]
    public abstract void appendln(string text);

    public virtual void connect() => this.appendln("Naked Objects Text Console");

    public virtual void disconnect() => this.appendln("Bye");

    public virtual void displayEntry(string entry)
    {
    }

    public virtual void display(string message) => this.appendln(message);

    public virtual void display(NakedObject @object) => this.appendln(@object != null ? @object.titleString() : "");

    public virtual void error(string message) => this.display(new StringBuffer().append("ERR: ").append(message).ToString());

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractTextView abstractTextView = this;
      ObjectImpl.clone((object) abstractTextView);
      return ((object) abstractTextView).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void clear();

    public abstract void prompt(string prompt);
  }
}
