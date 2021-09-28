// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.Action
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using System.ComponentModel;

namespace org.nakedobjects.@object
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectMember;")]
  [JavaInterface]
  public interface Action : NakedObjectMember
  {
    static readonly Action.Type DEBUG;
    static readonly Action.Type EXPLORATION;
    static readonly Action.Type USER;
    static readonly Action.Type SET;
    static readonly Action.Target LOCAL;
    static readonly Action.Target REMOTE;
    static readonly Action.Target DEFAULT;

    Action[] getActions();

    int getParameterCount();

    Action.Type getType();

    Action.Target getTarget();

    bool hasReturn();

    bool isOnInstance();

    NakedObjectSpecification[] getParameterTypes();

    Naked[] parameterStubs();

    NakedObjectSpecification getReturnType();

    Naked execute(NakedReference target, Naked[] parameters);

    Consent isParameterSetValid(NakedReference @object, Naked[] parameters);

    ActionParameterSet getParameterSet(NakedReference @object);

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Action()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaInterfaces("1;java/io/Serializable;")]
    [JavaFlags(41)]
    class Type : Serializable
    {
      private const long serialVersionUID = 1;
      private string name;

      [JavaFlags(2)]
      [JavaFlags(2)]
      public Type(string name) => this.name = name;

      public override bool Equals(object @object) => @object is Action.Type && StringImpl.equals(this.name, (object) ((Action.Type) @object).name);

      public override int GetHashCode() => StringImpl.hashCode(this.name);

      public override string ToString() => this.name;

      public virtual string getName() => this.name;

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        Action.Type type = this;
        ObjectImpl.clone((object) type);
        return ((object) type).MemberwiseClone();
      }
    }

    [JavaFlags(41)]
    class Target
    {
      [JavaFlags(0)]
      public string name;

      [JavaFlags(2)]
      [JavaFlags(2)]
      public Target(string name) => this.name = name;

      public override bool Equals(object @object) => @object is Action.Target && StringImpl.equals(this.name, (object) ((Action.Target) @object).name);

      public override int GetHashCode() => StringImpl.hashCode(this.name);

      public override string ToString() => this.name;

      public virtual string getName() => this.name;

      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      [JavaFlags(4227077)]
      public new virtual object MemberwiseClone()
      {
        Action.Target target = this;
        ObjectImpl.clone((object) target);
        return ((object) target).MemberwiseClone();
      }
    }
  }
}
