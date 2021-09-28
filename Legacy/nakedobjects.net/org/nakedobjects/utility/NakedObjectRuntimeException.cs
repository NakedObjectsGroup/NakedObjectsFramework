// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.NakedObjectRuntimeException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using System;
using System.Threading;

namespace org.nakedobjects.utility
{
  public class NakedObjectRuntimeException : RuntimeException
  {
    [JavaFlags(0)]
    public bool IN_JAVA2;
    private readonly Throwable cause;

    public NakedObjectRuntimeException()
    {
      this.IN_JAVA2 = false;
      this.cause = (Throwable) null;
    }

    public NakedObjectRuntimeException(string msg)
      : base(msg)
    {
      this.IN_JAVA2 = false;
      this.cause = (Throwable) null;
    }

    public NakedObjectRuntimeException(Throwable cause)
      : base(cause?.ToString())
    {
      this.IN_JAVA2 = false;
      this.cause = cause;
    }

    public NakedObjectRuntimeException(string msg, Throwable cause)
      : base(msg)
    {
      this.IN_JAVA2 = false;
      this.cause = cause;
    }

    public NakedObjectRuntimeException(string msg, Exception innerException)
      : base(msg, innerException)
    {
      this.IN_JAVA2 = false;
      this.cause = (Throwable) null;
    }

    public virtual Throwable getCause() => this.cause == this ? (Throwable) null : this.cause;

    public virtual void printStackTrace(PrintStream s)
    {
      if (this.IN_JAVA2)
      {
        ((Throwable) this).printStackTrace(s);
      }
      else
      {
        object obj = (object) s;
        \u003CCorArrayWrapper\u003E.Enter(obj);
        try
        {
          s.println(new StringBuffer().append(" Exception: ").append((object) this).ToString());
          ((Throwable) this).printStackTrace(s);
          Throwable cause = this.getCause();
          if (cause == null)
            return;
          s.println(new StringBuffer().append("Caused by: ").append((object) this).ToString());
          cause.printStackTrace(s);
        }
        finally
        {
          Monitor.Exit(obj);
        }
      }
    }

    public virtual void printStackTrace(PrintWriter s)
    {
      if (this.IN_JAVA2)
      {
        ((Throwable) this).printStackTrace(s);
      }
      else
      {
        object obj = (object) s;
        \u003CCorArrayWrapper\u003E.Enter(obj);
        try
        {
          s.println(new StringBuffer().append(" Exception: ").append((object) this).ToString());
          ((Throwable) this).printStackTrace(s);
          Throwable cause = this.getCause();
          if (cause == null)
            return;
          s.println(new StringBuffer().append("Caused by: ").append((object) this).ToString());
          cause.printStackTrace(s);
        }
        finally
        {
          Monitor.Exit(obj);
        }
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      NakedObjectRuntimeException runtimeException = this;
      ObjectImpl.clone((object) runtimeException);
      return ((object) runtimeException).MemberwiseClone();
    }
  }
}
