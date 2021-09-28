// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ActionHelper
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ActionHelper
  {
    private readonly Action action;
    private readonly string[] labels;
    private readonly Naked[] parameters;
    private readonly NakedObjectSpecification[] types;
    private readonly NakedReference target;
    private readonly bool[] required;
    private readonly Naked[][] options;

    public static ActionHelper createInstance(NakedReference target, Action action)
    {
      int length1 = action.getParameterTypes().Length;
      int length2 = length1;
      Naked[] parameters1 = length2 >= 0 ? new Naked[length2] : throw new NegativeArraySizeException();
      int length3 = length1;
      Naked[][] options = length3 >= 0 ? new Naked[length3][] : throw new NegativeArraySizeException();
      ActionParameterSet parameters2 = target.getParameters(action);
      string[] labels;
      object[] objArray1;
      object[][] objArray2;
      bool[] required;
      if (parameters2 != null)
      {
        labels = parameters2.getParameterLabels();
        objArray1 = parameters2.getDefaultParameterValues();
        objArray2 = parameters2.getOptions();
        required = parameters2.getRequiredParameters();
      }
      else
      {
        int length4 = length1;
        labels = length4 >= 0 ? new string[length4] : throw new NegativeArraySizeException();
        int length5 = length1;
        objArray1 = length5 >= 0 ? (object[]) new Naked[length5] : throw new NegativeArraySizeException();
        int num = length1;
        int length6 = 0;
        int length7 = num;
        objArray2 = length6 >= 0 && length7 >= 0 ? (object[][]) new Naked[length7, length6][] : throw new NegativeArraySizeException();
        int length8 = length1;
        required = length8 >= 0 ? new bool[length8] : throw new NegativeArraySizeException();
      }
      NakedObjectSpecification[] parameterTypes = action.getParameterTypes();
      int length9 = parameterTypes.Length;
      Naked[] nakedArray1 = length9 >= 0 ? new Naked[length9] : throw new NegativeArraySizeException();
      for (int index = 0; index < parameterTypes.Length; ++index)
        nakedArray1[index] = !parameterTypes[index].isValue() ? (Naked) null : (Naked) NakedObjects.getObjectLoader().createValueInstance(parameterTypes[index]);
      int length10 = parameterTypes.Length;
      Naked[] nakedArray2 = length10 >= 0 ? new Naked[length10] : throw new NegativeArraySizeException();
      for (int index = 0; index < nakedArray2.Length; ++index)
        nakedArray2[index] = nakedArray1[index];
      for (int index1 = 0; index1 < length1; ++index1)
      {
        NakedObjectSpecification objectSpecification = parameterTypes[index1];
        labels[index1] = labels[index1] != null ? labels[index1] : objectSpecification.getShortName();
        if (objArray1[index1] == null)
        {
          parameters1[index1] = nakedArray2[index1];
        }
        else
        {
          parameters1[index1] = (Naked) NakedObjects.getObjectLoader().createAdapterForValue(objArray1[index1]);
          if (parameters1[index1] == null)
            parameters1[index1] = (Naked) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(objArray1[index1]);
        }
        if (objArray2[index1] != null)
        {
          Naked[][] nakedArray3 = options;
          int index2 = index1;
          int length11 = objArray2[index1].Length;
          NakedObject[] nakedObjectArray = length11 >= 0 ? new NakedObject[length11] : throw new NegativeArraySizeException();
          nakedArray3[index2] = (Naked[]) nakedObjectArray;
          for (int index3 = 0; index3 < options[index1].Length; ++index3)
          {
            options[index1][index3] = (Naked) NakedObjects.getObjectLoader().createAdapterForValue(objArray2[index1][index3]);
            if (options[index1][index3] == null)
              options[index1][index3] = (Naked) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(objArray2[index1][index3]);
          }
        }
      }
      return new ActionHelper(target, action, labels, parameters1, parameterTypes, required, options);
    }

    private ActionHelper(
      NakedReference target,
      Action action,
      string[] labels,
      Naked[] parameters,
      NakedObjectSpecification[] types,
      bool[] required,
      Naked[][] options)
    {
      this.target = target;
      this.action = action;
      this.labels = labels;
      this.parameters = parameters;
      this.types = types;
      this.required = required;
      this.options = options;
    }

    public virtual ParameterContent[] createParameters()
    {
      int length = this.parameters.Length;
      ParameterContent[] parameterContentArray = length >= 0 ? new ParameterContent[length] : throw new NegativeArraySizeException();
      for (int i = 0; i < this.parameters.Length; ++i)
        parameterContentArray[i] = !this.types[i].isValue() ? (ParameterContent) new ObjectParameter(this.labels[i], this.parameters[i], this.types[i], this.required[i], (NakedObject[]) this.options[i], i, this) : (ParameterContent) new ValueParameter(this.labels[i], this.parameters[i], this.types[i], this.required[i]);
      return parameterContentArray;
    }

    public virtual Consent disabled() => this.target.isValid(this.action, this.parameters);

    public virtual string getName() => this.action.getName();

    public virtual string getDescription() => this.action.getDescription();

    public virtual string getHelp() => this.action.getHelp();

    public virtual Naked getParameter(int index) => this.parameters[index];

    public virtual NakedReference getTarget() => this.target;

    public virtual Naked invoke() => this.target.execute(this.action, this.parameters);

    public virtual void setParameter(int index, Naked parameter) => this.parameters[index] = parameter;

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
