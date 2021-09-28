// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.AbstractNakedObjectMember
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectMember;")]
  public abstract class AbstractNakedObjectMember : NakedObjectMember
  {
    [JavaFlags(20)]
    public readonly string defaultLabel;
    private readonly string id;

    [JavaFlags(4)]
    public AbstractNakedObjectMember(string name)
    {
      this.defaultLabel = name != null ? NameConvertor.naturalName(name) : throw new IllegalArgumentException("Name must always be set");
      this.id = NameConvertor.simpleName(name);
    }

    public abstract object getExtension(Class cls);

    public virtual string getId() => this.id;

    public override string ToString() => new StringBuffer().append("id=").append(this.getId()).append(",label='").append(this.getName()).append("'").ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractNakedObjectMember nakedObjectMember = this;
      ObjectImpl.clone((object) nakedObjectMember);
      return ((object) nakedObjectMember).MemberwiseClone();
    }

    public abstract void debugData(DebugString debugString);

    public abstract string getDescription();

    public abstract Class[] getExtensions();

    public abstract string getHelp();

    public abstract string getName();

    public abstract bool isAuthorised();

    public abstract Consent isAvailable(NakedReference target);

    public abstract Consent isVisible(NakedReference target);
  }
}
