// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.SimpleActionAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.application.control;

namespace org.nakedobjects.reflector.java.control
{
  [JavaInterfaces("1;org/nakedobjects/application/control/ActionAbout;")]
  public class SimpleActionAbout : AbstractAbout, ActionAbout
  {
    private const long serialVersionUID = 1;
    private object[] defaultValues;
    private string[] labels;
    private bool[] required;
    private object[][] options;

    public SimpleActionAbout(Session session, object @object, object[] parameters)
      : base(session, @object)
    {
      int length1 = parameters.Length;
      int length2 = length1;
      this.labels = length2 >= 0 ? new string[length2] : throw new NegativeArraySizeException();
      int length3 = length1;
      this.defaultValues = length3 >= 0 ? new object[length3] : throw new NegativeArraySizeException();
      int length4 = length1;
      this.required = length4 >= 0 ? new bool[length4] : throw new NegativeArraySizeException();
      int length5 = length1;
      this.options = length5 >= 0 ? new object[length5][] : throw new NegativeArraySizeException();
    }

    public virtual object[][] getOptions() => this.options;

    public virtual object[] getDefaultParameterValues() => this.defaultValues;

    public virtual string[] getParameterLabels() => this.labels;

    public virtual bool[] getRequired() => this.required;

    public override void invisible() => base.invisible();

    public override void invisibleToUser(User user) => base.invisibleToUser(user);

    public override void invisibleToUsers(User[] users) => base.invisibleToUsers(users);

    public virtual void setParameter(int index, object defaultValue)
    {
      this.checkParameter(index);
      this.defaultValues[index] = defaultValue;
    }

    public virtual void setParameter(int index, object[] options)
    {
      this.checkParameter(index);
      this.options[index] = options;
    }

    private void checkParameter(int index)
    {
      if (index < 0 || index >= this.defaultValues.Length)
        throw new IllegalArgumentException(new StringBuffer().append("No parameter index ").append(index).ToString());
    }

    public virtual void setParameter(int index, string label)
    {
      this.checkParameter(index);
      this.labels[index] = label;
    }

    public virtual void setParameter(int index, bool required)
    {
      this.checkParameter(index);
      this.required[index] = required;
    }

    public virtual void setParameter(int index, string label, object defaultValue, bool required)
    {
      this.checkParameter(index);
      this.labels[index] = label;
      this.defaultValues[index] = defaultValue;
      this.required[index] = required;
    }

    public virtual void setParameters(object[] defaultValues)
    {
      if (this.defaultValues.Length != defaultValues.Length)
        throw new IllegalArgumentException(new StringBuffer().append("Expected ").append(this.defaultValues.Length).append(" defaults but got ").append(defaultValues.Length).ToString());
      this.defaultValues = defaultValues;
    }

    public virtual void setParameters(string[] labels)
    {
      if (this.labels.Length != labels.Length)
        throw new IllegalArgumentException(new StringBuffer().append("Expected ").append(this.labels.Length).append(" defaults but got ").append(labels.Length).ToString());
      this.labels = labels;
    }

    public virtual void setParameters(bool[] required)
    {
      if (this.labels.Length != this.labels.Length)
        throw new IllegalArgumentException(new StringBuffer().append("Expected ").append(this.labels.Length).append(" defaults but got ").append(this.labels.Length).ToString());
      this.required = required;
    }

    public virtual void unusable() => base.unusable("Cannot be invoked");

    public override void unusable(string reason) => base.unusable(reason);

    public override void unusableInState(State state) => base.unusableInState(state);

    public override void unusableInStates(State[] states) => base.unusableInStates(states);

    public override void unusableOnCondition(bool conditionMet, string reasonNotMet) => base.unusableOnCondition(conditionMet, reasonNotMet);

    public override void usableOnlyInState(State state) => base.usableOnlyInState(state);

    public override void usableOnlyInStates(State[] states) => base.usableOnlyInStates(states);

    public override void visibleOnlyToRole(Role role) => base.visibleOnlyToRole(role);

    public override void visibleOnlyToRoles(Role[] roles) => base.visibleOnlyToRoles(roles);

    public override void visibleOnlyToUser(User user) => base.visibleOnlyToUser(user);

    public override void visibleOnlyToUsers(User[] users) => base.visibleOnlyToUsers(users);
  }
}
