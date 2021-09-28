// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.performance.ListVsVector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.io;
using java.lang;
using java.util;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.apache.log4j.performance
{
  public class ListVsVector
  {
    [JavaFlags(8)]
    public static int RUN_LENGTH;
    [JavaFlags(8)]
    public static Vector v;
    [JavaFlags(8)]
    public static ListVsVector.Chain head;
    [JavaFlags(8)]
    public static string tmp;

    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      ListVsVector.v.addElement((object) "aaa");
      ListVsVector.v.addElement((object) "bbb");
      ListVsVector.v.addElement((object) "ccc");
      ListVsVector.v.addElement((object) "ddd");
      ListVsVector.v.addElement((object) "eee");
      ListVsVector.Chain chain = new ListVsVector.Chain("aaa");
      ListVsVector.head = chain;
      chain.next = new ListVsVector.Chain("bbb");
      ListVsVector.Chain next1 = chain.next;
      next1.next = new ListVsVector.Chain("ccc");
      ListVsVector.Chain next2 = next1.next;
      next2.next = new ListVsVector.Chain("ddd");
      next2.next.next = new ListVsVector.Chain("eee");
      double num1 = ListVsVector.loopChain();
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("Looping thourgh the chain took ").append(num1).ToString());
      double num2 = ListVsVector.loopVector();
      ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("Looping thourgh the vector took ").append(num2).ToString());
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(8)]
    public static double loopChain()
    {
      long num = java.lang.System.currentTimeMillis();
      for (int index = 0; index < ListVsVector.RUN_LENGTH; ++index)
      {
        for (ListVsVector.Chain chain = ListVsVector.head; chain != null; chain = chain.next)
          ListVsVector.tmp = chain.s;
      }
      return (double) (java.lang.System.currentTimeMillis() - num) * 1000.0 / (double) ListVsVector.RUN_LENGTH;
    }

    [JavaFlags(8)]
    public static double loopVector()
    {
      long num1 = java.lang.System.currentTimeMillis();
      int num2 = ListVsVector.v.size();
      for (int index1 = 0; index1 < ListVsVector.RUN_LENGTH; ++index1)
      {
        for (int index2 = 0; index2 < num2; ++index2)
          ListVsVector.tmp = \u003CVerifierFix\u003E.genCastToString(ListVsVector.v.elementAt(index2));
      }
      return (double) (java.lang.System.currentTimeMillis() - num1) * 1000.0 / (double) ListVsVector.RUN_LENGTH;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ListVsVector()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ListVsVector listVsVector = this;
      ObjectImpl.clone((object) listVsVector);
      return ((object) listVsVector).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(40)]
    public class Chain
    {
      public string s;
      public ListVsVector.Chain next;

      [JavaFlags(0)]
      public Chain(string s) => this.s = s;

      [JavaFlags(0)]
      public virtual void setNext(ListVsVector.Chain c) => this.next = c;

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        ListVsVector.Chain chain = this;
        ObjectImpl.clone((object) chain);
        return ((object) chain).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
