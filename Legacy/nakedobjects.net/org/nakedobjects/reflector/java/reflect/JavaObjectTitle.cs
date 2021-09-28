// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaObjectTitle
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
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/ObjectTitle;")]
  public class JavaObjectTitle : ObjectTitle
  {
    private static readonly Category LOG;
    private Method titleMethod;

    public JavaObjectTitle(Method titleMethod) => this.titleMethod = titleMethod;

    public virtual string title(NakedObject @object)
    {
      try
      {
        Method titleMethod = this.titleMethod;
        object obj = @object.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        return titleMethod.invoke(obj, objArray)?.ToString();
      }
      catch (InvocationTargetException ex)
      {
        Throwable targetException = ex.getTargetException();
        if (targetException is ResolveException)
          throw (ResolveException) targetException;
        JavaObjectTitle.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.titleMethod).ToString(), targetException);
      }
      catch (IllegalAccessException ex)
      {
        JavaObjectTitle.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.titleMethod).ToString(), (Throwable) ex);
      }
      return "title error...";
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static JavaObjectTitle()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      JavaObjectTitle javaObjectTitle = this;
      ObjectImpl.clone((object) javaObjectTitle);
      return ((object) javaObjectTitle).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
