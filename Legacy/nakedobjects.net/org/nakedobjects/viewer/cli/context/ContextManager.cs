// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.context.ContextManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.viewer.cli.classes;

namespace org.nakedobjects.viewer.cli.context
{
  public class ContextManager
  {
    private readonly Vector contextStack;
    private readonly ClassesAgent classes;

    public ContextManager(ClassesAgent classes)
    {
      this.contextStack = new Vector();
      this.classes = classes;
      this.newContext((Context) null);
    }

    public virtual void newContext() => this.newContext(this.current());

    public virtual void newContext(Agent agent)
    {
      this.newContext();
      this.current().addObject(agent);
    }

    private void newContext(Context parent) => this.contextStack.addElement((object) new Context(parent, (Agent) this.classes));

    public virtual bool canRemoveContext() => this.contextStack.size() > 1;

    public virtual Context current() => (Context) this.contextStack.lastElement();

    public virtual void removeContext() => this.contextStack.removeElementAt(this.contextStack.size() - 1);

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ContextManager contextManager = this;
      ObjectImpl.clone((object) contextManager);
      return ((object) contextManager).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
