// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.CollectionActionContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ActionContent;")]
  public class CollectionActionContent : CollectionContent, ActionContent
  {
    private readonly ActionHelper invocation;
    private readonly ParameterContent[] parameters;

    public CollectionActionContent(ActionHelper invocation)
    {
      this.invocation = invocation;
      this.parameters = invocation.createParameters();
    }

    public override void debugDetails(DebugString debug)
    {
      debug.appendln("action", (object) this.getActionName());
      debug.appendln("target", (object) this.getNaked());
      string str = "";
      for (int index = 0; index < this.parameters.Length; ++index)
        str = new StringBuffer().append(str).append((object) this.parameters[index]).ToString();
      debug.appendln("parameters", (object) str);
    }

    public override Consent canDrop(Content sourceContent) => (Consent) Veto.DEFAULT;

    public virtual Consent disabled() => this.invocation.disabled();

    public override Naked drop(Content sourceContent) => throw new NotImplementedException();

    public override NakedObject[] elements() => throw new NotImplementedException();

    public virtual Naked execute() => this.invocation.invoke();

    public virtual string getActionName() => this.invocation.getName();

    public override NakedCollection getCollection() => (NakedCollection) this.invocation.getTarget();

    public override string getDescription() => this.invocation.getDescription();

    public override string getHelp() => this.invocation.getHelp();

    public override string getIconName() => this.getNaked().getIconName();

    public override string getId() => this.invocation.getName();

    public override Naked getNaked() => (Naked) this.invocation.getTarget();

    public virtual int getNoParameters() => this.parameters.Length;

    public virtual ParameterContent getParameterContent(int index) => this.parameters[index];

    public virtual Naked getParameterObject(int index) => this.invocation.getParameter(index);

    public override NakedObjectSpecification getSpecification() => this.getNaked().getSpecification();

    public override bool isTransient() => true;

    public override string title() => this.getNaked().titleString();

    public override string windowTitle() => this.getActionName();
  }
}
