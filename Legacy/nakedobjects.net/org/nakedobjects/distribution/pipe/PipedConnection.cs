// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.pipe.PipedConnection
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.distribution.command;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.distribution.pipe
{
  public class PipedConnection
  {
    private static readonly Logger LOG;
    private Request request;
    private Response response;
    private RuntimeException exception;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void setRequest(Request request)
    {
      this.request = request;
      ObjectImpl.notify((object) this);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Request getRequest()
    {
      while (this.request == null)
      {
        try
        {
          ObjectImpl.wait((object) this);
        }
        catch (InterruptedException ex)
        {
          PipedConnection.LOG.error((object) "wait (getRequest) interrupted", (Throwable) ex);
        }
      }
      Request request = this.request;
      this.request = (Request) null;
      ObjectImpl.notify((object) this);
      return request;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void setResponse(Response response)
    {
      this.response = response;
      ObjectImpl.notify((object) this);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void setException(RuntimeException exception)
    {
      this.exception = exception;
      ObjectImpl.notify((object) this);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Response getResponse()
    {
      while (this.response == null)
      {
        if (this.exception == null)
        {
          try
          {
            ObjectImpl.wait((object) this);
          }
          catch (InterruptedException ex)
          {
            PipedConnection.LOG.error((object) "wait (getResponse) interrupted", (Throwable) ex);
          }
        }
        else
          break;
      }
      if (this.exception != null)
      {
        RuntimeException exception = this.exception;
        this.exception = (RuntimeException) null;
        throw exception;
      }
      Response response = this.response;
      this.response = (Response) null;
      ObjectImpl.notify((object) this);
      return response;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static PipedConnection()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PipedConnection pipedConnection = this;
      ObjectImpl.clone((object) pipedConnection);
      return ((object) pipedConnection).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
