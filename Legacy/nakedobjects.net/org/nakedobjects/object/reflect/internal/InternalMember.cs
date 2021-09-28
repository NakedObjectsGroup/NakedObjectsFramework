// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.InternalMember
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.reflect.@internal;

namespace org.nakedobjects.@object.reflect.@internal
{
  public abstract class InternalMember
  {
    [JavaFlags(4)]
    public MemberIdentifier identifeir;

    public virtual object getExtension(Class cls) => (object) null;

    public virtual Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual MemberIdentifier getIdentifier() => this.identifeir;

    public virtual string getDescription() => "";

    public virtual string getHelp() => "";

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      InternalMember internalMember = this;
      ObjectImpl.clone((object) internalMember);
      return ((object) internalMember).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
