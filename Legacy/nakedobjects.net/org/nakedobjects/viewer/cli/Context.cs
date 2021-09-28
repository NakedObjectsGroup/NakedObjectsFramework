// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.Context
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.cli
{
  public class Context
  {
    private readonly History history;
    private readonly Context parent;
    private readonly Agent classes;
    private Agent subjectAgent;

    public Context(Context parent, Agent resourcesAgent)
    {
      this.history = new History();
      this.parent = parent;
      this.classes = resourcesAgent;
    }

    public virtual void addObject(Agent agent)
    {
      if (!agent.isReplaceable())
        throw new NakedObjectRuntimeException();
      this.history.setCurrent(agent);
    }

    public virtual void addSubject(Agent agent) => this.subjectAgent = this.subjectAgent == null ? agent : throw new NakedObjectRuntimeException();

    public virtual void closeContext() => this.parent.history.add(this.getAgent());

    public virtual bool currentAgentIs(Class cls) => cls.isAssignableFrom(ObjectImpl.getClass((object) this.getAgent()));

    public virtual void debug(DebugString debug)
    {
      debug.appendln(new StringBuffer().append("agent:    ").append((object) this.getAgent()).ToString());
      debug.appendln(new StringBuffer().append("classes:  ").append((object) this.classes).ToString());
      debug.appendln(new StringBuffer().append("subject:    ").append((object) this.subjectAgent).ToString());
      this.history.debug(debug);
    }

    public virtual Agent findAgent(string path)
    {
      if (path == null || StringImpl.equals(path, (object) ""))
        return this.getAgent();
      StringTokenizer stringTokenizer = new StringTokenizer(new StringBuffer().append(" ").append(StringImpl.toLowerCase(path)).ToString(), ".");
      int length = stringTokenizer.countTokens();
      string[] strArray1 = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      int index1 = 0;
      while (stringTokenizer.hasMoreTokens())
      {
        strArray1[index1] = StringImpl.trim(stringTokenizer.nextToken());
        ++index1;
      }
      int num1 = 0;
      string[] strArray2 = strArray1;
      int num2;
      int index2 = (num2 = num1) + 1;
      int index3 = num2;
      string str = strArray2[index3];
      Agent agent = (Agent) null;
      if (StringImpl.equals(str, (object) "this") || StringImpl.length(str) == 0)
        agent = this.history.getCurrent();
      else if (StringImpl.equals(str, (object) "hist"))
      {
        string[] strArray3 = strArray1.Length != 1 ? strArray1 : throw new IllegalDispatchException("Object must be specified to be found in history");
        int num3;
        index2 = (num3 = index2) + 1;
        int index4 = num3;
        string criteria = strArray3[index4];
        agent = this.history.findAgent(criteria);
        if (agent == null)
          throw new IllegalDispatchException(new StringBuffer().append("Could not find '").append(criteria).append("' in history").ToString());
      }
      else if (StringImpl.equals(str, (object) "classes"))
      {
        string[] strArray4 = strArray1.Length != 1 ? strArray1 : throw new IllegalDispatchException("Object must be specified to be found in resources");
        int num4;
        index2 = (num4 = index2) + 1;
        int index5 = num4;
        string path1 = strArray4[index5];
        agent = this.classes.findAgent(path1);
        if (agent == null)
          throw new IllegalDispatchException(new StringBuffer().append("Could not find '").append(path1).append("' in resources").ToString());
      }
      else
      {
        if (this.history.getCurrent() != null)
          agent = this.history.getCurrent().findAgent(str);
        if (agent == null)
          agent = this.history.findAgent(str);
        if (agent == null)
          agent = this.classes.findAgent(str);
        if (agent == null)
          throw new IllegalDispatchException(new StringBuffer().append("Could not find '").append(str).append("'").ToString());
      }
      for (; index2 < strArray1.Length; ++index2)
        agent = agent.findAgent(strArray1[index2]) ?? throw new IllegalDispatchException(new StringBuffer().append("Failed to find '").append(strArray1[index2]).append("' in ").append(agent.getPrompt()).ToString());
      return agent;
    }

    public virtual Agent getAgent()
    {
      if (this.subjectAgent != null)
        return this.subjectAgent;
      return this.history.getCurrent() != null ? this.history.getCurrent() : this.classes;
    }

    public virtual History getHistory() => this.history;

    public virtual string getPrompt() => this.getAgent().getPrompt();

    public virtual Agent getResourceAgent() => this.classes;

    public virtual bool isValueEntry() => this.getAgent().isValueEntry();

    public virtual void removeAgent() => this.subjectAgent = this.subjectAgent != null ? (Agent) null : throw new NakedObjectRuntimeException();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Context context = this;
      ObjectImpl.clone((object) context);
      return ((object) context).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
