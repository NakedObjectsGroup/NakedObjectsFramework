// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.BackgroundThread
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace org.nakedobjects.viewer.skylark.core
{
  public sealed class BackgroundThread
  {
    private static readonly Logger LOG;

    private BackgroundThread()
    {
    }

    public static void run(View view, BackgroundTask task) => new BackgroundThread.\u0031("nofBackground", view, task).start();

    private static void repaint(View view)
    {
      view.markDamaged();
      view.getViewManager().repaint();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static BackgroundThread()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      BackgroundThread backgroundThread = this;
      ObjectImpl.clone((object) backgroundThread);
      return ((object) backgroundThread).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(32)]
    [Inner]
    public class \u0031 : Thread
    {
      [JavaFlags(16)]
      public readonly BackgroundTask task_\u003E;
      [JavaFlags(16)]
      public readonly View view_\u003E;

      public virtual void run()
      {
        this.view_\u003E.getState().setActive();
        this.view_\u003E.getViewManager().setBusy(this.view_\u003E);
        BackgroundThread.repaint(this.view_\u003E);
        try
        {
          this.task_\u003E.execute();
        }
        catch (Exception ex)
        {
          Throwable throwable = ThrowableWrapper.wrapThrowable(ex);
          if (!(throwable is NakedObjectApplicationException))
          {
            string str = new StringBuffer().append("Error while running background task ").append(this.task_\u003E.getName()).ToString();
            BackgroundThread.LOG.error((object) str, throwable);
          }
          this.view_\u003E.getViewManager().showException(throwable);
        }
        finally
        {
          this.view_\u003E.getState().setInactive();
          this.view_\u003E.getViewManager().clearBusy(this.view_\u003E);
          BackgroundThread.repaint(this.view_\u003E);
        }
      }

      public \u0031(string dummy0, [In] View obj1, [In] BackgroundTask obj2)
        : base(dummy0)
      {
        this.view_\u003E = obj1;
        this.task_\u003E = obj2;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public virtual object MemberwiseClone()
      {
        BackgroundThread.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }
    }
  }
}
