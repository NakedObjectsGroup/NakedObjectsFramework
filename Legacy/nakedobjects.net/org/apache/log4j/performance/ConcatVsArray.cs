// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.performance.ConcatVsArray
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace org.apache.log4j.performance
{
  public class ConcatVsArray
  {
    [JavaFlags(8)]
    public static void Usage()
    {
      ((PrintStream) System.err).println("Usage: java org.apache.log4j.performance.ConcatVsArray string1 string2 runLength\n       where runLength is an integer.");
      System.exit(1);
    }

    public static void main(string[] args)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ConcatVsArray concatVsArray = this;
      ObjectImpl.clone((object) concatVsArray);
      return ((object) concatVsArray).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
