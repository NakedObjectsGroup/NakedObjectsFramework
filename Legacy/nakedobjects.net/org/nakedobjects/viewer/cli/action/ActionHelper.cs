// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ActionHelper
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaFlags(32)]
  public class ActionHelper
  {
    private readonly string[] labels;
    private readonly object[] parameters;
    private readonly bool[] required;
    private readonly object[][] options;

    public static ActionHelper createInstance(NakedReference target, Action action)
    {
      int length1 = action.getParameterTypes().Length;
      ActionParameterSet parameters1 = target.getParameters(action);
      string[] labels;
      object[] parameters2;
      object[][] options;
      bool[] required;
      if (parameters1 != null)
      {
        labels = parameters1.getParameterLabels();
        parameters2 = parameters1.getDefaultParameterValues();
        options = parameters1.getOptions();
        required = parameters1.getRequiredParameters();
      }
      else
      {
        int length2 = length1;
        labels = length2 >= 0 ? new string[length2] : throw new NegativeArraySizeException();
        int length3 = length1;
        parameters2 = length3 >= 0 ? (object[]) new Naked[length3] : throw new NegativeArraySizeException();
        int length4 = length1;
        options = length4 >= 0 ? (object[][]) new Naked[length4][] : throw new NegativeArraySizeException();
        int length5 = length1;
        required = length5 >= 0 ? new bool[length5] : throw new NegativeArraySizeException();
      }
      for (int index1 = 0; index1 < options.Length; ++index1)
      {
        if (options[index1] == null)
        {
          object[][] objArray = options;
          int index2 = index1;
          int length6 = 0;
          Naked[] nakedArray = length6 >= 0 ? new Naked[length6] : throw new NegativeArraySizeException();
          objArray[index2] = (object[]) nakedArray;
        }
      }
      NakedObjectSpecification[] parameterTypes = action.getParameterTypes();
      for (int index = 0; index < length1; ++index)
      {
        NakedObjectSpecification objectSpecification = parameterTypes[index];
        labels[index] = labels[index] != null ? labels[index] : objectSpecification.getShortName();
      }
      return new ActionHelper(labels, parameters2, required, options);
    }

    private ActionHelper(
      string[] labels,
      object[] parameters,
      bool[] required,
      object[][] options)
    {
      this.labels = labels;
      this.parameters = parameters;
      this.required = required;
      this.options = options;
    }

    public virtual string[] getParameterLabels() => this.labels;

    public virtual object[] getDefaultParameterValues() => this.parameters;

    public virtual bool[] getRequiredParameters() => this.required;

    public virtual object[][] getOptions() => this.options;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ActionHelper actionHelper = this;
      ObjectImpl.clone((object) actionHelper);
      return ((object) actionHelper).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
