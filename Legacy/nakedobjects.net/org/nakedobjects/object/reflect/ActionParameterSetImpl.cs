// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.ActionParameterSetImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/ActionParameterSet;")]
  public class ActionParameterSetImpl : ActionParameterSet
  {
    private readonly object[] defaultValues;
    private readonly string[] labels;
    private readonly bool[] required;
    private object[][] options;

    public ActionParameterSetImpl(
      object[] defaultValues,
      object[][] options,
      string[] labels,
      bool[] required)
      : this(defaultValues, labels, required)
    {
      this.options = options;
    }

    public ActionParameterSetImpl(object[] defaultValues, string[] labels, bool[] required)
    {
      this.defaultValues = defaultValues;
      this.labels = labels;
      this.required = required;
      int length = defaultValues.Length;
      this.options = length >= 0 ? new object[length][] : throw new NegativeArraySizeException();
    }

    public virtual object[] getDefaultParameterValues() => this.defaultValues;

    public virtual object[][] getOptions() => this.options;

    public virtual string[] getParameterLabels() => this.labels;

    public virtual bool[] getRequiredParameters() => this.required;

    public virtual void checkParameters(string name, NakedObjectSpecification[] requiredTypes)
    {
      for (int index = 0; index < requiredTypes.Length; ++index)
      {
        NakedObjectSpecification requiredType = requiredTypes[index];
        if (this.defaultValues[index] != null)
        {
          NakedObjectSpecification objectSpecification = NakedObjects.getSpecificationLoader().loadSpecification(ObjectImpl.getClass(this.defaultValues[index]));
          if (!objectSpecification.isOfType(requiredType))
            throw new ReflectionException(new StringBuffer().append("Parameter ").append(index + 1).append(" in ").append(name).append(" is not of required type; expected type ").append(requiredType.getFullName()).append(" but got ").append(objectSpecification.getFullName()).append(".  Check the related about method").ToString());
        }
      }
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ActionParameterSetImpl parameterSetImpl = this;
      ObjectImpl.clone((object) parameterSetImpl);
      return ((object) parameterSetImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
