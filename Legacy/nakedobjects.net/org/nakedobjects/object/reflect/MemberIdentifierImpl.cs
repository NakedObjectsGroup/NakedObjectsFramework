// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.MemberIdentifierImpl
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
  [JavaInterfaces("1;org/nakedobjects/object/reflect/MemberIdentifier;")]
  public class MemberIdentifierImpl : MemberIdentifier
  {
    private readonly string className;
    private readonly string name;
    private readonly string[] parameters;
    private readonly bool isField;

    public MemberIdentifierImpl(string className)
    {
      this.className = className;
      this.name = "";
      int length = 0;
      this.parameters = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      this.isField = false;
    }

    public MemberIdentifierImpl(string className, string fieldName)
    {
      this.className = className;
      this.name = fieldName;
      int length = 0;
      this.parameters = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      this.isField = true;
    }

    public MemberIdentifierImpl(
      string className,
      string methodName,
      NakedObjectSpecification[] specifications)
    {
      this.className = className;
      this.name = methodName;
      int length = specifications != null ? specifications.Length : 0;
      this.parameters = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < this.parameters.Length; ++index)
        this.parameters[index] = specifications[index].getFullName();
      this.isField = false;
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is MemberIdentifierImpl))
        return false;
      MemberIdentifierImpl memberIdentifierImpl = (MemberIdentifierImpl) obj;
      return this.equals(memberIdentifierImpl.className, this.className) && this.equals(memberIdentifierImpl.name, memberIdentifierImpl.name) && this.equals(memberIdentifierImpl.parameters, this.parameters);
    }

    private bool equals(string[] a, string[] b)
    {
      if (a == null && b == null)
        return true;
      if (a == null && b != null || a != null && b == null || a.Length != b.Length)
        return false;
      for (int index = 0; index < b.Length; ++index)
      {
        if (!StringImpl.equals(a[index], (object) b[index]))
          return false;
      }
      return true;
    }

    private bool equals(string a, string b)
    {
      if ((object) a == (object) b)
        return true;
      return a != null && StringImpl.equals(a, (object) b);
    }

    public virtual string getClassName() => this.className;

    public virtual string getName() => this.name;

    public virtual string[] getParameters() => this.parameters;

    public virtual bool isField() => this.isField;

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(this.className);
      stringBuffer.append('#');
      stringBuffer.append(this.name);
      stringBuffer.append('(');
      for (int index = 0; index < this.parameters.Length; ++index)
      {
        if (index > 0)
          stringBuffer.append(", ");
        stringBuffer.append(this.parameters[index]);
      }
      stringBuffer.append(')');
      return stringBuffer.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      MemberIdentifierImpl memberIdentifierImpl = this;
      ObjectImpl.clone((object) memberIdentifierImpl);
      return ((object) memberIdentifierImpl).MemberwiseClone();
    }
  }
}
