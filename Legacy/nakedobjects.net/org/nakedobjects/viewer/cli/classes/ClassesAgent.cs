// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.classes.ClassesAgent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.classes
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Agent;")]
  public class ClassesAgent : Agent
  {
    private readonly NakedClass[] classes;

    public ClassesAgent(NakedClass[] classes) => this.classes = classes;

    public virtual string debug() => new StringBuffer().append("Classes, ").append(this.classes.Length).append(" classes").ToString();

    public virtual void list(View view, string[] layout)
    {
      for (int index = 0; index < this.classes.Length; ++index)
        view.display(this.classes[index].title());
    }

    public virtual NakedClass[] getClasses() => this.classes;

    public virtual string getName() => "Classes";

    public virtual Agent findAgent(string name)
    {
      Matcher matcher = (Matcher) new ClassesAgent.ClassMatcher(this.classes);
      NakedClass cls = (NakedClass) MatchAlgorithm.match(name, matcher);
      return cls == null ? (Agent) null : (Agent) new ClassAgent(cls);
    }

    public virtual string getPrompt() => "(classes)";

    public virtual bool isReplaceable() => false;

    public virtual bool isValueEntry() => false;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("classes", this.classes.Length);
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ClassesAgent classesAgent = this;
      ObjectImpl.clone((object) classesAgent);
      return ((object) classesAgent).MemberwiseClone();
    }

    [JavaFlags(58)]
    [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
    private sealed class ClassMatcher : Matcher
    {
      private readonly NakedClass[] classes;
      private int index;

      [JavaFlags(2)]
      public ClassMatcher(NakedClass[] classes)
      {
        this.index = 0;
        this.classes = classes;
      }

      public virtual bool hasMoreElements() => this.index < this.classes.Length;

      public virtual object getElement() => (object) this.classes[this.index - 1];

      public virtual string nextElement()
      {
        NakedClass[] classes = this.classes;
        int index1;
        this.index = (index1 = this.index) + 1;
        int index2 = index1;
        return classes[index2].title();
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ClassesAgent.ClassMatcher classMatcher = this;
        ObjectImpl.clone((object) classMatcher);
        return ((object) classMatcher).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
