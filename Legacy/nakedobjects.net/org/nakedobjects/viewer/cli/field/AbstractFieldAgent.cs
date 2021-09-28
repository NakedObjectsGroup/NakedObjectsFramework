// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.field.AbstractFieldAgent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.field
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Agent;")]
  public abstract class AbstractFieldAgent : Agent
  {
    [JavaFlags(20)]
    public readonly NakedObjectField field;
    [JavaFlags(20)]
    public readonly string name;
    [JavaFlags(20)]
    public readonly NakedObject @object;

    public AbstractFieldAgent(string name, NakedObject @object, NakedObjectField field)
    {
      this.name = name;
      this.@object = @object;
      this.field = field;
    }

    public virtual string debug()
    {
      string str = this.@object != null ? this.@object.titleString() : "null";
      return new StringBuffer().append("Set field '").append(this.name).append("' in ").append(str).ToString();
    }

    public virtual Agent findAgent(string lowecaseTitle) => (Agent) null;

    public virtual bool isReplaceable() => false;

    public virtual void list(View view, string[] layout)
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractFieldAgent abstractFieldAgent = this;
      ObjectImpl.clone((object) abstractFieldAgent);
      return ((object) abstractFieldAgent).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract string getName();

    public abstract string getPrompt();

    public abstract bool isValueEntry();
  }
}
