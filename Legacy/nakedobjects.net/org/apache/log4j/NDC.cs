// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.NDC
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j.helpers;
using System.ComponentModel;
using System.Threading;

namespace org.apache.log4j
{
  public class NDC
  {
    [JavaFlags(8)]
    public static Hashtable ht;
    [JavaFlags(8)]
    public static int pushCounter;
    [JavaFlags(24)]
    public const int REAP_THRESHOLD = 5;

    private NDC()
    {
    }

    public static void clear() => ((Vector) NDC.ht.get((object) Thread.currentThread()))?.setSize(0);

    public static Stack cloneStack()
    {
      object obj = NDC.ht.get((object) Thread.currentThread());
      return obj == null ? (Stack) null : (Stack) ((Vector) obj).MemberwiseClone();
    }

    public static void inherit(Stack stack)
    {
      if (stack == null)
        return;
      NDC.ht.put((object) Thread.currentThread(), (object) stack);
    }

    public static string get()
    {
      Stack stack = (Stack) NDC.ht.get((object) Thread.currentThread());
      return stack != null && !((Vector) stack).isEmpty() ? ((NDC.DiagnosticContext) stack.peek()).fullMessage : (string) null;
    }

    public static int getDepth()
    {
      Stack stack = (Stack) NDC.ht.get((object) Thread.currentThread());
      return stack == null ? 0 : ((Vector) stack).size();
    }

    private static void lazyRemove()
    {
      object ht = (object) NDC.ht;
      \u003CCorArrayWrapper\u003E.Enter(ht);
      Vector vector;
      try
      {
        if (++NDC.pushCounter <= 5)
          return;
        NDC.pushCounter = 0;
        int num = 0;
        vector = new Vector();
        Enumeration enumeration = NDC.ht.keys();
        while (enumeration.hasMoreElements())
        {
          if (num <= 4)
          {
            Thread thread = (Thread) enumeration.nextElement();
            if (thread.isAlive())
            {
              ++num;
            }
            else
            {
              num = 0;
              vector.addElement((object) thread);
            }
          }
          else
            break;
        }
      }
      finally
      {
        Monitor.Exit(ht);
      }
      int num1 = vector.size();
      for (int index = 0; index < num1; ++index)
      {
        Thread thread = (Thread) vector.elementAt(index);
        LogLog.debug(new StringBuffer().append("Lazy NDC removal for thread [").append(thread.getName()).append("] (").append(NDC.ht.size()).append(").").ToString());
        NDC.ht.remove((object) thread);
      }
    }

    public static string pop()
    {
      Thread thread = Thread.currentThread();
      Stack stack = (Stack) NDC.ht.get((object) thread);
      return stack != null && !((Vector) stack).isEmpty() ? ((NDC.DiagnosticContext) stack.pop()).message : "";
    }

    public static string peek()
    {
      Thread thread = Thread.currentThread();
      Stack stack = (Stack) NDC.ht.get((object) thread);
      return stack != null && !((Vector) stack).isEmpty() ? ((NDC.DiagnosticContext) stack.peek()).message : "";
    }

    public static void push(string message)
    {
      Thread thread = Thread.currentThread();
      Stack stack1 = (Stack) NDC.ht.get((object) thread);
      if (stack1 == null)
      {
        NDC.DiagnosticContext diagnosticContext = new NDC.DiagnosticContext(message, (NDC.DiagnosticContext) null);
        Stack stack2 = new Stack();
        NDC.ht.put((object) thread, (object) stack2);
        stack2.push((object) diagnosticContext);
      }
      else if (((Vector) stack1).isEmpty())
      {
        NDC.DiagnosticContext diagnosticContext = new NDC.DiagnosticContext(message, (NDC.DiagnosticContext) null);
        stack1.push((object) diagnosticContext);
      }
      else
      {
        NDC.DiagnosticContext parent = (NDC.DiagnosticContext) stack1.peek();
        stack1.push((object) new NDC.DiagnosticContext(message, parent));
      }
    }

    public static void remove()
    {
      NDC.ht.remove((object) Thread.currentThread());
      NDC.lazyRemove();
    }

    public static void setMaxDepth(int maxDepth)
    {
      Stack stack = (Stack) NDC.ht.get((object) Thread.currentThread());
      if (stack == null || maxDepth >= ((Vector) stack).size())
        return;
      ((Vector) stack).setSize(maxDepth);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static NDC()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      NDC ndc = this;
      ObjectImpl.clone((object) ndc);
      return ((object) ndc).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(42)]
    private class DiagnosticContext
    {
      [JavaFlags(0)]
      public string fullMessage;
      [JavaFlags(0)]
      public string message;

      [JavaFlags(0)]
      public DiagnosticContext(string message, NDC.DiagnosticContext parent)
      {
        this.message = message;
        if (parent != null)
          this.fullMessage = new StringBuffer().append(parent.fullMessage).append(' ').append(message).ToString();
        else
          this.fullMessage = message;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        NDC.DiagnosticContext diagnosticContext = this;
        ObjectImpl.clone((object) diagnosticContext);
        return ((object) diagnosticContext).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
