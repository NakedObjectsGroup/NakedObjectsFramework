// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.InternalObjectTitle
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
using org.nakedobjects.@object.reflect.@internal;
using System.ComponentModel;

namespace org.nakedobjects.@object.reflect.@internal
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ObjectTitle;")]
  public class InternalObjectTitle : ObjectTitle
  {
    private static readonly Category LOG;
    private Method titleMethod;

    public InternalObjectTitle(Method titleMethod) => this.titleMethod = titleMethod;

    public virtual string title(NakedObject @object)
    {
      try
      {
        Method titleMethod = this.titleMethod;
        object obj1 = @object.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        object obj2 = titleMethod.invoke(obj1, objArray);
        return obj2 != null ? obj2.ToString() : "";
      }
      catch (InvocationTargetException ex)
      {
        InternalObjectTitle.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.titleMethod).ToString(), ex.getTargetException());
      }
      catch (IllegalAccessException ex)
      {
        InternalObjectTitle.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.titleMethod).ToString(), (Throwable) ex);
      }
      return "title error...";
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static InternalObjectTitle()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      InternalObjectTitle internalObjectTitle = this;
      ObjectImpl.clone((object) internalObjectTitle);
      return ((object) internalObjectTitle).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
