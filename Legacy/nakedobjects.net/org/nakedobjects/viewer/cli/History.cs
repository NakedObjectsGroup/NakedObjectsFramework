// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.History
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.cli
{
  public class History
  {
    private readonly Stack next;
    private readonly Stack previous;
    private Agent current;

    public virtual void setCurrent(Agent item)
    {
      if (item == null)
        throw new NakedObjectRuntimeException();
      this.add(this.current);
      this.current = item;
    }

    public virtual void add(Agent item)
    {
      if (item == null)
        return;
      ((Vector) this.previous).removeElement((object) item);
      ((Vector) this.previous).addElement((object) item);
    }

    public virtual void show(View view)
    {
      for (int index = ((Vector) this.previous).size() - 1; index >= 0; index += -1)
      {
        Agent agent = (Agent) ((Vector) this.previous).elementAt(index);
        view.display(agent.getPrompt());
      }
    }

    public virtual bool hasNext() => ((Vector) this.next).size() > 0;

    public virtual bool hasPrevious() => ((Vector) this.previous).size() > 0;

    public virtual void previous()
    {
      this.next.push((object) this.current);
      this.current = (Agent) this.previous.pop();
    }

    public virtual void next()
    {
      this.previous.push((object) this.current);
      this.current = (Agent) this.next.pop();
    }

    public virtual Agent getCurrent() => this.current;

    public virtual Agent last() => ((Vector) this.previous).size() == 0 ? (Agent) null : (Agent) ((Vector) this.previous).lastElement();

    public virtual Agent findAgent(string criteria)
    {
      Matcher matcher = (Matcher) new History.HistoryMatcher(this);
      return (Agent) MatchAlgorithm.match(criteria, matcher);
    }

    public virtual void debug(DebugString debug)
    {
      debug.appendln("next:-");
      for (int index = 0; index < ((Vector) this.next).size(); ++index)
      {
        Agent agent = (Agent) ((Vector) this.next).elementAt(index);
        debug.appendln(new StringBuffer().append("   ").append(agent.getPrompt()).ToString());
      }
      debug.appendln(new StringBuffer().append("current:   ").append(this.current != null ? this.current.getPrompt() : "none").ToString());
      debug.appendln("previous:-");
      for (int index = ((Vector) this.previous).size() - 1; index >= 0; index += -1)
      {
        Agent agent = (Agent) ((Vector) this.previous).elementAt(index);
        debug.appendln(new StringBuffer().append("   ").append(agent.getPrompt()).ToString());
      }
    }

    public History()
    {
      this.next = new Stack();
      this.previous = new Stack();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      History history = this;
      ObjectImpl.clone((object) history);
      return ((object) history).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
    [JavaFlags(50)]
    [Inner]
    private sealed class HistoryMatcher : Matcher
    {
      private readonly Enumeration p;
      private Agent agent;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private History this\u00240;

      public virtual bool hasMoreElements() => this.p.hasMoreElements();

      public virtual object getElement() => (object) this.agent;

      public virtual string nextElement()
      {
        this.agent = (Agent) this.p.nextElement();
        return this.agent.getPrompt();
      }

      [JavaFlags(2)]
      public HistoryMatcher(History _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.p = ((Vector) this.this\u00240.previous).elements();
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        History.HistoryMatcher historyMatcher = this;
        ObjectImpl.clone((object) historyMatcher);
        return ((object) historyMatcher).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
