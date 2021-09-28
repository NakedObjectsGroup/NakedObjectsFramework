// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaMember
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.application;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  public abstract class JavaMember
  {
    private static readonly Logger LOG;
    private readonly MemberIdentifier identifier;
    private Method aboutMethod;

    [JavaFlags(4)]
    public JavaMember(MemberIdentifier identifier, Method about)
    {
      this.aboutMethod = about;
      this.identifier = identifier;
    }

    [JavaFlags(4)]
    public virtual Method getAboutMethod() => this.aboutMethod;

    public virtual object getExtension(Class cls) => (object) null;

    public virtual Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual MemberIdentifier getIdentifier() => this.identifier;

    public virtual string getHelp() => "No help available";

    public static void invocationException(string error, InvocationTargetException e)
    {
      if (e.getTargetException() is ApplicationException)
        throw new NakedObjectApplicationException(e.getTargetException());
      JavaMember.LOG.error((object) error, e.getTargetException());
      if (e.getTargetException() is RuntimeException)
        throw (RuntimeException) e.getTargetException();
      throw new ReflectiveActionException(e.getTargetException().getMessage(), e.getTargetException());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static JavaMember()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaMember javaMember = this;
      ObjectImpl.clone((object) javaMember);
      return ((object) javaMember).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
