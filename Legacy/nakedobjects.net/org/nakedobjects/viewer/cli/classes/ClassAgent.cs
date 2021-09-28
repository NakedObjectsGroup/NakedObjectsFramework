// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.classes.ClassAgent
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
  public class ClassAgent : Agent
  {
    private readonly NakedClass cls;

    public ClassAgent(NakedClass cls) => this.cls = cls;

    public virtual string debug() => new StringBuffer().append("Class ").append(this.cls.getFullName()).ToString();

    public virtual Agent findAgent(string lowecaseTitle) => (Agent) null;

    public virtual NakedObject getNakedClass() => NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient((object) this.cls);

    public virtual string getName() => "Class";

    public virtual string getPrompt() => this.cls.getPluralName();

    private NakedObjectSpecification getSpecification() => this.cls.forObjectType();

    public virtual bool isReplaceable() => true;

    public virtual bool isValueEntry() => false;

    public virtual void list(View view, string[] layout) => view.display(this.cls.getSingularName());

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("specification", this.cls.getFullName());
      return toString.ToString();
    }

    public virtual Action[] getActions() => this.getSpecification().getClassActions(Action.USER);

    public virtual TypedNakedCollection instances() => NakedObjects.getObjectPersistor().allInstances(this.getSpecification(), false);

    public virtual NakedObject newInstance()
    {
      NakedObjectPersistor objectPersistor = NakedObjects.getObjectPersistor();
      objectPersistor.startTransaction();
      NakedObject persistentInstance = objectPersistor.createPersistentInstance(this.getSpecification());
      objectPersistor.endTransaction();
      return persistentInstance;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ClassAgent classAgent = this;
      ObjectImpl.clone((object) classAgent);
      return ((object) classAgent).MemberwiseClone();
    }
  }
}
