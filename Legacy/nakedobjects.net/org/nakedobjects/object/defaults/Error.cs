// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.Error
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.reflect.@internal.about;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedError;")]
  public class Error : NakedError
  {
    private static readonly org.apache.log4j.Logger LOG;
    private string error;
    private string exception;
    private string trace;

    public Error(string msg)
    {
      this.error = msg;
      Error.LOG.error((object) this.error);
    }

    public Error(string msg, Throwable e)
    {
      this.error = msg;
      this.exception = e.getMessage();
      this.trace = ExceptionHelper.exceptionTraceAsString(e);
    }

    public virtual void aboutError(InternalAbout about, string entry) => about.unusable();

    public virtual void aboutException(InternalAbout about, string entry) => about.unusable();

    public virtual void aboutTrace(InternalAbout about, string entry) => about.unusable();

    public virtual string getError() => this.error;

    public virtual string getException() => this.exception;

    public virtual string getTrace() => this.trace;

    public virtual void setError(string error) => this.error = error;

    public virtual void setException(string exception) => this.exception = exception;

    public virtual void setTrace(string trace) => this.trace = trace;

    public virtual string titleString() => this.error;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Error()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Error error = this;
      ObjectImpl.clone((object) error);
      return ((object) error).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
